public class CheckPointEvent : GameEvent
{
    public bool isEntered;
    public int CharacterID;
    public CheckPointEvent(bool isEntered, int CharacterID)
    {
        this.isEntered = isEntered;
        this.CharacterID = CharacterID;
    }


}
