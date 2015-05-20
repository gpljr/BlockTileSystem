using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{

    [SerializeField]
    IntVector _dims;
    [SerializeField]
    float _tileSize;
    TileType[,] _world;
    List<WorldEntity>[,] _entityMap;

    List<WorldEntity> _entities = new List<WorldEntity>();

    [SerializeField]
    private GameObject _pusherPreFab;

    [SerializeField]
    private GameObject _mainCamera;

    [SerializeField]
    private Texture _character1Texture;
    [SerializeField]
    private Texture _character2Texture;
    [SerializeField]
    private Texture _floorTexture;
    [SerializeField]
    private Texture _wallTexture;
    [SerializeField]
    private Texture _pusherTexture;

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
        LoadLevel(1);
    }

    private void LoadLevel(int iLevel)
    {
        ClearMap();        
        mapEditor.LoadFile(iLevel);
        _dims = mapEditor.GetDim();
        _world = new TileType[_dims.x, _dims.y];
        _mainCamera.transform.position = new Vector3(_dims.x * _tileSize / 2, _dims.y * _tileSize / 2, -12f);
        _mainCamera.GetComponent<Camera>().orthographicSize = Mathf.Min(_dims.x, _dims.y) * _tileSize / 2 + 2;

        _entityMap = new List<WorldEntity>[_dims.x, _dims.y];
        
        for (int x = 0; x < _dims.x; x++)
        {
            for (int y = 0; y < _dims.y; y++)
            {
                _entityMap[x, y] = new List<WorldEntity>();
            }
        }
        mapEditor.SetMap();
        mapEditor.SetCharacters();
        mapEditor.SetPushers();
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

    public MoveResult CanMove(IntVector from, Direction direction)
    {
        IntVector destination = Destination(from, direction);

        int x = destination.x;
        int y = destination.y;
        if (_world[x, y] == TileType.Wall)
        {
            return MoveResult.Stuck;
        }
        else
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                if (_entities[i].Location == destination)
                {
                    switch (_entities[i].CollidingType)
                    {
                        case EntityCollidingType.Empty:
                            return MoveResult.Move;
                        case EntityCollidingType.Colliding:
                            return MoveResult.Stuck;
                        case EntityCollidingType.Pushable:
                            switch (CanMove(destination, direction))
                            {
                                case MoveResult.Move:
                                    PushEntity(_entities[i], direction);
                                    return MoveResult.Push;
                                case MoveResult.Stuck:
                                    return MoveResult.Stuck;
                                case MoveResult.Push:
                                    PushEntity(_entities[i], direction);
                                    return MoveResult.Push;
                                default:
                                    return MoveResult.Stuck;
                            }                      
                    }
                }
            }
            return MoveResult.Move;
        }
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
        Instantiate(_pusherPreFab);
        var entity = _pusherPreFab.GetComponent<WorldEntity>();
        entity.Location = location;
        var pusher = _pusherPreFab.GetComponent<Pusher>();
        pusher.direction = direction;
        pusher.isControlled = isControlled;
        pusher.iRange = range;
        pusher.iID = ID;
        pusher.fTimeInterval = timeInterval;

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
                        //Gizmos.color = Color.green;
                        Gizmos.DrawGUITexture(rect, _floorTexture);
                        break;
                    case TileType.Empty:
                        //Gizmos.color = Color.blue;
                        break;
                    case TileType.Wall:
                        Gizmos.DrawGUITexture(rect, _wallTexture);
                        //Gizmos.color = Color.red;
                        break;
                    default:
                        Gizmos.color = Color.black;
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
                }
            }
            
        }
    }
}
