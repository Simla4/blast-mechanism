using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
using sb.eventbus;
using UnityEngine;

public class FloatingBlockManager : MonoSingleton<FloatingBlockManager>
{
    [Header("References")]
    [SerializeField] private GameObject floatingBlockPrefab;
    [SerializeField] private Canvas canvas;

    private EventListener<OnExplotedTileListUpeded> onClickedTileEvent;
    private List<TileBase> validGoals = new List<TileBase>();
    private Tween moveTween;

    private void OnEnable()
    {
        onClickedTileEvent = new EventListener<OnExplotedTileListUpeded>(TryToSpawnFloatingBlock);
        EventBus<OnExplotedTileListUpeded>.AddListener(onClickedTileEvent);
    }

    private void OnDisable()
    {
        EventBus<OnExplotedTileListUpeded>.RemoveListener(onClickedTileEvent);
    }

    private void TryToSpawnFloatingBlock(OnExplotedTileListUpeded e)
    {
        StartCoroutine(TryToSpawnFloatingBlockNumerator());
    }

    private IEnumerator TryToSpawnFloatingBlockNumerator()
    {
        var explodableTiles = GridManager.Instance.GetExplodableTiles();
        
        validGoals.Clear();
        validGoals = explodableTiles
            .Where(t => t.GetTileData().tileType == TileTypes.Cube && IsGoal(t))
            .ToList();

        for (int i = 0; i < validGoals.Count; i++)
        {
            var uiElement = UIManager.Instance.GetGoalUIElement(validGoals[i].GetTileID());

            if (uiElement.IsDone())
            {
                break;
            }
            
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

    public int GetBlockCount()
    {
        return validGoals.Count;
    }
}
