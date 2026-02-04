using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Refferances")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private SpriteRenderer borderSpriteRenderer;
    [SerializeField] private Transform tilesParent;

    [Header("Grid Properties")] 
    [SerializeField] private float borderSpacingX = 0.11f;
    [SerializeField] private float borderSpacingY = 0.21f;
    
    private Pool<TileBase> blockPool;
    private TileBase[,] gridArray;
    private FloodFillService floodFill;


    private void Awake()
    {
        ChangeBorderSize();
    }

    private void Start()
    {
        blockPool = PoolManager.Instance.blockPool;
        gridArray = new TileBase[levelData.gridWidth, levelData.gridHeight];
        
        CreateBord();
    }

    private void ChangeBorderSize()
    {
        borderSpriteRenderer.size = new Vector2(levelData.gridWidth + borderSpacingX, levelData.gridHeight + borderSpacingY);
    }

    private void CreateBord()
    {
        for (int i = 0; i < levelData.tiles.Count; i++)
        {
            int x = i % levelData.gridWidth;
            int y = i / (levelData.gridWidth);
            
            Vector2Int position = new Vector2Int(x, y);
            var newTile = blockPool.Spawn(position, levelData.tiles[i]);
            newTile.transform.SetParent(tilesParent);
            gridArray[x, y] = newTile;
        }
        
        floodFill = new FloodFillService(gridArray);
    }
    
    public void OnBlockClicked(Vector2Int position)
    {
        var foundTiles = floodFill.Find(position);

        if (foundTiles != null)
        {
            DestroyBlocks(foundTiles);
        }
    }

    private void DestroyBlocks(List<TileBase> foundTiles)
    {
        if (foundTiles.Count >= 2)
        {
            for (int i = 0; i < foundTiles.Count; i++)
            {
                gridArray[foundTiles[i].TilePosition.x, foundTiles[i].TilePosition.y] = null;
            
                blockPool.ReturnToPool(foundTiles[i]);
                Debug.Log(foundTiles[i] + " returned to pool");
            }
        
            DropTiles();
            
            FillGrid();
        }
    }
    
    private void DropTiles()
    {
        int width = gridArray.GetLength(0);
        int height = gridArray.GetLength(1);

        // Her bir sütun için tek tek bak
        for (int x = 0; x < width; x++)
        {
            int nextEmptyY = 0;

            for (int y = 0; y < height; y++)
            {
                // Eğer o hücrede taş varsa
                if (gridArray[x, y] != null)
                {
                    // Taş olması gereken yerden yukarıdaysa kaydır
                    if (y != nextEmptyY)
                    {
                        TileBase movingTile = gridArray[x, y];
                        
                        var movementDistance = movingTile.TilePosition.y - nextEmptyY;

                        // Veriyi güncelle
                        gridArray[x, nextEmptyY] = movingTile;
                        gridArray[x, y] = null;
                    
                        // Taşa yeni yerini söyle ve görseli güncelle
                        movingTile.TilePosition = new Vector2Int(x, nextEmptyY);

                        var pos = movingTile.transform.position;
                        movingTile.transform.position = new Vector3(pos.x, pos.y - movementDistance, 0);
                    }
                    nextEmptyY++;
                }
            }
        }
    }
    
    private void FillGrid()
    {
        int width = gridArray.GetLength(0);
        int height = gridArray.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Eğer hücre boşsa (null ise) yeni taş lazım demektir
                if (gridArray[x, y] == null)
                {
                    // 1. Rastgele bir TileData seç (LevelData içinden)
                    TileData randomData = levelData.spawnableTileTypes[UnityEngine.Random.Range(0, levelData.spawnableTileTypes.Count)];

                    // 2. Havuzdan spawn et
                    // Pool sistemin ID (string) bekliyorsa randomData.tileId gönderiyoruz
                    Vector2Int position = new Vector2Int(x, y);
                    var newTile = blockPool.Spawn(position, randomData);

                    // 3. Veriyi enjekte et (Önceki adımda konuştuğumuz gibi)
                    newTile.OnSpawned(position, randomData);
                
                    // 4. GridManager referanslarını güncelle
                    newTile.transform.SetParent(tilesParent);
                    gridArray[x, y] = newTile;

                    // 5. Görsel konumlandırma (Opsiyonel: height + 1 yaparak tepeden düşürebilirsin)
                    // Şimdilik TileBase içindeki PlaceTile zaten position'a göre yerleştiriyor.
                }
            }
        }
    }
}
