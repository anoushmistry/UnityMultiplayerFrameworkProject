using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;
    
    private Vector3 previousPosition;
    public override int Collect()
    {
        if (!IsServer)
        {
            Show(false);
            return 0;
        }

        if (alreadyCollected)
        {
            return 0;
        }

        alreadyCollected = true;

        OnCollected?.Invoke(this);
       
        return coinValue;
    }

    private void Update()
    {
        if (previousPosition != transform.position)
        {
            Show(true);
        }
        previousPosition = transform.position;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}