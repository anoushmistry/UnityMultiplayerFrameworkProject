using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    public void LeaveGame() //Purely used when there is a self-host or client-host as it won't apply to a dedicated server
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.hostGameManager.Shutdown();
        }

        ClientSingleton.Instance.clientGameManager.Disconnect();

    }
}
