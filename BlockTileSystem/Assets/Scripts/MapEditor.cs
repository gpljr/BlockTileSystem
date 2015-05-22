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
    public void SetStars()
    {
        StarInXML[] stars = toLoad.sStars;
        for (int i = 0; i < stars.Length; i++)
        {          
            WorldManager.g.InstantiateStar(stars[i].vPosition);            
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
