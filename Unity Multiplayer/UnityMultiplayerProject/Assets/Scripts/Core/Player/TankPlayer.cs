using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")] [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    [Header("Settings")] [SerializeField] private int cameraPriority = 15;
    [field: SerializeField] public Health Health { get; private set; }

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData =
                HostSingleton.Instance.hostGameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

            OnPlayerSpawned?.Invoke(this);

            PlayerName.Value = userData.userName;
        }

        if (!IsOwner)
        {
            return;
        }

        virtualCamera.Priority = cameraPriority;
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}