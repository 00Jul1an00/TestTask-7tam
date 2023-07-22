using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerName
{
    White,
    Red,
    Pink,
    Blue
}

public class LobbyPlayerUI : MonoBehaviour
{
    [SerializeField] private Image _readyCheckMarkImage;
    [SerializeField] private TMP_Text _playerNameText;

    private string _playerName;

    private void Start()
    {
        int playersInLobby = LobbyManager.Instance.CurrentLobby.Players.Count;
        int maxPlayers = LobbyManager.Instance.CurrentLobby.MaxPlayers;
        int result = maxPlayers - playersInLobby;

        if (LobbyManager.Instance.IsPlayerHost())
        {
            _playerName = PlayerName.White.ToString();            
            _readyCheckMarkImage = null;
        }
        else
        {
            _playerName = ((PlayerName)result).ToString();
        }

        LobbyManager.Instance.ChangePlayerName(_playerName);
        _playerNameText.text = _playerName;
    }

    public void ChangeReadinessStatus(bool isReady)
    {
        _readyCheckMarkImage.gameObject.SetActive(isReady);
    }
}
