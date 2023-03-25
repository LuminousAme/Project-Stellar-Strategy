using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBlast : MonoBehaviour
{
    [HideInInspector] public Unit target;
    [SerializeField] private float speed = 20f;
    [SerializeField] private int maxDamage = 5;
    [SerializeField] private int minDamage = 1;
    [SerializeField] MultiParticle impact;

    private void Update()
    {
        if(target != null)
        {
            Vector3 velocity = (target.transform.position - transform.position).normalized * speed;
            transform.position = transform.position + velocity * Time.deltaTime;
            transform.rotation = Quaternion.FromToRotation(transform.forward, velocity.normalized) * transform.rotation;

            if (Vector3.Distance(target.transform.position, transform.position) < 1f)
            {
                Contact();
            }
        }
    }

    void Contact()
    {
        speed = 0f;
        target.TakeDamage(Random.Range(minDamage, maxDamage + 1));
        target = null;
        impact.Play();
        Destroy(gameObject, 0.2f);
    }

    public void SetColor(Color color)
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        for(int i = 0; i < particles.Length; i++)
        {
            ParticleSystem.MainModule main = particles[i].main;
            main.startColor = color;
        }

        Color baseColor = new Color(color.r, color.g, color.b, 1f);
        Color endColor = new Color(color.r, color.g, color.b, 0f);

        TrailRenderer trail = GetComponentInChildren<TrailRenderer>();
        trail.startColor = baseColor;
        trail.endColor = endColor;

        impact.SetColor(color);
    }
}
