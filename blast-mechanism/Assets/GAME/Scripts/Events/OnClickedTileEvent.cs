using sb.eventbus;
using UnityEngine;

public class OnClickedTileEvent : IEvent
{
    public Vector2Int position;

    public OnClickedTileEvent(Vector2Int position)
    {
        this.position = position;
    }
}
