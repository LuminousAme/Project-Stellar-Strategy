using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//based on the procedural planet generation series by Sebastian Lague 
//https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
//and the solar system series SphereMeshClass https://github.com/SebLague/Solar-System/blob/Episode_02/Assets/Celestial%20Body/Scripts/SphereMesh.cs
public class PlanetFullSphere
{
    PlanetShapeGenerator shapeGenerator;
    PlanetColorGenerator colorGenerator;
    Mesh mesh;
    int resolution;

    // Indices of the vertex pairs that make up each of the initial 12 edges
    static readonly int[] vertexPairs = { 0, 1, 0, 2, 0, 3, 0, 4, 1, 2, 2, 3, 3, 4, 4, 1, 5, 1, 5, 2, 5, 3, 5, 4 };
    // Indices of the edge triplets that make up the initial 8 faces
    static readonly int[] edgeTriplets = { 0, 1, 4, 1, 2, 5, 2, 3, 6, 3, 0, 7, 8, 9, 4, 9, 10, 5, 10, 11, 6, 11, 8, 7 };
    // The six initial vertices
    static readonly Vector3[] baseVertices = { Vector3.up, Vector3.left, Vector3.back, Vector3.right, Vector3.forward, Vector3.down };

    //other data
    int divisions;
    int vertsPerFace;
    List<Vector3> unitSpherePoints = new List<Vector3>();
    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();

    public PlanetFullSphere(PlanetShapeGenerator shapeGenerator, PlanetColorGenerator colorGenerator, Mesh mesh, int resolution)
    {
        this.shapeGenerator = shapeGenerator;
        this.colorGenerator = colorGenerator;
        this.mesh = mesh;
        this.resolution = resolution; 
    }

    public void GenerateMesh()
    {
        divisions = Mathf.Max(0, resolution);
        vertsPerFace = ((divisions + 3)) * ((divisions + 3)) - ((divisions + 3)) / 2;
        //int vertices = vertsPerFace * 8 - (divisions + 2) * 12 + 6;
        //int trisPerFace = (divisions + 1) * (divisions + 1);
        

        unitSpherePoints.Clear();
        verts.Clear();
        tris.Clear();

        unitSpherePoints.AddRange(baseVertices);

        //create 12 edges out of the base vertices
        Edge[] edges = new Edge[12];
        for (int i = 0; i < vertexPairs.Length; i += 2)
        {
            Vector3 startVert = unitSpherePoints[vertexPairs[i]];
            Vector3 endVert = unitSpherePoints[vertexPairs[i + 1]];

            int[] edgeVertIndices = new int[divisions + 2];
            edgeVertIndices[0] = vertexPairs[i];

            for (int j = 0; j < divisions; j++)
            {
                float t = (j + 1f) / (divisions + 1f);
                edgeVertIndices[j + 1] = unitSpherePoints.Count;
                unitSpherePoints.Add(Vector3.Slerp(startVert, endVert, t));
            }
            edgeVertIndices[divisions + 1] = vertexPairs[i + 1];
            edges[i / 2] = new Edge(edgeVertIndices);
        }

        //create faces
        for (int i = 0; i < edgeTriplets.Length; i += 3)
        {
            int index = i / 3;
            bool reverse = index >= 4;
            CreateFace(edges[edgeTriplets[i]], edges[edgeTriplets[i + 1]], edges[edgeTriplets[i + 2]], reverse);
        }

        //apply the planet shape and color noise
        List<Vector2> uv = new List<Vector2>();
        List<Vector2> uv2 = new List<Vector2>();
        for (int i = 0; i < unitSpherePoints.Count; i++)
        {
            float unscaled;
            Vector3 point;
            (point, unscaled) = shapeGenerator.CalculatePointOnPlanet(unitSpherePoints[i]);
            verts.Add(point);
            var r = Mathf.Sqrt(unitSpherePoints[i].x * unitSpherePoints[i].x + unitSpherePoints[i].y * unitSpherePoints[i].y + unitSpherePoints[i].z * unitSpherePoints[i].z);
            uv.Add(new Vector2(Mathf.Atan2(unitSpherePoints[i].z, unitSpherePoints[i].x) / Mathf.PI / 2, Mathf.Acos(unitSpherePoints[i].y / r) / Mathf.PI));
            uv2.Add(new Vector2(colorGenerator.CalculateLatitudeNoise(unitSpherePoints[i]), unscaled));
        }

        mesh.Clear();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = uv.ToArray();
        mesh.uv2 = uv2.ToArray();
    }

    void CreateFace(Edge sideA, Edge sideB, Edge bottom, bool reverse)
    {
        int pointsInEdge = sideA.vertIndices.Length;
        List<int> vertMap = new List<int>();
        vertMap.Add(sideA.vertIndices[0]);

        for (int i = 1; i < pointsInEdge - 1; i++)
        {
            vertMap.Add(sideA.vertIndices[i]);

            Vector3 sideAVert = unitSpherePoints[sideA.vertIndices[i]];
            Vector3 sideBVert = unitSpherePoints[sideB.vertIndices[i]];
            int innerpoints = i - 1;
            for (int j = 0; j < innerpoints; j++)
            {
                float t = (j + 1f) / (innerpoints + 1f);
                vertMap.Add(unitSpherePoints.Count);
                unitSpherePoints.Add(Vector3.Slerp(sideAVert, sideBVert, t));
            }

            vertMap.Add(sideB.vertIndices[i]);
        }

        for (int i = 0; i < pointsInEdge; i++)
        {
            vertMap.Add(bottom.vertIndices[i]);
        }

        //generate triangles
        int rows = divisions + 1;
        for (int i = 0; i < rows; i++)
        {
            int topVert = ((i + 1) * (i + 1) - i - 1) / 2;
            int bottomVert = ((i + 2) * (i + 2) - i - 2) / 2;

            int trisInRow = 1 + 2 * i;
            for (int j = 0; j < trisInRow; j++)
            {
                int v0, v1, v2;

                if (j % 2 == 0)
                {
                    v0 = topVert;
                    v1 = bottomVert + 1;
                    v2 = bottomVert;
                    topVert++;
                    bottomVert++;
                }
                else
                {
                    v0 = topVert;
                    v1 = bottomVert;
                    v2 = topVert - 1;
                }

                tris.Add(vertMap[v0]);
                tris.Add(vertMap[reverse ? v2 : v1]);
                tris.Add(vertMap[reverse ? v1 : v2]);
            }
        }
    }

    public class Edge
    {
        public int[] vertIndices;

        public Edge(int[] vertIndices)
        {
            this.vertIndices = vertIndices;
        }
    }

}
