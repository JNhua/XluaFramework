using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    Transform m_PoolParent;

    Dictionary<string, PoolBase> m_Pools = new Dictionary<string, PoolBase>();

    private void Awake()
    {
        m_PoolParent = this.transform.parent.Find("Pool");
    }

    private void CreatePool<T>(string poolName, float releaseTime) where T : PoolBase
    {
        PoolBase pool;
        if (!m_Pools.TryGetValue(poolName, out pool))
        {
            GameObject go = new GameObject(poolName);
            go.transform.SetParent(m_PoolParent);
            pool = go.AddComponent<T>();
            pool.Init(releaseTime);
            m_Pools.Add(poolName, pool);
        }
    }

    public void CreateGameObjectPool(string poolName, float releaseTime)
    {
        CreatePool<GameObjectPool>(poolName, releaseTime);
    }

    public void CreateAssetPool(string poolName, float releaseTime)
    {
        CreatePool<AssetPool>(poolName, releaseTime);
    }

    public Object Spawn(string poolName, string assetName)
    {
        PoolBase pool;
        if (m_Pools.TryGetValue(poolName, out pool))
        {
            return pool.Spawn(assetName);
        }
        return null;
    }

    public void UnSpawn(string poolName, string assetName, Object asset)
    {
        PoolBase pool;
        if (m_Pools.TryGetValue(poolName, out pool))
        {
            pool.UnSpawn(assetName, asset);
        }
    }
}
