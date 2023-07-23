using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterShooting : NetworkBehaviour
{
    [SerializeField] private Gun _gun;
    [SerializeField] private Transform _firePoint;

    private void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestFireServerRpc(_firePoint.position);
            _gun.Fire(_firePoint.position);
        }
    }

    [ServerRpc]
    private void RequestFireServerRpc(Vector3 direction)
    {
        FireClientRpc(direction);
    }

    [ClientRpc]
    private void FireClientRpc(Vector3 direction)
    {
        if(!IsOwner)
            _gun.Fire(direction);
    }
}
