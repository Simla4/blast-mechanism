using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BlockParticle : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> particleSystems;

    public void ChangeParticleColor(Color color)
    {
        for (int i = 0; i < particleSystems.Count; i++)
        {
            // .startColor yerine .main.startColor kullanıyoruz
            var mainModule = particleSystems[i].main;
            mainModule.startColor = color;
        }
    }
}