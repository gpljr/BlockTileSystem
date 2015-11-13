using UnityEngine;
using System.Collections;

public class WorldEntity : MonoBehaviour {
    //simulate priority later.

    public EntityType entityType;
    public int characterID;

    [SerializeField]
    private EntityCollidingType _collidingType;
    EntityCollidingType _tempCollisionType;
    bool tempCollisionTypeSet;
    public EntityCollidingType CollidingType {
        get { return _collidingType; }
        set {
            _collidingType = value; 
            _tempCollisionType = value;
        }
    }
    bool isPushed;
    public bool IsPushed {
        get { return isPushed; }
        set { isPushed=value;}
    }
    [HideInInspector]
    public Direction pushedDirection;

    [SerializeField]
    IntVector _location;

    public IntVector Location {
        get { return _location; }
        set { _location = value; }
    }

    IntVector tempLocation;

    [SerializeField]
    private GameObject _visualPrefab;
    private Transform _visuals;

    Animator _anim;

    //[System.Serializable] //what does this mean?
    public struct StateInformation {
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
    public StateInformation StateInfo {
        get { return _currStateInfo; }
    }
    public Vector2 visPosition {
        get {
            return _visuals.position;


        }
        set {
            _visuals.position = value;
        }
    }
    [HideInInspector] public bool instantMove;

    public AnimationCurve visMovingCurve;
    public AnimationCurve visPushedUpCurve;
    public AnimationCurve visPushedDownCurve;

    public AnimationCurve visStuckCurve;
    public AnimationCurve visPushStuckCurve;
    public AnimationCurve visPushedStuckCurve;

    [HideInInspector] public float movingDuration = 0.5f;
    [SerializeField] float stuckDuration = 0.2f;
    float timer;

    private Character _character;
    public StuckType stuckType;

    bool inStuckAnimation;

    public bool entityActive=true;

    public void SetCharacter () {
        if (gameObject.GetComponent<Character>() != null) {
            _character = gameObject.GetComponent<Character>();
        }
    }

    [HideInInspector] public bool isSpriteSet;
    public void SetVisual (Sprite sprite) {
        
        _visuals = ((GameObject)Instantiate(_visualPrefab, (_location.ToVector2() + new Vector2(0.5f, -0.5f)) * WorldManager.g.TileSize, new Quaternion(0, 0, 0, 0))).transform;
        if (sprite != null) {
            _visuals.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        }
        _anim = _visuals.gameObject.GetComponent<Animator>();
        _currStateInfo.lastLoc = _location.ToVector2();
        isSpriteSet = true;
        // if(entityType== EntityType.Bullet)
        // {
        //     print("bullet visual loc"+_currStateInfo.lastLoc);
        // }
    }
    public void ChangeVisual (Sprite sprite) {
        _visuals.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }
    public void SetOrderLayer (int layer) {
        _visuals.gameObject.GetComponent<SpriteRenderer>().sortingOrder = layer;
    }
    public void SetBoolAnimationParameter (string ParaName, bool ParaState) {
        if (_anim != null) {
            _anim.SetBool(ParaName, ParaState);
        }
    }
    public void Refresh () {
        timer = 0f;
        _currStateInfo.lastLoc = _location.ToVector2();
        _currStateInfo.fractionComplete = 0f;
        _currStateInfo.characterInMoving = false;
        isPushed = false;
    }
    public void PushedStuck (Direction direction) {
        if (_character != null) {
            _character.Stuck();
            _character.Direction = direction;
        }
    }
    private void Update () {
        
        //sprite moving animation
        if (isSpriteSet) {
            Vector2 fixedOffset = new Vector2(0.5f, -0.5f);
            bool stuck = false;
            float distance = Vector2.Distance(_location.ToVector2(), _currStateInfo.lastLoc);
            if (_character != null) {
                stuck = _character.isStuck;
            }
            //stuck animation
            if (stuck) {
                inStuckAnimation=true;
                Vector2 v = Vector2.zero;
                float x = 0f;
                float y = 0f;
                switch (_character.Direction) {
                    case Direction.North:
                        x = 0f;
                        y = 1f;
                        break;
                    case Direction.South:
                        x = 0f;
                        y = -1f;
                        break;
                    case Direction.West:
                        x = -1f;
                        y = 0f;
                        break;
                    case Direction.East:
                        x = 1f;
                        y = 0f;
                        break;
                }
                Vector2 visualOffset = new Vector2(x, y);
                visualOffset *= (_currStateInfo.fractionComplete);
                
                v = _currStateInfo.lastLoc + visualOffset + fixedOffset;
                _visuals.position = v * WorldManager.g.TileSize;

                
                var stuckCurve = visStuckCurve;
                switch (stuckType) {
                    case StuckType.MoveStuck:
                        stuckCurve = visStuckCurve;
                        break;
                    case StuckType.PushStuck:
                        stuckCurve = visPushStuckCurve;
                        break;
                    case StuckType.PushedStuck:
                        stuckCurve = visPushedStuckCurve;
                        break;
                }
                if (timer < stuckDuration) {
                    timer += Time.deltaTime;

                    _currStateInfo.fractionComplete = stuckCurve.Evaluate(timer / stuckDuration);
   
                } 
                if (timer >= stuckDuration) {
                    AnimationStop();
                    _currStateInfo.lastLoc = _location.ToVector2();
                    _currStateInfo.fractionComplete = 0f;
                    
                    timer = 0f;
                    _character.isStuck = false;
                    stuckType = StuckType.Null;
                    inStuckAnimation=false;
                }

                //non stuck
            } else {
                
                if (!instantMove && distance > 0f) {
                //     if(inStuckAnimation)
                // {
                //     AnimationStop();
                //     _currStateInfo.lastLoc = _location.ToVector2();
                //     _currStateInfo.fractionComplete = 0f;
                    
                //     timer = 0f;
                //     _character.isStuck = false;
                //     stuckType = StuckType.Null;
                //     inStuckAnimation=false;
                // }

                    Vector2 v = Vector2.zero;
                    Vector2 visualOffset = (_location.ToVector2() - _currStateInfo.lastLoc)
                                           * (_currStateInfo.fractionComplete);
                
                    v = _currStateInfo.lastLoc + visualOffset + fixedOffset;
                    _visuals.position = v * WorldManager.g.TileSize;
                
                    if (_character != null) {
                        if (!_character.onMergingStar) {
                            _collidingType = EntityCollidingType.Colliding;
                        }
                        _currStateInfo.characterInMoving = true;
                    }
                    if (timer < movingDuration) {

                        timer += Time.deltaTime;
                        if (_character != null) {
                            if (_character.isPushedUp) {
                                _currStateInfo.fractionComplete = visPushedUpCurve.Evaluate(timer / movingDuration);
                            } else if (_character.isPushedDown) {
                                _currStateInfo.fractionComplete = visPushedDownCurve.Evaluate(timer / movingDuration);
                            } else {
                                _currStateInfo.fractionComplete = visMovingCurve.Evaluate(timer / movingDuration);
                            }
                        } else {
                            _currStateInfo.fractionComplete = visMovingCurve.Evaluate(timer / movingDuration);
                        }
                        
                    } 
                    if (timer >= movingDuration) { // why can't else work???
                        AnimationStop();
                        _currStateInfo.lastLoc = _location.ToVector2();
                        _currStateInfo.fractionComplete = 0f;

                        timer = 0f;
                        _collidingType = _tempCollisionType;
                        if (_character != null) {
                            _currStateInfo.characterInMoving = false;
                            _character.isPushedUp = false;
                            _character.isPushedDown = false;
                            _character.characterInMoving = false;
                            
                        }
                        isPushed = false;
                    
                    }
                } else {
                    Vector2 v = _currStateInfo.lastLoc + fixedOffset;
                    _visuals.position = v * WorldManager.g.TileSize;
                    _currStateInfo.lastLoc = _location.ToVector2();
                    instantMove = false;
                    _collidingType = _tempCollisionType;
                }
            }

        }




    }
    private void AnimationStop () {
        SetBoolAnimationParameter("MoveUp", false);
        SetBoolAnimationParameter("MoveDown", false);
        SetBoolAnimationParameter("MoveLeft", false);
        SetBoolAnimationParameter("MoveRight", false);

        SetBoolAnimationParameter("MoveUpSlow", false);
        SetBoolAnimationParameter("MoveDownSlow", false);
        SetBoolAnimationParameter("MoveLeftSlow", false);
        SetBoolAnimationParameter("MoveRightSlow", false);

        SetBoolAnimationParameter("PushUp", false);
        SetBoolAnimationParameter("PushDown", false);
        SetBoolAnimationParameter("PushLeft", false);
        SetBoolAnimationParameter("PushRight", false);

        SetBoolAnimationParameter("PushedUp", false);
        SetBoolAnimationParameter("PushedDown", false);
        SetBoolAnimationParameter("PushedLeft", false);
        SetBoolAnimationParameter("PushedRight", false);

        SetBoolAnimationParameter("StuckUp1", false);
        SetBoolAnimationParameter("StuckDown1", false);
        SetBoolAnimationParameter("StuckLeft1", false);
        SetBoolAnimationParameter("StuckRight1", false);

        SetBoolAnimationParameter("PushStuckUp", false);
        SetBoolAnimationParameter("PushStuckDown", false);
        SetBoolAnimationParameter("PushStuckLeft", false);
        SetBoolAnimationParameter("PushStuckRight", false);

        SetBoolAnimationParameter("PushedStuckUp", false);
        SetBoolAnimationParameter("PushedStuckDown", false);
        SetBoolAnimationParameter("PushedStuckLeft", false);
        SetBoolAnimationParameter("PushedStuckRight", false);

        if (_character != null) {
            _character.SetSpriteByDistance(true);
        }

    }
    private bool _registered = false;

    public void RegisterMe () {
        if (!_registered) {
            WorldManager.g.RegisterEntity(this);
            _registered = true;
        }
    }

    public void DeregisterMe () {
        if (_registered) {
            WorldManager.g.DeregisterEntity(this);
            _registered = false;
        }
    }

    void Start () {
        RegisterMe();
        
    }
    // void OnEnable()
    // {
    //     RegisterMe();
    // }

    void OnDisable () {
        DestroyVisual();
        isSpriteSet = false;
    }
    public void DestroyVisual () {
        if (isSpriteSet) {
            if (_visuals != null) {
                Destroy(_visuals.gameObject);
            }
            isSpriteSet = false;
        }
    }
    public void Pushed (Direction direction) {
        //for character being pushed. play pushed animation
        isPushed = true;
        pushedDirection = direction;
        //AnimationStop();
    }

    public delegate void SimulatorDelegates();
    public SimulatorDelegates Simulators;

    public void Simulate () {
        if (Simulators != null) {
            Simulators();
        }
    }
}