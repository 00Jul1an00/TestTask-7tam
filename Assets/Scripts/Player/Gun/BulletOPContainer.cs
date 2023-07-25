using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletOPContainer : MonoBehaviour
{
    private Bullet _bulletPrefab;
    private ObjectPool<Bullet> _bulletPool;
    private const int BULLET_MAX_COUNT_IN_POOL = 200;

    public ObjectPool<Bullet> Pool => _bulletPool;

    public void Init(Bullet bulletPrefab)
    {
        _bulletPrefab = bulletPrefab;
        _bulletPool = new(_bulletPrefab, BULLET_MAX_COUNT_IN_POOL, transform);
    }
}
