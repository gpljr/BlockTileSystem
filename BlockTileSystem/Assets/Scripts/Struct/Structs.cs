using UnityEngine;
using System.Collections;

public struct SavableLevel
    {
        public string sLevelName;
        public int iLevelType;
        public IntVector vDim;
        public IntVector vChar1StartPos, vChar2StartPos;
        public Tile[] tMap;
        public PusherInXML[] pPushers;
        public StarInXML[] sStars;
        public DoorInXML[] dDoors;
        public StepTriggerInXML[] StepTriggers;
        public StayTriggerInXML[] StayTriggers;
    }

    public struct Tile
    {
        public IntVector vTilePosition;
        public int iTileType;
        public TileType tileType;
    }

    public enum TileType
    {
        Empty
        ,Floor
        ,Wall
    }
    public struct PusherInXML
    {
        public IntVector vPosition;
        public bool isControlled;
        public string sDirection;
        public Direction direction;
        public int range;
        public int ID;
        public float timeInterval;
    }
    public struct StarInXML
    {
        public IntVector vPosition;
    }
    public struct DoorInXML
    {
        public IntVector vPosition;
        public int iID;
        public int triggerNumber;
    }
    public struct StepTriggerInXML
    {
        public IntVector vPosition;
        public int iID;
    }
    public struct StayTriggerInXML
    {
        public IntVector vPosition;
        public int iID;
    }


    public enum MoveResult
    {
        Move
        ,Push
        ,Stuck
    }
    public enum EntityCollidingType
    {
        Empty
        ,Colliding
        ,Pushable
    }
    public enum Direction {
        North,
        South,
        East,
        West
    }

    public enum EntityType{
        Character1
        ,Character2
        ,Pusher
        ,LevelStar
        ,Door
    }
    public enum TriggerType{
        StepTrigger
        ,StayTrigger
        ,Star
    }

public class Structs : MonoBehaviour {
	
}
