using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthBar : MonoBehaviour
{
    [SerializeField] private Character _character;
    [SerializeField] private Slider _healthSlider;

    private void OnEnable()
    {
        _healthSlider.value = _character.Health;
        _character.HealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        _character.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged()
    {
        _healthSlider.value = _character.Health;
    }
}
