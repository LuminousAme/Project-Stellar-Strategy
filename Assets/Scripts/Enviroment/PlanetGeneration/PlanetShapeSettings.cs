using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//based on the procedural planet generation series by Sebastian Lague 
//https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
[CreateAssetMenu(fileName = "PlanetShape", menuName = "StellarStrategy/ProceduralPlanets/ShapeSettings", order = 0)]
public class PlanetShapeSettings : ScriptableObject
{
    public float radius;
    public NoiseLayer[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerAsMask = true;
        public PlanetNoiseSettings noiseSettings;
    }
}
