using sb.eventbus;

public class ChangeMoveCountUIEvent : IEvent
{
    public int newValue;

    public ChangeMoveCountUIEvent(int newValue)
    {
        this.newValue = newValue;
    }
}
