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
        Debug.Log("grid width: " + grid.GetLength(0) + " grid height: " + grid.GetLength(1) + " target grid: " + targetGrid);
        
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
        
        Debug.Log("Toplam " + foundTiles.Count + " eşleşen tile bulundu ve gizleniyor.");
    
        
        // TO-DO: Burası ayrılacka. Ayrı methoda alıncak. Oyun tekrar incelenip duruma göre coroutine de eklenecek.
        
        return foundTiles;
    }

    public void DropTiles(List<TileBase> foundTiles)
    {
        // 1. Önce patlayanları grid'den sil
        foreach (var tile in foundTiles)
        {
            grid[tile.TilePosition.x, tile.TilePosition.y] = null;
        }

        // 2. Sadece etkilenen sütunları al
        HashSet<int> columns = new HashSet<int>();
        foreach (var tile in foundTiles) columns.Add(tile.TilePosition.x);

        foreach (int x in columns)
        {
            int height = grid.GetLength(1);
            int nextEmptyY = 0; // Bu sütunda bulduğumuz en alttaki boşluğun indexi

            // Sütunu aşağıdan yukarıya doğru tara
            for (int y = 0; y < height; y++)
            {
                // Eğer hücre doluysa
                if (grid[x, y] != null)
                {
                    // Eğer bu taş, olması gereken yerden (nextEmptyY) daha yukarıdaysa aşağı kaydır
                    if (y != nextEmptyY)
                    {
                        TileBase movingTile = grid[x, y];

                        // --- VERİ GÜNCELLEME ---
                        grid[x, nextEmptyY] = movingTile;
                        grid[x, y] = null;
                        var movementDistance = movingTile.TilePosition.y - nextEmptyY;
                        movingTile.TilePosition = new Vector2Int(x, nextEmptyY);

                        // --- GÖRSEL GÜNCELLEME ---
                        var pos = movingTile.transform.position;
                        movingTile.transform.position = new Vector3(pos.x, pos.y - movementDistance, 0);
                    }
                
                    // Bir sonraki boş yer, bu dolu taşın hemen üstü olacak
                    nextEmptyY++;
                }
            }
        }
    }
}