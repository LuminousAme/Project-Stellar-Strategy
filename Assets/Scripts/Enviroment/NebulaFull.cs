using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NebulaFull : MonoBehaviour
{
    private int seed;
    public int Seed
    {
        get { return seed; }
        set { seed = value; }
    }

    [Header("General")]
    [SerializeField] GameObject quadPrefab;
    [SerializeField] int textureWidth, textureHeight;

    [Space]
    [Header("Nebula Spawning")]
    [SerializeField] int coreSegments;
    [SerializeField] int brightSegments, dimSegments;
    [SerializeField] Vector2 corePositionRange, brightPositionRange, dimPositionRange;
    [SerializeField] Vector2 coreScaleRange, brightScaleRange, dimScaleRange;

    [Space]
    [Header("Nebula Settings")]
    [SerializeField] int octaves;
    [SerializeField] float persistance, lacunarity, scale;
    [SerializeField] Vector2 distortionRange, maskScaleRange;
    [SerializeField] Vector2 corePowerRange, brightPowerRange, dimPowerRange;

    [Space]
    [Header("Nebula Colors")]
    [SerializeField] List<NebulaColors> coreColors = new List<NebulaColors>();
    [SerializeField] List<NebulaColors> brightColors = new List<NebulaColors>();
    [SerializeField] List<NebulaColors> dimColors = new List<NebulaColors>();


    List<GameObject> quadObjects = new List<GameObject>();

    void Start()
    {
        //Run();
    }

    public void Run()
    {
        Clear();
        Generate();
    }

    void Clear()
    {
        for (int i = 0; i < quadObjects.Count; i++) if (quadObjects[i]) DestroyImmediate(quadObjects[i]);
        quadObjects.Clear();
    }

    void Generate()
    {
        Random.InitState(seed);

        if(quadPrefab)
        {
            //core segments
            for(int i = 0; i < coreSegments; i++)
            {
                GameObject go = Instantiate(quadPrefab, transform);
                quadObjects.Add(go);

                Transform trans = go.transform;
                float x = Random.Range(corePositionRange.x, corePositionRange.y);
                float y = Random.Range(-2.5f, 0.0f);
                float z = Random.Range(corePositionRange.x, corePositionRange.y);
                trans.localPosition = new Vector3(x, y, z);
                float s = Random.Range(coreScaleRange.x, coreScaleRange.y);
                trans.localScale = new Vector3(s, s, 1f);
                float r = Random.Range(0f, 360f);
                trans.localRotation = Quaternion.Euler(90f, 0f, r);

                NebulaQuad nq = go.GetComponent<NebulaQuad>();
                nq.width = textureWidth;
                nq.height = textureHeight;
                nq.noise1 = MakeNoiseSettings();
                nq.noise2 = MakeNoiseSettings();
                nq.distortion = Random.Range(distortionRange.x, distortionRange.y);
                nq.power = Random.Range(corePowerRange.x, corePowerRange.y);
                nq.maskScale = Random.Range(maskScaleRange.x, maskScaleRange.y);

                if (coreColors.Count > 0)
                {
                    int index = Random.Range(0, coreColors.Count);
                    NebulaColors colors = coreColors[index];
                    nq.highLight = colors.highlight;
                    nq.mainColor = colors.main;
                }
            }

            //bright segments
            for (int i = 0; i < brightSegments; i++)
            {
                GameObject go = Instantiate(quadPrefab, transform);
                quadObjects.Add(go);

                Transform trans = go.transform;
                float x = Random.Range(brightPositionRange.x, brightPositionRange.y);
                float y = Random.Range(0.0f, 2.5f);
                float z = Random.Range(brightPositionRange.x, brightPositionRange.y);
                trans.localPosition = new Vector3(x, y, z);
                float s = Random.Range(brightScaleRange.x, brightScaleRange.y);
                trans.localScale = new Vector3(s, s, 1f);
                float r = Random.Range(0f, 360f);
                trans.localRotation = Quaternion.Euler(90f, 0f, r);

                NebulaQuad nq = go.GetComponent<NebulaQuad>();
                nq.width = textureWidth;
                nq.height = textureHeight;
                nq.noise1 = MakeNoiseSettings();
                nq.noise2 = MakeNoiseSettings();
                nq.distortion = Random.Range(distortionRange.x, distortionRange.y);
                nq.power = Random.Range(brightPowerRange.x, brightPowerRange.y);
                nq.maskScale = Random.Range(maskScaleRange.x, maskScaleRange.y);

                if (brightColors.Count > 0)
                {
                    int index = Random.Range(0, brightColors.Count);
                    NebulaColors colors = brightColors[index];
                    nq.highLight = colors.highlight;
                    nq.mainColor = colors.main;
                }
            }

            //dim segments
            for (int i = 0; i < dimSegments; i++)
            {
                GameObject go = Instantiate(quadPrefab, transform);
                quadObjects.Add(go);

                Transform trans = go.transform;
                float x = Random.Range(dimPositionRange.x, dimPositionRange.y);
                float y = Random.Range(2.5f, 5.0f);
                float z = Random.Range(dimPositionRange.x, dimPositionRange.y);
                trans.localPosition = new Vector3(x, y, z);
                float s = Random.Range(dimScaleRange.x, dimScaleRange.y);
                trans.localScale = new Vector3(s, s, 1f);
                float r = Random.Range(0f, 360f);
                trans.localRotation = Quaternion.Euler(90f, 0f, r);

                NebulaQuad nq = go.GetComponent<NebulaQuad>();
                nq.width = textureWidth;
                nq.height = textureHeight;
                nq.noise1 = MakeNoiseSettings();
                nq.noise2 = MakeNoiseSettings();
                nq.distortion = Random.Range(distortionRange.x, distortionRange.y);
                nq.power = Random.Range(dimPowerRange.x, dimPowerRange.y);
                nq.maskScale = Random.Range(maskScaleRange.x, maskScaleRange.y);

                if (dimColors.Count > 0)
                {
                    int index = Random.Range(0, dimColors.Count);
                    NebulaColors colors = dimColors[index];
                    nq.highLight = colors.highlight;
                    nq.mainColor = colors.main;
                }
            }
        }

        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    NebulaQuad.PerlinNoiseSettings MakeNoiseSettings()
    {
        NebulaQuad.PerlinNoiseSettings settings = new NebulaQuad.PerlinNoiseSettings();
        settings.seed = Random.Range(0, 100000);
        settings.scale = scale;
        settings.octaves = octaves;
        settings.persistance = persistance;
        settings.lacunarity = lacunarity;
        settings.offset = Vector2.zero;
        return settings;
    }

    [System.Serializable]
    public struct NebulaColors
    {
        public Color highlight;
        public Color main;
    }
}
