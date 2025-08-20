using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera  virtualCamera;
    
    [Header("Settings")]
    [SerializeField] private int cameraPriority = 15;
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) { return; }
        
        virtualCamera.Priority = cameraPriority;
        
    }
}
