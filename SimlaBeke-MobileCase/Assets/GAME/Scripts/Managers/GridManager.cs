using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using sb.eventbus;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    [Header("Refferances")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private SpriteRenderer borderSpriteRenderer;
    [SerializeField] private Transform tilesParent;

    [Header("Grid Properties")] 
    [SerializeField] private Vector2 padding = new Vector2(0.11f, 0.21f);
    [SerializeField] private float cellSize = 1.0f; 
    [SerializeField] private float spawnYOffset = 1.5f;
    
    
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
        borderSpriteRenderer.size = new Vector2(levelData.gridWidth + padding.x, levelData.gridHeight + padding.y);
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
            newTile.transform.position = GetWorldPosition(position);
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
        Sequence dropSequence = DOTween.Sequence();

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            int nextEmptyY = 0;
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                if (gridArray[x, y] != null)
                {
                    if (y != nextEmptyY)
                    {
                        TileBase movingTile = gridArray[x, y];
                        gridArray[x, nextEmptyY] = movingTile;
                        gridArray[x, y] = null;
                        movingTile.SetTilePosition(new Vector2Int(x, nextEmptyY));

                        // Komut artık tertemiz: "Hangi taş, hangi grid koordinatına?"
                        var cmd = new MoveCommand(movingTile, movingTile.TilePosition, this);
                        dropSequence.Join(cmd.Execute());
                    }
                    nextEmptyY++;
                }
            }
        }
        FillGrid();
    }
    
    private void FillGrid()
    {
        Sequence fillSequence = DOTween.Sequence();
        bool hasNewTiles = false;

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                if (gridArray[x, y] == null)
                {
                    TileData randomData = levelData.spawnableTileTypes[Random.Range(0, levelData.spawnableTileTypes.Count)];
                    Vector2Int targetPos = new Vector2Int(x, y);

                    // Spawn Pozisyonu: Hedef X, Grid Tepesi + Ofset
                    Vector3 spawnWorldPos = GetWorldPosition(new Vector2Int(x, levelData.gridHeight));
                    spawnWorldPos.y += spawnYOffset;

                    var blockPool = PoolManager.Instance.GetPool(randomData.tileId);
                    var newTile = blockPool.Spawn(targetPos, randomData);
                    
                    newTile.transform.position = spawnWorldPos;
                    newTile.transform.SetParent(tilesParent);
                    gridArray[x, y] = newTile;

                    var cmd = new MoveCommand(newTile, targetPos, this);
                    fillSequence.Join(cmd.Execute());
                    hasNewTiles = true;
                }
            }
        }

        if (hasNewTiles)
        {
            fillSequence.OnComplete(() => EventBus<OnAnyBlockFallEvent>.Emit(onAnyBlockFall));
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
    
    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        // Formül: (Grid Koordinatı * Hücre Boyutu) + Board Boşluğu + (Hücre Boyutu / 2)
        // Bu formül sayesinde blok boyutu değişse de pivot Center'da kalsa da her şey milimetrik oturur.
        float x = (gridPos.x * cellSize) + padding.x + (cellSize / 2f);
        float y = (gridPos.y * cellSize) + padding.y + (cellSize / 2f);
        return new Vector3(x, y, 0);
    }
}
