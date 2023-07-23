using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;

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

    public void Init(Player player)
    {
        int playersInLobby = LobbyManager.Instance.CurrentLobby.Players.Count;
        int maxPlayers = LobbyManager.Instance.CurrentLobby.MaxPlayers;
        int result = maxPlayers - playersInLobby;
        _readyCheckMarkImage.gameObject.SetActive(false);

        if (LobbyManager.Instance.IsPlayerHost(player))
        {
            _playerName = PlayerName.White.ToString();
            Destroy(_readyCheckMarkImage);
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
        if (!LobbyManager.Instance.IsPlayerHost())
            _readyCheckMarkImage.gameObject.SetActive(isReady);
    }
}
