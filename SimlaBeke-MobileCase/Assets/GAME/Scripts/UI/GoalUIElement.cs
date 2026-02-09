using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GoalUIElement : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image tickImage;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private ParticleSystem particle;
    
    private Tween punchTween;
    private TileData tileData;
    private bool isDone = false;
    
    public string TargetTileId { get; private set; }

    private void Start()
    {
        particle.Stop();
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
        if(isDone) return;
        
        if (punchTween != null)
        {
            punchTween.Kill(true);
        }

        if (remaining <= 0)
        {
            countText.gameObject.SetActive(false);
            tickImage.gameObject.SetActive(true);
            isDone = true;
            return;
        }
        
        countText.text = remaining.ToString();
        
        punchTween = transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        
        DOVirtual.DelayedCall((FloatingBlockManager.Instance.GetBlockCount() * 0.1f)+ 0.5f, () =>
        {
            Debug.Log(FloatingBlockManager.Instance.GetBlockCount());
            
            particle.Play();
        });
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    public bool IsDone()
    {
        return isDone;
    }
}