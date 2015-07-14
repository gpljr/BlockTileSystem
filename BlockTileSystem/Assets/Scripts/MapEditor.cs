using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class MapEditor : MonoBehaviour
{
    //WorldManager worldManage;
    private SavableLevel toLoad;

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
    private GameObject _tutorialKeyPrefab;
        [SerializeField]
    private GameObject _checkPointPreFab;

    private CheckPointsManager checkPointsManager;

    void Awake()
    {
        checkPointsManager = gameObject.GetComponent<CheckPointsManager>();
    }

    public void LoadFile(int iLevel)
    {
        //read the file
        XmlSerializer levelDeserializer = new XmlSerializer(typeof(SavableLevel));
        string sLevelName = "Levels/" + iLevel.ToString() + ".xml";
        FileStream levelReader = new FileStream(Path.Combine(Application.streamingAssetsPath, sLevelName), FileMode.Open); 
        if (levelReader != null)
        {    
            XmlReader xmlReader = XmlReader.Create(levelReader);
            toLoad = (SavableLevel)levelDeserializer.Deserialize(xmlReader);
            SetLevelType();
            levelReader.Close();
            print("level " + iLevel + " is loaded.");
        }
        else
        {
            print("no level file");

        }
    }
    public IntVector GetDim()
    {
        return toLoad.vDim;
    }
    public void SetLevelType()
    {
        switch (toLoad.iLevelType)
        {
            case 1:
                LevelCode.levelType = LevelType.Normal;
                break;
            case 2:
                LevelCode.levelType = LevelType.Separation;
                break;
            case 3:
                LevelCode.levelType = LevelType.Merging;
                break;
            case 4:
                LevelCode.levelType = LevelType.Combined;
                break;
        }
    }
    // public int GetLevelType()
    // {
    //     return toLoad.iLevelType;
    // }
    public void SetCharacters()
    {
        WorldManager.g.SetCharacters(toLoad.vChar1StartPos, toLoad.vChar2StartPos);
    }
    public void SetPushers()
    {
        if (toLoad.pPushers != null)
        {
            PusherInXML[] pushers = toLoad.pPushers;
            for (int i = 0; i < pushers.Length; i++)
            {
                if (pushers[i].vPosition != new IntVector(0, 0))
                {
                    switch (pushers[i].sDirection)
                    {
                        case "North":
                            pushers[i].direction = Direction.North;
                            break;
                        case "South":
                            pushers[i].direction = Direction.South;
                            break;
                        case "West":
                            pushers[i].direction = Direction.West;
                            break;
                        case "East":
                            pushers[i].direction = Direction.East;
                            break;
                    }       
                    InstantiatePusher(pushers[i].vPosition, pushers[i].isControlled, 
                        pushers[i].direction, pushers[i].range, pushers[i].ID, pushers[i].timeInterval);
                }
            }
        }
    }
        public IntVector PositionFlip(IntVector Position)
    {
        int x = Position.x;
        int y = Position.y;
        IntVector newPosition = new IntVector(x, toLoad.vDim.y - y - 1);
        //IntVector newPosition = new IntVector(x, y);
        return newPosition;
    }
    private void InstantiatePusher(IntVector location, bool isControlled, Direction direction, 
        int range, int ID, float timeInterval)
    {
        // if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        // {
        //     print("error! entity on the wall!");
        // }
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
    public void SetStars()
    {
        if (toLoad.sStars != null)
        {
            StarInXML[] stars = toLoad.sStars;
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i].vPosition != new IntVector(0, 0))
                {          
                    InstantiateStar(stars[i].vPosition);
                }            
            }
        }
    }

    public void SetDoors()
    {
        if (toLoad.dDoors != null)
        {
            DoorInXML[] doors = toLoad.dDoors;
            for (int i = 0; i < doors.Length; i++)
            {          
                if (doors[i].vPosition != new IntVector(0, 0))
                {
                    InstantiateDoor(doors[i].vPosition, ID: doors[i].iID, triggerNumber: doors[i].triggerNumber);         
                }
            }
        }
    }
    public void SetStepTriggers()
    {
        if (toLoad.StepTriggers != null)
        {
            StepTriggerInXML[] stepTriggers = toLoad.StepTriggers;
            for (int i = 0; i < stepTriggers.Length; i++)
            {
                if (stepTriggers[i].vPosition != new IntVector(0, 0))
                {          
                    InstantiateStepTrigger(stepTriggers[i].vPosition, ID: stepTriggers[i].iID); 
                }       
            }
        }
    }
    public void SetStayTriggers()
    {
        if (toLoad.StayTriggers != null)
        {
            StayTriggerInXML[] stayTriggers = toLoad.StayTriggers;
            for (int i = 0; i < stayTriggers.Length; i++)
            {
                if (stayTriggers[i].vPosition != new IntVector(0, 0))
                {          
                    InstantiateStayTrigger(stayTriggers[i].vPosition, ID: stayTriggers[i].iID);  
                }      
            }
        }
    }
    public void SetShooters()
    {
        if (toLoad.sShooters != null)
        {
            ShooterInXML[] shooters = toLoad.sShooters;
            for (int i = 0; i < shooters.Length; i++)
            { 
                if (shooters[i].vPosition != new IntVector(0, 0))
                {          
                    switch (shooters[i].sShootingDirection)
                    {
                        case "North":
                            shooters[i].shootingDirection = Direction.North;
                            break;
                        case "South":
                            shooters[i].shootingDirection = Direction.South;
                            break;
                        case "West":
                            shooters[i].shootingDirection = Direction.West;
                            break;
                        case "East":
                            shooters[i].shootingDirection = Direction.East;
                            break;
                    }
                    switch (shooters[i].sMovingDirection)
                    {
                        case "North":
                            shooters[i].movingDirection = Direction.North;
                            break;
                        case "South":
                            shooters[i].movingDirection = Direction.South;
                            break;
                        case "West":
                            shooters[i].movingDirection = Direction.West;
                            break;
                        case "East":
                            shooters[i].movingDirection = Direction.East;
                            break;
                    }        
                    InstantiateShooter(location: shooters[i].vPosition,
                        fShootingTimeInterval: shooters[i].shootingTimeInterval, 
                        shootingDirection: shooters[i].shootingDirection, isMoving: shooters[i].isMoving,
                        movingDirection: shooters[i].movingDirection, iRange: shooters[i].range,
                        fMovingTimeInterval: shooters[i].movingTimeInterval);        
                }
            }
        }
    }
    
    public void InstantiateStar(IntVector location)
    {
        // if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        // {
        //     print("error! trigger on the wall!");
        // }
        var gameObject = Instantiate(_starPreFab);
        var trigger = gameObject.GetComponent<WorldTrigger>();
        trigger.Location = PositionFlip(location);
    }
    public void InstantiateDoor(IntVector location, int ID, int triggerNumber)
    {
        // if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        // {
        //     print("error! door on the wall!");
        // }
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
        // if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        // {
        //     print("error! step trigger on the wall!");
        // }
        var gameObject = Instantiate(_stepTriggerPreFab);
        var trigger = gameObject.GetComponent<WorldTrigger>();
        trigger.Location = PositionFlip(location);
        var stepTrigger = gameObject.GetComponent<StepTrigger>();
        stepTrigger.iID = ID;
    }
    public void InstantiateStayTrigger(IntVector location, int ID)
    {
        // if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        // {
        //     print("error! stay trigger on the wall!");
        // }
        var gameObject = Instantiate(_stayTriggerPreFab);
        var trigger = gameObject.GetComponent<WorldTrigger>();
        trigger.Location = PositionFlip(location);
        var stayTrigger = gameObject.GetComponent<StayTrigger>();
        stayTrigger.iID = ID;
    }

    private void InstantiateShooter(IntVector location, float fShootingTimeInterval, Direction shootingDirection, 
        bool isMoving, Direction movingDirection, int iRange, float fMovingTimeInterval)
    {
        // if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        // {
        //     print("error! shooter on the wall!");
        // }
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

    public void SetTutorialKeys()
    {
            InstantiateTutorialKeys(toLoad.vChar1StartPos + new IntVector(0, -1), 1);
            InstantiateTutorialKeys((toLoad.vChar1StartPos + new IntVector(-1, 0)), 2);
            InstantiateTutorialKeys((toLoad.vChar1StartPos + new IntVector(0, 1)), 3);
            InstantiateTutorialKeys((toLoad.vChar1StartPos + new IntVector(1, 0)), 4);
            InstantiateTutorialKeys((toLoad.vChar2StartPos + new IntVector(0, -1)), 5);
            InstantiateTutorialKeys((toLoad.vChar2StartPos + new IntVector(0, 1)), 6);
            InstantiateTutorialKeys((toLoad.vChar2StartPos + new IntVector(-1, 0)), 7);
            InstantiateTutorialKeys((toLoad.vChar2StartPos + new IntVector(1, 0)), 8);
    }
        public void InstantiateTutorialKeys(IntVector location, int ID)
    {
        var gameObject = Instantiate(_tutorialKeyPrefab);
        var trigger = gameObject.GetComponent<WorldTrigger>();
        trigger.Location = PositionFlip(location);
        var tutorialKey = gameObject.GetComponent<TutorialKey>();
        tutorialKey.iID = ID;
    }
    public void SetCheckPoints()
    {

        if (toLoad.cCheckPoints != null)
        {

            CheckPointInXML[] checkPoints = toLoad.cCheckPoints;
            if (checkPoints[0].vCheckPoint1Position != new IntVector(0, 0))
            {
                InstantiateCheckPoint(checkPoints[0].vCheckPoint1Position,1);
                if (toLoad.iLevelType != 4)
                {
                    InstantiateCheckPoint(checkPoints[0].vCheckPoint2Position,2); 
                }
            }
            checkPointsManager.CheckPoint1Locations = new IntVector [checkPoints.Length];
            checkPointsManager.CheckPoint2Locations = new IntVector [checkPoints.Length];
            for (int i = 0; i < checkPoints.Length; i++)
            {
                //if (checkPoints[i].vCheckPoint1Position != new IntVector(0, 0))
                {
                    checkPointsManager.CheckPoint1Locations[i] = checkPoints[i].vCheckPoint1Position;
                    checkPointsManager.CheckPoint2Locations[i] = checkPoints[i].vCheckPoint2Position;
                }
            }
                       
        }
        else
        {
            checkPointsManager.CheckPoint1Locations = null;
            checkPointsManager.CheckPoint2Locations = null;
        }
    }
private void InstantiateCheckPoint(IntVector location,int iID)
    {
        // if (_world[PositionFlip(location).x, PositionFlip(location).y] == TileType.Wall)
        // {
        //     print("error! check point on the wall!");
        // }
        if(iID==1)
        {
        checkPointsManager.checkPoint1Object = Instantiate(_checkPointPreFab);
        var trigger = checkPointsManager.checkPoint1Object.GetComponent<WorldTrigger>();
        trigger.Location = PositionFlip(location);
        checkPointsManager.checkPoint1Object.GetComponent<CheckPoint>().iID = iID;
    }
    else if(iID==2){
checkPointsManager.checkPoint2Object = Instantiate(_checkPointPreFab);
        var trigger = checkPointsManager.checkPoint2Object.GetComponent<WorldTrigger>();
        trigger.Location = PositionFlip(location);
        checkPointsManager.checkPoint2Object.GetComponent<CheckPoint>().iID = iID;
    }

    }

    public void SetMap()
    {
        Tile[] tMap = toLoad.tMap;
        for (int i = 0; i < tMap.Length; i++)
        {
            switch (toLoad.tMap[i].iTileType)
            {
                case 0:
                    tMap[i].tileType = TileType.Empty;
                    break;
                case 1:
                    tMap[i].tileType = TileType.Floor;
                    break;
                case 2:
                    tMap[i].tileType = TileType.Wall;
                    break;

                default:
                    tMap[i].tileType = TileType.Empty;
                    break;
            }
        }
        WorldManager.g.GenerateBasicMap(tMap);
    }

}
