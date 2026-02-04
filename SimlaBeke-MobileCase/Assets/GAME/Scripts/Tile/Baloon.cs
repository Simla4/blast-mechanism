using System.Collections;
using System.Collections.Generic;
using sb.eventbus;
using UnityEngine;

public class Baloon : TileBase, IFallable, IExplodable
{
    Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    private bool _isDestroyed = false;
    private Pool<TileBase> tilePool;

    public void OnNeighborExploded()
    {
        // Karar mekanizması burada: 
        // Eğer zaten patlıyorsa veya özel bir şartın varsa (örn: 2 can) burada kontrol edersin.
        if (_isDestroyed) return;

        ExecuteExplosion();
    }

    private void ExecuteExplosion()
    {
        _isDestroyed = true;

        // 2. Efektini oynat
        Debug.Log("Balon: Komşum patladı, ben de veda ediyorum!");
        
        // 3. Havuza geri dön
        tilePool = PoolManager.Instance.GetPool(tileData.tileId);
        tilePool.ReturnToPool(this);
    }
}

