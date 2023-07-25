using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Character))]
public class CharacterShooting : NetworkBehaviour
{
    [SerializeField] private Gun _gun;
    [SerializeField] private Transform _firePoint;

    private ShootButton _shootButton;

    private void Start()
    {
        _shootButton = FindObjectOfType<ShootButton>();
        _shootButton.ShootingButton.onClick.AddListener(Shoot);
    }

    public override void OnDestroy()
    {
        _shootButton.ShootingButton.onClick.RemoveListener(Shoot);
        base.OnDestroy();
    }

    private void Shoot()
    {
        if (!IsOwner)
            return;

        RequestFireServerRpc(_firePoint.position);
        _gun.Fire(_firePoint.position);
    }

    [ServerRpc]
    private void RequestFireServerRpc(Vector3 direction)
    {
        FireClientRpc(direction);
    }

    [ClientRpc]
    private void FireClientRpc(Vector3 direction)
    {
        if (!IsOwner)
            _gun.Fire(direction);
    }
}
