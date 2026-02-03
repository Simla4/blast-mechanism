using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    
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
        
        if (hit.collider.TryGetComponent<TileBase>(out TileBase tileBase))
        {
            Debug.Log("Block clicked. ID = " + tileBase.TilePosition);
            gridManager.OnBlockClicked(tileBase.TilePosition);
        }
    }
}