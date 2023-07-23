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
    [SerializeField] private GameObject _errorPanel;
    [SerializeField] private TMP_Text _errorText;

    private bool _readiness = false;

    private void OnEnable()
    {
        LobbyManager.Instance.LobbyUpdated += UpdateLobbyUI;
        LobbyManager.Instance.PlayerJoinedLobby += UpdateLobbyUI;
        LobbyManager.Instance.PlayerLeftLobby += UpdateLobbyUI;
        LobbyManager.Instance.ErrorTriggered += OnErrorTriggered;
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
        LobbyManager.Instance.ErrorTriggered -= OnErrorTriggered;
        _joinLobbyButton.onClick.RemoveListener(JoinLobby);
        _createLobbyButton.onClick.RemoveListener(CreateLobby);
        _closeLobbyButton.onClick.RemoveListener(CloseInLobbyPanel);
        _readyButton.onClick.RemoveListener(ChangeReadiness);
        _startGameButton.onClick.RemoveListener(StartGame);
    }

    private async void JoinLobby()
    {
        await LobbyManager.Instance.JoinLobby(_joinLobbyInputField.text);

        if (LobbyManager.Instance.CurrentLobby != null)
            _inLobbyPanel.gameObject.SetActive(true);

        _readyButton.gameObject.SetActive(true);
        _startGameButton.gameObject.SetActive(false);
    }

    private async void CreateLobby()
    {
        await LobbyManager.Instance.CreateLobby(_createLobbyInputField.text);

        if (LobbyManager.Instance.CurrentLobby != null)
            _inLobbyPanel.gameObject.SetActive(true);

        _readyButton.gameObject.SetActive(false);
        _startGameButton.gameObject.SetActive(true);
    }

    private void ChangeReadiness()
    {
        _readiness = !_readiness;
        LobbyManager.Instance.SetPlayerReadiness(_readiness);
    }

    private void StartGame()
    {
        if (LobbyManager.Instance.CurrentLobby.Players.Count >= 2 && LobbyManager.Instance.IsAllPlayerAreReady())
        {
            SceneManagerHandler.Instance.LoadGameScene();
        }
        else
        {
            OnErrorTriggered("In lobby less than 2 player or not all ready");
        }
    }

    private void ClearLobby()
    {
        foreach (Transform child in _playerInLobbyUIContainer)
        {
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
            lobbyPlayersUI.Init(player);
            bool isReady = player.Data[LobbyManager.READINESS_INFO].Value == "True";
            print("bool " + isReady);
            lobbyPlayersUI.ChangeReadinessStatus(isReady);
            print(player.Id + LobbyManager.Instance.IsPlayerHost(player) + player.Data[LobbyManager.READINESS_INFO].Value);
        }
    }

    private void CloseInLobbyPanel()
    {
        LobbyManager.Instance.LeaveLobby();
        _inLobbyPanel.gameObject.SetActive(false);
    }

    private void OnErrorTriggered(string message)
    {
        _errorPanel.SetActive(true);
        _errorText.text = message;
    }
}
