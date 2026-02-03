
using UnityEngine;

public class Block : TileBase, IMatchable
{
    public void ExplodeTile()
    {
        Debug.Log("Block exploded");
    }
}
