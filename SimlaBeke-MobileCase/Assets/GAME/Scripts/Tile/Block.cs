
using Lean.Pool;
using sb.eventbus;
using UnityEngine;

public class Block : TileBase, IMatchable, IClickable
{
    public void ExplodeTile()
    {
        SoundManager.PlaySound("cube_explode");

        var particle = LeanPool.Spawn(blastParticle, transform.position, Quaternion.identity);
        if (particle.TryGetComponent<BlockParticle>(out BlockParticle blockParticle))
        {
            blockParticle.Init(tileData.tileColor);
        }
        
        EventBus<OnBlockCollected>.Emit(new OnBlockCollected(tileData));
        
        var tilePool = PoolManager.Instance.GetPool(GetTileID());
        tilePool.ReturnToPool(this);
    }

    public void OnClickedTileEvent(bool isChainReaction)
    {
        EventBus<OnClickedTileEvent>.Emit(new OnClickedTileEvent(TilePosition));

    }
}
