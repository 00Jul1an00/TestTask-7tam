using UnityEngine;
using System;

[RequireComponent(typeof(CharacterMovement))]
public class Character : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private Gun _gun;
    [SerializeField] private int _health;

    private CharacterMovement _playerMovement;
    private int _coins;

    public string Name => _name;
    public int Health => _health;
    public int Coins => _coins;

    public event Action CoinsChanged;
    public event Action HealthChanged;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _gun.Fire();

        if (Input.GetKeyDown(KeyCode.Alpha0))
            TakeDamage(10);
    }
    public void TakeDamage(int damage)
    {
        if (damage > 0)
        {
            _health -= damage;
            HealthChanged?.Invoke();
        }

    }

    public void AddCoin()
    {
        _coins++;
        CoinsChanged.Invoke();
    }
}
