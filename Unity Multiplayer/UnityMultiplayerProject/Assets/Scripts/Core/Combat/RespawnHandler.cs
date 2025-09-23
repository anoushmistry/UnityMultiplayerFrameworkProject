using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private TankPlayer playerPrefab;
    [SerializeField] private float retainedCoinPercentage = 50f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
        {
            return;
        }

        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        player.Health.OnDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        player.Health.OnDie -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(TankPlayer player)
    {
        Destroy(player.gameObject);

        int retainedCoins = (int)(player.CoinWallet.TotalCoins.Value * (retainedCoinPercentage / 100f));

        StartCoroutine(RespawnPlayer(player.OwnerClientId, retainedCoins));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId, int retainedCoins) // Waits for a frame to allow the Destroy to happen
    {
        yield return null;
        
       TankPlayer playerInstance =
           Instantiate(playerPrefab,SpawnPoint.GetRandomSpawnPoint(),Quaternion.identity);
       
       playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);
       playerInstance.CoinWallet.TotalCoins.Value += retainedCoins;
    }
}