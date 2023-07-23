using UnityEngine;

public class Gun : MonoBehaviour
{
    private BulletOPContainer _bulletContainer;

    private void Start()
    {
        _bulletContainer = FindObjectOfType<BulletOPContainer>();
    }

    public void Fire(Vector3 direction)
    {
        var bullet = _bulletContainer.Pool.ActivateObject();
        bullet.transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z - 90);
        bullet.transform.position = direction;
    }
}
