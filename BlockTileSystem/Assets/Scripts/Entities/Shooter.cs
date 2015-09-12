using UnityEngine;
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
    private Sprite _spriteUp;
    [SerializeField]
    private Sprite _spriteDown;
    [SerializeField]
    private Sprite _spriteLeft;
    [SerializeField]
    private Sprite _spriteRight;

    [SerializeField]
    private Sprite _spriteUpShooting;
    [SerializeField]
    private Sprite _spriteDownShooting;
    [SerializeField]
    private Sprite _spriteLeftShooting;
    [SerializeField]
    private Sprite _spriteRightShooting;

    private bool _isShooting;

    [SerializeField]
    private float _shootingDuration;//the lasting time for the shooter animation
    [SerializeField]
    private float _beforeShootingDuration;//time before playing shooting animation/sound on the shooter
    
    [SerializeField]
    AudioClip _audioShoot;

    private bool _bPlayer1Entered,_bPlayer2Entered;

   
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
        Events.g.AddListener<LevelStarEvent>(OnLevelStar);
    }

    void OnDisable()
    {
        _worldEntity.Simulators -= Simulate;
        Events.g.RemoveListener<LevelStarEvent>(OnLevelStar);
    }
    void OnLevelStar(LevelStarEvent e)
    {
        if (e.isEntered)
        {
            switch (e.CharacterID)
            {
                case 1:
                    _bPlayer1Entered = true;
                    break;
                case 2:
                    _bPlayer2Entered = true;
                    break;
                case 3:
                    _bPlayer1Entered = true;
                    _bPlayer2Entered = true;
                    break;
            }
        }
        else
        {
            switch (e.CharacterID)
            {
                case 1:
                    _bPlayer1Entered = false;
                    break;
                case 2:
                    _bPlayer2Entered = false;
                    break;
                case 3:
                    _bPlayer1Entered = false;
                    _bPlayer2Entered = false;
                    break;
            }
        }
    }
    void Update()
    {
        if (!_worldEntity.isSpriteSet)
        {
            _worldEntity.SetVisual(GetSpriteByID());
        }
        _worldEntity.movingDuration = fMovingTimeInterval;
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
        bulletEntity.instantMove=true;
        bulletEntity.Location = _worldEntity.Location;
        var bulletTrigger = bulletObject.GetComponent<WorldEntity>();
        bulletTrigger.Location = _worldEntity.Location;
        bullet.direction = shootingDirection;
        _needShoot = false;


        StartCoroutine(WaitBeforeShooting(_beforeShootingDuration));

        _worldEntity.ChangeVisual(GetSpriteByID());
        StartCoroutine(WaitForShooting(_shootingDuration));


    }
    private void PlayShootSound()
    {
        float soundDistanceRange;
    switch (LevelCode.levelType)
    {
        case LevelType.Normal:
        if(WorldManager.g.fCharacterDistance<2.2f)
        {
            soundDistanceRange=8f;
            PlayShootSoundByDistance(soundDistanceRange);
        }else if (WorldManager.g.fCharacterDistance>=2.2f && WorldManager.g.fCharacterDistance<4f)
        {
            soundDistanceRange=5.5f;
            PlayShootSoundByDistance(soundDistanceRange);
        }else
        {
            soundDistanceRange=4.8f;
            PlayShootSoundByDistance(soundDistanceRange);
        }
        break;

        case LevelType.Separation:
            soundDistanceRange=4.8f;
            PlayShootSoundByDistance(soundDistanceRange);
        break;

        case LevelType.Merging:
            soundDistanceRange=4.8f;
            PlayShootSoundByDistance(soundDistanceRange);
        break;

        case LevelType.Combined:
            soundDistanceRange=10f;
            float volume=LevelCode.audioVolume;
        Vector2 currentLocation = _worldEntity.Location.ToVector2();
        float distanceCombined=Vector2.Distance(WorldManager.g.charCombinedEntity.Location.ToVector2(),currentLocation);
        
            if(distanceCombined<soundDistanceRange)
            {
                volume*=Mathf.Pow((1.0f-distanceCombined/soundDistanceRange),2);
                AudioSource.PlayClipAtPoint(_audioShoot, currentLocation, volume);
            }
        break;
    }
}
    private void PlayShootSoundByDistance(float soundDistanceRange)
    {
        float volume=LevelCode.audioVolume;
        Vector2 currentLocation = _worldEntity.Location.ToVector2();
        float distance1=Vector2.Distance(WorldManager.g.char1Entity.Location.ToVector2(),currentLocation);
        float distance2=Vector2.Distance(WorldManager.g.char2Entity.Location.ToVector2(),currentLocation);
        
        if(_bPlayer1Entered)//dont play when a character is on the star
        {
            if(distance2<soundDistanceRange )
            {
                volume*=Mathf.Pow(1.0f-distance2/soundDistanceRange,2);
                AudioSource.PlayClipAtPoint(_audioShoot, currentLocation, volume);
            }
        }else if(_bPlayer2Entered)
        {
            if(distance1<soundDistanceRange)
            {
                volume*=Mathf.Pow(1.0f-distance1/soundDistanceRange,2);
                AudioSource.PlayClipAtPoint(_audioShoot, currentLocation, volume);
            }
        }else
        {
            if(Mathf.Min(distance1,distance2)<soundDistanceRange )
            {
                volume*=Mathf.Pow(1.0f-Mathf.Min(distance1,distance2)/soundDistanceRange,2);
                AudioSource.PlayClipAtPoint(_audioShoot, currentLocation, volume);
            }
        }
    }
    IEnumerator WaitBeforeShooting(float waitTime)
    {
        yield return new WaitForSeconds (waitTime);
        _isShooting=true;
        PlayShootSound();
        _worldEntity.ChangeVisual(GetSpriteByID());
    }
    
    IEnumerator WaitForShooting(float waitTime)
    {
        yield return new WaitForSeconds (waitTime);
        _isShooting=false;
        _worldEntity.ChangeVisual(GetSpriteByID());
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
                print("error! shooter stuck!");
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
        if(_isShooting)
        {
            switch (shootingDirection)
        {
            case Direction.North:
                sprite = _spriteUpShooting;
                break;
            case Direction.South:
                sprite = _spriteDownShooting;
                break;
            case Direction.West:
                sprite = _spriteLeftShooting;
                break;
            case Direction.East:
                sprite = _spriteRightShooting;
                break;
        }
    }
        else
        {
        switch (shootingDirection)
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
    }
        if( sprite == null)
        {
            print("sprite unset");
        }
        return sprite;
    }
}
