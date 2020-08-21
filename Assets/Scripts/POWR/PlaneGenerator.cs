using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Takes the bottom left and top right vectors of an object and makes a plane of four vectors with it.
public class PlaneGenerator : MonoBehaviour
{
    private enum PlaneAxis {
    
        Horizontal, 
        Vertical
    }
    public static GameObject GeneratePlane(Vector3 blVertex, Vector3 trVertex) {
        Vector3[] newVertices = {blVertex, trVertex};

        Mesh planeMesh = new Mesh {name = "Generated plane"};
        GameObject plane = new GameObject("plane");

        Vector3[] vertices = new Vector3[4];

        switch (DecideAxis(blVertex, trVertex))
        {
            // Vertex data of the new plane.
            case PlaneAxis.Vertical:
                vertices[0] = newVertices[0];
                vertices[1] = new Vector3(newVertices[0].x, newVertices[1].y, newVertices[0].z);
                vertices[2] = new Vector3(newVertices[1].x, newVertices[1].y, newVertices[1].z);
                vertices[3] = new Vector3(newVertices[1].x, newVertices[0].y, newVertices[1].z);
                break;
            case PlaneAxis.Horizontal:
                vertices[0] = newVertices[0];
                vertices[1] = new Vector3(newVertices[0].x, newVertices[0].y, newVertices[1].z);
                vertices[2] = new Vector3(newVertices[1].x, newVertices[0].y, newVertices[1].z);
                vertices[3] = new Vector3(newVertices[1].x, newVertices[0].y, newVertices[0].z);
                break;
        }
        // Mild hackery to get center to be correct
        Vector3 midPoint = (vertices[0] + vertices[2]) / 2;
        plane.transform.position = midPoint;
        vertices[0] -= midPoint;
        vertices[1] -= midPoint;
        vertices[2] -= midPoint;
        vertices[3] -= midPoint;

        planeMesh.vertices = vertices;
        planeMesh.triangles = new [] { 0, 1, 2, 2, 3, 0};
        planeMesh.RecalculateBounds();
        planeMesh.RecalculateNormals();

        MeshFilter mFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
        mFilter.mesh = planeMesh;
        MeshRenderer planeRenderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        // controllerVertices.Clear();
        // if (AssetsToSpawn[assetIndex] != null) SpawnScaledAsset(plane, planeMesh.normals[0]);

        return plane;
    }
    private static PlaneAxis DecideAxis(Vector3 a, Vector3 b) 
    {
        // TODO: Could be made to spawn horizontal planes.
        return PlaneAxis.Vertical;
    }
}
