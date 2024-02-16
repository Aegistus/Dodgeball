using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pool Object", menuName = "Pool Object")]
public class PoolObject : ScriptableObject
{
    /// <summary>
    /// Prefab of the object.
    /// </summary>
    public GameObject[] prefabs;
    /// <summary>
    /// How many instances should be created at runtime.
    /// </summary>
    public int instanceCount;

    private Queue<PoolInstance> inPool = new Queue<PoolInstance>();

    public PoolInstance GetNextInQueue()
    {
        return inPool.Dequeue();
    }

    public void PlaceInQueue(PoolInstance obj)
    {
        inPool.Enqueue(obj);
    }
}
