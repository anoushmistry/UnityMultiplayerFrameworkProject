using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float turningRate = 270f;
    [SerializeField] private Vector2 previousMoveInput; //Most recent movement input received


    public override void OnNetworkSpawn()       //Using this because the on client Start() method starts too early because... 
    {                                           // we have to factor in the network delays, loading of everything etc.
        if(!IsOwner) return;
        inputReader.OnMoveEvent += HandleMove; 

    }
    public override void OnNetworkDespawn()     // Using this because the on client (local) OnDestroy() method is too late..
    {                                           // And that point we can't check who the owner is
        if(!IsOwner) return; 
        inputReader.OnMoveEvent -= HandleMove;
    }
    private void Update()
    {
        if (!IsOwner) { return; }

        float zRotation = previousMoveInput.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);
    }
    private void FixedUpdate()
    {
        if (!IsOwner) { return; }

        rb.velocity = (Vector2)(bodyTransform.up) * previousMoveInput.y * movementSpeed; 
    }
    private void HandleMove(Vector2 movementInput)
    {
        previousMoveInput = movementInput;
    }
    
}
