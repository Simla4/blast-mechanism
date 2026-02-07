using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

public class BlockAnimation : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Image iconImage;
    [SerializeField] private RectTransform rectTransform;


    public void Initialize(TileData tileData, GoalUIElement goalElement)
    {
        iconImage.sprite = tileData.tileIcon;
        MoveTowardsGoal(goalElement);
    }
    
    private void MoveTowardsGoal(GoalUIElement goalElement)
    {
        transform.DOMove(goalElement.transform.position, duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            SoundManager.PlaySound("cube_collect");
            LeanPool.Despawn(gameObject);
        });
        
        rectTransform.DOSizeDelta(goalElement.GetRectTransform().sizeDelta, duration).SetEase(Ease.InOutSine);
    }
}