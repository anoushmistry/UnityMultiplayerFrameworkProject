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
using Unity.Services.Matchmaker.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerGameManager : IDisposable
{
    private string serverIP;
    private int serverPort, queryPort;


    private MultiplayAllocationService multiplayAllocationService;

    private MatchplayBackfiller matchplayBackfiller;
    private NetworkObject playerPrefab;
    
    private Dictionary<string, int> TeamIdToIndexDict =  new Dictionary<string, int>();


    public NetworkServer networkServer { get; private set; }

    public ServerGameManager(string serverIP, int serverPort, int queryPort,
        NetworkManager manager, NetworkObject playerPrefab)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = queryPort;
        networkServer = new NetworkServer(manager, playerPrefab);
        multiplayAllocationService = new MultiplayAllocationService();
        this.playerPrefab = playerPrefab;
    }

    public async Task StartGameServerAsync()
    {
        await multiplayAllocationService
            .BeginServerCheck(); // Starts constant loop that tells UGS regarding status of server (players, health etc)

        try
        {
            MatchmakingResults matchmakerPayload = await GetMatchPayload();

            if (matchmakerPayload != null)
            {
                await StartBackfill(matchmakerPayload);
                networkServer.OnUserJoined += UserJoined;
                networkServer.OnUserLeft += UserLeft;
            }
            else
            {
                Debug.LogError("Matchmaking failed due to timeout");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        if (!networkServer.OpenConenction(serverIP, serverPort))
        {
            Debug.LogError($"Network server could not be started as expected");
            return;
        }
    }

    private async Task<MatchmakingResults> GetMatchPayload()
    {
        Task<MatchmakingResults> matchmakingResults =
            multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

        if (await Task.WhenAny(matchmakingResults, Task.Delay(20000)) == matchmakingResults)
        {
            return matchmakingResults.Result;
        }

        return null;
    }

    private async Task StartBackfill(MatchmakingResults matchmakingResults)
    {
        matchplayBackfiller = new MatchplayBackfiller($"{serverIP}:{serverPort}",
            matchmakingResults.QueueName,
            matchmakingResults.MatchProperties,
            20);

        if (matchplayBackfiller.NeedsPlayers())
        {
            await matchplayBackfiller.BeginBackfilling();
        }
    }

    private void UserJoined(UserData user)
    {
        Team team = matchplayBackfiller.GetPlayerTeam(user.userAuthId);
        if (!TeamIdToIndexDict.TryGetValue(team.TeamId, out int teamIndex))
        {
            teamIndex = TeamIdToIndexDict.Count;
            TeamIdToIndexDict.Add(team.TeamId, teamIndex);
        }
        
        user.teamIndex = teamIndex;
        Debug.Log($"The User ID is: {user.userAuthId} and Team ID is: {team.TeamId}");
        //matchplayBackfiller.AddPlayerToMatch(user);
        multiplayAllocationService.AddPlayer();

        if (!matchplayBackfiller.NeedsPlayers() && matchplayBackfiller.IsBackfilling)
        {
            _ = matchplayBackfiller.StopBackfill();
        }
    }

    private void UserLeft(UserData user)
    {
        int playerCount = matchplayBackfiller.RemovePlayerFromMatch(user.userAuthId);
        multiplayAllocationService.RemovePlayer();

        if (playerCount <= 0)
        {
            CloseServer();
            return;
        }

        if (matchplayBackfiller.NeedsPlayers() && matchplayBackfiller.IsBackfilling)
        {
            _ = matchplayBackfiller.BeginBackfilling();
        }
    }

    private async void CloseServer()
    {
        await matchplayBackfiller.StopBackfill();
        Dispose();
        Application.Quit();
    }

    public void Dispose()
    {
        networkServer.OnUserJoined -= UserJoined;
        networkServer.OnUserLeft -= UserLeft;

        matchplayBackfiller?.Dispose();
        multiplayAllocationService?.Dispose();
        networkServer?.Dispose();
    }
}