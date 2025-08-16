using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();

            ClientSingleton
                clientSingleton =
                    Instantiate(
                        clientPrefab); // Async means to wait until a specific task is completed before continuing on to other tasks

            bool authenticated = await clientSingleton.CreateClient();

            if (authenticated)
            {
                clientSingleton.clientGameManager.GoToMenu();
            }
            // Go to the Main Menu
        }
    }
}