using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalUIElement : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image tickImage;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private ParticleSystem particleSystem;
    
    private Tween punchTween;
    private TileData tileData;
    
    public string TargetTileId { get; private set; }

    private void Start()
    {
        particleSystem.Stop();
    }

    public void Initialize(TileData tileData, int startCount)
    {
        TargetTileId = tileData.tileId;
        iconImage.sprite = tileData.tileIcon;
        this.tileData = tileData;
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

        particleSystem.Play();
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }
}