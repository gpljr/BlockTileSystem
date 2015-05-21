public class LevelStarEvent : GameEvent
{
    public bool isEntered;
    public int CharacterID;
    public LevelStarEvent(bool isEntered, int CharacterID)
    {
        this.isEntered = isEntered;
        this.CharacterID = CharacterID;
    }


}
