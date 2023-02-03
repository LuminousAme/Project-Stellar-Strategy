using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//based on the procedural planet generation series by Sebastian Lague 
//https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
public class PlanetNoiseFilter : IPlanetNoiseFilter
{
    PlanetNoiseSettings settings;
    Noise noise = new Noise();

    public PlanetNoiseFilter(PlanetNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseVal = 0f;
        float frequency = settings.baseRoughness;
        float amplitude = 1f;

        for(int i = 0; i < settings.layers; i++)
        {
            float v = noise.Evaluate(point * frequency + settings.centre);
            noiseVal += (v + 1) * 0.5f * amplitude;

            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noiseVal = noiseVal - settings.minimum;
        return noiseVal * settings.strenght;
    }
}