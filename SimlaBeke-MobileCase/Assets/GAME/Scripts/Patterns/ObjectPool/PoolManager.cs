using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    
    private Dictionary<string, PoolBase> allPools = new Dictionary<string, PoolBase>();

    [SerializeField] private List<TileData> allTiles; 

    private void Awake()
    {
        foreach (var data in allTiles)
        {
            if (data == null || data.tilePrefab == null) continue;

            var newPool = new Pool<TileBase>();
            newPool.Initialize(data.tilePrefab);
            
            allPools.Add(data.tileId, newPool);
        }
    }
    public Pool<TileBase> GetPool(string id)
    {
        if (allPools.ContainsKey(id))
        {
            return allPools[id] as Pool<TileBase>;
        }
        
        Debug.LogError($"Pool with ID {id} not found!");
        return null;
    }
}