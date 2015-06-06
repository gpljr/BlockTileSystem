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
    [SerializeField]
    GameObject CharCombinedObject;

    // [HideInInspector]
    // public Character Char1;
    // [HideInInspector]
    // public Character Char2;
    // [HideInInspector]
    // public Character CharCombined;

    [HideInInspector]
    public WorldEntity char1Entity;
    [HideInInspector]
    public WorldEntity char2Entity;
    [HideInInspector]
    public WorldEntity charCombinedEntity;
    [HideInInspector]
    public float fCharacterDistance;

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
    private GameObject _tutorialKeyPrefab;
    

    // [SerializeField]
    // private GameObject _mainCamera;
    // private Camera _camera;

    
    // [SerializeField]
    // private Texture _floorTexture;
    // [SerializeField]
    // private Texture _wallTexture;
    // [SerializeField]
    // private Texture _character1Texture;
    // [SerializeField]
    // private Texture _character2Texture;
    // [SerializeField]
    // private Texture _characterCombinedTexture;
    // [SerializeField]
    // private Texture _pusherTexture;
    // [SerializeField]
    // private Texture _starTexture;
    // [SerializeField]
    // private Texture _doorTexture;
    // [SerializeField]
    // private Texture _doorHalfOpenTexture;
    // [SerializeField]
    // private Texture _stepTriggerTexture;
    // [SerializeField]
    // private Texture _stayTriggerTexture;
    // [SerializeField]
    // private Texture _shooterTexture;
    // [SerializeField]
    // private Texture _bulletTexture;
    // [SerializeField]
    // private Texture _checkPointTexture;

    private GameObject checkPoint1Object, checkPoint2Object;
    private bool _bPlayer1Entered, _bPlayer2Entered;
    [HideInInspector]
    public IntVector[] CheckPoint1Locations;
    [HideInInspector]
    public IntVector[] CheckPoint2Locations;
    private int iCheckPointLocationID;
    public bool checkPointsMoved;
    private int iChar1InCheckPoint, iChar2InCheckPoint;

    [SerializeField]
    private Transform _floor;
    [SerializeField]
    private Transform _wall;
    GameObject[] Floors;
    GameObject[] Walls;

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
        _entityMap = new List<WorldEntity>[_dims.x, _dims.y];
        mapEditor = gameObject.GetComponent<MapEditor>();
        // Char1 = Char1Object.GetComponent<Character>();
        // Char2 = Char2Object.GetComponent<Character>();
        // CharCombined = CharCombinedObject.GetComponent<Character>();
        char1Entity = Char1Object.GetComponent<WorldEntity>();
        char2Entity = Char2Object.GetComponent<WorldEntity>();
        charCombinedEntity = CharCombinedObject.GetComponent<WorldEntity>();
        // Char1Object.SetActive(true);
        // Char2Object.SetActive(true);
        // CharCombinedObject.SetActive(false);
        //_camera = _mainCamera.GetComponent<Camera>();

        //LoadLevel(1);

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
        // int levelType = mapEditor.GetLevelType();
        if (iLevel == 1)//a temporary way to make tutorial key icons in the first level
        {
            InstantiateTutorialKeys(new IntVector(2, 1), 1);
            InstantiateTutorialKeys(new IntVector(1, 2), 2);
            InstantiateTutorialKeys(new IntVector(2, 3), 3);
            InstantiateTutorialKeys(new IntVector(3, 2), 4);
            InstantiateTutorialKeys(new IntVector(6, 7), 5);
            InstantiateTutorialKeys(new IntVector(6, 9), 6);
            InstantiateTutorialKeys(new IntVector(5, 8), 7);
            InstantiateTutorialKeys(new IntVector(7, 8), 8);
        }

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

    void Update()
    {
        if (LevelCode.gameState == GameState.InLevel)
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
            foreach (WorldEntity e in _entities)//check for bullet hit
            {
                if (e != null && e.entityType != EntityType.Bullet)
                {
                    IntVector eLocation = e.Location;
                    foreach (WorldTrigger t in _triggers)
                    {
                        IntVector tLocation = t.Location;
                        if (eLocation == tLocation && t.triggerType == TriggerType.Bullet)
                        {
                            
                            StepOnTrigger(t, e);
                        }
                    }
                }
            }
            CheckPointsCheck();
            if (LevelCode.levelType == LevelType.Normal && char1Entity!=null && char2Entity !=null)
            {
                fCharacterDistance = Vector2.Distance(char1Entity.visPosition, char2Entity.visPosition);
            }
        }
    }

    
    public void GenerateBasicMap(Tile[] tMap)
    {
        Floors = new GameObject[tMap.Length];
        Walls = new GameObject[tMap.Length];
        int floorIndex = 0;
        int wallIndex = 0;
        for (int i = 0; i < tMap.Length; i++)
        {

            int x = tMap[i].vTilePosition.x;
            int y = tMap[i].vTilePosition.y;
            if (x < _dims.x && y < _dims.y)
            {
                _world[x, y] = tMap[i].tileType;
                if (_world[x, y] == TileType.Floor)
                {
                    Floors[floorIndex] = (GameObject)Instantiate(_floor.gameObject, new Vector2(x + 0.5F, y - 0.5F), new Quaternion());
                    floorIndex++;
                }
                else if (_world[x, y] == TileType.Wall)
                {
                    Walls[wallIndex] = (GameObject)Instantiate(_wall.gameObject, new Vector2(x + 0.5F, y - 0.5F), new Quaternion());
                    wallIndex++;
                }
                
            }
            //_world[x, _dims.y-y-1] = tMap[i].tileType;//make levels in excel from top left
        }

    }
    private void ClearMap()
    {
        if (Floors != null)
        {
            foreach (GameObject f in Floors)
            {
                if (f != null)
                {
                    Destroy(f);
                }
            }

        }
        if (Walls != null)
        {
            foreach (GameObject w in Walls)
            {
                if (w != null)
                {
                    Destroy(w);
                }

            }
        }
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
            if ((e != null) && (e.entityType != EntityType.Character))
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
        checkPointsMoved = false;
    }
    private IntVector PositionFlip(IntVector Position)
    {
        int x = Position.x;
        int y = Position.y;
        IntVector newPosition = new IntVector(x, _dims.y - y - 1);
        //IntVector newPosition = new IntVector(x, y);
        return newPosition;
    }
    public void SetCharacters(IntVector Char1Pos, IntVector Char2Pos)
    {
        
        if (LevelCode.levelType == LevelType.Combined)
        {
            Char1Object.SetActive(false);
            Char2Object.SetActive(false);
            CharCombinedObject.SetActive(true);
            charCombinedEntity.instantMove = true;
            charCombinedEntity.Location = PositionFlip(Char1Pos);
        }
        else
        {
            Char1Object.SetActive(true);
            Char2Object.SetActive(true);
            CharCombinedObject.SetActive(false);
            char1Entity.instantMove = true;
            char1Entity.Location = PositionFlip(Char1Pos);
            char2Entity.instantMove = true;
            char2Entity.Location = PositionFlip(Char2Pos);
        }
        
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
                    StepOnTrigger(_triggers[i], movingEntity);
                }
                if (_triggers[i].Location == from)
                {
                    StepOutTrigger(_triggers[i], movingEntity);
                }
            }
                
        }
        return moveResult;
        
        

    }
    private void StepOnTrigger(WorldTrigger steppedTrigger, WorldEntity steppingEntity)
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
    public void InstantiateTutorialKeys(IntVector location, int ID)
    {
        var gameObject = Instantiate(_tutorialKeyPrefab);
        var trigger = gameObject.GetComponent<WorldTrigger>();
        trigger.Location = PositionFlip(location);
        var tutorialKey = gameObject.GetComponent<TutorialKey>();
        tutorialKey.iID = ID;
    }

    public void InstantiatePusher(IntVector location, bool isControlled, Direction direction, 
        int range, int ID, float timeInterval)
    {
        if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        {
            print("error! entity on the wall!");
        }
        var gameObject = Instantiate(_pusherPreFab);
        var entity = gameObject.GetComponent<WorldEntity>();
        entity.instantMove = true;
        entity.Location = PositionFlip(location);
        var pusher = gameObject.GetComponent<Pusher>();
        pusher.direction = direction;
        pusher.isControlled = isControlled;
        pusher.iRange = range;
        pusher.iID = ID;
        pusher.fTimeInterval = timeInterval;

    }
    public void InstantiateStar(IntVector location)
    {
        if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        {
            print("error! trigger on the wall!");
        }
        var gameObject = Instantiate(_starPreFab);
        var trigger = gameObject.GetComponent<WorldTrigger>();
        trigger.Location = PositionFlip(location);
    }
    public void InstantiateDoor(IntVector location, int ID, int triggerNumber)
    {
        if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        {
            print("error! door on the wall!");
        }
        var gameObject = Instantiate(_doorPreFab);
        var entity = gameObject.GetComponent<WorldEntity>();
        entity.instantMove = true;
        entity.Location = PositionFlip(location);
        var door = gameObject.GetComponent<Door>();
        door.iID = ID;
        door.iTriggerNumber = triggerNumber;
    }
    public void InstantiateStepTrigger(IntVector location, int ID)
    {
        if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        {
            print("error! step trigger on the wall!");
        }
        var gameObject = Instantiate(_stepTriggerPreFab);
        var trigger = gameObject.GetComponent<WorldTrigger>();
        trigger.Location = PositionFlip(location);
        var stepTrigger = gameObject.GetComponent<StepTrigger>();
        stepTrigger.iID = ID;
    }
    public void InstantiateStayTrigger(IntVector location, int ID)
    {
        if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        {
            print("error! stay trigger on the wall!");
        }
        var gameObject = Instantiate(_stayTriggerPreFab);
        var trigger = gameObject.GetComponent<WorldTrigger>();
        trigger.Location = PositionFlip(location);
        var stayTrigger = gameObject.GetComponent<StayTrigger>();
        stayTrigger.iID = ID;
    }

    public void InstantiateShooter(IntVector location, float fShootingTimeInterval, Direction shootingDirection, 
        bool isMoving, Direction movingDirection, int iRange, float fMovingTimeInterval)
    {
        if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        {
            print("error! shooter on the wall!");
        }
        var gameObject = Instantiate(_shooterPreFab);
        var entity = gameObject.GetComponent<WorldEntity>();
        entity.instantMove = true;
        entity.Location = PositionFlip(location);
        var shooter = gameObject.GetComponent<Shooter>();
        shooter.fShootingTimeInterval = fShootingTimeInterval;
        shooter.shootingDirection = shootingDirection;
        shooter.isMoving = isMoving;
        shooter.movingDirection = movingDirection;
        shooter.iRange = iRange;
        shooter.fMovingTimeInterval = fMovingTimeInterval;
    }
    public void InstantiateCheckPoint1(IntVector location)
    {
        if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        {
            print("error! check point on the wall!");
        }
        checkPoint1Object = Instantiate(_checkPoint1PreFab);
        var trigger = checkPoint1Object.GetComponent<WorldTrigger>();
        trigger.Location = PositionFlip(location);
        checkPoint1Object.GetComponent<CheckPoint>().iID = 1;
    }
    public void InstantiateCheckPoint2(IntVector location)
    {
        if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        {
            print("error! check point on the wall!");
        }
        checkPoint2Object = Instantiate(_checkPoint1PreFab);
        var trigger = checkPoint2Object.GetComponent<WorldTrigger>();
        trigger.Location = PositionFlip(location);
        checkPoint2Object.GetComponent<CheckPoint>().iID = 2;
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
        IntVector zero = new IntVector(0, 0);
        if (LevelCode.levelType != LevelType.Combined)
        {
            if (CheckPoint1Locations[iCheckPointLocationID] != null &&
                CheckPoint2Locations[iCheckPointLocationID] != null &&
                CheckPoint1Locations[iCheckPointLocationID] != zero &&
                CheckPoint2Locations[iCheckPointLocationID] != zero)
            {
            
                checkPoint1Object.GetComponent<WorldTrigger>().Location 
                = PositionFlip(CheckPoint1Locations[iCheckPointLocationID]);
                checkPoint2Object.GetComponent<WorldTrigger>().Location 
                = PositionFlip(CheckPoint2Locations[iCheckPointLocationID]);
            }
            else
            {
                Destroy(checkPoint1Object);
                Destroy(checkPoint2Object);
                iCheckPointLocationID--;
                checkPointsMoved = true;
            }
        }
        else
        {
            if (CheckPoint1Locations[iCheckPointLocationID] != null &&
                CheckPoint1Locations[iCheckPointLocationID] != zero)
            {
                checkPoint1Object.GetComponent<WorldTrigger>().Location 
                = PositionFlip(CheckPoint1Locations[iCheckPointLocationID]);
            }
            else
            {
                Destroy(checkPoint1Object);
                iCheckPointLocationID--;
                checkPointsMoved = true;
            }
        }
    }
    void CheckPointPass(CheckPointEvent e)
    {

        if (e.isEntered)
        {
            switch (e.CharacterID)
            {
                case 1:
                    _bPlayer1Entered = true;
                    iChar1InCheckPoint = e.iCheckPointID;
        
                    break;
                case 2:
                    _bPlayer2Entered = true;
                    iChar2InCheckPoint = e.iCheckPointID;
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
                    iChar1InCheckPoint = 0;
                    break;
                case 2:
                    _bPlayer2Entered = false;
                    iChar2InCheckPoint = 0;
                    break;
            }
        }
    }
    public void RestartToCheckPoints()
    {
        if (LevelCode.levelType != LevelType.Combined)
        {
            switch (iChar1InCheckPoint)
            {
                case 1:
                    char1Entity.instantMove = true;
                    char1Entity.Location = PositionFlip(CheckPoint1Locations[iCheckPointLocationID]);
                    break;
                case 2:
                    char1Entity.instantMove = true;
                    char1Entity.Location = PositionFlip(CheckPoint2Locations[iCheckPointLocationID]);
                    break;
            }
            switch (iChar2InCheckPoint)
            {
                case 1:
                    char2Entity.instantMove = true;
                    char2Entity.Location = PositionFlip(CheckPoint1Locations[iCheckPointLocationID]);
                    break;
                case 2:
                    char2Entity.instantMove = true;
                    char2Entity.Location = PositionFlip(CheckPoint2Locations[iCheckPointLocationID]);
                    break;
            }
        }
        else
        {
            charCombinedEntity.instantMove = true;
            charCombinedEntity.Location = PositionFlip(CheckPoint1Locations[iCheckPointLocationID]);
                    
        }
        
    }


    // void OnGUI()
    // {
        
    //     if (_world == null)
    //         return;
    // if (inLevel)
    // {
    //     float screenTileSize = Vector3.Distance(Camera.main.WorldToScreenPoint(new Vector3(0f * _tileSize, 0f * _tileSize, 0f)),
    //                                Camera.main.WorldToScreenPoint(new Vector3(1f * _tileSize, 0f * _tileSize, 0f)));
    // for (int x = 0; x < _dims.x; x++)
    // {
    //     for (int y = 0; y < _dims.y; y++)
    //     {
    //         Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(x * _tileSize, y * _tileSize, 0f));
    //         Rect rect = new Rect(screenPos.x, Screen.height - screenPos.y, screenTileSize, screenTileSize);
    //         switch (_world[x, y])
    //         {
    //             case TileType.Floor:
    //                 GUI.DrawTexture(rect, _floorTexture);
    //                 break;
    //             case TileType.Wall:
    //                 GUI.DrawTexture(rect, _wallTexture);
    //                 break;
    //         }
    //     }
    // }
    // foreach (WorldTrigger t in _triggers)
    // {
    //     if (t != null)
    //     {
    //         IntVector l = t.Location;
    //         Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(l.x * _tileSize, l.y * _tileSize, 0f));
    //         Rect rect = new Rect(screenPos.x, Screen.height - screenPos.y, screenTileSize, screenTileSize);
    //         switch (t.triggerType)
    //         {
    // case TriggerType.LevelStar:
    //     GUI.DrawTexture(rect, _starTexture);
    //     break;
    // case TriggerType.StayTrigger:
    //     GUI.DrawTexture(rect, _stayTriggerTexture);
    //     break;
    // case TriggerType.CheckPoint:
    //     GUI.DrawTexture(rect, _checkPointTexture);
    //     break;
    // case TriggerType.StepTrigger:
    //     var stepTrigger = t.gameObject.GetComponent<StepTrigger>();
    //     if (stepTrigger != null && !stepTrigger.isTriggered)
    //     {
    //         GUI.DrawTexture(rect, _stepTriggerTexture);
    //     }
                        
    //     break;
    //         }
    //     }
    // }
    // foreach (WorldEntity e in _entities)
    // {
    //     if (e != null)
    //     {
    //         IntVector l = e.Location;
    //         Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(l.x * _tileSize, l.y * _tileSize, 0f));
    //         Rect rect = new Rect(screenPos.x, Screen.height - screenPos.y, screenTileSize, screenTileSize);
    //         switch (e.entityType)
    //         {
    // case EntityType.Character:
    //     switch (e.characterID)
    //     {
    //         case 1:
    //             if (mapEditor.GetLevelType() != 4)
    //             {
    //                 GUI.DrawTexture(rect, _character1Texture);
    //             }
    //             break;
    //         case 2:
    //             if (mapEditor.GetLevelType() != 4)
    //             {
    //                 GUI.DrawTexture(rect, _character2Texture);
    //             }
    //             break;
    //         case 3:
    //             if (mapEditor.GetLevelType() == 4)
    //             {
    //                 GUI.DrawTexture(rect, _characterCombinedTexture);
    //             }
    //             break;
    //     }
                        
    //     break;

    // case EntityType.Pusher:
    //     GUI.DrawTexture(rect, _pusherTexture);
    //     break;
    // case EntityType.Door:
    //     var door = e.gameObject.GetComponent<Door>();
    //     if (door.isOpen)
    //     {

    //     }
    //     else if (door.isHalfOpen)
    //     {
    //         GUI.DrawTexture(rect, _doorHalfOpenTexture);
    //     }
    //     else
    //     {
    //         GUI.DrawTexture(rect, _doorTexture);
    //     }
                        
    //     break;
    //             case EntityType.Shooter:
    //                 GUI.DrawTexture(rect, _shooterTexture);
    //                 break;
    //         }
    //     }
            
    // }
    // foreach (WorldTrigger t in _triggers)
    // {
    //     if (t != null)
    //     {
    //         IntVector l = t.Location;
    //         Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(l.x * _tileSize, l.y * _tileSize, 0f));
    //         Rect rect = new Rect(screenPos.x, Screen.height - screenPos.y, screenTileSize, screenTileSize);
    //         switch (t.triggerType)
    //         {
    //             case TriggerType.Bullet:
    //                 GUI.DrawTexture(rect, _bulletTexture);
    //                 break;

    //         }
    //     }
    // }
    //     }
    // }
}
