public class CheckPointEvent : GameEvent
{
    public bool isEntered;
    public int CharacterID;
    public int iCheckPointID;
    public CheckPointEvent(bool isEntered, int CharacterID, int iCheckPointID)
    {
        this.isEntered = isEntered;
        this.CharacterID = CharacterID;
        this.iCheckPointID = iCheckPointID;
    }


}
