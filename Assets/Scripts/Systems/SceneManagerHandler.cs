using UnityEngine;
using UnityEngine.SceneManagement;
using System;

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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLobbyScene()
    {
        SceneManager.LoadScene(LOBBY_SCENE_INDEX);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(GAME_SCENE_INDEX);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.buildIndex == GAME_SCENE_INDEX)
            GameSceneLoaded?.Invoke();
    }
}
