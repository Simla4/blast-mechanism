using System.Numerics;
using sb.eventbus;

public class OnTileCollectedEvent : IEvent
{
    public TileData tileData;
    public Vector3 tilePosition;

    public OnTileCollectedEvent(TileData tileData, Vector3 tilePosition)
    {
        this.tileData = tileData;
        this.tilePosition = tilePosition;
    }
}
