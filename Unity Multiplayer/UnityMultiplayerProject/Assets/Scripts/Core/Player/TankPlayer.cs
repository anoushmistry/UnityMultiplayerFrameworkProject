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

    [SerializeField] private SpriteRenderer minimapSpriteRenderer;
    [SerializeField] private Texture2D crosshairTexture;
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public CoinWallet CoinWallet { get; private set; }

    [Header("Settings")] [SerializeField] private int cameraPriority = 15;
    [SerializeField] private Color playerMinimapColor;


    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = null;
            if (IsHost)
            {
                userData =
                    HostSingleton.Instance.hostGameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }
            else
            {
                userData = ServerSingleton.Instance.serverGameManager.networkServer.GetUserDataByClientId(OwnerClientId);
            }

            PlayerName.Value = userData.userName;

            OnPlayerSpawned?.Invoke(this);
        }

        if (!IsOwner)
        {
            return;
        }

        virtualCamera.Priority = cameraPriority;

        minimapSpriteRenderer.color = playerMinimapColor;

        Cursor.SetCursor(crosshairTexture, new Vector2(crosshairTexture.width / 2, crosshairTexture.height / 2),
            CursorMode.Auto);
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}