
using DG.Tweening;
using sb.eventbus;
using UnityEngine;

public class Duck : TileBase
{
    private EventListener<OnAnyBlockFallEvent> onChangeAnyBlockFall;
    private Tween scaleTween;
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
            if (scaleTween != null)
            {
                scaleTween.Kill();
            }
            
            scaleTween = transform.DOScale(0, 0.2f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        EventBus<OnBlockCollected>.Emit(new OnBlockCollected(tileData));
                        
                        var blockPool = PoolManager.Instance.GetPool(GetTileID());
                        blockPool.ReturnToPool(this);
            
                        SoundManager.PlaySound("duck");
                        
                        EventBus<OnDuckCollectEvent>.Emit(new OnDuckCollectEvent(this));
                    });
        }
    }
}
