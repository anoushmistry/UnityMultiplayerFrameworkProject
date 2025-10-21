using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button FindMatchButton;
    [SerializeField] private TMP_Text findMatchText;

    [FormerlySerializedAs("queueTimeText")] [SerializeField]
    private TMP_Text queueTimerText;

    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private Toggle teamFillToggle; // To Toggle Team Mode Based Matching
    [SerializeField] private Toggle privateLobbyToggle;
    
    private float timeInQueue;

    private bool isMatchmaking, isCancelling;

    private bool
        isBusy; //Used to prevent player starting as a host or client while matchmaking on a server or vice versa

    private void Start()
    {
        if (ClientSingleton.Instance == null)
        {
            return;
        }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        queueStatusText.text = string.Empty;
        queueTimerText.text = string.Empty;
    }

    private void Update()
    {
        if (isMatchmaking)
        {
            timeInQueue += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeInQueue);
            queueTimerText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }

    public async void FindMatch()
    {
        if (isCancelling)
        {
            return;
        }

        if (isMatchmaking)
        {
            queueStatusText.text = "Cancelling...";
            isCancelling = true;
            await ClientSingleton.Instance.clientGameManager.CancelMatchmaking(); //Cancel Queue
            isCancelling = false;
            isMatchmaking = false;
            findMatchText.text = "Find Match";
            queueStatusText.text = string.Empty;
            queueTimerText.text = string.Empty;
            isBusy = false;
            return;
        }

        if (isBusy)
        {
            return;
        }

        ClientSingleton.Instance.clientGameManager.MatchmakeAsync(OnMatchmake, teamFillToggle.isOn); //Start Queue
        findMatchText.text = "Cancel";
        queueStatusText.text = "Searching...";
        timeInQueue = 0;
        isMatchmaking = true;
        isBusy = true;
    }

    private void OnMatchmake(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                queueStatusText.text = "Connecting...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                queueStatusText.text = "Ticket creation error";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                queueStatusText.text = "Match assignment error";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                queueStatusText.text = "Ticket cancellation error";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                queueStatusText.text = "Ticket retrieval error";
                break;
        }
    }

    public async void StartHost()
    {
        if (isBusy)
        {
            return;
        }

        isBusy = true;
        await HostSingleton.Instance.hostGameManager.StartHostAsync(privateLobbyToggle.isOn);
        isBusy = false; // If start host fails or finishes, the busy flag is set to false
    }

    public async void StartClient()
    {
        if (isBusy)
        {
            return;
        }

        isBusy = true;
        await ClientSingleton.Instance.clientGameManager.StartClientAsync(joinCodeField.text);
        isBusy = false; // If start host fails or finishes, the busy flag is set to false
    }
    public async void JoinAsync(Lobby lobby) // Imported from the LobbiesList.cs file as the logic is similar to start client
    {
        if (isBusy)
        {
            return;
        }

        isBusy = true;
        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.clientGameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        isBusy = false;
    }
}