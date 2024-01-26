using System.Collections;
using System.Collections.Generic;
using System.IO;
using TadWhat.ACraft.Constructor;
using TadWhat.Auth;
using UnityEngine;
using UnityEngine.UI;

namespace TadWhat.ACraft.ChunckEditor
{

    public class MyMeshesView : MonoBehaviour
    {

        private List<MeshView> _meshes = new();

        [SerializeField] private GameObject _meshViewFab;

        [SerializeField] private Transform _content;

        [SerializeField] private Button _load, _back;



       
        public void InitView(FreeFlyCamera flyCam, EditChunckAndCreation chunckEditor)
        {

            string path = Application.dataPath.Replace("/Assets", "/Meshes");
          

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] info = dir.GetFiles("*.*");

            foreach (FileInfo f in info)
            {

                if (f.Name[f.Name.Length-1] ==  'a') continue;

                Debug.Log($"Найден объект: <color=green>{f.Name}</color>");

                var meshViewInstance = GameObject.Instantiate(_meshViewFab, _content);
                var meshView = meshViewInstance.GetComponent<MeshView>();
                meshView.FullPathToFile = f.FullName;
                _meshes.Add(meshView);
                meshView.Init(f.Name);
            }

            _back.onClick.AddListener(() => this.gameObject.SetActive(false));

            _load.onClick.AddListener(() =>
            {

                
                LoadChuncksRequest request = new();

                request.Chuncks = new();

                _meshes.ForEach(mesh =>
                {

                    if (mesh.CanAddToCollection)
                    {
                        
                        request.Chuncks.Add(new ChunckRequest()
                        {
                            FileName = mesh.Name,
                            WorldPosition = mesh.WorldPosition,
                            MeshView = mesh
                        });
                       
                    }
                    
                });

                
                if(request.Chuncks.Count > 0)
                {
                   
                   StartCoroutine(LoadChuncks(flyCam, chunckEditor,request));
                }
            });
        }


        private IEnumerator LoadChuncks(FreeFlyCamera flyCam, EditChunckAndCreation chunckEditor,LoadChuncksRequest request)
        {

            yield return new WaitForSeconds(2);
            chunckEditor.LoadChuncks(request);
            flyCam.enabled = true;
            this.gameObject.SetActive(false);
        }



        private void OnDestroy()
        {

            _back.onClick.RemoveAllListeners();
            _load.onClick.RemoveAllListeners();
        }
    }
}