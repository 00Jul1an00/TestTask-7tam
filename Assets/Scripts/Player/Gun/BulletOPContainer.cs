using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletOPContainer : MonoBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;

    private ObjectPool<Bullet> _bulletPool;
    private const int BULLET_MAX_COUNT_IN_POOL = 1000;

    public ObjectPool<Bullet> Pool => _bulletPool;

    private void Start()
    {
        _bulletPool = new(_bulletPrefab, BULLET_MAX_COUNT_IN_POOL, transform);
    }
}
