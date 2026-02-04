using System;
using System.Collections;
using System.Collections.Generic;
using sb.eventbus;
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
    
    
    private TileBase[,] gridArray;
    private FloodFillService floodFill;
    private EventListener<OnBlockCollected> onBlockCollected;
    private OnAnyBlockFallEvent onAnyBlockFall = new OnAnyBlockFallEvent();


    private void OnEnable()
    {
        
        onBlockCollected = new EventListener<OnBlockCollected>(HandleTileCollection);
        EventBus<OnBlockCollected>.AddListener(onBlockCollected);
    }
    
    private void OnDisable()
    {
        EventBus<OnBlockCollected>.RemoveListener(onBlockCollected);
    }
    
    private void Awake()
    {
        ChangeBorderSize();
    }

    private void Start()
    {
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
            var blockPool = PoolManager.Instance.GetPool(levelData.tiles[i].tileId);
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
                var foundTile = foundTiles[i];
                
                NotifyNeighbors(foundTile.TilePosition);
                gridArray[foundTile.TilePosition.x, foundTile.TilePosition.y] = null;
            
                PoolManager.Instance.GetPool(foundTile.GetTileID()).ReturnToPool(foundTile);
            }
        
            DropTiles();
        }
    }
    
    public void NotifyNeighbors(Vector2Int explodedPos)
    {
        Vector2Int[] neighbors = {
            explodedPos + Vector2Int.up,
            explodedPos + Vector2Int.down,
            explodedPos + Vector2Int.left,
            explodedPos + Vector2Int.right
        };

        foreach (var targetPos in neighbors)
        {
            // 1. Grid sınırları içinde mi?
            if (IsInsideGrid(targetPos))
            {
                TileBase neighborTile = gridArray[targetPos.x, targetPos.y];

                // 2. Orada bir taş var mı ve bu taş patlamayla ilgileniyor mu?
                if (neighborTile != null && neighborTile is IExplodable listener)
                {
                    // 3. Kararı ona bırakıyoruz!
                    listener.OnNeighborExploded();
                    gridArray[targetPos.x, targetPos.y] = null;
                }
            }
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
        
        FillGrid();
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
                    
                    var blockPool = PoolManager.Instance.GetPool(randomData.tileId);
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
        
        EventBus<OnAnyBlockFallEvent>.Emit(onAnyBlockFall);
    }
    
    private void HandleTileCollection(OnBlockCollected e)
    {
        var tilePos = e.tile.TilePosition;
        
        gridArray[tilePos.x, tilePos.y] = null;
        var blockPool = PoolManager.Instance.GetPool(e.tile.GetTileID());
        blockPool.ReturnToPool(e.tile);
        
        DropTiles();
    }
    
    private bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < levelData.gridWidth && pos.y >= 0 && pos.y < levelData.gridHeight;
    }
}
