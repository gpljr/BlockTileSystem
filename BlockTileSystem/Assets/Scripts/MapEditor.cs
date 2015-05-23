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
        FileStream levelReader = new FileStream(Path.Combine(Application.dataPath, sLevelName), FileMode.Open); // TODO(Julian): Varying filenames
        XmlReader xmlReader = XmlReader.Create(levelReader);
        toLoad = (SavableLevel)levelDeserializer.Deserialize(xmlReader);
        levelReader.Close();
        print("level "+iLevel+" is loaded.");
    }
    public IntVector GetDim()
    {
        return toLoad.vDim;
    }
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
    public void SetStars()
    {
        if (toLoad.sStars != null)
        {
            StarInXML[] stars = toLoad.sStars;
            for (int i = 0; i < stars.Length; i++)
            {          
                WorldManager.g.InstantiateStar(stars[i].vPosition);            
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
                WorldManager.g.InstantiateDoor(doors[i].vPosition, ID: doors[i].iID, triggerNumber: doors[i].triggerNumber);         
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
                WorldManager.g.InstantiateStepTrigger(stepTriggers[i].vPosition, ID: stepTriggers[i].iID);        
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
                WorldManager.g.InstantiateStayTrigger(stayTriggers[i].vPosition, ID: stayTriggers[i].iID);        
            }
        }
    }
    public void SetShooters()
    {
        if (toLoad.sShooters != null)
        {
            print("setshooters");
            ShooterInXML[] shooters = toLoad.sShooters;
            for (int i = 0; i < shooters.Length; i++)
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
                //WorldManager.g.InstantiateShooter(shooters[i].vPosition,  ) ;        
            }
        }
    }
    public void SetCheckPoints()
    {
        if (toLoad.cCheckPoints != null)
        {
            print("checkpoints");
            CheckPointInXML[] checkPoints = toLoad.cCheckPoints;
            for (int i = 0; i < checkPoints.Length; i++)
            {          
                //WorldManager.g.InstantiateStayTrigger(checkPoints[i].vCheckPoint1Position, checkPoints[i].vCheckPoint2Position);        
            }
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
