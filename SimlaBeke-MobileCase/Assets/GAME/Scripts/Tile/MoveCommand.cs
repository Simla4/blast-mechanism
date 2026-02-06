using DG.Tweening;
using UnityEngine;

public class MoveCommand : ICommand
{
    private TileBase tile;
    private Vector2Int targetGridPos;
    private GridManager gridManager;
    private Tween moveTween;
    
    public float Duration => 0.25f;

    public MoveCommand(TileBase tile, Vector2Int targetGridPos, GridManager gridManager)
    {
        this.tile = tile;
        this.targetGridPos = targetGridPos;
        this.gridManager = gridManager;
    }

    public Tween Execute()
    {
        if (moveTween != null)
        {
            moveTween.Kill();
        }
        
        Vector3 targetWorldPos = gridManager.GetWorldPosition(targetGridPos);
        
        moveTween = tile.transform.DOMove(targetWorldPos, Duration)
            .SetEase(Ease.OutBounce);

        return moveTween;
    }
}