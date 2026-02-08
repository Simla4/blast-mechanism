using UnityEngine;

public class EditorTile : MonoBehaviour
{
    public Vector2Int cell;
    public TileData tileData;

#if UNITY_EDITOR
    private void OnValidate()
    {
        transform.position = new Vector3(cell.x, cell.y, 0);
    }
#endif
}