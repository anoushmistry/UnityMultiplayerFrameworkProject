using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;

    private FixedString32Bytes displayName { get; set; }
    public ulong ClientId { get; private set; }
    public int Coins { get; private set; }

    public void Initialize(ulong clientId, FixedString32Bytes displayName, int coins)
    {
        ClientId = clientId;
        this.displayName = displayName;

        UpdatePlayerCoins(coins);
    }

    public void UpdatePlayerCoins(int amount)
    {
        Coins = amount;
        UpdateDisplayText();
    }

    private void UpdateDisplayText()
    {
        displayText.text = $"1. {displayName} - {Coins}";
    }
}