using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using System;
using Unity.Netcode;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private int _maxPlayers;
    [SerializeField] private float _lobbyHeartbeatDelay;

    private float _lobbyHeartbeatCurrentTime;
    private Lobby _currentLobby;
    private Player _player;

    public Player Player => _player;
    public Lobby CurrentLobby => _currentLobby;
    public static LobbyManager Instance { get; private set; }

    public event Action<string> ErrorTriggered;

    public const string RELAY_JOIN_CODE = "JoinCode";

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.parent = null;
            _lobbyHeartbeatCurrentTime = _lobbyHeartbeatDelay;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        await PlayerAunthetication();

    }

    private async Task PlayerAunthetication()
    {
        try
        {
            var options = new InitializationOptions();

#if UNITY_EDITOR
            if (ClonesManager.IsClone())
                options.SetProfile(ClonesManager.GetArgument());
            else
                options.SetProfile("Primary");
#endif
            await UnityServices.InitializeAsync(options);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (ServicesInitializationException e)
        {
            print(e);
        }
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
    }

    public async Task StartLobbyRelay()
    {
        if (_currentLobby == null)
            return;

        try
        {
            string joinCode = await RelayHelper.Instance.CreateRelay(_maxPlayers);
            var data = new Dictionary<string, DataObject>();
            data.Add(RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, joinCode));
            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions() { Data = data });
            _currentLobby = lobby;
        }
        catch(LobbyServiceException e)
        {
            print(e);
        }
    }

    public async Task CreateLobby(string lobbyName)
    {
        try
        {
            _player = InitPlayer();
            var filters = new List<QueryFilter>() { new(QueryFilter.FieldOptions.Name, lobbyName, QueryFilter.OpOptions.EQ) };
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(new QueryLobbiesOptions() { Filters = filters });

            if (response.Results.Count != 0)
            {
                ErrorTriggered?.Invoke("lobby with name " + lobbyName + " already exist");
                return;
            }

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, _maxPlayers, new CreateLobbyOptions() { Player = _player} );
            _currentLobby = lobby;
            SceneManagerHandler.Instance.LoadGameScene();
        }
        catch (LobbyServiceException e)
        {
            print(e);
        }
    }

    public async Task JoinLobby(string lobbyName)
    {
        try
        {
            _player = InitPlayer();
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();
            var result = response.Results.Find(lobby => lobby.Name == lobbyName);

            if (result == null || result.AvailableSlots == 0)
            {
                ErrorTriggered?.Invoke("lobby with name " + lobbyName + " does not exist or full");
                return;
            }

            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(result.Id, new() { Player = _player });
            _currentLobby = lobby;

            if(lobby.Data == null)
            {
                ErrorTriggered?.Invoke("Lobby not Initialized try later");
                await LeaveLobby();
                return;
            }
            
            SceneManagerHandler.Instance.LoadGameScene();
        }
        catch (LobbyServiceException e)
        {
            print(e);
        }
    }

    public async Task LeaveLobby()
    {
        if (_currentLobby == null)
            return;

        try
        {
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();
            var result = response.Results.Find(lobby => lobby.Id == _currentLobby.Id);

            if (result == null)
            {
                _currentLobby = null;
                return;
            }

            if (_currentLobby.HostId == _player.Id)
            {

                await Lobbies.Instance.DeleteLobbyAsync(_currentLobby.Id);
                _currentLobby = null;
                return;
            }

            await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, _player.Id);
        }
        catch (LobbyServiceException e)
        {
            print(e);
        }

    }

    private async void HandleLobbyHeartbeat()
    {
        if (IsPlayerHost())
        {
            _lobbyHeartbeatCurrentTime -= Time.deltaTime;

            if (_lobbyHeartbeatCurrentTime < 0)
            {
                _lobbyHeartbeatCurrentTime = _lobbyHeartbeatDelay;
                await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
            }
        }
    }

    private Player InitPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject>());
    }

    public bool IsPlayerHost()
    {
        return _currentLobby != null && _player.Id == _currentLobby.HostId;
    }
}
