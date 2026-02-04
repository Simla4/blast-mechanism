using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]

public class LevelData : ScriptableObject
{
    public int gridHeight;
    public int gridWidth;
    public int moveCount;
    public List<TileData> tiles;
    public List<TileData> spawnableTileTypes;
}
