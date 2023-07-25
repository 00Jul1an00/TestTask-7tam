using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Character))]
public class CharacterNetworkInit : NetworkBehaviour
{
    [SerializeField] private Transform _spawnPoints;
    [SerializeField] private Character _character;
    private List<Transform> _spawnPointsList = new();

    public override void OnNetworkSpawn()
    {
        foreach (Transform child in _spawnPoints)
            _spawnPointsList.Add(child);

        int index = (int)OwnerClientId;

        if (index >= _spawnPointsList.Count) 
            index = _spawnPointsList.Count;

        transform.position = _spawnPointsList[index].position;
        PlayerChangesSetup(index);
    }

    private void PlayerChangesSetup(int posNumber)
    {
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
            case "Black":
                {
                    return Color.black;
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
