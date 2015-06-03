public class MergingStarEvent : GameEvent
{
    public bool isEntered;
    public int CharacterID;
    public MergingStarEvent(bool isEntered, int CharacterID)
    {
        this.isEntered = isEntered;
        this.CharacterID = CharacterID;
    }


}
