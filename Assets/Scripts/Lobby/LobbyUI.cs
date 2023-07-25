using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button _joinLobbyButton;
    [SerializeField] private Button _createLobbyButton;
    [SerializeField] private TMP_InputField _joinLobbyInputField;
    [SerializeField] private TMP_InputField _createLobbyInputField;
    [SerializeField] private GameObject _errorPanel;
    [SerializeField] private TMP_Text _errorText;

    private void OnEnable()
    {
        LobbyManager.Instance.ErrorTriggered += OnErrorTriggered;
        _joinLobbyButton.onClick.AddListener(JoinLobby);
        _createLobbyButton.onClick.AddListener(CreateLobby);
    }

    private void OnDisable()
    {
        LobbyManager.Instance.ErrorTriggered -= OnErrorTriggered;
        _joinLobbyButton.onClick.RemoveListener(JoinLobby);
        _createLobbyButton.onClick.RemoveListener(CreateLobby);
    }

    private async void JoinLobby()
    {
        if(_errorPanel.activeSelf == false)
            await LobbyManager.Instance.JoinLobby(_joinLobbyInputField.text);
    }

    private async void CreateLobby()
    {
        if (_errorPanel.activeSelf == false)
            await LobbyManager.Instance.CreateLobby(_createLobbyInputField.text);
    }

    private void OnErrorTriggered(string message)
    {
        _errorPanel.SetActive(true);
        _errorText.text = message;
    }
}
