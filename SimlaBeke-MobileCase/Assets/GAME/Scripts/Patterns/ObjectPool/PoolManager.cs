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
            
            // Sözlüğe eklerken PoolBase olarak saklanır
            allPools.Add(data.tileId, newPool);
        }
    }
    
    // HATA BURADAYDI: PoolBase'i Pool<TileBase>'e cast etmemiz lazım
    public Pool<TileBase> GetPool(string id)
    {
        if (allPools.ContainsKey(id))
        {
            // "as" kullanarak güvenli bir şekilde senin tipine çeviriyoruz
            return allPools[id] as Pool<TileBase>;
        }
        
        Debug.LogError($"Pool with ID {id} not found!");
        return null;
    }
}