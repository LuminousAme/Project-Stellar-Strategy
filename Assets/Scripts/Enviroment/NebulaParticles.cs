using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(ParticleSystem))]
public class NebulaParticles : MonoBehaviour
{
    [SerializeField] int seed;
    [SerializeField] int particleCount = 5000;
    [SerializeField] Vector2 xRange, yRange, zRange;
    ParticleSystem.Particle[] particles;
    ParticleSystem ps;
    int numAlive;
    bool hasRun = false;
    ParticleSystem.EmitParams emitOverride;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init()
    {

    }
         
    void Run()
    {

    }
}
