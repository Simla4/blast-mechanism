using sb.eventbus;

public class LevelCompletedEvent : IEvent
{
    public bool isLevelSuccess;

    public LevelCompletedEvent(bool isLevelSuccess)
    {
        this.isLevelSuccess = isLevelSuccess;
    }
}
