using System;
using System.Diagnostics.Tracing;
using sb.eventbus;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer borderSpriteRenderer;

    EventListener<OnGameStartEvent> onGameStart;

    private void OnEnable()
    {
        onGameStart = new EventListener<OnGameStartEvent>(ChangeCameraSize);
        EventBus<OnGameStartEvent>.AddListener(onGameStart);
    }

    private void OnDisable()
    {
        EventBus<OnGameStartEvent>.RemoveListener(onGameStart);
    }

    private void ChangeCameraSize(OnGameStartEvent e)
    {
        Camera.main.orthographicSize = borderSpriteRenderer.size.x + 1f;
        transform.position = new Vector3(borderSpriteRenderer.size.x / 2, borderSpriteRenderer.size.y / 2, transform.position.z);

    }
}
