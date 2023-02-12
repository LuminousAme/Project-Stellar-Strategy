using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//based on the procedural planet generation series by Sebastian Lague 
//https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
public class Planet : MonoBehaviour
{
    public bool autoUpdate = true;

    [Range(2, 128)]
    public int resolution = 10;

    public PlanetShapeSettings shapeSettings;
    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;
    public PlanetColorSettings colorSettings;

    PlanetShapeGenerator shapeGenerator = new PlanetShapeGenerator();
    PlanetColorGenerator colorGenerator = new PlanetColorGenerator();

    [SerializeField, HideInInspector] MeshFilter sphereMeshFilter;
    PlanetFullSphere fullSphere;

    void Init()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);

        if (sphereMeshFilter == null)
        {
            GameObject meshObj = new GameObject("Planet Mesh");
            meshObj.transform.parent = transform;
            meshObj.transform.localPosition = Vector3.zero;

            meshObj.AddComponent<MeshRenderer>().sharedMaterial = colorGenerator.mat;
            sphereMeshFilter = meshObj.AddComponent<MeshFilter>();
            sphereMeshFilter.sharedMesh = new Mesh();
        }

        if (sphereMeshFilter.sharedMesh == null)
        {
            sphereMeshFilter.sharedMesh = new Mesh();
        }

        if (sphereMeshFilter.gameObject.GetComponent<MeshRenderer>() == null)
        {
            sphereMeshFilter.gameObject.AddComponent<MeshRenderer>().sharedMaterial = colorGenerator.mat;
        }
        else if (sphereMeshFilter.gameObject.GetComponent<MeshRenderer>().sharedMaterial == null)
        {
            sphereMeshFilter.gameObject.GetComponent<MeshRenderer>().sharedMaterial = colorGenerator.mat;
        }

        fullSphere = new PlanetFullSphere(shapeGenerator, colorGenerator, sphereMeshFilter.sharedMesh, resolution);
    }

    public void GeneratePlanet()
    {
        Init();
        GenerateMesh();
        GenerateColors();
    }

    public void OnShapeSettingsUpdated()
    {
        if(autoUpdate)
        {
            Init();
            GenerateMesh();
        }
    }

    public void OnColorSettingsUpdated()
    {
        if(autoUpdate)
        {
            Init();
            GenerateColors();
        }
    }

    void GenerateMesh()
    {
        fullSphere.GenerateMesh();
        colorGenerator.UpdateElevation(shapeGenerator.oceanElevationMinMax, shapeGenerator.elevationMinMax, shapeGenerator.latitudeMinMax);
    }

    void GenerateColors()
    {
        colorGenerator.UpdateColors();
    }
}
