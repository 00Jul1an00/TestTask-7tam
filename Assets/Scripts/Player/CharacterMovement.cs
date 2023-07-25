using UnityEngine;
using System;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Character))]
public class CharacterMovement : NetworkBehaviour
{
    [SerializeField] private Joystick _joystick;
    [SerializeField] private float _speed;

    private Vector2 _screenBounds;
    private Rigidbody2D _rb;
    private bool _xScreenBoundReached;
    private bool _yScreenBoundReached;

    private void Start()
    {
        if (!IsOwner)
            return;

        _screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        _rb = GetComponent<Rigidbody2D>();
        _joystick = FindObjectOfType<Joystick>();

        if (_joystick == null)
            throw new NullReferenceException();
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        CheckScreenBounds();

        if (_xScreenBoundReached && _yScreenBoundReached)
        {
            _rb.velocity = Vector2.zero;
            return;
        }

        if (_yScreenBoundReached)
        {
            _rb.velocity = new Vector2(_joystick.Position.x, 0) * _speed;
            return;
        }

        if (_xScreenBoundReached)
        {
            _rb.velocity = new Vector2(0, _joystick.Position.y) * _speed;
            return;
        }

        _rb.velocity = _joystick.Position * _speed;
    }

    private void CheckScreenBounds()
    {
        Vector2 position = (Vector2)gameObject.transform.position + (_joystick.Position * 10);
        _yScreenBoundReached = position.y > _screenBounds.y || position.y < -_screenBounds.y;
        _xScreenBoundReached = position.x > _screenBounds.x || position.x < -_screenBounds.x;
    }
}
