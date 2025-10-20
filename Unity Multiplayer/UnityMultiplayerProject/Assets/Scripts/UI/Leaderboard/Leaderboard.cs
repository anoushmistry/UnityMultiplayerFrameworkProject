using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;
    [SerializeField] private Transform leaderboardEntityParent;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;
    private List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>();

    [SerializeField]
    private int entitesToDisplay = 7; // Based off the Leaderboard UI (which can hold 7 visible entities at once)

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;
            foreach (LeaderboardEntityState entity in leaderboardEntities)
            {
                HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityState>
                {
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

        if (IsServer)
        {
            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
            foreach (TankPlayer player in players)
            {
                HandlePlayerSpawned(player);
            }

            TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        if (!gameObject.scene.isLoaded)
        {
            return;
        }

        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                if (!entityDisplays.Any(entity => entity.ClientId == changeEvent.Value.clientId))
                {
                    LeaderboardEntityDisplay leaderboardEntityDisplay =
                        Instantiate(leaderboardEntityPrefab, leaderboardEntityParent);
                    leaderboardEntityDisplay.Initialize(changeEvent.Value.clientId, changeEvent.Value.playerName,
                        changeEvent.Value.coins);

                    entityDisplays.Add(leaderboardEntityDisplay);
                }

                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                LeaderboardEntityDisplay entityToRemove =
                    entityDisplays.FirstOrDefault(entity => entity.ClientId == changeEvent.Value.clientId);
                if (entityToRemove != null)
                {
                    entityToRemove.transform.SetParent(null);
                    entityDisplays.Remove(entityToRemove);
                    Destroy(entityToRemove.gameObject);
                }

                break;

            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                LeaderboardEntityDisplay entityToUpdate =
                    entityDisplays.FirstOrDefault(entity => entity.ClientId == changeEvent.Value.clientId);

                if (entityToUpdate != null)
                {
                    entityToUpdate.UpdatePlayerCoins(changeEvent.Value.coins);
                }

                break;
        }

        entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));
        for (int i = 0; i < entityDisplays.Count; i++)
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateDisplayText();
            bool shouldShow = i <= entitesToDisplay - 1;
            entityDisplays[i].gameObject.SetActive(shouldShow);
        }

        LeaderboardEntityDisplay myEntityDisplay =
            entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

        if (myEntityDisplay != null)
        {
            if (myEntityDisplay.transform.GetSiblingIndex() >= entitesToDisplay)
            {
                leaderboardEntityParent.GetChild(entitesToDisplay - 1).gameObject.SetActive(false);
                myEntityDisplay.gameObject.SetActive(true);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;
        }

        if (IsServer)
        {
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        leaderboardEntities.Add(new LeaderboardEntityState
        {
            clientId = player.OwnerClientId,
            playerName = player.PlayerName.Value,
            coins = 0
        });

        player.CoinWallet.TotalCoins.OnValueChanged +=
            (oldCoins, newCoins) => HandlePlayerCoins(player.OwnerClientId, newCoins);
    }

    private void HandlePlayerCoins(ulong clientId, int newValue)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].clientId != clientId)
            {
                continue;
            }

            leaderboardEntities[i] = new LeaderboardEntityState
            {
                clientId = leaderboardEntities[i].clientId,
                playerName = leaderboardEntities[i].playerName,
                coins = newValue
            };
            Debug.Log("Updated player coins");
            return;
        }
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        if (leaderboardEntities == null) return;

        foreach (LeaderboardEntityState entity in leaderboardEntities)
        {
            if (entity.clientId != player.OwnerClientId)
            {
                continue;
            }

            leaderboardEntities.Remove(entity);
            break;
        }

        player.CoinWallet.TotalCoins.OnValueChanged -=
            (oldCoins, newCoins) => HandlePlayerCoins(player.OwnerClientId, newCoins);
    }
}