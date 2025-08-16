using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : MonoBehaviour
{
    
    
    private Allocation allocation;
    private string joinCode;
    private const int maxConnections = 20;

    private const string GameSceneName = "Game";
    public async Task StartHostAsync()
    {
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
            
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
            return;
        }
        
        try
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Join Code is {joinCode}");
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(allocation, "udp");
        transport.SetRelayServerData(relayServerData);
        
        NetworkManager.Singleton.StartHost();
        
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }
}