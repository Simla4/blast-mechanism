using System;
using System.Collections.Generic;
using sb.eventbus;
using Unity.VisualScripting;
using UnityEngine;

// Tile behaviors are currently resolved by GridManager.
// With more time, responsibility would be shifted to tiles themselves
// to reduce conditional logic and improve extensibility.

[RequireComponent(typeof(SpriteRenderer))]
public abstract class TileBase : MonoBehaviour, ISpawned, IDespawned
{
    [Header("Refferances")]
    [SerializeField] protected GameObject blastParticle;
    
    protected TileData tileData;
    protected bool isDestroyed = false;
    
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
        if (data.tileIcon != null)
        {
            spriteRenderer.sprite = data.tileIcon;
        }
        currentTileDataId = data.tileId;
        tileData = data;
    
        spriteRenderer.sortingOrder = position.y;
        
        isDestroyed = false;
        transform.localScale = Vector3.one;
        
        Inıt();
    }

    public void OnDespawned()
    {
        gameObject.SetActive(false);
    }

    public void SetTilePosition(Vector2Int position)
    {
        tilePosition = position;
        spriteRenderer.sortingOrder = position.y;
    }

    public string GetTileID()
    {
        return currentTileDataId;
    }

    public TileData GetTileData()
    {
        return tileData;
    }
    
    protected virtual void Inıt(){}
}
