using sb.eventbus;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
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
}