using Base.UI;
using sb.eventbus;
using UnityEngine;

public class LosePanelControl : MonoBehaviour
{
    EasyPanel uiPanel;
    EasyPanel UiPanel { get { return (uiPanel == null) ? uiPanel = GetComponent<EasyPanel>() : uiPanel; } }
    
    private EventListener<LevelCompletedEvent> onLevelCompleted;
    
    private void OnEnable()
    {
        UiPanel.HidePanel();

        onLevelCompleted = new EventListener<LevelCompletedEvent>(ShowLosePanel);
        EventBus<LevelCompletedEvent>.AddListener(onLevelCompleted);
    }

    private void OnDisable()
    {
        EventBus<LevelCompletedEvent>.RemoveListener(onLevelCompleted);
    }
    
    private void ShowLosePanel(LevelCompletedEvent e)
    {
        if (!e.isLevelSuccess)
        {
            UIManager.Instance.HideAllPanels();
            UIManager.Instance.ShowPanel(PanelID.LosePanel);
        }
    }
}
