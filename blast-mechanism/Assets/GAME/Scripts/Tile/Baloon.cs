using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using sb.eventbus;
using Unity.VisualScripting;
using UnityEngine;

public class Baloon : TileBase, IExplodable
{
    Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    
    private Pool<TileBase> tilePool;

    public void OnNeighborExploded()
    {
        if (isDestroyed) return;
        
        isDestroyed = true;
        
        SoundManager.PlaySound("balloon");
        
        var particle = LeanPool.Spawn(blastParticle, transform.position, Quaternion.identity);
        if (particle.TryGetComponent<BlockParticle>(out BlockParticle blockParticle))
        {
            blockParticle.Init(tileData.tileColor);
            particle.SetActive(true);
        }
        
        EventBus<OnBlockCollected>.Emit(new OnBlockCollected(tileData));
        
        tilePool = PoolManager.Instance.GetPool(GetTileID());
        tilePool.ReturnToPool(this);
    }
}

