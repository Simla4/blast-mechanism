using System;
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
    public List<LevelGoals> levelGoals;
    
    public void EnsureGrid()
    {
        int size = gridWidth * gridHeight;

        if (tiles == null)
            tiles = new List<TileData>();

        while (tiles.Count < size)
            tiles.Add(null);

        if (tiles.Count > size)
            tiles.RemoveRange(size, tiles.Count - size);
    }
}

[Serializable]
public class LevelGoals
{
    public int count;
    public TileData goalType;
}
