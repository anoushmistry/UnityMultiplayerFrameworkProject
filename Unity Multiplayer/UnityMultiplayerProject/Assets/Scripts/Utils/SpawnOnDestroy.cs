using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    private void OnDestroy()
    {
        Instantiate(objectToSpawn, transform.position, Quaternion.identity);
    }
}
