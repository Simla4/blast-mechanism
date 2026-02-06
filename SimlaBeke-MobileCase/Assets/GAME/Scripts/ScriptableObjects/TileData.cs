using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/Tile Data")]
public class TileData : ScriptableObject
{
    public Sprite tileIcon;
    public string tileId;
    public TileTypes tileType;
    public Color tileColor;
    public TileBase tilePrefab;
}
