using UnityEngine;
using Unity.Netcode;

public class Coin : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Character character))
        {
            character.AddCoin();
            Destroy(gameObject);
        }
    }
}
