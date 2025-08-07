using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public NetworkVariable<int> CurrentHealth = new();
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;
    

    private bool isDead;

    public Action<Health> OnDie;
    public override void OnNetworkSpawn()
    {
        if(!IsServer) { return;}
        
        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int amount)
    {
        ModifyHealth(-amount);
    }

    public void RestoreHealth(int amount)
    {
        ModifyHealth(amount);
    }

    private void ModifyHealth(int amount)
    {
        if (isDead) { return;}
        
        int newHealth = CurrentHealth.Value + amount;
        CurrentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);    

        if (CurrentHealth.Value == 0)
        {
            OnDie?.Invoke(this);
            isDead = true;
        }
    }
}


