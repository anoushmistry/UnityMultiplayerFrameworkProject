using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for preventing friendly fire when on a team game (prevent damage to team members)
/// </summary>
public class Projectile : MonoBehaviour
{ 
    public int TeamIndex { get; private set; }
    
    public void Initialize(int teamIndex)
    {
        TeamIndex = teamIndex;
    }
}
