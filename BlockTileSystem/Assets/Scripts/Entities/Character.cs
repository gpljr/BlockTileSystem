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

    [SerializeField]
    Texture _character1Texture;

    private WorldEntity _worldEntity;
    private IntVector _input;

    private Direction _direction;
    private bool _bMove;

    [SerializeField]
    int _iCharacterID;


    [SerializeField]
    private GameObject _visualPrefab;

    private Transform _visuals;

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
    }
    private StateInformation _currStateInfo;
    public StateInformation StateInfo
    {
        get { return _currStateInfo; }
    }
    private IntVector _location;

    //[SerializeField]
    //Direction _facing;

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
        _location = _worldEntity.Location;
        _currStateInfo.lastLoc = _location;
        _visuals = Instantiate(_visualPrefab).transform;

    }

    void OnEnable()
    {
        _worldEntity.Simulators += Simulate;
    }

    void OnDisable()
    {
        _worldEntity.Simulators -= Simulate;
        Destroy(_visuals.gameObject);
    }


    void Update()
    {
        _location = _worldEntity.Location;

        Vector2 v = Vector2.zero;
        Vector2 visualOffset = (_location.ToVector2() - _currStateInfo.lastLoc.ToVector2())
                               * (_currStateInfo.fractionComplete);
        Vector2 fixedOffset = new Vector2(0.5f, -0.5f);
        v = _currStateInfo.lastLoc.ToVector2() + visualOffset + fixedOffset;
        _visuals.position = v * WorldManager.g.TileSize;
        float speed = 1f;

        _currStateInfo.fractionComplete += speed * Time.deltaTime;
        if (_currStateInfo.fractionComplete >= 1f)
        {
            _currStateInfo.lastLoc = _location;
            _currStateInfo.fractionComplete = 0f;
        }

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
    // void OnDrawGizmos()
    // {
    //     if (_worldEntity != null)
    //     {
    //         IntVector l = _worldEntity.Location;
    //         //Rect rect = new Rect(l.ToVector2().x * _tileSize, l.ToVector2().y * _tileSize, _tileSize, _tileSize);
    //         Rect rect = new Rect(l.ToVector2().x, l.ToVector2().y, 1, 1);
                
    //         Gizmos.DrawGUITexture(rect, _character1Texture);
    //     }
    // }
    // void OnGUI()
    // {
    //     IntVector l = _worldEntity.Location;
    //     Rect rect = new Rect(l.ToVector2().x, l.ToVector2().y, 1, 1);
    //     //GUI.depth=-1;
    //     GUI.DrawTexture(rect, _character1Texture);
        
    // }
}