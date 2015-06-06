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
    private Sprite _spriteHorizontal1;
    [SerializeField]
    private Sprite _spriteVertical2;
    [SerializeField]
    private Sprite _spriteHorizontal2;
    [SerializeField]
    private Sprite _spriteVertical3;
    [SerializeField]
    private Sprite _spriteHorizontal3;

    
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
        _worldEntity.movingDuration = _fTimeBetweenMoves;
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
                    //print("move");
                break;
            case MoveResult.Stuck:
                print("error! pusher stuck!");
                break;
            case MoveResult.Push:
                MoveOneStep(tryDirection);
                    //print("push");
                break;
            default:
                break;
        }
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

        switch (direction)
        {
            case Direction.North:
                switch (iID%3)
                {
                    case 1:
                        sprite = _spriteVertical1;
                        break;
                    case 2:
                        sprite = _spriteVertical2;
                        break;
                    case 0:
                        sprite = _spriteVertical3;
                        break;
                }

                break;
            case Direction.South:
                switch (iID%3)
                {
                    case 1:
                        sprite = _spriteVertical1;
                        break;
                    case 2:
                        sprite = _spriteVertical2;
                        break;
                    case 0:
                        sprite = _spriteVertical3;
                        break;
                }
                break;
            case Direction.West:
                switch (iID%3)
                {
                    case 1:
                        sprite = _spriteHorizontal1;
                        break;
                    case 2:
                        sprite = _spriteHorizontal2;
                        break;
                    case 0:
                        sprite = _spriteHorizontal3;
                        break;
                }
                break;
            case Direction.East:
                switch (iID%3)
                {
                    case 1:
                        sprite = _spriteHorizontal1;
                        break;
                    case 2:
                        sprite = _spriteHorizontal2;
                        break;
                    case 0:
                        sprite = _spriteHorizontal3;
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
