using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class TileBase : MonoBehaviour, ISpawned, IDespawned
{

    [Header("Tile Properties")] 
    [SerializeField] private float offsetX = 0.61f;
    [SerializeField] private float offsetY = 0.7f;
    
    protected TileData tileData;
    
    private SpriteRenderer spriteRenderer;
    private string currentTileDataId;
    private Vector2Int tilePosition;
    
    public Vector2Int TilePosition{ get => tilePosition; set => tilePosition = value; }


    protected virtual void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void OnSpawned(Vector2Int position, TileData data)
    {
        gameObject.SetActive(true);
        tilePosition = position;
        spriteRenderer.sprite = data.tileIcon;
        currentTileDataId = data.tileId;
        tileData = data;
    
        PlaceTile(position);
        spriteRenderer.sortingOrder = position.y;
    }

    public void OnDespawned()
    {
        gameObject.SetActive(false);
    }

    private void PlaceTile(Vector2Int position)
    {
        tilePosition = position;
        transform.localPosition = new Vector3(tilePosition.x + offsetX, tilePosition.y + offsetY, transform.localPosition.z);
    }

    // private void GetCurrentData(string id)
    // {
    //     for (int i = 0; i < tileDatas.Count; i++)
    //     {
    //         if (tileDatas[i].tileId == id)
    //         {
    //             currentTileDataIndex = i;
    //         }
    //     }
    // }

    public string GetTileID()
    {
        return currentTileDataId;
    }
}
