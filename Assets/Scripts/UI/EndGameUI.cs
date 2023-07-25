using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class EndGameUI : NetworkBehaviour
{
    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_Text _winnerNameText;
    [SerializeField] private TMP_Text _winnerCoinsAmmountText;

    private void OnEnable()
    {
        _exitButton.onClick.AddListener(ExitFromGameScene);
    }

    private void OnDisable()
    {
        _exitButton.onClick.RemoveListener(ExitFromGameScene);
    }

    public void SetWinnerName(string name)
    {
        _winnerNameText.text = "WINNER: " + name;
    }

    public void SetWinnerCoinsAmmount(int coins)
    {
        _winnerCoinsAmmountText.text = coins.ToString();
    }

    private async void ExitFromGameScene()
    {
        if(IsHost)
        {
            foreach(var clientID in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (clientID == OwnerClientId)
                    continue;

                NetworkManager.Singleton.DisconnectClient(clientID);
            }
        }
        
        NetworkManager.Singleton.Shutdown();
        await LobbyManager.Instance.LeaveLobby();
        
        SceneManagerHandler.Instance.LoadLobbyScene();
    }
}
