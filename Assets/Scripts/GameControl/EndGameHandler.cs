using System.Collections.Generic;
using UnityEngine;

public class EndGameHandler : MonoBehaviour
{
    [SerializeField] private EndGameUI _endGameUIPanel;

    private List<Character> _players;

    private void OnEnable()
    {
        SceneManagerHandler.Instance.GameSceneLoaded += OnGameSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManagerHandler.Instance.GameSceneLoaded -= OnGameSceneLoaded;
    }

    private void OnGameSceneLoaded()
    {
        var players = new Character[LobbyManager.Instance.CurrentLobby.Players.Count];
        players = FindObjectsOfType<Character>();
        _players = new List<Character>(players);

        foreach (Character player in _players)
            player.Die += OnPlayerDie;
    }

    private void OnPlayerDie(Character player)
    {
        _players.Remove(player);

        if(_players.Count == 1)
        {
            _endGameUIPanel.gameObject.SetActive(true);
            _endGameUIPanel.SetWinnerCoinsAmmount(player);
            _endGameUIPanel.SetWinnerName(player);
            Destroy(player.gameObject);
        }
    }
}
