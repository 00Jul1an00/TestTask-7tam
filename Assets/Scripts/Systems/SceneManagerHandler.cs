using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.Netcode;
using System.Collections.Generic;

public class SceneManagerHandler : MonoBehaviour
{
    public static SceneManagerHandler Instance { get; private set; }

    public event Action GameSceneLoaded;

    private const int LOADING_SCENE_INDEX = 0;
    private const int LOBBY_SCENE_INDEX = 1;
    private const int GAME_SCENE_INDEX = 2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        NetworkManager.Singleton.SceneManager.ActiveSceneSynchronizationEnabled = true;
    }

    public void LoadLobbyScene()
    {
        
        if (LobbyManager.Instance.CurrentLobby != null)
            NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetSceneAt(LOBBY_SCENE_INDEX).ToString(), LoadSceneMode.Single);
        else
            SceneManager.LoadScene(LOBBY_SCENE_INDEX);
    }

    public void LoadGameScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetSceneAt(GAME_SCENE_INDEX).ToString(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode mode, List<ulong> completed, List<ulong> timedOut)
    {
        if(SceneManager.GetSceneAt(GAME_SCENE_INDEX).ToString() == sceneName)
            GameSceneLoaded?.Invoke();
    }
}
