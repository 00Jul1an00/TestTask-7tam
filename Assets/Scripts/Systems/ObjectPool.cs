using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly T _prefab;
    private readonly List<T> _spawnList;
    private readonly int _spawnCount;
    private readonly Transform _prefabContainer;

    public ObjectPool(T prefab, int spawnCount, Transform prefabContainer)
    {
        _prefab = prefab;
        _spawnCount = spawnCount;
        _prefabContainer = prefabContainer;

        _spawnList = new List<T>(_spawnCount);

        for (int i = 0; i < _spawnCount; i++)
            SpawnObject();
    }

    private void SpawnObject()
    {
        T spawned = UnityEngine.Object.Instantiate(_prefab, _prefabContainer);    
        spawned.gameObject.SetActive(false);
        _spawnList.Add(spawned);
    }

    public T ActivateObject()
    {
        T obj = _spawnList.Where(obj => obj.gameObject.activeSelf == false).FirstOrDefault();

        if (obj == null)
            throw new Exception("Cant activate object in pool");

        obj.gameObject.SetActive(true);
        return obj;
    }
}
