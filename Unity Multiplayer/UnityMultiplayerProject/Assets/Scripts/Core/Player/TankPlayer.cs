using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera  virtualCamera;
    
    [Header("Settings")]
    [SerializeField] private int cameraPriority = 15;
    
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
           UserData userData =
               HostSingleton.Instance.hostGameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

           PlayerName.Value = userData.userName;
        }
        if(!IsOwner) { return; }
        
        virtualCamera.Priority = cameraPriority;
        
    }
}
