using UnityEngine;
using System.Collections;

public class Pusher : MonoBehaviour
{
    //set in the XML
    public int iID;
    //to associate with the trigger
    public bool isControlled;
    public Direction direction;
    public int iRange;
    public float fTimeInterval = 0.5f;

    public bool isTriggered;

    private WorldEntity _worldEntity;
    private bool _isForward = true;
    private bool _needMove;
    private float _fTimeBetweenMoves;
    private int _iStep;

    [SerializeField]
    private Sprite _spriteVertical1;
    [SerializeField]
    private Sprite _spriteVertical1Contract;
    [SerializeField]
    private Sprite _spriteVertical1Stretch;
    [SerializeField]
    private Sprite _spriteHorizontal1;
    [SerializeField]
    private Sprite _spriteHorizontal1Contract;
    [SerializeField]
    private Sprite _spriteHorizontal1Stretch;

    [SerializeField]
    private Sprite _spriteVertical2;
    [SerializeField]
    private Sprite _spriteVertical2Contract;
    [SerializeField]
    private Sprite _spriteVertical2Stretch;
    [SerializeField]
    private Sprite _spriteHorizontal2;
    [SerializeField]
    private Sprite _spriteHorizontal2Contract;
    [SerializeField]
    private Sprite _spriteHorizontal2Stretch;

    [SerializeField]
    private Sprite _spriteVertical3;
    [SerializeField]
    private Sprite _spriteVertical3Contract;
    [SerializeField]
    private Sprite _spriteVertical3Stretch;
    [SerializeField]
    private Sprite _spriteHorizontal3;
    [SerializeField]
    private Sprite _spriteHorizontal3Contract;
    [SerializeField]
    private Sprite _spriteHorizontal3Stretch;

    [SerializeField]
    private Sprite _spriteVertical4;
    [SerializeField]
    private Sprite _spriteVertical4Contract;
    [SerializeField]
    private Sprite _spriteVertical4Stretch;
    [SerializeField]
    private Sprite _spriteHorizontal4;
    [SerializeField]
    private Sprite _spriteHorizontal4Contract;
    [SerializeField]
    private Sprite _spriteHorizontal4Stretch;

    [SerializeField]
    private Sprite _spriteVertical5;
    [SerializeField]
    private Sprite _spriteVertical5Contract;
    [SerializeField]
    private Sprite _spriteVertical5Stretch;
    [SerializeField]
    private Sprite _spriteHorizontal5;
    [SerializeField]
    private Sprite _spriteHorizontal5Contract;
    [SerializeField]
    private Sprite _spriteHorizontal5Stretch;


    [SerializeField]
    private Sprite _spriteVertical6;
    [SerializeField]
    private Sprite _spriteVertical6Contract;
    [SerializeField]
    private Sprite _spriteVertical6Stretch;
    [SerializeField]
    private Sprite _spriteHorizontal6;   
    [SerializeField]
    private Sprite _spriteHorizontal6Contract;
    [SerializeField]
    private Sprite _spriteHorizontal6Stretch;
    

    //     [SerializeField]
    // AudioClip _audioMove;
    [SerializeField]
    AudioClip _audioPush;

    [SerializeField]
    private float _pushingContractDuration=0.3f;
    [SerializeField]
    private float _pushingStrechDuration=0.6f;

    private PushingState _pushingState;

    
    public void Cache()
    {
        _worldEntity = GetComponent<WorldEntity>();
    }

    void Awake()
    {
        Cache();
        _worldEntity.CollidingType = EntityCollidingType.Colliding;
        _worldEntity.entityType = EntityType.Pusher;


    }

    void OnEnable()
    {
        _worldEntity.Simulators += Simulate;
        Events.g.AddListener<StayTriggerEvent>(Triggered);
    }

    void OnDisable()
    {
        _worldEntity.Simulators -= Simulate;
        Events.g.RemoveListener<StayTriggerEvent>(Triggered);

    }
    void Update()
    {
        if (!_worldEntity.isSpriteSet)
        {
            _worldEntity.SetVisual(GetSpriteByID());
            
        }
        if (!_needMove)
        {
            _fTimeBetweenMoves += Time.deltaTime;
            if (_fTimeBetweenMoves >= fTimeInterval)
            {
                _needMove = true;
                _fTimeBetweenMoves = 0f;
            }
        }
        _worldEntity.movingDuration = fTimeInterval;
    }
    private void Simulate()
    {
        if (_needMove)
        {
            if (!isControlled)
            {
                AutoMove();
            }
            else
            {
                TriggeredMove();
            }
        }
    }
    private void AutoMove()
    {
        if (_iStep >= iRange && _isForward)
        {
            _isForward = false;
        }
        if (_iStep <= 0 && !_isForward)
        {
            _isForward = true;
        }
        

        if (_isForward)
        {
            TryMove(direction);
        }
        else
        {
            TryMove(DirectionFlip(direction));
        }
    }
    private void Triggered(StayTriggerEvent e)
    {
        if (e.triggerID == iID)
        {
            isTriggered = e.isEntered;
        }
    }
    private void TriggeredMove()
    {
        if (isTriggered)
        {
            if (_iStep < iRange && _iStep >= 0)
            {
                _isForward = true; 
                TryMove(direction);   
            }                
        }
        else
        {
            if (_iStep <= iRange && _iStep > 0)
            {
                _isForward = false; 
                TryMove(DirectionFlip(direction));   
            }
        }
    }
    private void TryMove(Direction tryDirection)
    {
        switch (WorldManager.g.CanMove(_worldEntity.Location, tryDirection, _worldEntity))
        {
            case MoveResult.Move:
                MoveOneStep(tryDirection);
                //AudioSource.PlayClipAtPoint(_audioMove, _worldEntity.Location.ToVector2(), LevelCode.audioVolume);
                
                    //print("move");
                break;
            case MoveResult.Stuck:
                print("error! pusher stuck!");
                break;
            case MoveResult.Push:
                MoveOneStep(tryDirection);
                AudioSource.PlayClipAtPoint(_audioPush, _worldEntity.Location.ToVector2(), LevelCode.audioVolume);
                _pushingState=PushingState.Contract;
                _worldEntity.ChangeVisual(GetSpriteByID());
                StartCoroutine(PushingContract(_pushingContractDuration));
                StartCoroutine(PushingStretch(_pushingStrechDuration));
                
                    //print("push");
                break;
            default:
                break;
        }
    }
    IEnumerator PushingContract(float waitTime)
    {
        yield return new WaitForSeconds (waitTime);
        //ends
        _pushingState=PushingState.Stretch;
        _worldEntity.ChangeVisual(GetSpriteByID());
    }
    IEnumerator PushingStretch(float waitTime)
    {
        yield return new WaitForSeconds (waitTime);
        //ends
        _pushingState=PushingState.Normal;
        _worldEntity.ChangeVisual(GetSpriteByID());
    }
    private void MoveOneStep(Direction stepDirection)
    {
        IntVector vec = _worldEntity.Location;
        switch (stepDirection)
        {
            case Direction.North:
                vec.y++;
                break;
            case Direction.South:
                vec.y--;
                break;
            case Direction.West:
                vec.x--;
                break;
            case Direction.East:
                vec.x++;
                break;
            default:
                break;
        }
        _worldEntity.Location = vec;
        _needMove = false;
        if (_isForward)
        {
            _iStep++;
        }
        else
        {
            _iStep--;
        }
        
    }
    private Direction DirectionFlip(Direction direction)
    {
        Direction flippedDirection=Direction.North;
        switch (direction)
        {
            case Direction.East:
                flippedDirection=Direction.West;
                break;
            case Direction.West:
                flippedDirection=Direction.East;
                break;
            case Direction.North:
                flippedDirection=Direction.South;
                break;
            case Direction.South:
                flippedDirection=Direction.North;
                break;
        }
        return flippedDirection;
    }
    private Sprite GetSpriteByID()
    {
        Sprite sprite = new Sprite();
        switch (_pushingState)
        {
            case PushingState.Contract:
            switch (direction)
        {
            case Direction.North:
                switch (iID%6)
                {
                    case 1:
                        sprite = _spriteVertical1Contract;
                        break;
                    case 2:
                        sprite = _spriteVertical2Contract;
                        break;
                    case 3:
                        sprite = _spriteVertical3Contract;
                        break;
                    case 4:
                        sprite = _spriteVertical4Contract;
                        break;
                    case 5:
                        sprite = _spriteVertical5Contract;
                        break;
                    case 0:
                        sprite = _spriteVertical6Contract;
                        break;
                }

                break;
            case Direction.South:
                switch (iID%6)
                {
                    case 1:
                        sprite = _spriteVertical1Contract;
                        break;
                    case 2:
                        sprite = _spriteVertical2Contract;
                        break;
                    case 3:
                        sprite = _spriteVertical3Contract;
                        break;
                    case 4:
                        sprite = _spriteVertical4Contract;
                        break;
                    case 5:
                        sprite = _spriteVertical5Contract;
                        break;
                    case 0:
                        sprite = _spriteVertical6Contract;
                        break;

                }
                break;
            case Direction.West:
                switch (iID%6)
                {
                    case 1:
                        sprite = _spriteHorizontal1Contract;
                        break;
                    case 2:
                        sprite = _spriteHorizontal2Contract;
                        break;
                    case 3:
                        sprite = _spriteHorizontal3Contract;
                        break;
                    case 4:
                        sprite = _spriteHorizontal4Contract;
                        break;
                    case 5:
                        sprite = _spriteHorizontal5Contract;
                        break;
                    case 0:
                        sprite = _spriteHorizontal6Contract;
                        break;
                }
                break;
            case Direction.East:
                switch (iID%6)
                {
                    case 1:
                        sprite = _spriteHorizontal1Contract;
                        break;
                    case 2:
                        sprite = _spriteHorizontal2Contract;
                        break;
                    case 3:
                        sprite = _spriteHorizontal3Contract;
                        break;
                    case 4:
                        sprite = _spriteHorizontal4Contract;
                        break;
                    case 5:
                        sprite = _spriteHorizontal5Contract;
                        break;
                    case 0:
                        sprite = _spriteHorizontal6Contract;
                        break;
                }
                break;
        }
        break;

        case PushingState.Normal:
        switch (direction)
        {
            case Direction.North:
                switch (iID%6)
                {
                    case 1:
                        sprite = _spriteVertical1;
                        break;
                    case 2:
                        sprite = _spriteVertical2;
                        break;
                    case 3:
                        sprite = _spriteVertical3;
                        break;
                    case 4:
                        sprite = _spriteVertical4;
                        break;
                    case 5:
                        sprite = _spriteVertical5;
                        break;
                    case 0:
                        sprite = _spriteVertical6;
                        break;
                }

                break;
            case Direction.South:
                switch (iID%6)
                {
                    case 1:
                        sprite = _spriteVertical1;
                        break;
                    case 2:
                        sprite = _spriteVertical2;
                        break;
                    case 3:
                        sprite = _spriteVertical3;
                        break;
                    case 4:
                        sprite = _spriteVertical4;
                        break;
                    case 5:
                        sprite = _spriteVertical5;
                        break;
                    case 0:
                        sprite = _spriteVertical6;
                        break;

                }
                break;
            case Direction.West:
                switch (iID%6)
                {
                    case 1:
                        sprite = _spriteHorizontal1;
                        break;
                    case 2:
                        sprite = _spriteHorizontal2;
                        break;
                    case 3:
                        sprite = _spriteHorizontal3;
                        break;
                    case 4:
                        sprite = _spriteHorizontal4;
                        break;
                    case 5:
                        sprite = _spriteHorizontal5;
                        break;
                    case 0:
                        sprite = _spriteHorizontal6;
                        break;
                }
                break;
            case Direction.East:
                switch (iID%6)
                {
                    case 1:
                        sprite = _spriteHorizontal1;
                        break;
                    case 2:
                        sprite = _spriteHorizontal2;
                        break;
                    case 3:
                        sprite = _spriteHorizontal3;
                        break;
                    case 4:
                        sprite = _spriteHorizontal4;
                        break;
                    case 5:
                        sprite = _spriteHorizontal5;
                        break;
                    case 0:
                        sprite = _spriteHorizontal6;
                        break;
                }
                break;
        }
        break;

        case PushingState.Stretch:
        switch (direction)
        {
            case Direction.North:
                switch (iID%6)
                {
                    case 1:
                        sprite = _spriteVertical1Stretch;
                        break;
                    case 2:
                        sprite = _spriteVertical2Stretch;
                        break;
                    case 3:
                        sprite = _spriteVertical3Stretch;
                        break;
                    case 4:
                        sprite = _spriteVertical4Stretch;
                        break;
                    case 5:
                        sprite = _spriteVertical5Stretch;
                        break;
                    case 0:
                        sprite = _spriteVertical6Stretch;
                        break;
                }

                break;
            case Direction.South:
                switch (iID%6)
                {
                    case 1:
                        sprite = _spriteVertical1Stretch;
                        break;
                    case 2:
                        sprite = _spriteVertical2Stretch;
                        break;
                    case 3:
                        sprite = _spriteVertical3Stretch;
                        break;
                    case 4:
                        sprite = _spriteVertical4Stretch;
                        break;
                    case 5:
                        sprite = _spriteVertical5Stretch;
                        break;
                    case 0:
                        sprite = _spriteVertical6Stretch;
                        break;

                }
                break;
            case Direction.West:
                switch (iID%6)
                {
                    case 1:
                        sprite = _spriteHorizontal1Stretch;
                        break;
                    case 2:
                        sprite = _spriteHorizontal2Stretch;
                        break;
                    case 3:
                        sprite = _spriteHorizontal3Stretch;
                        break;
                    case 4:
                        sprite = _spriteHorizontal4Stretch;
                        break;
                    case 5:
                        sprite = _spriteHorizontal5Stretch;
                        break;
                    case 0:
                        sprite = _spriteHorizontal6Stretch;
                        break;
                }
                break;
            case Direction.East:
                switch (iID%6)
                {
                    case 1:
                        sprite = _spriteHorizontal1Stretch;
                        break;
                    case 2:
                        sprite = _spriteHorizontal2Stretch;
                        break;
                    case 3:
                        sprite = _spriteHorizontal3Stretch;
                        break;
                    case 4:
                        sprite = _spriteHorizontal4Stretch;
                        break;
                    case 5:
                        sprite = _spriteHorizontal5Stretch;
                        break;
                    case 0:
                        sprite = _spriteHorizontal6Stretch;
                        break;
                }
                break;
        }
        break;
    }
    
        if( sprite == null)
        {
            print("sprite unset");
        }
        return sprite;
    }

}
