using UnityEngine;
using System;

public class Character : MonoBehaviour
{
    [SerializeField] private int _health;
    [SerializeField] private SpriteRenderer _playerSprite;

    private string _name;
    private int _coins;

    public string Name => _name;
    public int Health => _health;
    public int Coins => _coins;

    public event Action CoinsChanged;
    public event Action HealthChanged;
    public event Action<Character> Die;

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
            Destroy(gameObject);
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
