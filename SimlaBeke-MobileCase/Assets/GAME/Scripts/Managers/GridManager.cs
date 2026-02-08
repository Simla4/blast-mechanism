using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using sb.eventbus;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoSingleton<GridManager>
{
    [Header("Refferances")]
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private SpriteRenderer borderSpriteRenderer;
    [SerializeField] private Transform tilesParent;

    [Header("Grid Properties")] 
    [SerializeField] private Vector2 padding = new Vector2(0.11f, 0.21f);
    [SerializeField] private float cellSize = 1.0f; 
    [SerializeField] private float spawnYOffset = 1.5f;
    
    
    private TileBase[,] gridArray;
    private FloodFillService floodFill;
    private LevelData levelData;
    private List<TileBase> explodableTiles = new List<TileBase>();
    private EventListener<OnDuckCollectEvent> onDuckCollected;
    private EventListener<OnClickedTileEvent> onClickedTile;
    private EventListener<OnRocketActivated> onRocketActivated;

    private OnAnyBlockFallEvent onAnyBlockFall = new OnAnyBlockFallEvent();
    private OnMoveCountChnagedEvent onMoveCountChnaged = new OnMoveCountChnagedEvent();


    private void OnEnable()
    {
        
        onDuckCollected = new EventListener<OnDuckCollectEvent>(HandleTileCollection);
        EventBus<OnDuckCollectEvent>.AddListener(onDuckCollected);

        onClickedTile = new EventListener<OnClickedTileEvent>(OnBlockClicked);
        EventBus<OnClickedTileEvent>.AddListener(onClickedTile);

        onRocketActivated = new EventListener<OnRocketActivated>(HandleRocketActivated);
        EventBus<OnRocketActivated>.AddListener(onRocketActivated);
    }
    
    private void OnDisable()
    {
        EventBus<OnDuckCollectEvent>.RemoveListener(onDuckCollected);
        EventBus<OnClickedTileEvent>.RemoveListener(onClickedTile);
        EventBus<OnRocketActivated>.RemoveListener(onRocketActivated);
    }
    
    private void Awake()
    {
        levelData = LevelManager.Instance.GetLevelData();
        ChangeBorderSize();
    }

    private void Start()
    {
        gridArray = new TileBase[levelData.gridWidth, levelData.gridHeight];
        
        CreateBord();
    }

    private void ChangeBorderSize()
    {
        borderSpriteRenderer.size = new Vector2(levelData.gridWidth + padding.x * 2, levelData.gridHeight + padding.y * 2);
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
    
    private void OnBlockClicked(OnClickedTileEvent e)
    {
        explodableTiles  = floodFill.Find(e.position);

        if (explodableTiles.Count() >= 2)
        {
            DestroyBlocks(explodableTiles);
        }
    }

    private void DestroyBlocks(List<TileBase> foundTiles)
    {
        EventBus<OnMoveCountChnagedEvent>.Emit(onMoveCountChnaged);
        for (int i = 0; i < foundTiles.Count; i++)
        {
            var foundTile = foundTiles[i];
            gridArray[foundTile.TilePosition.x, foundTile.TilePosition.y] = null;
            if (foundTile.TryGetComponent<IMatchable>(out IMatchable matchable))
            {
                matchable.ExplodeTile();
            }
        }
            
        foreach (var foundTile in foundTiles)
        {
            NotifyNeighbors(foundTile.TilePosition);
        }
            
        CheckAndSpawnPowerUp(foundTiles.Count, foundTiles[0].TilePosition);

        DropTiles();
    }

    private void CheckAndSpawnPowerUp(int count, Vector2Int pos)
    {
        foreach (var threshold in gameSettings.powerUpThresholds)
        {
            if (count >= threshold.requiredCount)
            {
                SpawnPowerUp(threshold.powerUpData, pos);
                break;
            }
        }
    }

    private void SpawnPowerUp(TileData powerupData, Vector2Int pos)
    {
        var pool = PoolManager.Instance.GetPool(powerupData.tileId);
        var newPowerUp = pool.Spawn(pos, powerupData);
    
        newPowerUp.transform.SetParent(tilesParent);
        newPowerUp.transform.position = GetWorldPosition(pos);
        gridArray[pos.x, pos.y] = newPowerUp;
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
            if (IsInsideGrid(targetPos))
            {
                TileBase neighborTile = gridArray[targetPos.x, targetPos.y];

                if (neighborTile != null && neighborTile is IExplodable listener)
                {
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
    
    private void HandleTileCollection(OnDuckCollectEvent e)
    {
        var tilePos = e.tile.TilePosition;
        
        gridArray[tilePos.x, tilePos.y] = null;
        
        DropTiles();
    }

    private void HandleRocketActivated(OnRocketActivated e)
    {
        if (e.direction == RocketDirections.Horizontal)
            ClearRow(e.position, e.timePerUnit);
        else
            ClearColumn(e.position, e.timePerUnit);
        
        gridArray[e.position.x, e.position.y] = null;
        
        DOVirtual.DelayedCall(e.animationDuration, () => {
            DropTiles();
        });
    }
    
    private void ClearRow(Vector2Int rocketPos, float timePerUnit)
    {
        explodableTiles.Clear();
        
        for (int x = 0; x < levelData.gridWidth; x++)
        {
            if(rocketPos.x == x) continue;

            float distance = Mathf.Abs(x - rocketPos.x);
            float delay = distance * timePerUnit; 
            
            explodableTiles.Add(gridArray[x, rocketPos.y]);
            
            StartCoroutine(DelayedClearAt(x, rocketPos.y, RocketDirections.Horizontal, delay));
        }
        EventBus<OnMoveCountChnagedEvent>.Emit(new OnMoveCountChnagedEvent());

    }

    private void ClearColumn(Vector2Int rocketPos, float timePerUnit)
    {
        explodableTiles.Clear();
        
        for (int y = 0; y < levelData.gridHeight; y++)
        {
            if(rocketPos.y == y) continue;

            float distance = Mathf.Abs(y - rocketPos.y);
            float delay = distance * timePerUnit;
            
            explodableTiles.Add(gridArray[rocketPos.x, y]);

            StartCoroutine(DelayedClearAt(rocketPos.x, y, RocketDirections.Vertical, delay));
        }
        
        EventBus<OnMoveCountChnagedEvent>.Emit(new OnMoveCountChnagedEvent());
    }
    
    private IEnumerator DelayedClearAt(int x, int y, RocketDirections direction, float delay)
    {
        yield return new WaitForSeconds(delay);
        ExecuteClear(x, y, direction);
    }

    private void ExecuteClear(int x, int y, RocketDirections direction)
    {
        TileBase tile = gridArray[x, y];
    
        if (tile == null || tile is Duck) return;

        if (tile is Rocket rocket)
        {
            if (rocket.Direction == direction)
            {
                PoolManager.Instance.GetPool(tile.GetTileID()).ReturnToPool(tile);
                
            }
            else
            {
                rocket.OnClickedTileEvent();
            }
            
        }

        if (tile .TryGetComponent<IExplodable>(out IExplodable explodable))
        {
            explodable.OnNeighborExploded();
        }

        if (tile.TryGetComponent<IMatchable>(out IMatchable matchable))
        {
            matchable.ExplodeTile();
        }
        gridArray[x, y] = null;
    }
    
    private bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < levelData.gridWidth && pos.y >= 0 && pos.y < levelData.gridHeight;
    }
    
    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        float x = (gridPos.x * cellSize) + padding.x + (cellSize / 2f);
        float y = (gridPos.y * cellSize) + padding.y + (cellSize / 2f);
        return new Vector3(x, y, 0);
    }

    public List<TileBase> GetExplodableTiles()
    {
        return explodableTiles;
    }
}
