using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : NetworkBehaviour
{
    [SerializeField] private TMP_Text lobbyJoinCodeText;

    private NetworkVariable<FixedString32Bytes> lobbyJoinCode = new NetworkVariable<FixedString32Bytes>("");

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            lobbyJoinCode.OnValueChanged += HandleLobbyCodeChanged;
            HandleLobbyCodeChanged(string.Empty,lobbyJoinCode.Value);
        }
        if (IsHost)
        {
            lobbyJoinCode.Value = HostSingleton.Instance.hostGameManager.JoinCode;
            
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            lobbyJoinCode.OnValueChanged -= HandleLobbyCodeChanged;
        }
    }
    private void HandleLobbyCodeChanged(FixedString32Bytes oldValue, FixedString32Bytes newValue)
    {
        lobbyJoinCodeText.text = newValue.ToString();
    }
    public void LeaveGame() //Purely used when there is a self-host or client-host as it won't apply to a dedicated server (as in wouldn't shutdown the server)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.hostGameManager.Shutdown();
        }

        ClientSingleton.Instance.clientGameManager.Disconnect();

    }
}
