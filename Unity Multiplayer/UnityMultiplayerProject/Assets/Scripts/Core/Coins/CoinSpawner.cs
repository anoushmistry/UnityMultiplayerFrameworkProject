using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawner : NetworkBehaviour
{
    
    [SerializeField] private RespawningCoin coinPrefab;
    [SerializeField] private int maxCoins = 50;
    [SerializeField] private int coinValue = 10;

    [SerializeField] private Vector2 xSpawnRange, ySpawnRange;

    [SerializeField] private LayerMask layerMask;

    private Collider2D[] coinBuffer = new Collider2D[1];
    private float coinRadius;
    public override void OnNetworkSpawn()
    {
        if(!IsServer) {return;}

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        RespawningCoin coinInstance = Instantiate(coinPrefab,GetSpawnPosition(),Quaternion.identity);
        coinInstance.SetValue(coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.OnCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPosition();
        coin.Reset();
    }

    private Vector2 GetSpawnPosition()
    {
        float x = 0, y = 0;

        while (true)
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector2 spawnPosition = new Vector2(x, y);
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPosition, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0)
            {
                return spawnPosition;
            }
            
        }
    }
}