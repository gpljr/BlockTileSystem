﻿using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour
{

    //set in the XML
    public float fShootingTimeInterval;
    public Direction shootingDirection;

    public bool isMoving;
    public Direction movingDirection;
    public int iRange;
    public float fMovingTimeInterval;

    private WorldEntity _worldEntity;

    private bool _needShoot;
    private float _fTimeBetweenShots;

    private bool _isForward = true;
    private bool _needMove;
    private float _fTimeBetweenMoves;
    private int _iStep;
    [SerializeField]
    private GameObject _bullet;

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
        _worldEntity.entityType = EntityType.Shooter;

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
            _worldEntity.SetVisual(_sprite);
        }
        
        if (!_needMove)
        {
            _fTimeBetweenMoves += Time.deltaTime;
            if (_fTimeBetweenMoves >= fMovingTimeInterval)
            {
                _needMove = true;
                _fTimeBetweenMoves = 0f;
            }
        }
        if (!_needShoot)
        {
            _fTimeBetweenShots += Time.deltaTime;
            if (_fTimeBetweenShots >= fShootingTimeInterval)
            {
                _needShoot = true;
                _fTimeBetweenShots = 0f;
            }
        }
    }
    private void Simulate()
    {
        if (_needShoot)
        {
            Shoot();
        }
        if (_needMove && isMoving)
        {
            AutoMove();
        }
    }
    private void Shoot()
    {
        GameObject bulletObject = (GameObject)Instantiate(_bullet);
        var bullet = bulletObject.GetComponent<Bullet>();
        var bulletEntity = bulletObject.GetComponent<WorldEntity>();
        bulletEntity.Location = _worldEntity.Location;
        bullet.direction = shootingDirection;
        _needShoot = false;
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
            TryMove(movingDirection);
        }
        else
        {
            TryMove(DirectionFlip(movingDirection));
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
