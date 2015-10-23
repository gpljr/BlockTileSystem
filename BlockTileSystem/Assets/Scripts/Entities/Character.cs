using UnityEngine;
using System.Collections;
using InControl;


public class Character : MonoBehaviour {
    [SerializeField]
    KeyCode _leftKey;
    [SerializeField]
    KeyCode _rightKey;
    [SerializeField]
    KeyCode _upKey;
    [SerializeField]
    KeyCode _downKey;

    private WorldEntity _worldEntity;
    private IntVector _input;

    private Direction _direction;
    public Direction Direction {
        get { return _direction; }
        set{ _direction = value; }
    }
    private bool _bMove;
    public bool MoveInput {
        get { return _bMove; }
    }

    [SerializeField]
    AudioClip _audioMove;
    [SerializeField]
    AudioClip _audioPush;
    [SerializeField]
    AudioClip _audioStuck;

    [SerializeField]
    int _iCharacterID;

    // [SerializeField]
    // private Sprite _char1YellowSprite;
    // [SerializeField]
    // private Sprite _char2GreenSprite;
    // [SerializeField]
    // private Sprite _char3CombinedSprite;
    // [SerializeField]
    // private Sprite _char1MergingYellowSprite;
    // [SerializeField]
    // private Sprite _char2MergingGreenSprite;

    // [SerializeField]
    // private Sprite _char1Face1Sprite;
    // [SerializeField]
    // private Sprite _char1Face2Sprite;
    // [SerializeField]
    // private Sprite _char1Face3Sprite;
    // [SerializeField]
    // private Sprite _char2Face1Sprite;
    // [SerializeField]
    // private Sprite _char2Face2Sprite;
    // [SerializeField]
    // private Sprite _char2Face3Sprite;

    public static bool oneEnteredMergingStar;

    private bool onMergingStar;

    [HideInInspector] public bool isPushedDown;
    [HideInInspector] public bool isPushedUp;
    [HideInInspector] public bool isStuck;

    float idleTimer;
    [SerializeField] float idleTimeLimit = 3f;
    [SerializeField] float idleTimeRandLimit = 2f;
    bool idling;
    float idlingTimer;

    bool pushed;

    [HideInInspector] public bool characterInMoving;

    public void Cache () {
        _worldEntity = GetComponent<WorldEntity>();
    }

    void Awake () {
        Cache();
        _worldEntity.CollidingType = EntityCollidingType.Pushable;
        _worldEntity.entityType = EntityType.Character;
        _worldEntity.characterID = _iCharacterID;
        _worldEntity.SetCharacter();
    }
    void RefreshOnLevelLoaded (LevelLoadedEvent e) {
        Refresh();
    }
    void RefreshOnBulletHit (BulletHitEvent e) {
        Refresh();
    }
    void Refresh () {
        _worldEntity.CollidingType = EntityCollidingType.Pushable;
        onMergingStar = false;
        oneEnteredMergingStar = false;
        _worldEntity.DestroyVisual();
        _worldEntity.isSpriteSet = false;
        //StartCoroutine(WaitForSetDistance());
        ExitIdle();
        _worldEntity.Refresh();
    }
    void OnEnable () {
        _worldEntity.Simulators += Simulate;
        Events.g.AddListener<MergingStarEvent>(StepOnMergingStar);
        Events.g.AddListener<LevelLoadedEvent>(RefreshOnLevelLoaded);
        Events.g.AddListener<BulletHitEvent>(RefreshOnBulletHit);
    }

    void OnDisable () {
        _worldEntity.Simulators -= Simulate;
        Events.g.RemoveListener<MergingStarEvent>(StepOnMergingStar);
        Events.g.RemoveListener<LevelLoadedEvent>(RefreshOnLevelLoaded);
        Events.g.AddListener<BulletHitEvent>(RefreshOnBulletHit);
    }


    void Update () {
        if (!_worldEntity.isSpriteSet) {
            _worldEntity.SetVisual(null);
            SetLayer(false);
            
        } 
        if (!isStuck && !characterInMoving && !onMergingStar && !idling && !_worldEntity.IsPushed) {
            SetSpriteByDistance(true);
        }
        
        if (LevelCode.gameState == GameState.InLevel) {
            switch (LevelCode.levelType) {
                case LevelType.Normal: 
                    _worldEntity.movingDuration = 0.5f;
                    break;
                case LevelType.Separation: 
                    _worldEntity.movingDuration = 0.75f;
                    break;
                case LevelType.Merging: 
                    _worldEntity.movingDuration = 0.75f;
                    break;
                case LevelType.Combined: 
                    _worldEntity.movingDuration = 0.35f;
                    break;
            }
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTimeLimit + Random.Range(0f, idleTimeRandLimit)) {
                EnterIdle();
            }
            if (idling) {
                idlingTimer += Time.deltaTime;
            }
            if (idlingTimer >= 0.55f) {
                ExitIdle();
            }

            if (!_worldEntity.StateInfo.characterInMoving && !onMergingStar) {
                //_input = new IntVector(Vector2.zero);
                if (Input.GetKeyDown(_leftKey)) {
                    _input.x -= 1;
                    _direction = Direction.West;
                    _bMove = true;
                    idleTimer = 0f;
                }
                if (Input.GetKeyDown(_rightKey)) {
                    _input.x += 1;
                    _direction = Direction.East;
                    _bMove = true;
                    idleTimer = 0f;
                }
                if (Input.GetKeyDown(_upKey)) {
                    _input.y += 1;
                    _direction = Direction.North;
                    _bMove = true;
                    idleTimer = 0f;
                }
                if (Input.GetKeyDown(_downKey)) {
                    _input.y -= 1;
                    _direction = Direction.South;
                    _bMove = true;
                    idleTimer = 0f;
                }
            }
            if (_worldEntity.IsPushed) {
                Pushed(_worldEntity.pushedDirection);
                //_worldEntity.isPushed = false;
                idleTimer = 0f;
            }
            if (onMergingStar) {
                idleTimer = 0f;
            }
        }
    }

    void EnterIdle () {
        _worldEntity.SetBoolAnimationParameter("Idle", true);
        idleTimer = 0f;
        idling = true;
    }
    void ExitIdle () {
        _worldEntity.SetBoolAnimationParameter("Idle", false);
        idlingTimer = 0f;
        idleTimer = 0f;
        idling = false;
    }
    private void Pushed (Direction direction) {
        //play pushed animation
        SetSpriteByDistance(false);
        ExitIdle();
        switch (direction) {
            case Direction.North:
                _worldEntity.SetBoolAnimationParameter("PushedUp", true);
                isPushedUp = true;
                _worldEntity.SetOrderLayer(8);
                break;
            case Direction.South:
                _worldEntity.SetBoolAnimationParameter("PushedDown", true);
                isPushedDown = true;
                _worldEntity.SetOrderLayer(12);
                break;
            case Direction.West:
                _worldEntity.SetBoolAnimationParameter("PushedLeft", true);
                break;
            case Direction.East:
                _worldEntity.SetBoolAnimationParameter("PushedRight", true);
                break;
        }

        
    }
    private void Simulate () {
        if (_bMove) {
            characterInMoving = true;
            SetSpriteByDistance(false);
            ExitIdle();
            Events.g.Raise(new MoveInputEvent(_iCharacterID, _direction));
            switch (WorldManager.g.CanMove(_worldEntity.Location, _direction, _worldEntity)) {
                case MoveResult.Move:
                    Move();
                    break;
                case MoveResult.Stuck:
                    Stuck();
                    break;
                case MoveResult.Push:
                    Push();
                    break;
            }
            
            _bMove = false;
        }       
    }
    private void Push () {
        //play push animation

        AudioSource.PlayClipAtPoint(_audioPush, CameraControl.cameraLoc, LevelCode.audioVolume);

        switch (_direction) {
            case Direction.North:
                _worldEntity.SetBoolAnimationParameter("PushUp", true);
                break;
            case Direction.South:
                _worldEntity.SetBoolAnimationParameter("PushDown", true);
                break;
            case Direction.West:
                _worldEntity.SetBoolAnimationParameter("PushLeft", true);
                break;
            case Direction.East:
                _worldEntity.SetBoolAnimationParameter("PushRight", true);
                break;
        }

        IntVector vec = _worldEntity.Location;
        if (_input.x != 0) {
            vec.x += _input.x;
            _input.x = 0;
        } else if (_input.y != 0) {
            vec.y += _input.y;
            _input.y = 0;       
        }
        _worldEntity.Location = vec;
    }
    public void Stuck () {
        SetSpriteByDistance(false);
        ExitIdle();
        
        //play stuck animation
        switch (_worldEntity.stuckType) {
            case StuckType.MoveStuck:
                switch (_direction) {
                    case Direction.North:
                        _worldEntity.SetBoolAnimationParameter("StuckUp1", true);
                        break;
                    case Direction.South:
                        _worldEntity.SetBoolAnimationParameter("StuckDown1", true);
                        break;
                    case Direction.West:
                        _worldEntity.SetBoolAnimationParameter("StuckLeft1", true);
                        break;
                    case Direction.East:
                        _worldEntity.SetBoolAnimationParameter("StuckRight1", true);
                        break;
                }
                break;
            case StuckType.PushStuck:
                switch (_direction) {
                    case Direction.North:
                        _worldEntity.SetBoolAnimationParameter("PushStuckUp", true);
                        break;
                    case Direction.South:
                        _worldEntity.SetBoolAnimationParameter("PushStuckDown", true);
                        break;
                    case Direction.West:
                        _worldEntity.SetBoolAnimationParameter("PushStuckLeft", true);
                        break;
                    case Direction.East:
                        _worldEntity.SetBoolAnimationParameter("PushStuckRight", true);
                        break;
                }
                break;
            case StuckType.PushedStuck:
                switch (_direction) {
                    case Direction.North:
                        _worldEntity.SetBoolAnimationParameter("PushedStuckUp", true);
                        break;
                    case Direction.South:
                        _worldEntity.SetBoolAnimationParameter("PushedStuckDown", true);
                        break;
                    case Direction.West:
                        _worldEntity.SetBoolAnimationParameter("PushedStuckLeft", true);
                        break;
                    case Direction.East:
                        _worldEntity.SetBoolAnimationParameter("PushedStuckRight", true);
                        break;
                }
                break;
        }
        
        AudioSource.PlayClipAtPoint(_audioStuck, CameraControl.cameraLoc, LevelCode.audioVolume);

        _input.x = 0;
        _input.y = 0; 
        isStuck = true;
        
    }
    private void Move () {
        //play move animaition

        AudioSource.PlayClipAtPoint(_audioMove, CameraControl.cameraLoc, LevelCode.audioVolume);
        if (LevelCode.levelType == LevelType.Separation || LevelCode.levelType == LevelType.Merging) {
            switch (_direction) {
                case Direction.North:
                    _worldEntity.SetBoolAnimationParameter("MoveUpSlow", true);
                    break;
                case Direction.South:
                    _worldEntity.SetBoolAnimationParameter("MoveDownSlow", true);
                    break;
                case Direction.West:
                    _worldEntity.SetBoolAnimationParameter("MoveLeftSlow", true);
                    break;
                case Direction.East:
                    _worldEntity.SetBoolAnimationParameter("MoveRightSlow", true);
                    break;
            }
        } else {
            switch (_direction) {
                case Direction.North:
                    _worldEntity.SetBoolAnimationParameter("MoveUp", true);
                    break;
                case Direction.South:
                    _worldEntity.SetBoolAnimationParameter("MoveDown", true);
                    break;
                case Direction.West:
                    _worldEntity.SetBoolAnimationParameter("MoveLeft", true);
                    break;
                case Direction.East:
                    _worldEntity.SetBoolAnimationParameter("MoveRight", true);
                    break;
            }
        }

        IntVector vec = _worldEntity.Location;
        if (_input.x != 0) {
            vec.x += _input.x;
            _input.x = 0;
        } else if (_input.y != 0) {
            vec.y += _input.y;
            _input.y = 0;            
        }
        _worldEntity.Location = vec;
    }

    void StepOnMergingStar (MergingStarEvent e) {
        if (e.CharacterID == _iCharacterID) {

            if (e.isEntered) {
                onMergingStar = true;
                _worldEntity.CollidingType = EntityCollidingType.Empty;
                StartCoroutine(SetSpriteForMerging());
                SetLayer(isInMerging: true);
            } 
            // else {
            //     onMergingStar = false;
            //     _worldEntity.CollidingType = EntityCollidingType.Pushable;
            //     SetSprite(isInMerging: false);
            //     SetLayer(isInMerging: false);
            //     oneEnteredMergingStar = false;
            // }
        }
    }

    void SetLayer (bool isInMerging) {
        if (!isInMerging || oneEnteredMergingStar) {
            
            _worldEntity.SetOrderLayer(10);
        } else {
            _worldEntity.SetOrderLayer(9);
        }
    }
    IEnumerator SetSpriteForMerging () {
        yield return new WaitForSeconds(0.7f);
        if (oneEnteredMergingStar) {
            _worldEntity.SetBoolAnimationParameter("Merging2", true);
            _worldEntity.SetBoolAnimationParameter("Merging", true);
        } else {
            _worldEntity.SetBoolAnimationParameter("Merging2", false);
            _worldEntity.SetBoolAnimationParameter("Merging", true);

            oneEnteredMergingStar = true;

        }
        
    }
    public void SetSpriteByDistance (bool showEmotion) {
        if (showEmotion) {
            if (WorldManager.g.fCharacterDistance > 5.5f || LevelCode.levelType == LevelType.Separation || LevelCode.levelType == LevelType.Merging) {
                _worldEntity.SetBoolAnimationParameter("Sad", true);
                _worldEntity.SetBoolAnimationParameter("MidSad", false);
            } else if (WorldManager.g.fCharacterDistance >= 2.1f && WorldManager.g.fCharacterDistance < 5.5f) {
                _worldEntity.SetBoolAnimationParameter("MidSad", true);
                _worldEntity.SetBoolAnimationParameter("Sad", false);
            } else {
                _worldEntity.SetBoolAnimationParameter("Sad", false);
                _worldEntity.SetBoolAnimationParameter("MidSad", false);
            }
        } else {
            _worldEntity.SetBoolAnimationParameter("Sad", false);
            _worldEntity.SetBoolAnimationParameter("MidSad", false);
        }
    }
    
}