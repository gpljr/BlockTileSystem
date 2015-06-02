using UnityEngine;
using System.Collections;
using InControl;


public class Character : MonoBehaviour
{

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
    int _iCharacterID;

    [SerializeField]
    private Sprite _char1YellowSprite;
    [SerializeField]
    private Sprite _char2GreenSprite;
    [SerializeField]
    private Sprite _char3CombinedSprite;

    public void Cache()
    {
        _worldEntity = GetComponent<WorldEntity>();
    }

    void Awake()
    {
        Cache();
        _worldEntity.CollidingType = EntityCollidingType.Pushable;
        _worldEntity.entityType = EntityType.Character;
        _worldEntity.characterID = _iCharacterID;


    }

    void OnEnable()
    {
        _worldEntity.Simulators += Simulate;
    }

    void OnDisable()
    {
        _worldEntity.Simulators -= Simulate;
    }


    void Update()
    {
        if (!_worldEntity.isSpriteSet)
        {
            switch (_iCharacterID)
            {
                case 1:
                    _worldEntity.SetVisual(_char1YellowSprite);
                    break;
                case 2:
                    _worldEntity.SetVisual(_char2GreenSprite);
                    break;
                case 3:
                    _worldEntity.SetVisual(_char3CombinedSprite);
                    break;
            }
            _worldEntity.SetOrderLayer(10);
        }

        print("WorldEntity.StateInformation.inMoving "+WorldEntity.StateInformation.inMoving);
        if (!WorldEntity.StateInformation.inMoving)
        {
            //_input = new IntVector(Vector2.zero);
            if (Input.GetKeyDown(_leftKey))
            {
                _input.x -= 1;
                _direction = Direction.West;
                _bMove = true;
            }
            if (Input.GetKeyDown(_rightKey))
            {
                _input.x += 1;
                _direction = Direction.East;
                _bMove = true;
            }
            if (Input.GetKeyDown(_upKey))
            {
                _input.y += 1;
                _direction = Direction.North;
                _bMove = true;
            }
            if (Input.GetKeyDown(_downKey))
            {
                _input.y -= 1;
                _direction = Direction.South;
                _bMove = true;
            }
        }
        if (_worldEntity.isPushed)
        {
            Pushed(_worldEntity.pushedDirection);
        }
    }

    private void Pushed(Direction direction)
    {
        //play pushed animation
    }
    private void Simulate()
    {
        if (_bMove)
        {
            switch (WorldManager.g.CanMove(_worldEntity.Location, _direction, _worldEntity))
            {
                case MoveResult.Move:
                    Move();
                    break;
                case MoveResult.Stuck:
                    Stuck();
                    break;
                case MoveResult.Push:
                    Push();
                    break;
                default:
                    break;
            }
            _bMove = false;
        }       
    }
    private void Push()
    {
        //play push animation
        IntVector vec = _worldEntity.Location;
        if (_input.x != 0)
        {
            vec.x += _input.x;
            _input.x = 0;
        }
        else if (_input.y != 0)
        {
            vec.y += _input.y;
            _input.y = 0;       
        }
        _worldEntity.Location = vec;
    }
    private void Stuck()
    {
        //play stuck animation
        _input.x = 0;
        _input.y = 0; 
        
    }
    private void Move()
    {
        //play move animaition

        IntVector vec = _worldEntity.Location;
        if (_input.x != 0)
        {
            vec.x += _input.x;
            _input.x = 0;
        }
        else if (_input.y != 0)
        {
            vec.y += _input.y;
            _input.y = 0;            
        }
        _worldEntity.Location = vec;
    }
}