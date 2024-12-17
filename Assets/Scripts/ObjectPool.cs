using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public ItemManager player;
    public GameObject objectPrefab;
    public List<GameObject> pool;
    public int poolSize;
    public bool hasItems;

    void Start()
    {
        pool = new List<GameObject>();

    }

    private void Update()
    {
        hasItems = player.currentItemAmount > 0 ? true : false;
    }

    public void UpdateProjectile()
    {
        objectPrefab = player.currentProjectilPrefab;

        if (objectPrefab != null)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(objectPrefab);
                obj.SetActive(false);
                pool.Add(obj);
            }
        }
        else
        {
            pool.Clear();
        }
        
    }

    public GameObject GetPooledObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        GameObject newObj = Instantiate(objectPrefab);
        newObj.SetActive(false);
        pool.Add(newObj);
        return newObj;
    }

}
