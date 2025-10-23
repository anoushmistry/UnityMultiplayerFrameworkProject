using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorDisplay : MonoBehaviour
{
    [SerializeField] private TeamColorLookup teamColorLookup;
    [SerializeField] private TankPlayer tankPlayer;
    [SerializeField] private SpriteRenderer[] spriteRenderers; // For tank turret and body

    private void Start()
    {
        UpdatePlayerSprites(-1, tankPlayer.TeamIndex.Value);
        tankPlayer.TeamIndex.OnValueChanged += UpdatePlayerSprites;
    }

    private void UpdatePlayerSprites(int previousValue, int teamId)
    {
        Color color = teamColorLookup.GetTeamColor(teamId);
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = color;
        }
    }

    private void OnDestroy()
    {
        tankPlayer.TeamIndex.OnValueChanged -= UpdatePlayerSprites;
    }
}
