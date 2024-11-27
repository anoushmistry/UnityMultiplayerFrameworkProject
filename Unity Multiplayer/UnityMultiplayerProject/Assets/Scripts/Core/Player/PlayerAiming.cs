using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretTransform;
    [SerializeField] private InputReader inputReader;

    [Header("Settings")]
    [SerializeField] private Vector2 aimScreenPosition;


    private void LateUpdate()
    {
        if(!IsOwner) { return; }

        aimScreenPosition = inputReader.AimPosition;  //Storing the Aim position from the input reader to the local AimPosition variable
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);
        turretTransform.up = new Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y);

    }

}
