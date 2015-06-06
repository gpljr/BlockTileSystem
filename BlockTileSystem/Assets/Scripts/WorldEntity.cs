using UnityEngine;
using System.Collections;

public class WorldEntity : MonoBehaviour
{
    //simulate priority later.

    public EntityType entityType;
    public int characterID;

    [SerializeField]
    private EntityCollidingType _collidingType;
    EntityCollidingType _tempCollisionType;
    bool tempCollisionTypeSet;
    public EntityCollidingType CollidingType
    {
        get { return _collidingType; }
        set
        {
            _collidingType = value; 
            _tempCollisionType = value;
        }
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

    IntVector tempLocation;

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
        public Vector2 lastLoc;
        // [HideInInspector]
        // public bool inactive;
        [HideInInspector]
        public bool characterInMoving;
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
    public bool instantMove;

    public AnimationCurve visMovingCurve;

    public float movingDuration = 0.35f;
    float timer;

    public bool isSpriteSet;
    public void SetVisual(Sprite sprite)
    {
        _visuals = ((GameObject)Instantiate(_visualPrefab, (_location.ToVector2() + new Vector2(0.5f, -0.5f)) * WorldManager.g.TileSize, new Quaternion(0, 0, 0, 0))).transform;
        _visuals.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        _currStateInfo.lastLoc = _location.ToVector2();
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
    public void Refresh()
    {
        timer = 0f;
        _currStateInfo.lastLoc = _location.ToVector2();
        _currStateInfo.fractionComplete = 0f;
        _currStateInfo.characterInMoving=false;
    }
    private void Update()
    {
        // if (!tempCollisionTypeSet)
        // {
        //     _tempCollisionType = _collidingType;
        //     tempCollisionTypeSet = true;
        // }
        
        if (isSpriteSet)
        {
            Vector2 fixedOffset = new Vector2(0.5f, -0.5f);
            
            float distance = Vector2.Distance(_location.ToVector2(), _currStateInfo.lastLoc);
            
            if (!instantMove && distance > 0f)
            {
                Vector2 v = Vector2.zero;
                Vector2 visualOffset = (_location.ToVector2() - _currStateInfo.lastLoc)
                                       * (_currStateInfo.fractionComplete);

                v = _currStateInfo.lastLoc + visualOffset + fixedOffset;
                _visuals.position = v * WorldManager.g.TileSize;
                    
                if (entityType == EntityType.Character)
                {
                    _collidingType = EntityCollidingType.Colliding;
                    _currStateInfo.characterInMoving = true;
                }
                // if (tempLocation != _location && timer>0)
                //     {
                //         //an input when the move is not finished
                //         timer = 0f;
                //         _currStateInfo.lastLoc += visualOffset;
                //         print("reset");
                //         movingDuration+=0.05f;
                //     }
                if (timer < movingDuration)
                {

                    timer += Time.deltaTime;
                    _currStateInfo.fractionComplete = visMovingCurve.Evaluate(timer / movingDuration);
                }
            

                if (_currStateInfo.fractionComplete >= 1f)
                {
                    _currStateInfo.lastLoc = _location.ToVector2();
                    _currStateInfo.fractionComplete = 0f;
                    if (entityType == EntityType.Character)
                    {
                        _currStateInfo.characterInMoving = false;
                    }
                    timer = 0f;
                    _collidingType = _tempCollisionType;
                }
                    
            }
            else
            {
                Vector2 v = _currStateInfo.lastLoc + fixedOffset;
                _visuals.position = v * WorldManager.g.TileSize;
                _currStateInfo.lastLoc = _location.ToVector2();
                instantMove = false;
                _collidingType = _tempCollisionType;
            }
            //tempLocation = _location;
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
            if (_visuals != null)
            {
                Destroy(_visuals.gameObject);
            }
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