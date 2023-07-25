using UnityEngine;
using UnityEngine.UI;

public class ShootButton : MonoBehaviour
{
    [SerializeField] private Button _shootButton;

    public Button ShootingButton => _shootButton;
}
