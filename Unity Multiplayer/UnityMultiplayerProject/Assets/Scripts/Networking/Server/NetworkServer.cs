using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();
    private NetworkObject playerPrefab;

    public Action<string> OnClientLeft;
    public Action<UserData> OnUserJoined,OnUserLeft;

    
    public NetworkServer(NetworkManager networkManager, NetworkObject playerPrefab)
    {
        this.networkManager = networkManager;
        this.playerPrefab = playerPrefab;
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }

    public bool OpenConenction(string serverIP, int serverPort)
    {
        UnityTransport transport = networkManager.gameObject.GetComponent<UnityTransport>();
        transport.SetConnectionData(serverIP, (ushort)serverPort);
        return networkManager.StartServer();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        authIdToUserData[userData.userAuthId] = userData;

        Debug.Log(userData.userName);
        
        OnUserJoined?.Invoke(userData);

        _ = SpawnPlayer(request.ClientNetworkId);
        response.Approved = true;
        response.CreatePlayerObject = false;
    }

    private async Task SpawnPlayer(ulong clientId)
    {
        await Task.Delay(1000);
       NetworkObject playerInstance =
           GameObject.Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPoint(), Quaternion.identity);
       
       playerInstance.SpawnAsPlayerObject(clientId);
        
    }
    private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);
            OnUserLeft?.Invoke(authIdToUserData[authId]);
            authIdToUserData.Remove(authId);
            OnClientLeft?.Invoke(authId);
        }
    }

    public UserData GetUserDataByClientId(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            if (authIdToUserData.TryGetValue(authId, out UserData userData))
            {
                return userData;
            }
            return null;
        }

        return null;
    }

    public void Dispose()
    {
        if (networkManager != null)
        {
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            networkManager.OnServerStarted -= OnNetworkReady;

            if (networkManager.IsListening)
            {
                networkManager.Shutdown();
            }
        }
    }
}