using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private Color personalEntityColor;

    private FixedString32Bytes displayName { get; set; }
    public ulong ClientId { get; private set; }
    public int Coins { get; private set; }

    public void Initialize(ulong clientId, FixedString32Bytes displayName, int coins)
    {
        ClientId = clientId;
        this.displayName = displayName;

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            displayText.color = personalEntityColor;
        }
        UpdatePlayerCoins(coins);
    }

    public void UpdatePlayerCoins(int amount)
    {
        Coins = amount;
        UpdateDisplayText();
    }

    public void UpdateDisplayText()
    {
        displayText.text = $"{transform.GetSiblingIndex() + 1}. {displayName} - {Coins}";
    }
}