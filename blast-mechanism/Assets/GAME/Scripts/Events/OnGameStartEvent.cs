
using System.Collections.Generic;
using sb.eventbus;

public class OnGameStartEvent : IEvent
{
    public int moves;
    public List<LevelGoals> levelGoals;

    public OnGameStartEvent(int moves, List<LevelGoals> levelGoals)
    {
        this.moves = moves;
        this.levelGoals = levelGoals;
    }
}
