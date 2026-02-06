
using sb.eventbus;
using UnityEngine;

public class Block : TileBase, IMatchable
{
    public void ExplodeTile()
    {
        SoundManager.PlaySound("cube_explode");
        
        Debug.Log(tileData.tileId + " exploded");
        
        var tilePool = PoolManager.Instance.GetPool(GetTileID());
        tilePool.ReturnToPool(this);
        
    }
}
