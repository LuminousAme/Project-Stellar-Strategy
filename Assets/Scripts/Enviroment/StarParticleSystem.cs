using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//based on this video https://youtu.be/v_z2X9QjqsE 
[ExecuteAlways]
[RequireComponent(typeof(ParticleSystem))]
public class StarParticleSystem : MonoBehaviour
{
    private int seed;
    public int Seed
    {
        get { return seed; }
        set { seed = value; }
    }
    [SerializeField] int particleCount = 5000;
    [SerializeField] Vector2 xRange, yRange, zRange;
    [SerializeField] List<Color32> starColors = new List<Color32>();
    ParticleSystem.Particle[] particles;
    ParticleSystem ps;
    int numAlive;
    bool hasRun = false;
    ParticleSystem.EmitParams emitOverride;

    // Start is called before the first frame update
    void Start()
    {
        RunAgain();
    }

    void LateUpdate()
    {
        Init();
        ps.SetParticles(particles, numAlive);
        ps.Emit(emitOverride, particleCount);
        numAlive = ps.GetParticles(particles);
        if (!hasRun) Run();
    }

    void OnValidate()
    {
        RunAgain();
    }

    private void OnEnable()
    {
        RunAgain();
    }

    public void RunAgain()
    {
        if (ps == null) ps = GetComponent<ParticleSystem>();
        ps.Stop();
        ps.Play();
        emitOverride = new ParticleSystem.EmitParams();
        hasRun = false;
    }

    void Init()
    {
        if (ps == null) ps = GetComponent<ParticleSystem>();

        if (ps.main.maxParticles != particleCount)
        {
            var main = ps.main;
            main.maxParticles = particleCount;
        }

        if (particles == null || particles.Length < ps.main.maxParticles) particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void Run()
    {
        Random.InitState(seed);
        for(int i = 0; i < particles.Length; i++)
        {
            particles[i].position = new Vector3(Random.Range(xRange.x, xRange.y), Random.Range(yRange.x, yRange.y), Random.Range(zRange.x, zRange.y));
            particles[i].velocity = Vector3.zero;
            if(starColors.Count > 1)
            {
                int index = Random.Range(0, starColors.Count);
                particles[i].startColor = starColors[index];
            }
        }
        Random.InitState((int)System.DateTime.Now.Ticks);
        hasRun = true;
    }
}
