public class StayTriggerEvent : GameEvent
{
    public bool isEntered;
    public int triggerID;
    public StayTriggerEvent(bool isEntered, int triggerID)
    {
        this.isEntered = isEntered;
        this.triggerID = triggerID;
    }


}
