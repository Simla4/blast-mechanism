using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalUIElement : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image tickImage;
    
    private Tween punchTween;
    
    public string TargetTileId { get; private set; }

    public void Initialize(string id, Sprite icon, int startCount)
    {
        TargetTileId = id;
        iconImage.sprite = icon;
        UpdateUI(startCount);
    }

    public void UpdateUI(int remaining)
    {
        if (punchTween != null)
        {
            punchTween.Kill();
        }

        if (remaining <= 0)
        {
            countText.gameObject.SetActive(false);
            tickImage.gameObject.SetActive(true);
            return;
        }
        
        countText.text = remaining.ToString();
        
        punchTween = transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
    }
}