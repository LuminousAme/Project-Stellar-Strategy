using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise
{
    public static Texture2D GenerateTexture(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        Texture2D texture = new Texture2D(width, height);

        //generate random offsets for each level of the noise
        System.Random rand = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            octaveOffsets[i] = new Vector2(rand.Next(-100000, 100000), rand.Next(-100000, 100000)) + offset;
        }

        //generate the noise
        float[,] noiseMap = new float[width, height];
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        //loop over each pixel
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                //loops over each level of noise
                for (int i = 0; i < octaves; i++)
                {
                    float xCoord = (x - halfWidth) / width * scale * frequency + octaveOffsets[i].x;
                    float yCoord = (y - halfHeight) / height * scale * frequency + octaveOffsets[i].y;

                    float shade = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
                    noiseHeight += shade * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;
            }
        }

        //look over each pixel brining them back into the 0-1 range and adding them to the color map
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = MathUtils.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        //apply the color map to the texture and return it
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D GenerateTexture3(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        Texture2D texture = new Texture2D(width, height);

        //generate random offsets for each level of the noise
        System.Random rand = new System.Random(seed);

        Vector2[] octaveOffsetsR = new Vector2[octaves];
        Vector2[] octaveOffsetsG = new Vector2[octaves];
        Vector2[] octaveOffsetsB = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            octaveOffsetsR[i] = new Vector2(rand.Next(-100000, 100000), rand.Next(-100000, 100000)) + offset;
            octaveOffsetsG[i] = new Vector2(rand.Next(-100000, 100000), rand.Next(-100000, 100000)) + offset;
            octaveOffsetsB[i] = new Vector2(rand.Next(-100000, 100000), rand.Next(-100000, 100000)) + offset;
        }

        //red
        float[,] noiseMapR = new float[width, height];
        float maxNoiseHeightR = float.MinValue;
        float minNoiseHeightR = float.MaxValue;
        //green
        float[,] noiseMapG = new float[width, height];
        float maxNoiseHeightG = float.MinValue;
        float minNoiseHeightG = float.MaxValue;
        //blue
        float[,] noiseMapB = new float[width, height];
        float maxNoiseHeightB = float.MinValue;
        float minNoiseHeightB = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        //loop over each pixel
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeightR = 0f;
                float noiseHeightG = 0f;
                float noiseHeightB = 0f;

                //loops over each level of noise
                for (int i = 0; i < octaves; i++)
                {
                    //red 
                    float xCoordR = (x - halfWidth) / width * scale * frequency + octaveOffsetsR[i].x;
                    float yCoordR = (y - halfHeight) / height * scale * frequency + octaveOffsetsR[i].y;
                    float shadeR = Mathf.PerlinNoise(xCoordR, yCoordR) * 2 - 1;
                    noiseHeightR += shadeR * amplitude;

                    //green
                    float xCoordG = (x - halfWidth) / width * scale * frequency + octaveOffsetsG[i].x;
                    float yCoordG = (y - halfHeight) / height * scale * frequency + octaveOffsetsG[i].y;
                    float shadeG = Mathf.PerlinNoise(xCoordG, yCoordG) * 2 - 1;
                    noiseHeightG += shadeG * amplitude;

                    //blue
                    float xCoordB = (x - halfWidth) / width * scale * frequency + octaveOffsetsB[i].x;
                    float yCoordB = (y - halfHeight) / height * scale * frequency + octaveOffsetsB[i].y;
                    float shadeB = Mathf.PerlinNoise(xCoordB, yCoordB) * 2 - 1;
                    noiseHeightB += shadeB * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                //red
                if (noiseHeightR > maxNoiseHeightR) maxNoiseHeightR = noiseHeightR;
                if (noiseHeightR < minNoiseHeightR) minNoiseHeightR = noiseHeightR;
                noiseMapR[x, y] = noiseHeightR;

                //green
                if (noiseHeightG > maxNoiseHeightG) maxNoiseHeightG = noiseHeightG;
                if (noiseHeightG < minNoiseHeightG) minNoiseHeightG = noiseHeightG;
                noiseMapG[x, y] = noiseHeightG;

                //blue
                if (noiseHeightB > maxNoiseHeightB) maxNoiseHeightB = noiseHeightB;
                if (noiseHeightB < minNoiseHeightB) minNoiseHeightB = noiseHeightB;
                noiseMapB[x, y] = noiseHeightB;
            }
        }

        //look over each pixel brining them back into the 0-1 range and adding them to the color map
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMapR[x, y] = MathUtils.InverseLerp(minNoiseHeightR, maxNoiseHeightR, noiseMapR[x, y]);
                noiseMapG[x, y] = MathUtils.InverseLerp(minNoiseHeightG, maxNoiseHeightG, noiseMapG[x, y]);
                noiseMapB[x, y] = MathUtils.InverseLerp(minNoiseHeightB, maxNoiseHeightB, noiseMapB[x, y]);

                colorMap[y * width + x] = new Color(noiseMapR[x, y], noiseMapG[x, y], noiseMapB[x, y]);
            }
        }

        //apply the color map to the texture and return it
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }
}
