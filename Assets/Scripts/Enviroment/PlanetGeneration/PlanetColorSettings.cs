using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//based on the procedural planet generation series by Sebastian Lague 
//https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
[CreateAssetMenu(fileName = "PlanetColor", menuName = "StellarStrategy/ProceduralPlanets/ColorSettings", order = 1)]
public class PlanetColorSettings : ScriptableObject
{
    public Biome[] biomes;
    public Gradient oceanGradient;
    public Shader shader;
    public PlanetNoiseSettings latitudeNoiseSettings;

    [System.Serializable]
    public class Biome
    {
        public Gradient gradient;
        [Range(0f, 1f)]
        public float latitude;
        public bool overOcean;
    }
}
