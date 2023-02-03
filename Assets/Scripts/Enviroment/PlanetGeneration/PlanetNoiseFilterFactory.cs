using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//based on the procedural planet generation series by Sebastian Lague 
//https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
public static class PlanetNoiseFilterFactory
{
    public static IPlanetNoiseFilter CreateNoiseFilter(PlanetNoiseSettings settings)
    {
        switch(settings.filterType)
        {
            case PlanetNoiseSettings.FilterType.Simple:
                return new PlanetNoiseFilter(settings);
            case PlanetNoiseSettings.FilterType.Rigid:
                return new PlanetRigidNoiseFilter(settings);
        }
        return null;
    }
}
