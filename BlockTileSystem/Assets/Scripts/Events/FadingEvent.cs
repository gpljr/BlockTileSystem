public class FadingEvent : GameEvent
{
    public bool isFading;
    public bool fadeIn;
    public FadingEvent(bool isFading,bool fadeIn)
    {
        this.isFading = isFading;
        this.fadeIn= fadeIn;
    }


}
