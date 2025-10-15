using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
   [SerializeField] private Button FindMatchButton;
   [SerializeField] private TMP_Text findMatchText;
   [SerializeField] private TMP_Text queueTimeText;
   [SerializeField] private TMP_Text queueStatusText;
   [SerializeField] private TMP_InputField joinCodeField;

   private void Start()
   {
      if (ClientSingleton.Instance == null)
      {
         return;
      }
      Cursor.SetCursor(null,Vector2.zero, CursorMode.Auto);
      queueStatusText.text = string.Empty;
      queueTimeText.text = string.Empty;
   }
   public async void StartHost()
   {
      await HostSingleton.Instance.hostGameManager.StartHostAsync();
   }

   public async void StartClient()
   {
      await ClientSingleton.Instance.clientGameManager.StartClientAsync(joinCodeField.text);
   }
}
