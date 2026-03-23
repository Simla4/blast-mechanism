using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolBase
{
    public abstract void ReturnToPool(Component component);
}
public class Pool<T> : PoolBase where T : Component
{
    private T _prefab;

    private List<T> active = new List<T>();
    private Stack<T> inactive = new Stack<T>();

    public void Initialize(T prefab)
    {
        _prefab = prefab;
    }

    public T Spawn(Vector2Int position, TileData tileData)
    {
        if (inactive.Count > 0)
        {
            var item = inactive.Pop();
            active.Add(item);
            
            if (item.TryGetComponent(out ISpawned iSpawn))
            {
                iSpawn.OnSpawned(position, tileData);
            }
            
            return item;
        }
        T clone = UnityEngine.Object.Instantiate(_prefab);
        
        if (clone.TryGetComponent(out ISpawned iSpawned))
        {
            iSpawned.OnSpawned(position, tileData);
        }
        
        active.Add(clone);
        return clone;
    }

    public override void ReturnToPool(Component obj)
    {
        T item = obj as T;
        if (item == null) return;

        if (inactive.Contains(item))
            return;

        active.Remove(item);

        if (item.TryGetComponent(out IDespawned despawned))
            despawned.OnDespawned();

        item.gameObject.SetActive(false);
        inactive.Push(item);
    }


}