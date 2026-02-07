using System.Linq;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

public class FloatingBlockManager : MonoSingleton<FloatingBlockManager>
{
    [SerializeField] private GameObject floatingBlockPrefab;
    [SerializeField] private Canvas canvas;
    
    public void TryToSpawnFloatingBlock(Block block)
    {
        if(!IsGoal(block)) return;

        var uiElement = UIManager.Instance.GetGoalUIElement(block.GetTileID());
        var spawnPosition = WorldToUISpace(block.transform.position);
        
        var floatingBlock = LeanPool.Spawn(floatingBlockPrefab, spawnPosition, Quaternion.identity, canvas.transform);

        floatingBlock.GetComponent<BlockAnimation>().Initialize(block.GetTileData(), uiElement);
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
}
