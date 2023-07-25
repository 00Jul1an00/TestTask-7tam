using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    [Min(0)][SerializeField] private int _damage;
    [Min(0)][SerializeField] private float _speed;
    [Min(1)][SerializeField] private float _lifetime;

    private Character _shooter;

    private void OnEnable() => StartCoroutine(BulletResetAfterLifetimeEnd());
   
    public void SetShooter(Character shooter)
    {
        if (shooter == null)
            return;

        _shooter = shooter;
    }

    private void Update() => transform.position += transform.up * _speed * Time.deltaTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Character character))
        {
            if (character != null && _shooter != character)
            {
                character.TakeDamage(_damage);   
                ResetBullet();
            }
        }
    }

    private IEnumerator BulletResetAfterLifetimeEnd()
    {
        yield return new WaitForSeconds(_lifetime);
        ResetBullet();
    }

    private void ResetBullet()
    {
        gameObject.SetActive(false);
        transform.localPosition = Vector2.zero;
    }
}
