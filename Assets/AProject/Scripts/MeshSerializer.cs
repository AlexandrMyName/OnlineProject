using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
 
#endif


public class MeshSerializer : MonoBehaviour
{

    [SerializeField]
    GameObject objectReference;

    [SerializeField]
    string name;


    [ContextMenu("SaveToAsset")]
    public void SaveAsset()
    {
        if (objectReference != null && name != string.Empty)
        {
#if UNITY_EDITOR
            Mesh m1 = objectReference.GetComponent<MeshFilter>().mesh;
            AssetDatabase.CreateAsset(m1, "Assets/" + name + ".asset");
            AssetDatabase.SaveAssets();
#endif
        }
    }
}
