using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (projectile.TeamIndex != -1)
        {
            if (collision.attachedRigidbody == null)
            {
                return;
            }
            if (collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
            {
                if (player.TeamIndex.Value == this.projectile.TeamIndex)
                {
                    return;
                }
            }
        }
        
        Destroy(this.gameObject);
    }
}
