using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;
    [SerializeField] private NetworkObject playerPrefab;
    
    private const string GameSceneName = "Game";
    
    private ApplicationData applicationData;
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            Application.targetFrameRate = 60; // To avoid Excessive CPU usage
            applicationData = new ApplicationData();
            ServerSingleton serverSingleton = Instantiate(serverPrefab);
            
            StartCoroutine(LoadGameplaySceneAsync(serverSingleton));
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost(playerPrefab);

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
    private IEnumerator LoadGameplaySceneAsync(ServerSingleton serverSingleton)
    {
       AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameSceneName, LoadSceneMode.Single);

       while (asyncOperation.isDone == false)
       {
           yield return null;
       }
       Task createServerTask = serverSingleton.CreateServer(playerPrefab);
       yield return
           new WaitUntil(() =>
               createServerTask
                   .IsCompleted); // Way to await a Task in a coroutine without using the await keyword (as it isn't accessible)
       
       Task startServerTask = serverSingleton.serverGameManager.StartGameServerAsync();
       yield return new WaitUntil(() => startServerTask.IsCompleted);
    }
}