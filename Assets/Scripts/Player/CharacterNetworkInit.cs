using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterNetworkInit : NetworkBehaviour
{
    [SerializeField] private Transform _spawnPoints;
    private List<Transform> _spawnPointsList = new();
    private Character _character;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        foreach (Transform child in _spawnPoints)
            _spawnPointsList.Add(child);

        PlayerInit();
    }

    private void PlayerInit()
    {
        var player = LobbyManager.Instance.Player;
        var playersInLobby = LobbyManager.Instance.CurrentLobby.Players;
        int posNumber = 0;

        for(int i = 0; i < playersInLobby.Count; i++)
            if(player.Id == playersInLobby[i].Id)
                posNumber = i;

        transform.position = _spawnPointsList[posNumber].position;
        _character = GetComponent<Character>();
        _character.ChangeName(((PlayerName)posNumber).ToString());
        _character.ChangeColor(GetColor(((PlayerName)posNumber).ToString()));
    }

    private Color GetColor(string name)
    {
        switch (name)
        {
            case "Blue":
                {
                    return Color.blue;
                }
            case "Pink":
                {
                    return new Color(248, 24, 148);
                }
            case "Red":
                {
                    return Color.red;
                }
            default:
                {
                    return Color.white;
                }
        }
    }
}
