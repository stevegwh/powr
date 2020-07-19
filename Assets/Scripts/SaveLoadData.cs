
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
        private class PlaneData
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

        public void SavePlanesToFile(ref List<GameObject> generatedPlanes)
        {
            // if (File.Exists(Application.persistentDataPath + "/vertexdata.dat"))
            // {
            //     Debug.LogError("File exists");
            //     return;
            // }
            List<PlaneData> toSerialize = new List<PlaneData>();
            foreach (var plane in generatedPlanes)
            {
                var vertices = plane.GetComponent<MeshFilter>().mesh.vertices;
                PlaneType planeType = plane.GetComponent<CalibrationType>().planeType;
                toSerialize.Add(new PlaneData(plane.transform.TransformPoint(vertices[0]), plane.transform.TransformPoint(vertices[2]), planeType));
            }
            BinaryFormatter bf = new BinaryFormatter ();
            FileStream file = File.Create (Application.persistentDataPath + "/vertexdata.dat");
            bf.Serialize (file, toSerialize);
            file.Close ();
        }

         public void LoadPlanesFromFile( ref List<GameObject> generatedPlanes)
         {
             if(File.Exists(Application.persistentDataPath + "/vertexdata.dat"))
             {
                 BinaryFormatter bf = new BinaryFormatter ();
                 FileStream file = File.Open (Application.persistentDataPath + "/vertexdata.dat", System.IO.FileMode.Open);
                 List<PlaneData> data = (List<PlaneData>)bf.Deserialize(file);
                 foreach (var plane in data)
                 {
                    Vector3 v1 = new Vector3(plane.blVertex.x, plane.blVertex.y, plane.blVertex.z);
                    Vector3 v2 = new Vector3(plane.trVertex.x, plane.trVertex.y, plane.trVertex.z);
                    GameObject deserializedPlane = PlaneGenerator.GeneratePlane(v1, v2);
                    deserializedPlane.AddComponent<CalibrationType>().planeType = plane.planeType;
                    generatedPlanes.Add(deserializedPlane);
                 }
                 file.Close ();
             }
         }
}
