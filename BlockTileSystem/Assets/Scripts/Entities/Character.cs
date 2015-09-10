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
    private bool _bMove;

    [SerializeField]
    AudioClip _audioMove;
    [SerializeField]
    AudioClip _audioPush;
    [SerializeField]
    AudioClip _audioStuck;

    [SerializeField]
    int _iCharacterID;

    [SerializeField]
    private Sprite _char1YellowSprite;
    [SerializeField]
    private Sprite _char2GreenSprite;
    [SerializeField]
    private Sprite _char3CombinedSprite;
    [SerializeField]
    private Sprite _char1MergingYellowSprite;
    [SerializeField]
    private Sprite _char2MergingGreenSprite;

    [SerializeField]
    private Sprite _char1Face1Sprite;
    [SerializeField]
    private Sprite _char1Face2Sprite;
    [SerializeField]
    private Sprite _char1Face3Sprite;
    [SerializeField]
    private Sprite _char2Face1Sprite;
    [SerializeField]
    private Sprite _char2Face2Sprite;
    [SerializeField]
    private Sprite _char2Face3Sprite;

    public static bool oneEnteredMergingStar;

    private bool onMergingStar;

    public void Cache () {
        _worldEntity = GetComponent<WorldEntity>();
    }

    void Awake () {
        Cache();
        _worldEntity.CollidingType = EntityCollidingType.Pushable;
        _worldEntity.entityType = EntityType.Character;
        _worldEntity.characterID = _iCharacterID;
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
            _worldEntity.SetVisual(GetSpriteByID(false));
            _worldEntity.SetOrderLayer(10);

        } else if (!onMergingStar) {
            SetSprite(false);
        }
        
        if (LevelCode.gameState == GameState.InLevel) {
            switch (LevelCode.levelType) {
                case LevelType.Normal: 
                    _worldEntity.movingDuration = 0.35f;
                    break;
                case LevelType.Separation: 
                    _worldEntity.movingDuration = 0.5f;
                    break;
                case LevelType.Merging: 
                    _worldEntity.movingDuration = 0.5f;
                    break;
                case LevelType.Combined: 
                    _worldEntity.movingDuration = 0.25f;
                    break;
            }
        }
        if (!_worldEntity.StateInfo.characterInMoving
            && LevelCode.gameState == GameState.InLevel
            && !onMergingStar) {
            //_input = new IntVector(Vector2.zero);
            if (Input.GetKeyDown(_leftKey)) {
                _input.x -= 1;
                _direction = Direction.West;
                _bMove = true;
            }
            if (Input.GetKeyDown(_rightKey)) {
                _input.x += 1;
                _direction = Direction.East;
                _bMove = true;
                _worldEntity.SetBoolAnimationParameter("MoveRight", true);
                //Invoke("StopAnimation", 0.35f);
            }
            if (Input.GetKeyDown(_upKey)) {
                _input.y += 1;
                _direction = Direction.North;
                _bMove = true;
            }
            if (Input.GetKeyDown(_downKey)) {
                _input.y -= 1;
                _direction = Direction.South;
                _bMove = true;
            }
        }
        if (_worldEntity.isPushed) {
            Pushed(_worldEntity.pushedDirection);
        }
    }
    void StopAnimation()
    {
        _worldEntity.SetBoolAnimationParameter("MoveRight", false);
    }
    private void Pushed (Direction direction) {
        //play pushed animation
    }
    private void Simulate () {
        if (_bMove) {
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

        AudioSource.PlayClipAtPoint(_audioPush, _worldEntity.Location.ToVector2(), LevelCode.audioVolume);

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
    private void Stuck () {
        //play stuck animation

        AudioSource.PlayClipAtPoint(_audioStuck, _worldEntity.Location.ToVector2(), LevelCode.audioVolume);

        _input.x = 0;
        _input.y = 0; 
        
    }
    private void Move () {
        //play move animaition

        AudioSource.PlayClipAtPoint(_audioMove, _worldEntity.Location.ToVector2(), LevelCode.audioVolume);

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
                SetSprite(isInMerging: true);
                oneEnteredMergingStar = true;
            } else {
                onMergingStar = false;
                _worldEntity.CollidingType = EntityCollidingType.Pushable;
                SetSprite(isInMerging: false);
                oneEnteredMergingStar = false;
            }
        }
    }
    void SetSprite (bool isInMerging) {
        _worldEntity.ChangeVisual(GetSpriteByID(isInMerging));
        if (!isInMerging || oneEnteredMergingStar) {
            
            _worldEntity.SetOrderLayer(10);
        } else {
            _worldEntity.SetOrderLayer(9);
        }

    }
    private Sprite GetSpriteByID (bool isInMerging) {
        Sprite sprite = new Sprite();
        if (!isInMerging) {
            sprite = GetSpriteByDistance();
        } else {//on merging star
            print("on merging star");
            if (oneEnteredMergingStar) {
                sprite = _char3CombinedSprite;
                print("one entered");
            } else {
                print("first entered");
                switch (_iCharacterID) {
                    case 1:
                        sprite = _char1MergingYellowSprite;
                        break;
                    case 2:
                        sprite = _char2MergingGreenSprite;
                        break;
                }
            }
        }
        if (sprite == null) {
            print("sprite unset");
        }
        return sprite;
    }
    private Sprite GetSpriteByDistance () {
        Sprite sprite = new Sprite();
        switch (_iCharacterID) {
            case 1:
                if (WorldManager.g.fCharacterDistance > 3.1f || LevelCode.levelType == LevelType.Separation || LevelCode.levelType == LevelType.Merging) {
                    sprite = _char1Face1Sprite;//sad
                } else if (WorldManager.g.fCharacterDistance >= 1.1f && WorldManager.g.fCharacterDistance < 2.1f) {
                    sprite = _char1Face3Sprite;
                } else if (WorldManager.g.fCharacterDistance >= 2.1f && WorldManager.g.fCharacterDistance < 3.1f) {
                    sprite = _char1Face2Sprite;
                } else {
                    sprite = _char1YellowSprite;
                }
                break;
            case 2:
                if (WorldManager.g.fCharacterDistance > 3.1f || LevelCode.levelType == LevelType.Separation || LevelCode.levelType == LevelType.Merging) {
                    sprite = _char2Face1Sprite;//sad
                } else if (WorldManager.g.fCharacterDistance >= 1.1f && WorldManager.g.fCharacterDistance < 2.1f) {
                    sprite = _char2Face3Sprite;
                } else if (WorldManager.g.fCharacterDistance >= 2.1f && WorldManager.g.fCharacterDistance < 3.1f) {
                    sprite = _char2Face2Sprite;
                } else {
                    sprite = _char2GreenSprite;
                }
                break;
            case 3:
                sprite = _char3CombinedSprite;
                break;
        }
        return sprite;
            
    }
}