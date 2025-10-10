using System;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")] [SerializeField]
    private Health health;

    [SerializeField] private BountyCoin coinPrefab;

    [Header("Settings")] [SerializeField] private float coinSpread = 3f;
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minBountyCoinValue = 5;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float bountyPercentage = 50f; //How much percent of coins should drop when player dies

    private Collider2D[] coinBuffer = new Collider2D[1];
    private float coinRadius;

    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }
        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        health.OnDie += HandleDeath;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
        {
            return;
        }

        health.OnDie -= HandleDeath;
    }

    private void HandleDeath(Health healthObj)
    {
        int bountyValue = (int)(TotalCoins.Value * (bountyPercentage / 100f));
        int bountyCoinValue = bountyValue / bountyCoinCount;

        if (bountyCoinValue < minBountyCoinValue)
        {
            return;
        }

        for (int i = 0; i < bountyCoinCount; i++)
        {
            BountyCoin bountyCoin = Instantiate(coinPrefab, GetSpawnPosition(), Quaternion.identity);
            bountyCoin.SetValue(bountyCoinValue);
            bountyCoin.NetworkObject.Spawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.TryGetComponent<Coin>(out Coin coin);

        if (coin != null)
        {
            int coinValue = coin.Collect();
            if (!IsServer)
            {
                return;
            }
            else
            {
                TotalCoins.Value += coinValue;
            }
        }
    }

    public void SpendCoins(int value)
    {
        TotalCoins.Value -= value;
    }
    private Vector2 GetSpawnPosition()
    {
        while (true)
        {
            Vector2 spawnPosition = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPosition, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0)
            {
                return spawnPosition;
            }
            
        }
    }
}