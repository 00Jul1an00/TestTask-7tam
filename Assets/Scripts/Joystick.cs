using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject _handle;

    private Camera _mainCamera;
    private CircleCollider2D _circleCollider;
    private float _moveRadius;

    public Vector2 Position { get { return (_handle.transform.position - gameObject.transform.position).normalized; } }

    private void Start()
    {
        _mainCamera = Camera.main;
        _circleCollider = GetComponent<CircleCollider2D>();
        _moveRadius = _circleCollider.radius / 100;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 inputPosition = _mainCamera.ScreenToWorldPoint(eventData.position);
        Vector2 handleDistanceFromMiddle = inputPosition - (Vector2)gameObject.transform.position; 
        _handle.transform.position = (Vector2)gameObject.transform.position + Vector2.ClampMagnitude(handleDistanceFromMiddle, _moveRadius);
    }

    public void OnEndDrag(PointerEventData eventData) => _handle.transform.localPosition = Vector2.zero;
}
