using System.Collections;
using System.Collections.Generic;
using sb.eventbus;
using Unity.VisualScripting;
using UnityEngine;

public class Baloon : TileBase, IExplodable
{
    Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    private bool _isDestroyed = false;
    private Pool<TileBase> tilePool;

    public void OnNeighborExploded()
    {
        if (_isDestroyed) return;
        
        _isDestroyed = true;
        
        
        
        tilePool = PoolManager.Instance.GetPool(tileData.tileId);
        tilePool.ReturnToPool(this);
        
        // Sadece görsel/ses efektini burada yap.
        // Pool'a dönme ve Event işini GridManager halledecek.
        // NOT: GridManager'da zaten Pool'a dönüyor, burada yaparsan hata alırsın.
    }
}

