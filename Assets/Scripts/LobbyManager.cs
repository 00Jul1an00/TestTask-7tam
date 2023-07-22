using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using System;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private int _maxPlayers;
    [SerializeField] private float _lobbyUpdatingDelay;
    [SerializeField] private float _lobbyHeartbeatDelay;

    private float _lobbyUpdatingCurrentTime;
    private float _lobbyHeartbeatCurrentTime;
    private Lobby _currentLobby;
    private Player _player;

    public Player Player => _player;
    public Lobby CurrentLobby => _currentLobby;
    public static LobbyManager Instance { get; private set; }

    public const string READINESS_INFO = "Readiness";
    public const string PLAYER_NAME = "PlayerName";

    public event Action PlayerJoinedLobby;
    public event Action PlayerLeftLobby;
    public event Action LobbyUpdated;

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.parent = null;
            _lobbyUpdatingCurrentTime = _lobbyUpdatingDelay;
            _lobbyHeartbeatCurrentTime = _lobbyHeartbeatDelay;
        }
        else
        {
            Destroy(this);
        }

        await PlayerAunthetication();
        
    }

    private async Task PlayerAunthetication()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            _player = InitPlayer();
        }
        catch (ServicesInitializationException e)
        {
            print(e);
        }
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        UpdateLobby();
    }

    public async Task CreateLobby(string lobbyName)
    {
        try
        {
            var filters = new List<QueryFilter>() { new(QueryFilter.FieldOptions.Name, lobbyName, QueryFilter.OpOptions.EQ) };
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(new QueryLobbiesOptions() { Filters = filters });

            if (response.Results.Count != 0)
            {
                print("lobby with name " + lobbyName + " already exist");
                return;
            }
            
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, _maxPlayers, new() { Player = _player });
            _currentLobby = lobby;
            UpdateLobbyImmediately();

        }
        catch(LobbyServiceException e)
        {
            print(e);
        }
    }

    public async Task JoinLobby(string lobbyName)
    {
        try
        {
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();
            var result = response.Results.Find(lobby => lobby.Name == lobbyName);

            if(result == null || result.AvailableSlots == 0)
            {
                print("lobby does not exist or lobby is full");
                return;
            }

            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(result.Id, new() { Player = _player });
            _currentLobby = lobby;
            PlayerJoinedLobby?.Invoke();
            UpdateLobbyImmediately();
        }
        catch(LobbyServiceException e)
        {
            print(e);
        }
    }

    public async void LeaveLobby()
    {
        if (_currentLobby == null)
            return;

        try
        {
            if (_currentLobby.HostId == _player.Id)
            {
                if (_currentLobby.Players.Count - 1 > 0)
                {
                    MigrateHost();
                }
                else
                {
                    await Lobbies.Instance.DeleteLobbyAsync(_currentLobby.Id);
                    _currentLobby = null;
                    return;
                }
            }

            await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, _player.Id);
            PlayerLeftLobby?.Invoke();
            UpdateLobbyImmediately();
            SetPlayerReadiness(false);
            _currentLobby = null;
        }
        catch(LobbyServiceException e)
        {
            print(e);
        }
    }

    private void UpdateLobby()
    {
        if (_currentLobby == null)
            return;

        _lobbyUpdatingCurrentTime -= Time.deltaTime;

        if (_lobbyUpdatingCurrentTime < 0)
        {
            _lobbyUpdatingCurrentTime = _lobbyUpdatingDelay;
            UpdateLobbyImmediately();
        }
    }

    private async void UpdateLobbyImmediately()
    {
        try
        {
            _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
            LobbyUpdated?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            print(e);
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if(IsPlayerHost())
        {
            _lobbyHeartbeatCurrentTime -= Time.deltaTime;

            if(_lobbyHeartbeatCurrentTime < 0)
            {
                _lobbyHeartbeatCurrentTime = _lobbyHeartbeatDelay;
                await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
            }
        }
    }

    private async void MigrateHost()
    {
        try
        {
            _currentLobby = await Lobbies.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions() { HostId = _currentLobby.Players[1].Id });
        }
        catch(LobbyServiceException e)
        {
            print(e);
        }
    }

    public void SetPlayerReadiness(bool isReady)
    {
        if (_currentLobby == null)
            return;

        if (IsPlayerHost())
            return;

        _player.Data[READINESS_INFO] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, isReady.ToString());
        UpdateLobbyImmediately();
    }

    private Player InitPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { READINESS_INFO, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "false") },
            { PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, string.Empty) } 
        });
    }

    public bool IsPlayerHost()
    {
        return _currentLobby != null && _player.Id == _currentLobby.HostId;
    }

    public bool IsAllPlayerAreReady()
    {
        if (_currentLobby == null)
            return false;

        foreach(var player in _currentLobby.Players)
        {
            if (player.Id == _currentLobby.HostId)
                continue;

            if (player.Data[READINESS_INFO].Value == "false")
                return false;
        }

        return true;
    }

    public void ChangePlayerName(string name)
    {
        _player.Data[PLAYER_NAME].Value = name;
    }
}
