using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();


    private void OnEnable()
    {
        spawnPoints.Add(this);
    }

    public static Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            return Vector3.zero;
        }

        return spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
    }

    private void OnDisable()
    {
        spawnPoints.Remove(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(this.transform.position, 1f);
    }
}