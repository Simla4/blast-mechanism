using sb.eventbus;

public class OnBlockCollected : IEvent
{
    public TileBase tile;

    public OnBlockCollected(TileBase tile)
    {
        this.tile = tile;
    }
}
