using System;
using Base.UI;
using System.Collections;
using System.Collections.Generic;
using sb.eventbus;
using UnityEngine;

public class WinPanelControl : MonoBehaviour
{
    EasyPanel uiPanel;
    EasyPanel UiPanel { get { return (uiPanel == null) ? uiPanel = GetComponent<EasyPanel>() : uiPanel; } }
    
    private EventListener<LevelCompletedEvent> onLevelCompleted;
    
    private void OnEnable()
    {
        UiPanel.HidePanel();

        onLevelCompleted = new EventListener<LevelCompletedEvent>(ShowWinPanel);
        EventBus<LevelCompletedEvent>.AddListener(onLevelCompleted);
    }

    private void OnDisable()
    {
        EventBus<LevelCompletedEvent>.RemoveListener(onLevelCompleted);
    }

    private void ShowWinPanel(LevelCompletedEvent e)
    {
        if (e.isLevelSuccess)
        {
            UIManager.Instance.HideAllPanels();
            UIManager.Instance.ShowPanel(PanelID.WinPanel);
        }
    }

    public void HideWinPanel()
    {
        UIManager.Instance.HideAllPanels();
        UIManager.Instance.HidePanel(PanelID.WinPanel);
    }
}
