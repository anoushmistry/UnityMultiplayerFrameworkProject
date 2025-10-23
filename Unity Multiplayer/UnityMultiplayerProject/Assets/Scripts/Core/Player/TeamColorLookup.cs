using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for assigning colors to players' tanks depending on their team in a game
/// </summary>
[CreateAssetMenu(fileName = "NewTeamColorLookup", menuName = "Team Color Lookup")]
public class TeamColorLookup : ScriptableObject
{
    [SerializeField] private Color[] teamColors;

    public Color GetTeamColor(int teamIndex)
    {
        Debug.Log($"Team Index is {teamIndex}");
        if (teamIndex < 0 || teamIndex >= teamColors.Length)
        {
            return Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        else
        {
            return teamColors[teamIndex];
        }
    }
}