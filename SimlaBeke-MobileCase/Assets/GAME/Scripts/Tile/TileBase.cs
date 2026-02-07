using System;
using System.Collections.Generic;
using sb.eventbus;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class TileBase : MonoBehaviour, ISpawned, IDespawned
{
    [Header("Refferances")]
    [SerializeField] protected GameObject blastParticle;
    
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
        if (data.tileIcon != null)
        {
            spriteRenderer.sprite = data.tileIcon;
        }
        currentTileDataId = data.tileId;
        tileData = data;
    
        spriteRenderer.sortingOrder = position.y;
        
        Inıt();
    }

    public void OnDespawned()
    {
        EventBus<OnBlockCollected>.Emit(new OnBlockCollected(tileData));
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
