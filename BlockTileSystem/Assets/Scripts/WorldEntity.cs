using UnityEngine;
using System.Collections;

public class WorldEntity : MonoBehaviour
{
    //simulate priority later.

    public EntityType entityType;
    public int characterID;

    private EntityCollidingType _collidingType;
    public EntityCollidingType CollidingType
    {
        get { return _collidingType; }
        set { _collidingType = value; }
    }
    [HideInInspector]
    public bool isPushed;
    [HideInInspector]
    public Direction pushedDirection;

    [SerializeField]
    IntVector _location;

    public IntVector Location
    {
        get { return _location; }
        set { _location = value; }
    }
    [SerializeField]
    private GameObject _visualPrefab;
    private Transform _visuals;

    [System.Serializable]
    public struct StateInformation
    {
        // [HideInInspector]
        // public Char2DState lastState;
        // public Char2DState state;

        [HideInInspector]
        public float fractionComplete;
        // Range from 0-1, inclusive
        [HideInInspector]
        public IntVector lastLoc;
        // [HideInInspector]
        // public bool inactive;
        [HideInInspector]
        public static bool characterInMoving;
    }
    private StateInformation _currStateInfo;
    public StateInformation StateInfo
    {
        get { return _currStateInfo; }
    }
    public Vector2 visPosition
    {
        get
        {
            return _visuals.position;
        }
        set
        {
            _visuals.position = value;
        }
    }

    public AnimationCurve visMovingCurve;
    public float movingDuration;
    float timer;

    public bool isSpriteSet;
    public void SetVisual(Sprite sprite)
    {
        _visuals = Instantiate(_visualPrefab).transform;
        _visuals.position = (_location.ToVector2() + new Vector2(0.5f, -0.5f)) * WorldManager.g.TileSize;
        _visuals.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        isSpriteSet = true;
    }
    public void ChangeVisual(Sprite sprite)
    {
        _visuals.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }
    public void SetOrderLayer(int layer)
    {
        _visuals.gameObject.GetComponent<SpriteRenderer>().sortingOrder = layer;
    }

    private void Update()
    {
        if (isSpriteSet)
        {
            Vector2 fixedOffset = new Vector2(0.5f, -0.5f);
        
            float distance = Vector2.Distance(_location.ToVector2(), _currStateInfo.lastLoc.ToVector2());
        
//print("_location "+ _location.ToVector2()+" last loc "+_currStateInfo.lastLoc.ToVector2()+ " distance "+ distance);
            if (distance < 1.1f && distance > 0f)
            {
                
                Vector2 v = Vector2.zero;
                Vector2 visualOffset = (_location.ToVector2() - _currStateInfo.lastLoc.ToVector2())
                                       * (_currStateInfo.fractionComplete);
            
                v = _currStateInfo.lastLoc.ToVector2() + visualOffset + fixedOffset;
                _visuals.position = v * WorldManager.g.TileSize;
                if (entityType == EntityType.Character)
                {
                    StateInformation.characterInMoving = true;
                }

            
                if (timer < movingDuration)
                {
                    timer += Time.deltaTime;
                    _currStateInfo.fractionComplete = visMovingCurve.Evaluate(timer / movingDuration);
                }
            

                if (_currStateInfo.fractionComplete >= 1f)
                {
                    _currStateInfo.lastLoc = _location;
                    _currStateInfo.fractionComplete = 0f;
                    if (entityType == EntityType.Character)
                    {
                        StateInformation.characterInMoving = false;
                    }
                    timer = 0f;
                }
            }
            else
            {
                Vector2 v = _currStateInfo.lastLoc.ToVector2() + fixedOffset;
                _visuals.position = v * WorldManager.g.TileSize;
                _currStateInfo.lastLoc = _location;
            }
        }
    }

    private bool _registered = false;

    public void RegisterMe()
    {
        if (!_registered)
        {
            WorldManager.g.RegisterEntity(this);
            _registered = true;
        }
    }

    public void DeregisterMe()
    {
        if (_registered)
        {
            WorldManager.g.DeregisterEntity(this);
            _registered = false;
        }
    }

    void Awake()
    {
        _currStateInfo.lastLoc = _location;
    }
    void Start()
    {
        RegisterMe();
        
    }
    // void OnEnable()
    // {
    //     RegisterMe();
    // }

    void OnDisable()
    {
        DestroyVisual();
        isSpriteSet = false;
    }
        public void DestroyVisual()
    {
        if (isSpriteSet)
        {
            Destroy(_visuals.gameObject);
            isSpriteSet = false;
        }
    }
    public void Pushed(Direction direction)
    {
        //for character being pushed. play pushed animation
        isPushed = true;
        pushedDirection = direction;
    }

    public delegate void SimulatorDelegates();
    public SimulatorDelegates Simulators;

    public void Simulate()
    {
        if (Simulators != null)
        {
            Simulators();
        }
    }
}