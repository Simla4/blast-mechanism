using DG.Tweening;
using Lean.Pool;
using sb.eventbus;
using UnityEngine;

public class BlockAnimation : MonoBehaviour, IPoolable
{
    [SerializeField] private float duration;
    [SerializeField] private Transform coinParent;
    [SerializeField] private SpriteRenderer spriteRenderer;
    //[SerializeField] private float coinSpawnOffset = 2;


    public void SetTarget(TileData tileData, RectTransform targetPos, Canvas targetCanvas)
    {
        spriteRenderer.sprite = tileData.tileIcon;
        MoveCoin(targetPos, targetCanvas);
    }
    
    
    private void MoveCoin(RectTransform targetPos, Canvas targetCanvas)
    {
        var spawnPos = transform.position;
        
        transform.position = WorldToUISpace(spawnPos + Vector3.up, targetCanvas);
        transform.DOMove(targetPos.position, duration).OnComplete(() =>
        {
            LeanPool.Despawn(gameObject);
            Debug.Log("obje spawn oldu");
        });
    }

    private Vector3 WorldToUISpace(Vector3 worldPosition, Canvas targetCanvas)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);            
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetCanvas.transform as RectTransform, screenPos, targetCanvas.worldCamera, out Vector2 localPoint);
        return targetCanvas.transform.TransformPoint(localPoint);
    }

    public void OnSpawn()
    {
        EventBus<OnBlockAnimationCreatedEvent>.Emit(new OnBlockAnimationCreatedEvent(this));
    }

    public void OnDespawn()
    {
    }
}