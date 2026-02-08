using System;
using System.Collections;
using System.Collections.Generic;
using sb.eventbus;
using TMPro;
using UnityEngine;

public class LevelTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    
    private EventListener<LevelCompletedEvent> levelCompletedEvent;


    private void OnEnable()
    {
        levelCompletedEvent = new EventListener<LevelCompletedEvent>(ChangeLevelText);
        EventBus<LevelCompletedEvent>.AddListener(levelCompletedEvent);
    }

    private void OnDisable()
    {
        EventBus<LevelCompletedEvent>.RemoveListener(levelCompletedEvent);
    }

    private void Start()
    {
        ChangeLevelText(null);
    }

    private void ChangeLevelText(LevelCompletedEvent e)
    {
        levelText.text = $"Level {PlayerPrefs.GetInt("levelIndex") + 1}";
    }
}
