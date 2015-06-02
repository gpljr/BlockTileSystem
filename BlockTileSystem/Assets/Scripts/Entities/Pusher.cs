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
    private Sprite _sprite;
    
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
            _worldEntity.SetVisual(_sprite);
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
        switch (direction)
        {
            case Direction.East:
                return Direction.West;
                break;
            case Direction.West:
                return Direction.East;
                break;
            case Direction.North:
                return Direction.South;
                break;
            case Direction.South:
                return Direction.North;
                break;
            default:
                return Direction.North;//error
                break;
        }
    }

}
