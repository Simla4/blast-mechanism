using sb.eventbus;

public class OnBlockCollected : IEvent
{
    public TileData tileData;

    public OnBlockCollected(TileData tileData)
    {
        this.tileData = tileData;
    }
}
