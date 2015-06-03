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
        public ShooterInXML[] sShooters;
        public CheckPointInXML[] cCheckPoints;
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

    public struct ShooterInXML
    {
        public IntVector vPosition;
        public float shootingTimeInterval;
        public string sShootingDirection;
        public Direction shootingDirection;

        public bool isMoving;
        public string sMovingDirection;
        public Direction movingDirection;
        public int range;
        public float movingTimeInterval;
    }
    public struct CheckPointInXML
    {
        public IntVector vCheckPoint1Position;
        public IntVector vCheckPoint2Position;
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
        Character
        ,Pusher
        ,Door
        ,Shooter
        ,Bullet
        
    }
    public enum TriggerType{
        LevelStar
        ,StepTrigger
        ,StayTrigger
        ,CheckPoint
        ,Bullet
        ,TutorialKey
    }

    public enum GameState
    {
        Starting,
        InTransition,
        InLevel,
        Ending
    }
    
    public enum LevelType{
        Normal
        ,Separation
        ,Merging
        ,Combined
    }

public class Structs : MonoBehaviour {
	
}
