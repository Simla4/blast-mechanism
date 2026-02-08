using System;
using System.Collections;
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

    private EventListener<OnMoveCountChnagedEvent> onClickedTileEvent;
    private Tween moveTween;

    private void OnEnable()
    {
        onClickedTileEvent = new EventListener<OnMoveCountChnagedEvent>(TryToSpawnFloatingBlock);
        EventBus<OnMoveCountChnagedEvent>.AddListener(onClickedTileEvent);
    }

    private void OnDisable()
    {
        EventBus<OnMoveCountChnagedEvent>.RemoveListener(onClickedTileEvent);
    }

    private void TryToSpawnFloatingBlock(OnMoveCountChnagedEvent e)
    {
        StartCoroutine(TryToSpawnFloatingBlockNumerator());
    }

    private IEnumerator TryToSpawnFloatingBlockNumerator()
    {
        
        var explodableTiles = GridManager.Instance.GetExplodableTiles();

        for (int i = 0; i < explodableTiles.Count; i++)
        {
            if(!IsGoal(explodableTiles[i])) continue;
            
            Debug.Log(explodableTiles.Count);

            var uiElement = UIManager.Instance.GetGoalUIElement(explodableTiles[i].GetTileID());
            var spawnPosition = WorldToUISpace(explodableTiles[i].transform.position);

            var floatingBlock = LeanPool.Spawn(floatingBlockPrefab, spawnPosition, Quaternion.identity, canvas.transform);
            floatingBlock.GetComponent<BlockAnimation>().Initialize(explodableTiles[i].GetTileData(), uiElement);
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool IsGoal(TileBase tile)
    {
        var goals = LevelManager.Instance.GetGoals();

        foreach (var goal in goals)
        {
            if (goal.goalType.tileType == TileTypes.Cube && goal.goalType.tileId == tile.GetTileID())
            {
                Debug.Log("goal type: " + goal.goalType.tileType  + ", goal id: " + goal.goalType.tileId + ", tile type: " + tile.GetTileData().tileType + ", tile id: " + tile.GetTileData().tileId);
                return true;
            }
        }

        return false;
    }
    
    private Vector3 WorldToUISpace(Vector3 worldPosition)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);            
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPos, canvas.worldCamera, out Vector2 localPoint);
        return canvas.transform.TransformPoint(localPoint);
    }
}
