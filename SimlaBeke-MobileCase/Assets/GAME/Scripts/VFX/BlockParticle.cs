using System;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BlockParticle : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> particleSystems;

    public void Init(Color color)
    {
        for (int i = 0; i < particleSystems.Count; i++)
        {
            var mainModule = particleSystems[i].main;
            mainModule.startColor = color;
        }
    }

    private void OnParticleSystemStopped()
    {
        LeanPool.Despawn(gameObject);
    }
}