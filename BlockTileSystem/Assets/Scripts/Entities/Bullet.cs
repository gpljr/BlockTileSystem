using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

    public Direction direction;

    //for all the bullets
    [SerializeField]
    private float _fBulletMoveInterval = 0.5f;//time between moves
    [SerializeField]
    private int _iRange = 5;

    private bool _needMove;
    private float _fTimeBetweenMoves;
    private int _iStep;

    [SerializeField]
    Texture _bulletTexture;
    
    private WorldTrigger _worldTrigger;
    public void Cache()
    {
        _worldTrigger = GetComponent<WorldTrigger>();
    }
    void Awake()
    {
        Cache();
        _worldTrigger.triggerType = TriggerType.Bullet;
    }
    void LateUpdate()
    {

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
                (_worldTrigger.steppingEntityType == EntityType.Character1 ||
                _worldTrigger.steppingEntityType == EntityType.Character2))
            {
                BulletHit();
                _worldTrigger.isMessageSent = true;                
            }            
        }
    }
    void BulletHit()
    {
        //texture change, sound, raise event
        BulletDestroySelf();
        print("bullet hit");
        
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
            if (_iStep >= _iRange)
            {
                BulletDestroySelf();
            }
        }
    }
    private void MoveOneStep(Direction stepDirection)
    {
        IntVector vec = _worldTrigger.Location;
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
        _worldTrigger.Location = vec;
        _needMove = false;
        _iStep++;
    }
}
