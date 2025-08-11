using System;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
   public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

   private void OnTriggerEnter2D(Collider2D other)
   {
      other.TryGetComponent<Coin>(out Coin coin);

      if (coin != null)
      {
         int coinValue = coin.Collect();
         if(!IsServer) {return;}
         else
         {
            TotalCoins.Value += coinValue;
         }
         
      }
   }
}
