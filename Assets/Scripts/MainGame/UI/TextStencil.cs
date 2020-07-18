     using UnityEngine;
     using UnityEngine.UI;
     
     // Allows for the assist mode text to appear above all objects.
     [ExecuteInEditMode]
     public class TextStencil : MonoBehaviour {
     
         public UnityEngine.Rendering.CompareFunction comparison = UnityEngine.Rendering.CompareFunction.Always;
     
         // public bool apply = false;
     
         void Start()
         {
             // if (apply)
             // {
             //     apply = false;
                 Graphic image = GetComponent<Graphic>();
                 Material existingGlobalMat = image.materialForRendering;
                 Material updatedMaterial = new Material(existingGlobalMat);
                 updatedMaterial.SetInt("unity_GUIZTestMode", (int)comparison);
                 image.material = updatedMaterial;
             // }
         }
     }