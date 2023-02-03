using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetColorGenerator
{
    PlanetColorSettings settings;
    public Material mat { get; private set; }
    Texture2D gradientTexture;
    Texture2D oceanTexture;
    const int textureResolution = 50;
    IPlanetNoiseFilter noiseFilter;

    public void UpdateSettings(PlanetColorSettings settings)
    {
        this.settings = settings;
        mat = new Material(settings.shader);
        if (gradientTexture == null) gradientTexture = new Texture2D(textureResolution, textureResolution);
        if (oceanTexture == null) oceanTexture = new Texture2D(textureResolution, 1);
        noiseFilter = PlanetNoiseFilterFactory.CreateNoiseFilter(settings.latitudeNoiseSettings);
    }

    public void UpdateElevation(MinMaxTracker oceanMinMax, MinMaxTracker elevationMinMax, MinMaxTracker latitudeMinMax)
    {
        mat.SetVector("_OceanMinMax", oceanMinMax.ToVec2());
        mat.SetVector("_ElevationMinMax", elevationMinMax.ToVec2());
        mat.SetVector("_LatitudeMinMax", latitudeMinMax.ToVec2());
    }

    public void UpdateColors()
    {
        int width = gradientTexture.width;
        int height = gradientTexture.height;
        Color[] colors = new Color[width * height];

        for(int y = 0; y < height; y++)
        {
            float yT = y / (height-1f);
            for(int x = 0; x < width; x++)
            {
                float xT = x / (width - 1f);
                int lowerI, higherI;
                float gradientT;
                (lowerI, higherI, gradientT) = GetGradientIndices(yT);

                if (lowerI == higherI)
                {
                    colors[y * width + x] = settings.biomes[lowerI].gradient.Evaluate(xT);
                    colors[y * width + x].a = (settings.biomes[lowerI].overOcean) ? 0f : 1f;
                }
                else
                {
                    colors[y * width + x] = Color.Lerp(settings.biomes[lowerI].gradient.Evaluate(xT), settings.biomes[higherI].gradient.Evaluate(xT), gradientT);
                    float a = (settings.biomes[lowerI].overOcean) ? 0f : 1f;
                    float b = (settings.biomes[higherI].overOcean) ? 0f : 1f;

                    if (a == b || gradientT < 0.5f) colors[y * width + x].a = a;
                    else colors[y * width + x].a = b;
                }
            }
        }

        gradientTexture.SetPixels(colors);
        gradientTexture.Apply();
        mat.SetTexture("_PlanetTexture", gradientTexture);

        Color[] oceanColors = new Color[textureResolution];
        for(int i = 0; i < textureResolution; i++)
        {
            oceanColors[i] = settings.oceanGradient.Evaluate(i / (textureResolution -1f));
        }

        oceanTexture.SetPixels(oceanColors);
        oceanTexture.Apply();
        mat.SetTexture("_OceanTexture", oceanTexture);
    }

    (int, int, float) GetGradientIndices(float lat)
    {
        float belowDist = float.MaxValue;
        int belowIndex = 0;
        float aboveDist = float.MaxValue;
        int aboveIndex = 0;
        
        for(int i = 0; i < settings.biomes.Length; i++)
        {
            float biomeLat = settings.biomes[i].latitude;
            if (lat == biomeLat) return (i, i, 0f);

            float diff = Mathf.Abs(biomeLat - lat);
            //if the latitude is higher, and the distance is closer than any previous save it
            if(biomeLat > lat && diff < aboveDist)
            {
                aboveDist = diff;
                aboveIndex = i;
            }
            //if the latitude is lower, and the distance is closer than any previous save it
            else if (biomeLat < lat && diff < belowDist)
            {
                belowDist = diff;
                belowIndex = i;
            }
        }

        float lowerVal = settings.biomes[belowIndex].latitude;
        float upperVal = settings.biomes[aboveIndex].latitude;
        
        return (belowIndex, aboveIndex, MathUtils.InverseLerp(lowerVal, upperVal, lat));
    }

    public float CalculateLatitudeNoise(Vector3 pointOnUnitCircle)
    {
        return Mathf.Max(0f, noiseFilter.Evaluate(pointOnUnitCircle));
    }
}
