using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

    public Direction direction;

    [SerializeField]
    AudioClip audio;

    //for all the bullets
    [SerializeField]
    private float _fBulletMoveInterval = 0.5f;
    //time between moves
    [SerializeField]
    private int _iRange;

    private bool _needMove;
    private float _fTimeBetweenMoves;
    private int _iStep;

    [SerializeField]
    private Sprite _spriteVertical;
    [SerializeField]
    private Sprite _spriteHorizontal;
    
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
    void LateUpdate()
    {
        
        if (!_worldEntity.isSpriteSet && _worldEntity.Location != new IntVector(0, 0))
        {
            _worldEntity.SetVisual(GetSpriteByDirection());
            _worldEntity.SetOrderLayer(11);
        }
        _worldEntity.movingDuration = _fTimeBetweenMoves;
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
            if (_worldTrigger.isSteppedOn &&
                (_worldTrigger.steppingEntityType == EntityType.Character))
            {
                BulletHit();
                _worldTrigger.isMessageSent = true;                
            }            
        }
    }
    void BulletHit()
    {
        AudioSource.PlayClipAtPoint(audio, _worldEntity.Location.ToVector2(), LevelCode.audioVolume);
        BulletDestroySelf();
        Events.g.Raise(new BulletHitEvent());
        
    }
    private void BulletDestroySelf()
    {
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
        if (_iStep >= _iRange)
        {
            BulletDestroySelf();
        }
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
                sprite = _spriteVertical;
                break;
            case Direction.South:
                sprite = _spriteVertical;
                break;
            case Direction.West:
                sprite = _spriteHorizontal;
                break;
            case Direction.East:
                sprite = _spriteHorizontal;
                break;
        }
        
        return sprite;
    }
}
