using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//based on the procedural planet generation series by Sebastian Lague 
//https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
[System.Serializable]
public class PlanetNoiseSettings
{
    public enum FilterType { Simple, Rigid };
    public FilterType filterType;

    public float strenght = 1f;
    [Range(1, 8)]
    public int layers = 1;
    public float baseRoughness = 1f;
    public float roughness = 2f;
    public float persistence = 0.5f;
    public Vector3 centre = Vector3.zero;
    public float minimum;
}
