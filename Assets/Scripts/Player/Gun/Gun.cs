using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Character _shooter;

    private BulletOPContainer _bulletContainer;

    private void Start()
    {
        _bulletContainer = FindObjectOfType<BulletOPContainer>();
        _bulletContainer.Init(_bulletPrefab);
    }

    public void Fire(Vector3 direction)
    {
        var bullet = _bulletContainer.Pool.ActivateObject();
        bullet.SetShooter(_shooter);
        bullet.transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z - 90);
        bullet.transform.position = direction;
    }
}
