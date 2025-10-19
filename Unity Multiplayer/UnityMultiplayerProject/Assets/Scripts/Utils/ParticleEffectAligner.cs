using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffectAligner : MonoBehaviour
{
    private ParticleSystem.MainModule mainModule;
    private void Start()
    {
        mainModule = GetComponent<ParticleSystem>().main;
    }
    
    private void Update()
    {
        mainModule.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }
}
