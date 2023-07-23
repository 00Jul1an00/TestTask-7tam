using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_Text _winnerNameText;
    [SerializeField] private TMP_Text _winnerCoinsAmmountText;

    private void OnEnable()
    {
        _exitButton.onClick.AddListener(ExitFromGameScene);
    }

    private void OnDisable()
    {
        _exitButton.onClick.RemoveListener(ExitFromGameScene);
    }

    public void SetWinnerName(Character character)
    {
        _winnerNameText.text = character.Name;
    }

    public void SetWinnerCoinsAmmount(Character character)
    {
        _winnerCoinsAmmountText.text = character.Coins.ToString();
    }

    private void ExitFromGameScene()
    {
        SceneManagerHandler.Instance.LoadLobbyScene();
    }
}
