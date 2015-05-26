using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{

    [SerializeField]
    IntVector _dims;
    public IntVector Dims
    {
        get { return _dims; }
        //set { _dims = value; }
    }
    [SerializeField]
    float _tileSize;
    public float TileSize
    {
        get { return _tileSize; }
        //set { _tileSize = value; }
    }
    TileType[,] _world;
    List<WorldEntity>[,] _entityMap;

    List<WorldEntity> _entities = new List<WorldEntity>();

    List<WorldTrigger>[,] _triggerMap;

    List<WorldTrigger> _triggers = new List<WorldTrigger>();


    [SerializeField]
    GameObject Char1Object;
    [SerializeField]
    GameObject Char2Object;

    [HideInInspector]
    public WorldEntity Char1;
    [HideInInspector]
    public WorldEntity Char2;

    [SerializeField]
    private GameObject _pusherPreFab;
    [SerializeField]
    private GameObject _starPreFab;
    [SerializeField]
    private GameObject _doorPreFab;
    [SerializeField]
    private GameObject _stepTriggerPreFab;
    [SerializeField]
    private GameObject _stayTriggerPreFab;
    [SerializeField]
    private GameObject _shooterPreFab;
    [SerializeField]
    private GameObject _checkPoint1PreFab;
    [SerializeField]
    private GameObject _checkPoint2PreFab;

    // [SerializeField]
    // private GameObject _mainCamera;
    // private Camera _camera;

    
    [SerializeField]
    private Texture _floorTexture;
    [SerializeField]
    private Texture _wallTexture;
    [SerializeField]
    private Texture _character1Texture;
    [SerializeField]
    private Texture _character2Texture;
    [SerializeField]
    private Texture _pusherTexture;
    [SerializeField]
    private Texture _starTexture;
    [SerializeField]
    private Texture _doorTexture;
    [SerializeField]
    private Texture _doorHalfOpenTexture;
    [SerializeField]
    private Texture _stepTriggerTexture;
    [SerializeField]
    private Texture _stayTriggerTexture;
    [SerializeField]
    private Texture _shooterTexture;
    [SerializeField]
    private Texture _bulletTexture;
    [SerializeField]
    private Texture _checkPointTexture;

    private GameObject checkPoint1Object, checkPoint2Object;
    private bool _bPlayer1Entered, _bPlayer2Entered;
    [HideInInspector]
    public IntVector[] CheckPoint1Locations;
    [HideInInspector]
    public IntVector[] CheckPoint2Locations;
    private int iCheckPointLocationID;

    private MapEditor mapEditor;
    public static WorldManager g;

    public void RegisterEntity(WorldEntity e)
    {
        _entities.Add(e);
        IntVector l = e.Location;
        _entityMap[l.x, l.y].Add(e);
    }

    public void DeregisterEntity(WorldEntity e)
    {
        _entities.Remove(e);
        IntVector l = e.Location;
        _entityMap[l.x, l.y].Remove(e);
    }
    public void RegisterTrigger(WorldTrigger t)
    {
        _triggers.Add(t);
        IntVector l = t.Location;
        _triggerMap[l.x, l.y].Add(t);
    }

    public void DeregisterTrigger(WorldTrigger t)
    {
        _triggers.Remove(t);
        IntVector l = t.Location;
        _triggerMap[l.x, l.y].Remove(t);
    }

    void Awake()
    {
        if (g == null)
        {
            g = this;
        }
        else
        {
            Destroy(this);
        }
        _world = new TileType[_dims.x, _dims.y];
        mapEditor = gameObject.GetComponent<MapEditor>();
        Char1 = Char1Object.GetComponent<WorldEntity>();
        Char2 = Char2Object.GetComponent<WorldEntity>();
        //_camera = _mainCamera.GetComponent<Camera>();

        LoadLevel(1);

        // InstantiateDoor(new IntVector(4, 5), ID: 1, triggerNumber: 1);
        // InstantiateStepTrigger(new IntVector(4, 4), ID: 1);
        // InstantiateStayTrigger(new IntVector(9, 5), ID: 1);
    }

    private void LoadLevel(int iLevel)
    {
        ClearMap();        
        mapEditor.LoadFile(iLevel);
        _dims = mapEditor.GetDim();
        _world = new TileType[_dims.x, _dims.y];
        //_mainCamera.transform.position = new Vector3(_dims.x * _tileSize / 2, _dims.y * _tileSize / 2, -12f);
        
        //_mainCamera.GetComponent<Camera>().orthographicSize = Mathf.Min(_dims.x, _dims.y) * _tileSize / 2 + 2;

        _entityMap = new List<WorldEntity>[_dims.x, _dims.y];
        _triggerMap = new List<WorldTrigger>[_dims.x, _dims.y];
        
        for (int x = 0; x < _dims.x; x++)
        {
            for (int y = 0; y < _dims.y; y++)
            {
                _entityMap[x, y] = new List<WorldEntity>();
                _triggerMap[x, y] = new List<WorldTrigger>();
            }
        }
        mapEditor.SetMap();
        mapEditor.SetCharacters();
        mapEditor.SetPushers();
        mapEditor.SetStars();
        mapEditor.SetDoors();
        mapEditor.SetStepTriggers();
        mapEditor.SetStayTriggers();
        mapEditor.SetShooters();
        mapEditor.SetCheckPoints();
        iCheckPointLocationID = 0;
        Events.g.Raise(new LevelLoadedEvent(iLevel));
    }
    private void LoadLevel(LoadLevelEvent e)
    {
        LoadLevel(e.iLevel);
    }
    void OnEnable()
    {
        Events.g.AddListener<LoadLevelEvent>(LoadLevel);
        Events.g.AddListener<CheckPointEvent>(CheckPointPass);
    }
    
    void OnDisable()
    {
        Events.g.RemoveListener<LoadLevelEvent>(LoadLevel);
        Events.g.RemoveListener<CheckPointEvent>(CheckPointPass);
    }

    void FixedUpdate()
    {
        foreach (WorldEntity e in _entities)
        {
            if (e != null)
            {
                IntVector l = e.Location;
                _entityMap[l.x, l.y].Remove(e);
                e.Simulate();
                l = e.Location;
                _entityMap[l.x, l.y].Add(e);
            }
        }
        CheckPointsCheck();
    }

    
    public void GenerateBasicMap(Tile[] tMap)
    {
        for (int i = 0; i < tMap.Length; i++)
        {
            int x = tMap[i].vTilePosition.x;
            int y = tMap[i].vTilePosition.y;
            _world[x, y] = tMap[i].tileType;
            //_world[x, _dims.y-y-1] = tMap[i].tileType;//make levels in excel from top left
        }

    }
    private void ClearMap()
    {
        for (int x = 0; x < _dims.x; x++)
        {
            for (int y = 0; y < _dims.y; y++)
            {
                _world[x, y] = TileType.Empty;
            }

        }
        List<WorldEntity> entitiesToRemove = new List<WorldEntity>();
        foreach (WorldEntity e in _entities)
        {
            if ((e != null) && (e.entityType != EntityType.Character1) && (e.entityType != EntityType.Character2))
            {
                entitiesToRemove.Add(e);
                IntVector l = e.Location;
                _entityMap[l.x, l.y].Remove(e);
                Destroy(e.gameObject);

            }
        }
        foreach (WorldEntity e in entitiesToRemove)
        {
            _entities.Remove(e);
        }

        List<WorldTrigger> triggersToRemove = new List<WorldTrigger>();
        foreach (WorldTrigger t in _triggers)
        {
            if (t != null)
            {
                triggersToRemove.Add(t);
                IntVector l = t.Location;
                _triggerMap[l.x, l.y].Remove(t);
                Destroy(t.gameObject);

            }
        }
        foreach (WorldTrigger e in triggersToRemove)
        {
            _triggers.Remove(e);
        }
    }
    public void SetCharacters(IntVector Char1Pos, IntVector Char2Pos)
    {
        Char1.Location = Char1Pos;
        Char2.Location = Char2Pos;
    }
    private IntVector Destination(IntVector from, Direction direction)
    {
        IntVector destination = from;
        switch (direction)
        {
            case Direction.North:
                destination = new IntVector(from.x, from.y + 1);
                break;
            case Direction.South:
                destination = new IntVector(from.x, from.y - 1);
                break;
            case Direction.West:
                destination = new IntVector(from.x - 1, from.y);
                break;
            case Direction.East:
                destination = new IntVector(from.x + 1, from.y);
                break;
        }
        return destination;
    }

    public MoveResult CanMove(IntVector from, Direction direction, WorldEntity movingEntity)
    {
        IntVector destination = Destination(from, direction);
        int x = destination.x;
        int y = destination.y;
        
        if (_world[x, y] == TileType.Wall)
        {
            return MoveResult.Stuck;
        }

        MoveResult moveResult = MoveResult.Move;
        for (int i = 0; i < _entities.Count; i++)
        {
            if (_entities[i].Location == destination)
            {
                switch (_entities[i].CollidingType)
                {
                    case EntityCollidingType.Empty:
                        moveResult = MoveResult.Move;
                        break;
                    case EntityCollidingType.Colliding:
                        moveResult = MoveResult.Stuck;
                        break;
                    case EntityCollidingType.Pushable:
                        var pushMoveResult = CanMove(destination, direction, _entities[i]);
                        switch (pushMoveResult)
                        {
                            case MoveResult.Move:
                                PushEntity(_entities[i], direction);
                                moveResult = MoveResult.Push;
                                break;
                            case MoveResult.Stuck:
                                moveResult = MoveResult.Stuck;
                                break;
                            case MoveResult.Push:
                                PushEntity(_entities[i], direction);
                                moveResult = MoveResult.Push;
                                break;
                        }
                        break;                      
                }
            }
        }
        if (moveResult == MoveResult.Push || moveResult == MoveResult.Move)
        {
            for (int i = 0; i < _triggers.Count; i++)
            {
                if (_triggers[i].Location == destination)
                {
                    StepOnTriger(_triggers[i], movingEntity);
                }
                if (_triggers[i].Location == from)
                {
                    StepOutTrigger(_triggers[i], movingEntity);
                }
            }
                
        }
        return moveResult;
        
        

    }
    private void StepOnTriger(WorldTrigger steppedTrigger, WorldEntity steppingEntity)
    {
        steppedTrigger.SteppedOn(steppingEntity);
    }
    public void StepOutTrigger(WorldTrigger steppedTrigger, WorldEntity steppingEntity)
    {
        steppedTrigger.SteppedOut(steppingEntity);
    }
    private void PushEntity(WorldEntity entity, Direction direction)
    {
        entity.Location = Destination(entity.Location, direction);
        entity.Pushed(direction);
    }

    public void InstantiatePusher(IntVector location, bool isControlled, Direction direction, 
        int range, int ID, float timeInterval)
    {
        if (_world[location.x, location.y] == TileType.Wall)
        {
            print("error! entity on the wall!");
        }
        var gameObject = Instantiate(_pusherPreFab);
        var entity = gameObject.GetComponent<WorldEntity>();
        entity.Location = location;
        var pusher = gameObject.GetComponent<Pusher>();
        pusher.direction = direction;
        pusher.isControlled = isControlled;
        pusher.iRange = range;
        pusher.iID = ID;
        pusher.fTimeInterval = timeInterval;

    }
    public void InstantiateStar(IntVector location)
    {
        if (_world[location.x, location.y] == TileType.Wall)
        {
            print("error! trigger on the wall!");
        }
        var gameObject = Instantiate(_starPreFab);
        var trigger = gameObject.GetComponent<WorldTrigger>();
        trigger.Location = location;
    }
    public void InstantiateDoor(IntVector location, int ID, int triggerNumber)
    {
        if (_world[location.x, location.y] == TileType.Wall)
        {
            print("error! door on the wall!");
        }
        var gameObject = Instantiate(_doorPreFab);
        var entity = gameObject.GetComponent<WorldEntity>();
        entity.Location = location;
        var door = gameObject.GetComponent<Door>();
        door.iID = ID;
        door.iTriggerNumber = triggerNumber;
    }
    public void InstantiateStepTrigger(IntVector location, int ID)
    {
        if (_world[location.x, location.y] == TileType.Wall)
        {
            print("error! step trigger on the wall!");
        }
        var gameObject = Instantiate(_stepTriggerPreFab);
        var trigger = gameObject.GetComponent<WorldTrigger>();
        trigger.Location = location;
        var stepTrigger = gameObject.GetComponent<StepTrigger>();
        stepTrigger.iID = ID;
    }
    public void InstantiateStayTrigger(IntVector location, int ID)
    {
        if (_world[location.x, location.y] == TileType.Wall)
        {
            print("error! stay trigger on the wall!");
        }
        var gameObject = Instantiate(_stayTriggerPreFab);
        var trigger = gameObject.GetComponent<WorldTrigger>();
        trigger.Location = location;
        var stayTrigger = gameObject.GetComponent<StayTrigger>();
        stayTrigger.iID = ID;
    }

    public void InstantiateShooter(IntVector location, float fShootingTimeInterval, Direction shootingDirection, 
        bool isMoving, Direction movingDirection, int iRange, float fMovingTimeInterval)
    {
        if (_world[location.x, location.y] == TileType.Wall)
        {
            print("error! shooter on the wall!");
        }
        var gameObject = Instantiate(_shooterPreFab);
        var entity = gameObject.GetComponent<WorldEntity>();
        entity.Location = location;
        var shooter = gameObject.GetComponent<Shooter>();
        shooter.fMovingTimeInterval = fShootingTimeInterval;
        shooter.shootingDirection = shootingDirection;
        shooter.isMoving = isMoving;
        shooter.movingDirection = movingDirection;
        shooter.iRange = iRange;
        shooter.fMovingTimeInterval = fMovingTimeInterval;
    }
    public void InstantiateCheckPoint1(IntVector location)
    {
        if (_world[location.x, location.y] == TileType.Wall)
        {
            print("error! check point on the wall!");
        }
        checkPoint1Object = Instantiate(_checkPoint1PreFab);
        var trigger = checkPoint1Object.GetComponent<WorldTrigger>();
        trigger.Location = location;
print("checkPoint1Object " + location);
    }
    public void InstantiateCheckPoint2(IntVector location)
    {
        if (_world[location.x, location.y] == TileType.Wall)
        {
            print("error! check point on the wall!");
        }
        var checkPoint2Object = Instantiate(_checkPoint2PreFab);
        var trigger = checkPoint2Object.GetComponent<WorldTrigger>();
        trigger.Location = location;
        print("checkPoint2Object " + location);
    }
    private void CheckPointsCheck()
    {
        if (_bPlayer1Entered && _bPlayer2Entered)
        {
            _bPlayer1Entered = false;
            _bPlayer2Entered = false;
            MoveCheckPoints();
        }
    }
    private void MoveCheckPoints()
    {
        iCheckPointLocationID++;
        print("iCheckPointLocationID " + iCheckPointLocationID);
        print("CheckPoint1Locations[iCheckPointLocationID] " + CheckPoint1Locations[iCheckPointLocationID]);
        print("CheckPoint2Locations[iCheckPointLocationID] " + CheckPoint2Locations[iCheckPointLocationID]);
        if (CheckPoint1Locations[iCheckPointLocationID] != null &&
           CheckPoint2Locations[iCheckPointLocationID] != null)
        {
            checkPoint1Object.GetComponent<WorldTrigger>().Location = CheckPoint1Locations[iCheckPointLocationID];
            //var trigger = checkPoint2Object.GetComponent<WorldTrigger>();
            //trigger.Location = CheckPoint2Locations[iCheckPointLocationID];
        }
        else
        {
            //destroy
        }
    }
    void CheckPointPass(CheckPointEvent e)
    {

        if (e.isEntered)
        {
            print("enter");
            switch (e.CharacterID)
            {
                case 1:
                    _bPlayer1Entered = true;
                    break;
                case 2:
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
            }
        }
    }

    void OnDrawGizmos()
    {
        if (_world == null)
            return;
        for (int x = 0; x < _dims.x; x++)
        {
            for (int y = 0; y < _dims.y; y++)
            {
                Rect rect = new Rect(x * _tileSize, y * _tileSize, _tileSize, _tileSize);
                switch (_world[x, y])
                {
                    case TileType.Floor:
                        Gizmos.DrawGUITexture(rect, _floorTexture);
                        break;
                    case TileType.Empty:
                        break;
                    case TileType.Wall:
                        Gizmos.DrawGUITexture(rect, _wallTexture);
                        break;
                }
            }
        }


        foreach (WorldTrigger t in _triggers)
        {
            if (t != null)
            {
                IntVector l = t.Location;
                Rect rect = new Rect(l.ToVector2().x * _tileSize, l.ToVector2().y * _tileSize, _tileSize, _tileSize);
                switch (t.triggerType)
                {
                    case TriggerType.Star:
                        Gizmos.DrawGUITexture(rect, _starTexture);
                        break;
                    case TriggerType.StayTrigger:
                        Gizmos.DrawGUITexture(rect, _stayTriggerTexture);
                        break;
                    case TriggerType.CheckPoint:
                        Gizmos.DrawGUITexture(rect, _checkPointTexture);
                        break;
                    case TriggerType.StepTrigger:
                        var stepTrigger = t.gameObject.GetComponent<StepTrigger>();
                        if (stepTrigger != null && !stepTrigger.isTriggered)
                        {
                            Gizmos.DrawGUITexture(rect, _stepTriggerTexture);
                        }
                        
                        break;
                }
            }
        }
        foreach (WorldEntity e in _entities)
        {
            if (e != null)
            {
                IntVector l = e.Location;
                Rect rect = new Rect(l.ToVector2().x * _tileSize, l.ToVector2().y * _tileSize, _tileSize, _tileSize);
                switch (e.entityType)
                {
                    case EntityType.Character1:
                        Gizmos.DrawGUITexture(rect, _character1Texture);
                        break;
                    case EntityType.Character2:
                        Gizmos.DrawGUITexture(rect, _character2Texture);
                        break;
                    case EntityType.Pusher:
                        Gizmos.DrawGUITexture(rect, _pusherTexture);
                        break;
                    case EntityType.Door:
                        var door = e.gameObject.GetComponent<Door>();
                        if (door.isOpen)
                        {

                        }
                        else if (door.isHalfOpen)
                        {
                            Gizmos.DrawGUITexture(rect, _doorHalfOpenTexture);
                        }
                        else
                        {
                            Gizmos.DrawGUITexture(rect, _doorTexture);
                        }
                        
                        break;
                    case EntityType.Shooter:
                        Gizmos.DrawGUITexture(rect, _shooterTexture);
                        break;
                }
            }
            
        }
        foreach (WorldTrigger t in _triggers)
        {
            if (t != null)
            {
                IntVector l = t.Location;
                Rect rect = new Rect(l.ToVector2().x * _tileSize, l.ToVector2().y * _tileSize, _tileSize, _tileSize);
                switch (t.triggerType)
                {
                    case TriggerType.Bullet:
                        Gizmos.DrawGUITexture(rect, _bulletTexture);
                        break;

                }
            }
        }
        
    }
    // void OnGUI()
    // {
    //     if (_world == null)
    //         return;
    //     for (int x = 0; x < _dims.x; x++)
    //     {
    //         for (int y = 0; y < _dims.y; y++)
    //         {
    //             Rect rect = new Rect(x * _tileSize, y * _tileSize, _tileSize, _tileSize);
    //             switch (_world[x, y])
    //             {
    //                 case TileType.Floor:
    //                     GUI.DrawTexture(rect, _floorTexture);
    //                     break;
    //                 case TileType.Empty:
    //                     break;
    //                 case TileType.Wall:
    //                     GUI.DrawTexture(rect, _wallTexture);
    //                     break;
    //             }
    //         }
    //     }
               
    // }
}
