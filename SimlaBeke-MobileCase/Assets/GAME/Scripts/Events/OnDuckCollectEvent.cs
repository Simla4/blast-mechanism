using System.Collections;
using System.Collections.Generic;
using sb.eventbus;
using UnityEngine;

public class OnDuckCollectEvent : IEvent
{
    public TileBase tile;

    public OnDuckCollectEvent(TileBase tile)
    {
        this.tile = tile;
    }
}
