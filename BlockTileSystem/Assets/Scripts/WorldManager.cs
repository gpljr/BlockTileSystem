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

    public WorldEntity Char1;
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
        Events.g.Raise(new LevelLoadedEvent(iLevel));
    }
    private void LoadLevel(LoadLevelEvent e)
    {
        LoadLevel(e.iLevel);
    }
    void OnEnable()
    {
        Events.g.AddListener<LoadLevelEvent>(LoadLevel);
    }
    
    void OnDisable()
    {
        Events.g.RemoveListener<LoadLevelEvent>(LoadLevel);
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
        //Vector2 cameraLocation = (Char1.Location + Char2.Location).ToVector2() * _tileSize / 2f;
        //_mainCamera.transform.position = new Vector3(cameraLocation.x, cameraLocation.y, -12f);
        //float distance = Vector2.Distance(Char1.Location.ToVector2(), Char2.Location.ToVector2());
        //_camera.orthographicSize = Mathf.Max(distance, 6f) * _tileSize;
    }
    public void GenerateBasicMap(Tile[] tMap)
    {
        for (int i = 0; i < tMap.Length; i++)
        {
            int x = tMap[i].vTilePosition.x;
            int y = tMap[i].vTilePosition.y;
            _world[x, y] = tMap[i].tileType;
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

    public void InstantiatePusher(IntVector location, bool isControlled, Direction direction, int range, int ID, float timeInterval)
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
                    case TriggerType.StepTrigger:
                        var stepTrigger = t.gameObject.GetComponent<StepTrigger>();
                        if (stepTrigger!= null && !stepTrigger.isTriggered)
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
