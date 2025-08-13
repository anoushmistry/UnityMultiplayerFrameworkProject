using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;
    [SerializeField] private HostGameManager hostGameManager;

    public static HostSingleton Instance
    {
        get
        {
            if(instance != null) return instance;

            instance = FindObjectOfType<HostSingleton>();
            if (instance == null)
            {
                Debug.LogError("Failed to find HostSingleton");
                return null;
            }
            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        hostGameManager =  new HostGameManager();
        
    }
}
