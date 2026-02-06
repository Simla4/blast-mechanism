
using sb.eventbus;
using UnityEngine;

public class Duck : TileBase
{
    private EventListener<OnAnyBlockFallEvent> onChangeAnyBlockFall;
    protected override void OnEnable()
    {
        base.OnEnable();
        
        onChangeAnyBlockFall = new EventListener<OnAnyBlockFallEvent>(CheckCollectionCondition);
        EventBus<OnAnyBlockFallEvent>.AddListener(onChangeAnyBlockFall);
    }
    
    private void OnDisable()
    {
        EventBus<OnAnyBlockFallEvent>.RemoveListener(onChangeAnyBlockFall);
    }

    private void CheckCollectionCondition(OnAnyBlockFallEvent e)
    {
        if (TilePosition.y == 0)
        {
            
            var blockPool = PoolManager.Instance.GetPool(GetTileID());
            blockPool.ReturnToPool(this);
            
            SoundManager.PlaySound("duck");
            
            EventBus<OnDuckCollectEvent>.Emit(new OnDuckCollectEvent(this));
        }
    }
}
