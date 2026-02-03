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
        floodFill.Find(position);
    }
    
}
