using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//based on the procedural planet generation series by Sebastian Lague 
//https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
public class PlanetShapeGenerator
{
    PlanetShapeSettings settings;
    IPlanetNoiseFilter[] noiseFilters;
    public MinMaxTracker oceanElevationMinMax;
    public MinMaxTracker elevationMinMax;
    public MinMaxTracker latitudeMinMax;

    public void UpdateSettings(PlanetShapeSettings settings)
    {
        this.settings = settings;
        noiseFilters = new IPlanetNoiseFilter[settings.noiseLayers.Length];
        for(int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = PlanetNoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
        oceanElevationMinMax = new MinMaxTracker();
        elevationMinMax = new MinMaxTracker();
        latitudeMinMax = new MinMaxTracker();
    }

    public (Vector3, float) CalculatePointOnPlanet(Vector3 pointOnUnitSphere) {
        float unscaled = CalculateUnScaledElevation(pointOnUnitSphere);
        float elevation = ConvertToScaledElevation(unscaled);
        elevationMinMax.AddValue(elevation);
        oceanElevationMinMax.AddValue(unscaled);
        Vector3 point = pointOnUnitSphere * elevation;
        latitudeMinMax.AddValue(point.y);
        return (point, unscaled);
    }

    public float CalculateUnScaledElevation(Vector3 pointOnUnitSphere)
    {
        float firstLayer = 0;
        float elevation = 0;

        if (noiseFilters.Length > 0)
        {
            firstLayer = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled) elevation = firstLayer;
        }

        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayer : 1f;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }

        return elevation;
    }

    public float ConvertToScaledElevation(float unscaledElevation)
    {
        float elevation = Mathf.Max(0, unscaledElevation);
        elevation = settings.radius * (1f + elevation);
        return elevation;
    }
}
