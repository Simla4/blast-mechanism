using sb.eventbus;

public class ChangeGoalsUIEvent : IEvent
{
    public int newGoal;
    public TileData tileData;

    public ChangeGoalsUIEvent(int newGoal, TileData tileData)
    {
        this.newGoal = newGoal;
        this.tileData = tileData;
    }
}
