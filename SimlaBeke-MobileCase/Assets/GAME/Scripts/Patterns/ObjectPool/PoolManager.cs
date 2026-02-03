using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoolManager: MonoSingleton<PoolManager>
{
    public Pool<TileBase> blockPool { get; } = new Pool<TileBase>();
    [SerializeField] private TileBase blockPrefab;
    private void Awake()
    {
        blockPool.Initialize(blockPrefab);
    }
}