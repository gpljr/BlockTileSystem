using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

    public Direction direction;

    [SerializeField]
    AudioClip _audio;

    //for all the bullets
    [SerializeField]
    private float _fBulletMoveInterval = 0.5f;
    //time between moves
    
    // [SerializeField]
    // private int _iRange;

    private bool _needMove;
    private float _fTimeBetweenMoves;
    private int _iStep;

    [SerializeField]
    private Sprite _spriteUp;
    [SerializeField]
    private Sprite _spriteDown;
    [SerializeField]
    private Sprite _spriteLeft;
    [SerializeField]
    private Sprite _spriteRight;
    
    private WorldTrigger _worldTrigger;
    private WorldEntity _worldEntity;
    public void Cache()
    {
        _worldTrigger = GetComponent<WorldTrigger>();
        _worldEntity = GetComponent<WorldEntity>();
    }
    void Awake()
    {
        Cache();
        _worldTrigger.triggerType = TriggerType.Bullet;
        _worldEntity.CollidingType = EntityCollidingType.Empty;
        _worldEntity.entityType = EntityType.Bullet;

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
        
        if (!_worldEntity.isSpriteSet && _worldEntity.Location != new IntVector(0, 0))
        {
            _worldEntity.SetVisual(GetSpriteByDirection());
            _worldEntity.SetOrderLayer(11);
        }
        _worldEntity.movingDuration = _fBulletMoveInterval;
        if (!_needMove)
        {
            _fTimeBetweenMoves += Time.deltaTime;
            if (_fTimeBetweenMoves >= _fBulletMoveInterval)
            {
                _needMove = true;
                _fTimeBetweenMoves = 0f;
            }
        }
        if (!_worldTrigger.isMessageSent)
        {
            if (_worldTrigger.isSteppedOn)
            {
                BulletHitAnything();
                _worldTrigger.isMessageSent = true;                
            }            
        }
    }
    void BulletHitAnything()
    {
        if(_worldTrigger.steppingEntityType == EntityType.Character)
        {
            AudioSource.PlayClipAtPoint(_audio, _worldEntity.Location.ToVector2(), LevelCode.audioVolume);
            Events.g.Raise(new BulletHitEvent());//bullet kill
        }
        if(_worldTrigger.steppingEntityType != EntityType.Shooter && _worldTrigger.steppingEntityType != EntityType.Bullet)
        BulletDestroySelf();
        
    }
    private void BulletDestroySelf()
    {
        _worldEntity.DeregisterMe();
        _worldTrigger.DeregisterMe();
        Destroy(gameObject);
    }
    private void Simulate()
    {
        
        if (_needMove)
        {
            MoveOneStep(direction);            
        }
    }
    private void MoveOneStep(Direction stepDirection)
    {
        _iStep++;
        // if (_iStep >= _iRange)
        // {
        //     BulletDestroySelf();
        // }
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
        _worldTrigger.Location = vec;
        _needMove = false;
        
    }
    private Sprite GetSpriteByDirection()
    {
        Sprite sprite = new Sprite();

        switch (direction)
        {
            case Direction.North:
                sprite = _spriteUp;
                break;
            case Direction.South:
                sprite = _spriteDown;
                break;
            case Direction.West:
                sprite = _spriteLeft;
                break;
            case Direction.East:
                sprite = _spriteRight;
                break;
        }
        
        return sprite;
    }
}
