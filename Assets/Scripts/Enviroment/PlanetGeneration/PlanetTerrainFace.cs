using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//based on the procedural planet generation series by Sebastian Lague 
//https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
public class PlanetTerrainFace 
{
    PlanetShapeGenerator shapeGenerator;
    Mesh mesh;
    int resolution;
    Vector3 normal;
    Vector3 tangent;
    Vector3 bitangent;

    public PlanetTerrainFace(PlanetShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 normal)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.normal = normal;

        tangent = new Vector3(normal.y, normal.z, normal.x);
        bitangent = Vector3.Cross(normal, tangent);
    }

    public void GenerateMesh()
    {
        Vector3[] verts = new Vector3[resolution * resolution];
        int[] tris = new int[(resolution - 1) * (resolution - 1) * 6];

        int triIndex = 0;
        for(int y = 0; y < resolution; y++)
        {
            for(int x = 0; x < resolution; x++)
            {
                //calculate the vertex
                int index = x + resolution * y;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = normal + (percent.x - 0.5f) * 2f * tangent + (percent.y - 0.5f) * 2f * bitangent;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                //verts[index] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                //if it should form a triangle (not along the bottom or right edges) than calculate the indices that triangle should form
                if(x != resolution - 1 && y != resolution - 1)
                {
                    //winding order matters
                    tris[triIndex] = index;
                    tris[triIndex + 1] = index + resolution + 1;
                    tris[triIndex + 2] = index + resolution;

                    tris[triIndex + 3] = index;
                    tris[triIndex + 4] = index + 1;
                    tris[triIndex + 5] = index + resolution + 1;

                    triIndex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }
}
