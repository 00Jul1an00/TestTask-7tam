using UnityEngine;
using Unity.Netcode;

public class StartGame : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManagerHandler.Instance.GameSceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManagerHandler.Instance.GameSceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded()
    {
        if(LobbyManager.Instance.IsPlayerHost())
            NetworkManager.Singleton.StartHost();
        else
            NetworkManager.Singleton.StartClient();
    }
}
