
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Serialization;
public class SaveLoadData: MonoBehaviour
{
    [Serializable]
    public struct Vertex
    {
        public float x, y, z;
        public Vertex(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    [Serializable]
    public struct Rotation
    {
        public float x, y, z, w;
        public Rotation(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
    [Serializable]
    private class PlaneData
    {

        public Vertex trVertex;
        public Vertex blVertex;
        public PlaneType planeType;
        public PlaneData(Vector3 blVertex, Vector3 trVertex, PlaneType planeType)
        {
            this.blVertex = new Vertex(blVertex.x, blVertex.y, blVertex.z);
            this.trVertex = new Vertex(trVertex.x, trVertex.y, trVertex.z);
            this.planeType = planeType;
        }
    }

    [Serializable]
    private class SerializedGameObject
    {
        public Vertex position;
        public Rotation rotation;
        public SerializedGameObject(Vertex position, Rotation rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    public void SavePlanesToFile(ref List<GameObject> generatedPlanes, Transform vrAnchor)
    {
        // if (File.Exists(Application.persistentDataPath + "/vertexdata.dat"))
        // {
        //     Debug.LogError("File exists");
        //     return;
        // }

        List<PlaneData> serializedPlaneVectors = new List<PlaneData>();
        List<SerializedGameObject> serializedGameObjects = new List<SerializedGameObject>();

        Vertex anchorPos = new Vertex(vrAnchor.position.x, vrAnchor.position.y, vrAnchor.position.z);
        Rotation anchorRot = new Rotation(vrAnchor.rotation.x, vrAnchor.rotation.y, vrAnchor.rotation.z, vrAnchor.rotation.w);
        SerializedGameObject serializedVrAnchor = new SerializedGameObject(anchorPos, anchorRot);
        serializedGameObjects.Add(serializedVrAnchor);

        foreach (var plane in generatedPlanes)
        {
            // Save the transform data of the plane's game object
            Vertex pos = new Vertex(plane.transform.localPosition.x, plane.transform.localPosition.y, plane.transform.localPosition.z);
            Rotation rot = new Rotation(plane.transform.localRotation.x, plane.transform.localRotation.y, plane.transform.localRotation.z, plane.transform.localRotation.w);
            SerializedGameObject serializedGameObject = new SerializedGameObject(pos, rot);
            serializedGameObjects.Add(serializedGameObject);
            // Save the procedural mesh data to be reconstructed later
            var vertices = plane.GetComponent<MeshFilter>().mesh.vertices;
            PlaneType planeType = plane.GetComponent<CalibrationType>().planeType;
            serializedPlaneVectors.Add(new PlaneData(plane.transform.TransformPoint(vertices[0]), plane.transform.TransformPoint(vertices[2]), planeType));
        }
        BinaryFormatter bf = new BinaryFormatter ();
        FileStream file = File.Create (Application.persistentDataPath + "/vertexdata.dat");
        bf.Serialize (file, serializedPlaneVectors);
        file.Close ();
        FileStream file2 = File.Create (Application.persistentDataPath + "/gameobjectdata.dat");
        bf.Serialize (file2, serializedGameObjects);
        file2.Close ();
    }

    public void LoadPlanesFromFile( ref List<GameObject> generatedPlanes, ref GameObject vrAnchor)
    {
        BinaryFormatter bf = new BinaryFormatter ();
        if (File.Exists(Application.persistentDataPath + "/gameobjectdata.dat"))
        {
            FileStream gofile = File.Open (Application.persistentDataPath + "/gameobjectdata.dat", System.IO.FileMode.Open);
            List<SerializedGameObject> gameObjectData = (List<SerializedGameObject>)bf.Deserialize(gofile);
            SerializedGameObject serializedAnchor = gameObjectData[0];
            vrAnchor.transform.position = new Vector3(serializedAnchor.position.x, serializedAnchor.position.y, serializedAnchor.position.z);
            vrAnchor.transform.rotation = new Quaternion(serializedAnchor.rotation.x, serializedAnchor.rotation.y, serializedAnchor.rotation.z, serializedAnchor.rotation.w);
            gameObjectData.RemoveAt(0);

            if (File.Exists(Application.persistentDataPath + "/vertexdata.dat"))
            {
                FileStream file = File.Open(Application.persistentDataPath + "/vertexdata.dat",
                System.IO.FileMode.Open);
                List<PlaneData> data = (List<PlaneData>) bf.Deserialize(file);
                int goIndex = 0;
                foreach (var plane in data)
                {
                    Vector3 v1 = new Vector3(plane.blVertex.x, plane.blVertex.y, plane.blVertex.z);
                    Vector3 v2 = new Vector3(plane.trVertex.x, plane.trVertex.y, plane.trVertex.z);
                    GameObject deserializedPlane = PlaneGenerator.GeneratePlane(v1, v2);

                    deserializedPlane.AddComponent<CalibrationType>().planeType = plane.planeType;
                    deserializedPlane.transform.parent = vrAnchor.transform;
                    deserializedPlane.transform.localPosition = new Vector3(gameObjectData[goIndex].position.x, gameObjectData[goIndex].position.y, gameObjectData[goIndex].position.z);
                    deserializedPlane.transform.localRotation = new Quaternion(gameObjectData[goIndex].rotation.x, gameObjectData[goIndex].rotation.y, gameObjectData[goIndex].rotation.z, gameObjectData[goIndex].rotation.w);
                    generatedPlanes.Add(deserializedPlane);
                    goIndex++;
                }

                file.Close();
            }
            gofile.Close ();
        }
    }
}
