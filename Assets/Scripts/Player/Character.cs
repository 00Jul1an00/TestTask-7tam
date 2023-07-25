using UnityEngine;
using System;

public class Character : MonoBehaviour
{
    [SerializeField] private int _health;
    [SerializeField] private SpriteRenderer _playerSprite;
    [SerializeField] private CharacterShooting _shooting;
    [SerializeField] private CharacterMovement _movement;

    private string _name;
    private int _coins;

    public string Name => _name;
    public int Health => _health;
    public int Coins => _coins;

    public event Action CoinsChanged;
    public event Action HealthChanged;
    public event Action<Character> Die;

    public void EnablingControl(bool enable)
    {
        _shooting.enabled = enable;
        _movement.enabled = enable;
    }

    public void TakeDamage(int damage)
    {
        if (damage > 0)
        {
            _health -= damage;
            HealthChanged?.Invoke();
        }

        if (_health <= 0)
        {
            Die?.Invoke(this);
        }
    }

    public void AddCoin()
    {
        _coins++;
        CoinsChanged.Invoke();
    }

    public void ChangeName(string name)
    {
        _name = name;
    }

    public void ChangeColor(Color color)
    {
        _playerSprite.color = color;
    }
}
