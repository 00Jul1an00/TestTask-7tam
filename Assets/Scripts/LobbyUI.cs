using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button _joinLobbyButton;
    [SerializeField] private Button _createLobbyButton;
    [SerializeField] private Button _closeLobbyButton;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private TMP_InputField _joinLobbyInputField;
    [SerializeField] private TMP_InputField _createLobbyInputField;
    [SerializeField] private Transform _playerInLobbyUIContainer;
    [SerializeField] private LobbyPlayerUI _playerInLobbyUIPrefab;
    [SerializeField] private GameObject _inLobbyPanel;

    private bool _readiness = false;

    private void OnEnable()
    {
        LobbyManager.Instance.LobbyUpdated += UpdateLobbyUI;
        LobbyManager.Instance.PlayerJoinedLobby += UpdateLobbyUI;
        LobbyManager.Instance.PlayerLeftLobby += UpdateLobbyUI;
        _joinLobbyButton.onClick.AddListener(JoinLobby);
        _createLobbyButton.onClick.AddListener(CreateLobby);
        _closeLobbyButton.onClick.AddListener(CloseInLobbyPanel);
        _startGameButton.onClick.AddListener(StartGame);
        _readyButton.onClick.AddListener(ChangeReadiness);
        _inLobbyPanel.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        LobbyManager.Instance.LobbyUpdated -= UpdateLobbyUI;
        LobbyManager.Instance.PlayerJoinedLobby -= UpdateLobbyUI;
        LobbyManager.Instance.PlayerLeftLobby -= UpdateLobbyUI;
        _joinLobbyButton.onClick.RemoveListener(JoinLobby);
        _createLobbyButton.onClick.RemoveListener(CreateLobby);
        _closeLobbyButton.onClick.RemoveListener(CloseInLobbyPanel);
        _readyButton.onClick.RemoveListener(ChangeReadiness);
        _startGameButton.onClick.RemoveListener(StartGame);
    }

    private async void JoinLobby()
    {
        await LobbyManager.Instance.JoinLobby(_joinLobbyInputField.text);

        if(LobbyManager.Instance.CurrentLobby != null)
            _inLobbyPanel.gameObject.SetActive(true);
    }

    private async void CreateLobby()
    {
        await LobbyManager.Instance.CreateLobby(_createLobbyInputField.text);

        if (LobbyManager.Instance.CurrentLobby != null)
            _inLobbyPanel.gameObject.SetActive(true);

        if (LobbyManager.Instance.IsPlayerHost())
        {
            _readyButton.gameObject.SetActive(false);
            _startGameButton.gameObject.SetActive(true);
        }
    }
    private void ChangeReadiness()
    {
        LobbyManager.Instance.SetPlayerReadiness(!_readiness);
        _readiness = !_readiness;
    }

    private void StartGame()
    {
        if (LobbyManager.Instance.IsAllPlayerAreReady())
        {
            print("start");
        }
    }

    private void ClearLobby()
    {
        foreach (Transform child in _playerInLobbyUIContainer)
        {
            if (child == _playerInLobbyUIPrefab)
                continue;

            Destroy(child.gameObject);
        }
    }

    private void UpdateLobbyUI()
    {
        ClearLobby();

        foreach (var player in LobbyManager.Instance.CurrentLobby.Players)
        {
            var lobbyPlayersUI = Instantiate(_playerInLobbyUIPrefab, _playerInLobbyUIContainer);
            lobbyPlayersUI.gameObject.SetActive(true);
            bool isReady = player.Data[LobbyManager.READINESS_INFO].Value == "true";
            lobbyPlayersUI.ChangeReadinessStatus(isReady);
        }
    }

    private void CloseInLobbyPanel()
    {
        LobbyManager.Instance.LeaveLobby();
        _inLobbyPanel.gameObject.SetActive(false);
    }
}
