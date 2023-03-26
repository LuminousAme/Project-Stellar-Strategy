using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiParticle : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem>();
    [SerializeField] private SoundEffect sfx;
    [SerializeField] private AudioSource soundSource;

    public void Play()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            if(particles[i] != null) particles[i].Play();
            if(sfx != null && soundSource != null) sfx.Play(soundSource);
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
