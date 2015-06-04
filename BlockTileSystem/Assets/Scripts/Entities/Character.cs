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

    private bool onMergingStar;

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
    void RefreshOnLevelLoaded(LevelLoadedEvent e)
    {
        _worldEntity.CollidingType = EntityCollidingType.Pushable;
        onMergingStar=false;
        _worldEntity.DestroyVisual();
        _worldEntity.isSpriteSet=false;
    }
    void OnEnable()
    {
        _worldEntity.Simulators += Simulate;
        Events.g.AddListener<MergingStarEvent>(StepOnMergingStar);
        Events.g.AddListener<LevelLoadedEvent>(RefreshOnLevelLoaded);
    }

    void OnDisable()
    {
        _worldEntity.Simulators -= Simulate;
        Events.g.RemoveListener<MergingStarEvent>(StepOnMergingStar);
        Events.g.RemoveListener<LevelLoadedEvent>(RefreshOnLevelLoaded);
    }


    void Update()
    {
        if (!_worldEntity.isSpriteSet)
        {
            _worldEntity.SetVisual(GetSpriteByID(false));
            _worldEntity.SetOrderLayer(10);

        }

        //print("WorldEntity.StateInformation.inMoving "+WorldEntity.StateInformation.inMoving);
        if (!_worldEntity.StateInfo.characterInMoving
            &&LevelCode.gameState == GameState.InLevel
            && !onMergingStar)
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

    void StepOnMergingStar(MergingStarEvent e)
    {
        if (e.CharacterID == _iCharacterID)
        {

            if (e.isEntered)
            {
                onMergingStar=true;
                _worldEntity.CollidingType = EntityCollidingType.Empty;
                SetSprite(isInMerging: true);
            }
            else
            {
                onMergingStar=false;
                _worldEntity.CollidingType = EntityCollidingType.Colliding;
                SetSprite(isInMerging: false);
            }
        }
    }
    void SetSprite(bool isInMerging)
    {
        _worldEntity.ChangeVisual(GetSpriteByID(isInMerging));
        if (!isInMerging)
        {
            
            _worldEntity.SetOrderLayer(10);
        }
        else
        {
            _worldEntity.SetOrderLayer(9);
        }

    }
    private Sprite GetSpriteByID(bool isInMerging)
    {
        Sprite sprite = new Sprite();
        if (!isInMerging)
        {
            switch (_iCharacterID)
            {
                case 1:
                    sprite = _char1YellowSprite;
                    break;
                case 2:
                    sprite = _char2GreenSprite;
                    break;
                case 3:
                    sprite = _char3CombinedSprite;
                    break;
            }
        }
        else
        {
            switch (_iCharacterID)
            {
                case 1:
                    sprite = _char1YellowSprite;
                    break;
                case 2:
                    sprite = _char2GreenSprite;
                    break;
                case 3:
                    sprite = _char3CombinedSprite;
                    break;
            }
        }
        if( sprite == null)
        {
            print("sprite unset");
        }
        return sprite;
    }
}