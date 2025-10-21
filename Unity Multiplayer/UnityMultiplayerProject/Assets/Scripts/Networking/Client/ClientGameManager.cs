using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private const string MenuSceneName = "MainMenu";
    private JoinAllocation allocation;

    private const string GameSceneName = "Game";

    private NetworkClient networkClient;
    private MatchplayMatchmaker matchplayMatchmaker;
    
    private UserData userData;

    public async Task<bool> InitializeAsync() // Used for authenticationg and initializing a client
    {
        await UnityServices.InitializeAsync();

        matchplayMatchmaker = new MatchplayMatchmaker();
        networkClient = new NetworkClient(NetworkManager.Singleton);

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            userData = new UserData
            {
                userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "MissingName"),
                userAuthId = AuthenticationService.Instance.PlayerId
            };
            return true;
        }

        return false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task
        StartClientAsync(
            string joinCode) // Used for joining a given connection with a join code (For Relay and Lobby (not dedicated server)
    {
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);
        
        ConnectClient();
    }

    private void StartClient(string ip, int port) // Used for dedicated server
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort)port);
        ConnectClient();    
    }
    private void ConnectClient()
    {
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    public async void MatchmakeAsync(Action<MatchmakerPollingResult> OnMatchmake, bool isTeamQueue)
    {
        if(matchplayMatchmaker.IsMatchmaking) {return;}

        userData.userGamePreferences.gameQueue = isTeamQueue ? GameQueue.Team : GameQueue.Solo;
        MatchmakerPollingResult matchmakerPollingResult = await GetMatchAsync();
        OnMatchmake?.Invoke(matchmakerPollingResult);

    }
    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult matchmakingResult = await matchplayMatchmaker.Matchmake(userData);

        if (matchmakingResult.result == MatchmakerPollingResult.Success)
        {
            //Join server
            StartClient(matchmakingResult.ip, matchmakingResult.port);
        }
        return matchmakingResult.result;
    }
    public async Task CancelMatchmaking()
    {
        await matchplayMatchmaker.CancelMatchmaking();
    }
    public void Disconnect()
    {
        networkClient.Disconnect();
    }

    public void Dispose()
    {
        networkClient?.Dispose();
    }

    
}