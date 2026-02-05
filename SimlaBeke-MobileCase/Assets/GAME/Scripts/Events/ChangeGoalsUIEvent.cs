using sb.eventbus;

public class ChangeGoalsUIEvent : IEvent
{
    public int newGoal;
    public TileData TileData;

    public ChangeGoalsUIEvent(int newGoal, TileData tileData)
    {
        this.newGoal = newGoal;
        this.TileData = tileData;
    }
}
