public class MoveInputEvent : GameEvent
{
	public int CharacterID;
	public Direction direction;
    public MoveInputEvent(int CharacterID, Direction direction)
    {
        this.CharacterID = CharacterID;
        this.direction= direction;
    }


}
