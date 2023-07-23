using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [Min(0)][SerializeField] private int _damage;
    [Min(0)][SerializeField] private float _speed;
    [Min(1)][SerializeField] private float _lifetime;

    private void OnEnable() => StartCoroutine(BulletResetAfterLifetimeEnd());
   
    private void Update() => transform.position += transform.up * _speed * Time.deltaTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Character character))
        {
            character.TakeDamage(_damage);
            ResetBullet();
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
