using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class MeshView : MonoBehaviour
{
     
    [SerializeField] private Toggle _toggleAddToCollection;
    [SerializeField] private TMP_Text _fileName;

    [SerializeField] private TMP_InputField _xWorld;
    [SerializeField] private TMP_InputField _yWorld;
    [SerializeField] private TMP_InputField _zWorld;

    [SerializeField] private Button _removeMesh;

    public bool CanAddToCollection { get; private set; }

    [HideInInspector] public string FullPathToFile;

    public string Name;

    public Vector3 WorldPosition;
    

    public void Init(string fileName)
    {

        Name = fileName;
        WorldPosition = Vector3.zero;
        _fileName.text = $"Τΰιλ: <color=green>{fileName}</color>";
        _toggleAddToCollection.onValueChanged.AddListener(value =>
        {
            Debug.Log(value);
            CanAddToCollection = value;
        });

        _removeMesh.onClick.AddListener(() =>
        {
            
                File.Delete(FullPathToFile);
                CanAddToCollection = false;
                this.gameObject.SetActive(false);
              
        });

        _xWorld.onValueChanged.AddListener(value =>
        {

            if(float.TryParse(value, out float result))
            {
                WorldPosition.x = result;
            }
            else
            {
                _xWorld.text = string.Empty;
            } 
        });

        _yWorld.onValueChanged.AddListener(value =>
        {

            if (float.TryParse(value, out float result))
            {
                WorldPosition.y = result;
            }
            else
            {
                _yWorld.text = string.Empty;
            }
        });

        _zWorld.onValueChanged.AddListener(value =>
        {

            if (float.TryParse(value, out float result))
            {
                WorldPosition.z = result;
            }
            else
            {
                _zWorld.text = string.Empty;
            }
        });

    }

    private void OnDestroy()
    {

        _toggleAddToCollection.onValueChanged.RemoveAllListeners();
        _xWorld.onValueChanged.RemoveAllListeners();
        _zWorld.onValueChanged.RemoveAllListeners();
        _yWorld.onValueChanged.RemoveAllListeners();
        _removeMesh.onClick.RemoveAllListeners();
    }
}
