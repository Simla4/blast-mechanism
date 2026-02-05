
using sb.eventbus;
using UnityEngine;

public class Block : TileBase, IMatchable
{
    public void ExplodeTile()
    {
        Debug.Log("Block exploded");
        //TO-DO: kendi pozisyonunu gödenerecek ve ben patladım Event'i tetiklenecek.(particle için)
        //TO-DO: Particle burada patlatılacak. Particle da pool'a bağlanacak. Particle rengi Spawn içinden kontrol edilecek.
        
        EventBus<OnBlockCollected>.Emit(new OnBlockCollected(this, tileData));
    }
}
