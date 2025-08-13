using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;
    private ClientGameManager clientGameManager;
    public static ClientSingleton Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindObjectOfType<ClientSingleton>();
            if (instance == null)
            {
                Debug.LogError("No ClientSingleton found");
                return null;
            }
            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task CreateClient()
    {
        clientGameManager = new ClientGameManager();

        await clientGameManager.InitializeAsync();
    }
}