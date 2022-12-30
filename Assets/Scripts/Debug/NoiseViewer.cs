using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseViewer : MonoBehaviour
{
    public int perlinWidth;
    public int perlinHeight;
    public int perlinSeed;
    public float perlinScale;
    public int perlinoctaves;
    public float perlinpersistance;
    public float perlinlacunarity;
    public Vector2 perlinOffset;

    public bool perlin3;

    // Start is called before the first frame update
    void Start()
    {
        Render();
    }

    private void OnValidate()
    {
        Render();
    }

    void Render()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        Texture2D perlinTexture;
        if (perlin3)
            perlinTexture = PerlinNoise.GenerateTexture3(perlinWidth, perlinHeight, perlinSeed, perlinScale, perlinoctaves, perlinpersistance, perlinlacunarity, perlinOffset);
        else
            perlinTexture = PerlinNoise.GenerateTexture(perlinWidth, perlinHeight, perlinSeed, perlinScale, perlinoctaves, perlinpersistance, perlinlacunarity, perlinOffset);

        renderer.sharedMaterial.mainTexture = perlinTexture;
    }
}
