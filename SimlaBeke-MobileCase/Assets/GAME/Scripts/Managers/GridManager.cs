using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    private EventListener<OnClickedTileEvent> onClickedTile;
    private OnAnyBlockFallEvent onAnyBlockFall = new OnAnyBlockFallEvent();


    private void OnEnable()
    {
        
        onBlockCollected = new EventListener<OnBlockCollected>(HandleTileCollection);
        EventBus<OnBlockCollected>.AddListener(onBlockCollected);

        onClickedTile = new EventListener<OnClickedTileEvent>(OnBlockClicked);
        EventBus<OnClickedTileEvent>.AddListener(onClickedTile);
    }
    
    private void OnDisable()
    {
        EventBus<OnBlockCollected>.RemoveListener(onBlockCollected);
        EventBus<OnClickedTileEvent>.RemoveListener(onClickedTile);
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
    
    public void OnBlockClicked(OnClickedTileEvent e)
    {
        var foundTiles = floodFill.Find(e.position);

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
    
        // 1. Bir Sequence oluştur (Animasyonları paketlemek için)
        Sequence dropSequence = DOTween.Sequence();
        bool hasMovement = false;

        for (int x = 0; x < width; x++)
        {
            int nextEmptyY = 0;
            for (int y = 0; y < height; y++)
            {
                if (gridArray[x, y] != null)
                {
                    if (y != nextEmptyY)
                    {
                        TileBase movingTile = gridArray[x, y];
                        var movementDistance = movingTile.TilePosition.y - nextEmptyY;

                        // --- MANTIKSAL GÜNCELLEME ---
                        gridArray[x, nextEmptyY] = movingTile;
                        gridArray[x, y] = null;
                        movingTile.SetTilePosition(new Vector2Int(x, nextEmptyY));

                        // --- KOMUT OLUŞTURMA VE SEQUENCE'A EKLEME ---
                        var pos = movingTile.transform.position;
                        
                        var cmd = new MoveCommand(movingTile, new Vector3(pos.x, pos.y - movementDistance, 0));
                    
                        // Join: Tüm hareketler aynı anda başlar
                        dropSequence.Join(cmd.Execute()); 
                    
                        hasMovement = true;
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
    
        Sequence fillSequence = DOTween.Sequence();
        bool hasNewTiles = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gridArray[x, y] == null)
                {
                    TileData randomData = levelData.spawnableTileTypes[UnityEngine.Random.Range(0, levelData.spawnableTileTypes.Count)];
                
                    // 1. Koordinatları belirle
                    Vector2Int targetPos = new Vector2Int(x, y);
                    // Başlangıç noktası: Gridin üst sınırı + biraz ofset (Gökten düşme efekti)
                    Vector3 spawnWorldPos = new Vector3(x + 0.61f, height + 1.5f, 0); 

                    // 2. Havuzdan çek ve konumlandır
                    var blockPool = PoolManager.Instance.GetPool(randomData.tileId);
                    var newTile = blockPool.Spawn(targetPos, randomData);
                    newTile.transform.position = spawnWorldPos; // Işınla (başlangıç noktasına)
                    newTile.transform.SetParent(tilesParent);
                    gridArray[x, y] = newTile;

                    // 3. KOMUTU ÇALIŞTIR VE SEQUENCE'A EKLE
                    // MoveCommand zaten transform.DOMove yaptığı için targetPos'a süzülecektir
                    var cmd = new MoveCommand(newTile, new Vector3(targetPos.x + 0.61f, targetPos.y +0.7f, 0));
                    fillSequence.Join(cmd.Execute());
                
                    hasNewTiles = true;
                }
            }
        }

        if (hasNewTiles)
        {
            fillSequence.OnComplete(() => 
            {
                // Yeni taşlar yerleşti, şimdi tekrar bir eşleşme kontrolü yapılabilir
                Debug.Log("Yeni taşlar yerleşti.");
                EventBus<OnAnyBlockFallEvent>.Emit(onAnyBlockFall);
                // Eğer yeni gelen taşlar da patlayabiliyorsa burada bir kontrol tetiklenebilir
            });
        }
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
