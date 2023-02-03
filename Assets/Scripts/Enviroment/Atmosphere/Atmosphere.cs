using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class Atmosphere : MonoBehaviour
{
    public Gradient gradient;
    static int resolution = 50;
    Texture2D gradientTexture;
    public CelestialBody sun;
    [Range(0f, 0.05f)]
    public float thickness;
    [Range(1f, 25f)]
    public float power;
    public float radius;
    Material mat;
    public Shader shader;

    void Start()
    {
        Init();
    }

    void Init()
    {
        if(shader != null)
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.sharedMaterial = new Material(shader);
            mat = mr.sharedMaterial;

            if(gradientTexture == null) gradientTexture = new Texture2D(resolution, 1);

            Color[] colors = new Color[resolution];
            for (int i = 0; i < resolution; i++)
            {
                colors[i] = gradient.Evaluate(i / (resolution - 1f));
            }

            gradientTexture.SetPixels(colors);
            gradientTexture.Apply();
            mat.SetTexture("_Gradient", gradientTexture);
        }
    }

    void OnValidate()
    {
        Init();    
    }

    void Update()
    {
        if(sun != null && mat!=null)
        {
            transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
            Vector3 sunDir = -(transform.position - sun.transform.position).normalized;
            mat.SetVector("_LightDir", sunDir);
            mat.SetFloat("_Radius", radius);
            mat.SetFloat("_Thickness", thickness);
            mat.SetFloat("_Power", power);
        }
    }
}
