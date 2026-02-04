using sb.eventbus;

public class OnBlockExplodedEvent : IEvent
{ 
    public TileBase tile;

    public OnBlockExplodedEvent(TileBase tile)
    {
        this.tile = tile;
    }
}
