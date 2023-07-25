using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class RelayHelper : MonoBehaviour
{
    public static RelayHelper Instance { get; private set; }

    private UnityTransport _transport;

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
        _transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    }

    public async Task<string> CreateRelay(int maxPlayers)
    {
        string joinCode = string.Empty;

        try
        {
            Allocation a = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
            _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
        }
        catch (RelayServiceException e)
        {
            print(e);
        }

        return joinCode;
    }

    public async Task JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCode);
            _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
        }
        catch(RelayServiceException e)
        {
            print(e);
        }
    }
}
