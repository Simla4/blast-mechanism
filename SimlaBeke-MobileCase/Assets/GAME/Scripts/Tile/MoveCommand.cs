using DG.Tweening;
using UnityEngine;

public class MoveCommand : ICommand
{
    private TileBase tile;
    private Vector3 targetPos;
    private Tween moveTween;
    
    public float Duration => 0.3f;

    public MoveCommand(TileBase tile, Vector3 targetPos)
    {
        this.tile = tile;
        this.targetPos = targetPos;
    }

    public Tween Execute()
    {
        if (moveTween != null)
        {
            moveTween.Kill();
        }
        moveTween = tile.transform.DOMove(new Vector3(targetPos.x, targetPos.y, 0), 0.3f)
            .SetEase(Ease.OutBounce);

        return moveTween;
    }
}