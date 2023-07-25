using UnityEngine;
using Unity.Netcode;

public class StartGame : NetworkBehaviour
{
    private void OnEnable()
    {
        SceneManagerHandler.Instance.GameSceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManagerHandler.Instance.GameSceneLoaded -= OnSceneLoaded;
    }

    private async void OnSceneLoaded()
    {
        if (LobbyManager.Instance.IsPlayerHost())
        {
            await LobbyManager.Instance.StartLobbyRelay();
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            string joinCode = LobbyManager.Instance.CurrentLobby.Data[LobbyManager.RELAY_JOIN_CODE].Value;
            await RelayHelper.Instance.JoinRelay(joinCode);
            NetworkManager.Singleton.StartClient();
        }    
    }
}
