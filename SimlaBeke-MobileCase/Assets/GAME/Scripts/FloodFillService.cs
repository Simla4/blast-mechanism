using System.Collections.Generic;
using System.Linq;
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

    public void Find(Vector2Int targetGrid)
    {
        Debug.Log("grid width: " + grid.GetLength(0) + " grid height: " + grid.GetLength(1) + " target grid: " + targetGrid);
        
        if(!grid[targetGrid.x, targetGrid.y].TryGetComponent<IMatchable>(out IMatchable matchable)) return;
        
        TileBase startTile = grid[targetGrid.x, targetGrid.y];
        
        if (targetGrid.x < 0 || targetGrid.x >= grid.GetLength(0) || 
            targetGrid.y < 0 || targetGrid.y >= grid.GetLength(1)) return;
        
        List<TileBase> foundTiles = new List<TileBase>();
        HashSet<TileBase> visited = new HashSet<TileBase>();
    
        var targetID = startTile.GetTileID();
        foundTiles.Add(startTile);
        visited.Add(startTile);
        int index = 0;
        
        foundTiles.Add(grid[targetGrid.x, targetGrid.y]);

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
        
        Debug.Log("Toplam " + foundTiles.Count + " eşleşen tile bulundu ve gizleniyor.");
    
        foreach (TileBase tile in foundTiles)
        {
            tile.gameObject.SetActive(false);
        }
    }
}