using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiParticle : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particles = new List<ParticleSystem>();

    public void Play()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Play();
        }
    }

    public void SetColor(Color color)
    {
        for (int i = 0; i < particles.Count; i++)
        {
            ParticleSystem.MainModule main = particles[i].main;
            main.startColor = color;
        }
    }
}
