using System;
using System.Diagnostics.Tracing;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
using sb.eventbus;
using UnityEngine;

public class FloatingBlockManager : MonoSingleton<FloatingBlockManager>
{
    [SerializeField] private GameObject floatingBlockPrefab;
    [SerializeField] private Canvas canvas;

    private EventListener<OnClickedTileEvent> onClickedTileEvent;
    private int index = 0;

    private void OnEnable()
    {
        onClickedTileEvent = new EventListener<OnClickedTileEvent>(ResetIndex);
        EventBus<OnClickedTileEvent>.AddListener(onClickedTileEvent);
    }

    private void OnDisable()
    {
        EventBus<OnClickedTileEvent>.RemoveListener(onClickedTileEvent);
    }

    public void TryToSpawnFloatingBlock(Block block)
    {
        if(!IsGoal(block)) return;

        var uiElement = UIManager.Instance.GetGoalUIElement(block.GetTileID());
        var spawnPosition = WorldToUISpace(block.transform.position);

        DOVirtual.DelayedCall(index * 0.1f, () =>
        {
            var floatingBlock = LeanPool.Spawn(floatingBlockPrefab, spawnPosition, Quaternion.identity, canvas.transform);

            floatingBlock.GetComponent<BlockAnimation>().Initialize(block.GetTileData(), uiElement);
        });
        
        index++;
    }

    private bool IsGoal(Block block)
    {
        var goals = LevelManager.Instance.GetGoals();
        
        return goals.Any(x => x.goalType.tileId == block.GetTileID());
    }
    
    private Vector3 WorldToUISpace(Vector3 worldPosition)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);            
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPos, canvas.worldCamera, out Vector2 localPoint);
        return canvas.transform.TransformPoint(localPoint);
    }

    private void ResetIndex(OnClickedTileEvent e)
    {
        index = 0;
    }
}
