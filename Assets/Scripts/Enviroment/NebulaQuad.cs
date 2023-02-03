using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NebulaQuad : MonoBehaviour
{
    public int width, height;
    public PerlinNoiseSettings noise1;
    public PerlinNoiseSettings noise2;
    public float distortion = 0.3f;
    public float power = 2f;
    public Vector2 maskOffset;
    public float maskScale;
    public Color highLight, mainColor;

    Material mat;
    Texture2D perlinTexture1, perlinTexture2;
    

    void Start()
    {
        Render();
    }

    public void Render()
    {
        mat = GetComponent<MeshRenderer>().material;
        perlinTexture1 = 
            PerlinNoise.GenerateTexture3(width, height, noise1.seed, noise1.scale, 
            noise1.octaves, noise1.persistance, noise1.lacunarity, noise1.offset);

        perlinTexture2 =
            PerlinNoise.GenerateTexture3(width, height, noise2.seed, noise2.scale,
            noise2.octaves, noise2.persistance, noise2.lacunarity, noise2.offset);

        mat.SetTexture("_FirstNoise", perlinTexture1);
        mat.SetTexture("_SecondNoise", perlinTexture2);
        mat.SetFloat("_DistortionLevel", distortion);
        mat.SetFloat("_Power", power);
        mat.SetVector("_MaskOffset", maskOffset);
        mat.SetFloat("_MaskScale", maskScale);
        mat.SetColor("_HighLight", highLight);
        mat.SetColor("_MainColor", mainColor);
    }

    [System.Serializable]
    public struct PerlinNoiseSettings
    {
        public int seed;
        public float scale;
        public int octaves;
        public float persistance;
        public float lacunarity;
        public Vector2 offset;
    }
}
