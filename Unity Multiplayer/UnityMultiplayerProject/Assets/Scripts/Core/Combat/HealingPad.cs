using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingPad : NetworkBehaviour
{
    [Header("References")] [SerializeField]
    private Image healPadCapacityBar;

    [Header("Settings")] [SerializeField] private int maxHealPower = 30; // The amount of times the pad can heal
    [SerializeField] private float healCooldown = 60f; // The cooldown when maxHealPower is fully depleted
    [SerializeField] private float healFrequency = 1f; // The time it takes between heals
    [SerializeField] private int coinsPerTick = 10; // The cost to heal per tick
    [SerializeField] private int healthPerTick = 10; // Health restored per frequency/tick

    private List<TankPlayer>
        playersInZone = new List<TankPlayer>(); // List that stores the players currently in the healing pad

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer)
        {
            return;
        }

        if
            (!other.attachedRigidbody
                 .TryGetComponent<
                     TankPlayer>(
                     out TankPlayer player)) // Getting attachedRigidbody's Collider because the Collider is not present on the root
        {
            return;
        }

        playersInZone.Add(player);
        
        Debug.Log($"Player {player.PlayerName.Value} has entered healing Pad.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsServer)
        {
            return;
        }

        if (!other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
        {
            return;
        }

        playersInZone.Remove(player);
        
        Debug.Log($"Player {player.PlayerName.Value} has left healing Pad.");
    }
}