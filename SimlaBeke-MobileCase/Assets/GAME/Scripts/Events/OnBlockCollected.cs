using sb.eventbus;

public class OnBlockCollected : IEvent
{
    public TileBase tile;
    public TileData tileData;

    public OnBlockCollected(TileBase tile, TileData tileData)
    {
        this.tile = tile;
        this.tileData = tileData;
    }
}
