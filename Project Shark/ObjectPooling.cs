using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    struct ObjectInfo
    {
        public List<GameObject> pool;
        public Transform holder;

        public ObjectInfo(Transform _holder)
        {
            pool = new List<GameObject>();
            holder = _holder;
        }
    }

    static Dictionary<GameObject, ObjectInfo> objectPools = new Dictionary<GameObject, ObjectInfo>();

    public static int GetObjectCount(GameObject objectInPool)
    {
        if (!objectPools.ContainsKey(objectInPool))
        {
            Debug.LogError($"[Object Pooling] {objectInPool.name} doesn't exist in the current context");
            return 0;
        }
        return objectPools[objectInPool].pool.Count;
    }
    
    public static GameObject GetAvailableObject(GameObject objectInPool)
    {
        if (objectPools.ContainsKey(objectInPool))
        {
            for (var i = 0; i < objectPools[objectInPool].pool.Count; i++)
            {
                if (objectPools[objectInPool].pool[i] == null)
                {
                    Debug.LogError($"[Object Pooling] {objectInPool.name} has a destruction effect on it.\n This breaks the List that handles Pooling");
                    objectPools[objectInPool].pool.RemoveAt(i);
                    objectPools[objectInPool].pool.Insert(i,AddObject(objectInPool));
                    CheckIfObjectListHasAnyNulls(objectInPool);
                    return objectPools[objectInPool].pool[i];
                }
                
                if (!objectPools[objectInPool].pool[i].activeInHierarchy)
                {
                    return objectPools[objectInPool].pool[i];
                }
            }
            return AddObject(objectInPool);
        }

        CreateNewObjectPool(objectInPool);
        return AddObject(objectInPool);
    }

    public static void CreateNewObjectPool(GameObject objectToAdd, int poolDepth = 0)
    {
        if (objectPools.ContainsKey(objectToAdd)) return;
        GameObject holder = Instantiate(new GameObject(objectToAdd.name));
        DontDestroyOnLoad(holder);
        objectPools.Add(objectToAdd, new ObjectInfo(holder.transform));
        for (int i = 0; i < poolDepth; i++)
        {
            AddObject(objectToAdd);
        }
    }

    static GameObject AddObject(GameObject objectToAdd)
    {
        GameObject pooledObject = Instantiate(objectToAdd, objectPools[objectToAdd].holder);
        pooledObject.SetActive(false);
        objectPools[objectToAdd].pool.Add(pooledObject);
        return pooledObject;
    }

    public static Transform GetObjectHolder(GameObject objectInPool)
    {
        foreach (var objectPool in objectPools)
        {
            if (objectPool.Value.holder.name == objectInPool.name)
            {
                return objectPool.Value.holder;
            }
        }
        return null;
    }

    static void CheckIfObjectListHasAnyNulls(GameObject objectToCheck)
    {
        if (objectPools[objectToCheck].pool.Contains(null))
        {
            print($"[Object Pooling] {objectToCheck.name} has a null in the list");
        }
    }
}