using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloodFillService
{
    private TileBase[,] grid;
    
    private List<Vector2Int> directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0)
    };

    public FloodFillService(TileBase[,] grid)
    {
        this.grid = grid;
    }

    public List<TileBase> Find(Vector2Int targetGrid)
    {
        if(!grid[targetGrid.x, targetGrid.y].TryGetComponent<IMatchable>(out IMatchable matchable)) return null;
        
        TileBase startTile = grid[targetGrid.x, targetGrid.y];
        
        if (targetGrid.x < 0 || targetGrid.x >= grid.GetLength(0) || 
            targetGrid.y < 0 || targetGrid.y >= grid.GetLength(1)) return null;
        
        List<TileBase> foundTiles = new List<TileBase>();
        HashSet<TileBase> visited = new HashSet<TileBase>();
    
        var targetID = startTile.GetTileID();
        foundTiles.Add(startTile);
        visited.Add(startTile);
        int index = 0;

        while (index < foundTiles.Count)
        {
            TileBase currentTile = foundTiles[index];

            for (int i = 0; i < directions.Count; i++)
            {
                Vector2Int neighborPos = currentTile.TilePosition + directions[i];

                if (neighborPos.x < 0 || neighborPos.x >= grid.GetLength(0) || 
                    neighborPos.y < 0 || neighborPos.y >= grid.GetLength(1))
                {
                    continue; 
                }

                TileBase neighborTile = grid[neighborPos.x, neighborPos.y];

                if (!visited.Contains(neighborTile) && neighborTile.GetTileID() == targetID)
                {
                    foundTiles.Add(neighborTile);
                    visited.Add(neighborTile);
                }
            }

            index++;
        }
        
        // TO-DO: Burası ayrılacka. Ayrı methoda alıncak. Oyun tekrar incelenip duruma göre coroutine de eklenecek.
        
        return foundTiles;
    }
}