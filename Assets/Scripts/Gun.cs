using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _bulletContainer;

    private ObjectPool<Bullet> _bulletPool;
    private const int BULLET_MAX_COUNT_IN_POOL = 1000;

    private void Start()
    {
        _bulletPool = new(_bulletPrefab, BULLET_MAX_COUNT_IN_POOL, _bulletContainer);
    }

    public void Fire()
    {
        var bullet = _bulletPool.ActivateObject();
        bullet.transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z - 90);
        bullet.transform.position = _firePoint.transform.position;
    }
}
