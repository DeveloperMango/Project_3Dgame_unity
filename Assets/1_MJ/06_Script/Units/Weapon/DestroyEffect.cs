using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    private ParticleSystem parts;

    void Awake()
    {
        parts = gameObject.GetComponent<ParticleSystem>();
        var mainModule = parts.main;
        Destroy(gameObject, mainModule.duration);
    }
}