using sb.eventbus;

public class OnTileCollectedEvent : IEvent
{
    public TileData tileData;

    public OnTileCollectedEvent(TileData tileData)
    {
        this.tileData = tileData;
    }
}
