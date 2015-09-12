public class LevelLoadedEvent : GameEvent
{
public int iLevel;
//public int iLevelType;
    public LevelLoadedEvent(int iLevel)
    {
        this.iLevel = iLevel;
        //this.iLevelType=iLevelType;
    }


}
