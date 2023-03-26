using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int seed = 197;
    [SerializeField] bool randomizeSeed = false;
    [SerializeField] int minSeed = 0;
    [SerializeField] int maxSeed = 25000;

    [Space]
    [Header("Generators")]
    [SerializeField] NebulaFull nebulaGenerator;
    [SerializeField] PlanetGenerator planetGenerator;
    [SerializeField] StarParticleSystem starGenerator;

    private void Start()
    {
        if (randomizeSeed) seed = Random.Range(minSeed, maxSeed);
        Run();
    }

    public void Run()
    {
        if(nebulaGenerator)
        {
            nebulaGenerator.Seed = seed;
            nebulaGenerator.Run();
        }

        if (planetGenerator)
        {
            planetGenerator.Seed = seed;
            planetGenerator.Run();
        }

        if (starGenerator)
        {
            starGenerator.Seed = seed;
            starGenerator.RunAgain();
        }
    }
}
