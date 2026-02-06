using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalUIElement : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    
    public string TargetTileId { get; private set; }

    public void Initialize(string id, Sprite icon, int startCount)
    {
        TargetTileId = id;
        iconImage.sprite = icon;
        UpdateUI(startCount);
    }

    public void UpdateUI(int remaining)
    {
        countText.text = remaining <= 0 ? "✔" : remaining.ToString();
        transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
    }
}