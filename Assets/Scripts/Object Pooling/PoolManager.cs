using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }
    public string poolObjectPath = "Pool Objects";
    PoolObject[] poolObjects;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } 
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(gameObject);
        GameObject poolParent = new GameObject("PoolParent");
        poolParent.transform.SetParent(transform);
        GameObject obj;
        poolObjects = Resources.LoadAll<PoolObject>(poolObjectPath);
        foreach (PoolObject poolObject in poolObjects)
        {
            for (int i = 0; i < poolObject.instanceCount; i++)
            {
                int variantIndex = UnityEngine.Random.Range(0, poolObject.prefabs.Length);
                obj = Instantiate(poolObject.prefabs[variantIndex], Vector3.one * 1000, Quaternion.identity, poolParent.transform);
                PoolInstance poolInstance = obj.AddComponent<PoolInstance>();
                poolInstance.lifeTime = 10f;
                obj.SetActive(false);
                poolObject.PlaceInQueue(poolInstance);
            }
        }
    }

    PoolInstance objectFromPool;
    /// <summary>
	/// Spawns a pool object with the given objectID at the given position and rotation.
    /// Logs a warning if the objectID is invalid.
	/// </summary>
	/// <returns> The pool object's GameObject. null if the objectID is invalid. </returns>
    public static GameObject SpawnObject(string objectID, Vector3 position, Quaternion rotation)
    {
        int poolIndex = Array.FindIndex(Instance.poolObjects, pool => pool.name == objectID);
        if (poolIndex < 0 || poolIndex >= Instance.poolObjects.Length)
        {
            Debug.LogWarning("Invalid Pool Object ID");
            return null;
        }
        Instance.objectFromPool = Instance.poolObjects[poolIndex].GetNextInQueue();
        Instance.objectFromPool.lifeTime = -1;
        Instance.objectFromPool.transform.position = position;
        Instance.objectFromPool.transform.rotation = rotation;
        Instance.objectFromPool.gameObject.SetActive(true);
        Instance.poolObjects[poolIndex].PlaceInQueue(Instance.objectFromPool);
        return Instance.objectFromPool.gameObject;
    }

    /// <summary>
	/// Spawns a pool object with the given objectID at the given position and rotation.
    /// Also sets the object's lifetime to lifeTime.
    /// Logs a warning if the objectID is invalid.
	/// </summary>
	/// <returns> The pool object's GameObject. null if the objectID is invalid. </returns>
    public static GameObject SpawnObjectWithLifetime(string objectID, Vector3 position, Quaternion rotation, float lifeTime)
    {
        int poolIndex = Array.FindIndex(Instance.poolObjects, pool => pool.name == objectID);
        if (poolIndex < 0 || poolIndex >= Instance.poolObjects.Length)
        {
            Debug.LogWarning("Invalid Pool Object ID:");
            return null;
        }
        Instance.objectFromPool = Instance.poolObjects[poolIndex].GetNextInQueue();
        Instance.objectFromPool.lifeTime = lifeTime;
        Instance.objectFromPool.transform.position = position;
        Instance.objectFromPool.transform.rotation = rotation;
        Instance.objectFromPool.gameObject.SetActive(true);
        Instance.poolObjects[poolIndex].PlaceInQueue(Instance.objectFromPool);
        return Instance.objectFromPool.gameObject;
    }

    /// <summary>
	/// Spawns a pool object with the given objectID with the given position, rotation, and scale.
    /// Logs a warning if the objectID is invalid.
	/// </summary>
	/// <returns> The pool object's GameObject. null if the objectID is invalid. </returns>
    public static GameObject SpawnObjectWithLifetime(string objectID, Vector3 position, Quaternion rotation, Vector3 scale, float lifeTime)
    {
        int poolIndex = Array.FindIndex(Instance.poolObjects, pool => pool.name == objectID);
        if (poolIndex < 0 || poolIndex >= Instance.poolObjects.Length)
        {
            Debug.LogWarning("Invalid Pool Object ID");
            return null;
        }
        Instance.objectFromPool = Instance.poolObjects[poolIndex].GetNextInQueue();
        Instance.objectFromPool.lifeTime = lifeTime;
        Instance.objectFromPool.transform.position = position;
        Instance.objectFromPool.transform.rotation = rotation;
        Instance.objectFromPool.transform.localScale = scale;
        Instance.objectFromPool.gameObject.SetActive(true);
        Instance.poolObjects[poolIndex].PlaceInQueue(Instance.objectFromPool);
        return Instance.objectFromPool.gameObject;

    }

}
