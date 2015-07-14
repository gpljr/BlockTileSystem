using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class MapEditor : MonoBehaviour
{
    //WorldManager worldManage;
    private SavableLevel toLoad;

    void Awake()
    {
        //worldManage = gameObject.GetComponent<WorldManager>();
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
                    WorldManager.g.InstantiatePusher(pushers[i].vPosition, pushers[i].isControlled, 
                        pushers[i].direction, pushers[i].range, pushers[i].ID, pushers[i].timeInterval);
                }
            }
        }
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
                    WorldManager.g.InstantiateStar(stars[i].vPosition);
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
                    WorldManager.g.InstantiateDoor(doors[i].vPosition, ID: doors[i].iID, triggerNumber: doors[i].triggerNumber);         
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
                    WorldManager.g.InstantiateStepTrigger(stepTriggers[i].vPosition, ID: stepTriggers[i].iID); 
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
                    WorldManager.g.InstantiateStayTrigger(stayTriggers[i].vPosition, ID: stayTriggers[i].iID);  
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
                    WorldManager.g.InstantiateShooter(location: shooters[i].vPosition,
                        fShootingTimeInterval: shooters[i].shootingTimeInterval, 
                        shootingDirection: shooters[i].shootingDirection, isMoving: shooters[i].isMoving,
                        movingDirection: shooters[i].movingDirection, iRange: shooters[i].range,
                        fMovingTimeInterval: shooters[i].movingTimeInterval);        
                }
            }
        }
    }
    public void SetCheckPoints()
    {

        if (toLoad.cCheckPoints != null)
        {

            CheckPointInXML[] checkPoints = toLoad.cCheckPoints;
            if (checkPoints[0].vCheckPoint1Position != new IntVector(0, 0))
            {
                WorldManager.g.InstantiateCheckPoint1(checkPoints[0].vCheckPoint1Position);
                if (toLoad.iLevelType != 4)
                {
                    WorldManager.g.InstantiateCheckPoint2(checkPoints[0].vCheckPoint2Position); 
                }
            }
            WorldManager.g.CheckPoint1Locations = new IntVector [checkPoints.Length];
            WorldManager.g.CheckPoint2Locations = new IntVector [checkPoints.Length];
            for (int i = 0; i < checkPoints.Length; i++)
            {
                //if (checkPoints[i].vCheckPoint1Position != new IntVector(0, 0))
                {
                    WorldManager.g.CheckPoint1Locations[i] = checkPoints[i].vCheckPoint1Position;
                    WorldManager.g.CheckPoint2Locations[i] = checkPoints[i].vCheckPoint2Position;
                }
            }
                       
        }
        else
        {
            WorldManager.g.CheckPoint1Locations = null;
            WorldManager.g.CheckPoint2Locations = null;
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
