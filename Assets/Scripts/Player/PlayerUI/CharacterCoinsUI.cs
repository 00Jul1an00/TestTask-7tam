using UnityEngine;
using TMPro;
public class CharacterCoinsUI : MonoBehaviour
{
    [SerializeField] private Character _character;
    [SerializeField] private TMP_Text _coinsText;

    private void OnEnable()
    {
        _coinsText.text = "0";
        _character.CoinsChanged += OnCoinsChanged;
    }

    private void OnDisable()
    {
        _character.CoinsChanged -= OnCoinsChanged;
    }

    private void OnCoinsChanged()
    {
        _coinsText.text = _character.Coins.ToString();
    }
}
