using UnityEngine;

[RequireComponent(typeof(Gun))]
public class GunTransform : MonoBehaviour
{
    [SerializeField] private Joystick _joystick;

    private SpriteRenderer _gunSprite;

    private const int VECTOR_CLAMPER = 100000;

    private void Start()
    {
        _gunSprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_joystick.Position != Vector2.zero)
        {
            Vector2 newPosition = new Vector2(_joystick.Position.x, _joystick.Position.y) * VECTOR_CLAMPER;
            transform.localPosition = Vector2.ClampMagnitude(newPosition, 2);
            float rotationZ = Mathf.Atan2(_joystick.Position.y, _joystick.Position.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, rotationZ);
            
            if (rotationZ > 90 || rotationZ < -90)
                _gunSprite.flipY = true;
            else
                _gunSprite.flipY = false;
        }
    }
}
