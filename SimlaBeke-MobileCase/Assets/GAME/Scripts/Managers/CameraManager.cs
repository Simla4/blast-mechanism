using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Refferances")]
    [SerializeField] private SpriteRenderer borderSpriteRenderer;

    private void Awake()
    {
        ChangeCameraSize();
        
        transform.position = new Vector3(borderSpriteRenderer.size.x / 2, borderSpriteRenderer.size.y / 2, transform.position.z);
    }

    private void ChangeCameraSize()
    {
        var borderWidth = LevelManager.Instance.GetLevelData().gridWidth;

        Camera.main.orthographicSize = borderWidth + 1;
    }
}
