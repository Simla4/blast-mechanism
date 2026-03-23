using System;
using sb.eventbus;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool canTouchTiles = true;

    private EventListener<OnGameStartEvent> onGameStart;
    private EventListener<LevelCompletedEvent> onLevelCompleted;

    private void OnEnable()
    {
        onGameStart = new EventListener<OnGameStartEvent>(SetActiveInputSystem);
        EventBus<OnGameStartEvent>.AddListener(onGameStart);

        onLevelCompleted = new EventListener<LevelCompletedEvent>(SetDeactiveInputSystem);
        EventBus<LevelCompletedEvent>.AddListener(onLevelCompleted);
    }

    private void OnDisable()
    {
        EventBus<OnGameStartEvent>.RemoveListener(onGameStart);
        EventBus<LevelCompletedEvent>.RemoveListener(onLevelCompleted);
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(!canTouchTiles) return;
            
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider == null)
            return;


        if (hit.collider.TryGetComponent<IClickable>(out var clickable))
        {
            clickable.OnClickedTileEvent(false);
        }
    }

    private void SetActiveInputSystem(OnGameStartEvent e)
    {
        canTouchTiles = true;
    }
    
    private void SetDeactiveInputSystem(LevelCompletedEvent e)
    {
        canTouchTiles = false;
    }
}