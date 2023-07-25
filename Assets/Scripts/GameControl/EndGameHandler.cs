using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EndGameHandler : NetworkBehaviour
{
    [SerializeField] private EndGameUI _endGameUIPanel;

    private List<Character> _players;

    private void OnEnable()
    {
        NetworkManager.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDisable()
    {
        if (NetworkManager != null)
            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if(IsHost)
            PlayersListSetup();
    }

    private void OnPlayerDie(Character player)
    {
        if (!IsHost)
            return;

        RequestDestroyPlayerServerRpc(_players.IndexOf(player));

        if (_players.Count == 1)
        {
            CharacterNetworkData winner = new() { CoinsCount = _players[0].Coins, Name = _players[0].Name };
            RequestEndGameUIPanelSetupServerRpc(winner);
            RequestDestroyPlayerServerRpc(0);
        }
    }

    private void PlayersListSetup()
    {
        _players = new List<Character>();
        var clients = NetworkManager.Singleton.ConnectedClientsList;

        foreach (var client in clients)
            _players.Add(client.PlayerObject.GetComponent<Character>());

        foreach (Character player in _players)
        {
            player.Die += OnPlayerDie;
            player.EnablingControl(false);

            if (_players.Count >= 2)
                player.EnablingControl(true);
        }
    }

    private void SetupEndGameUIPanel(CharacterNetworkData winnerData)
    {
        _endGameUIPanel.gameObject.SetActive(true);
        _endGameUIPanel.SetWinnerCoinsAmmount(winnerData.CoinsCount);
        _endGameUIPanel.SetWinnerName(winnerData.Name);
    }

    [ServerRpc]
    private void RequestEndGameUIPanelSetupServerRpc(CharacterNetworkData winnerData)
    {
        SetupEndGameUIPanel(winnerData);
        EndGameUIPanelSetupClientRpc(winnerData);
    }
    
    [ClientRpc]
    private void EndGameUIPanelSetupClientRpc(CharacterNetworkData winnerData)
    {
        SetupEndGameUIPanel(winnerData);
    }


    [ServerRpc]
    private void RequestDestroyPlayerServerRpc(int index)
    {
        if (index >= _players.Count || index < 0)
            return;

        var player = _players[index];
        player.GetComponent<NetworkObject>().Despawn();
        Destroy(player);
        _players.Remove(player);
    }
}
