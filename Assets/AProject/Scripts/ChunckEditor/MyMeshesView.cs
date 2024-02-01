using Cryptograph;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TadWhat.ACraft.Constructor;
using TadWhat.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TadWhat.ACraft.ChunckEditor
{

    public class MyMeshesView : MonoBehaviour
    {

        private List<MeshView> _meshes = new();

        [SerializeField] private GameObject _meshViewFab;

        [SerializeField] private Transform _content;

        [SerializeField] private Button _load, _back, _unblockRemoving;

        [SerializeField] private TMP_InputField _newNameFile;

        [SerializeField] private TMP_Text _errorText;

        [SerializeField] private AdminView _adminView;

        [SerializeField] private List<GameObject> _hidenOnLoad;
        [SerializeField] private List<GameObject> _shownOnLoad;


        public void InitView(FreeFlyCamera flyCam, EditChunckAndCreation chunckEditor)
        {

            _back.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
                _adminView.gameObject.SetActive(true);
            });

            var objectsInContent = _content.GetComponentsInChildren<MeshView>();
            foreach(var obj in objectsInContent)
            {
                obj.gameObject.SetActive(false);
            }
             
            DirectoryInfo mainDir = new DirectoryInfo(FileMetaData.Path);


            DirectoryInfo[] infoFolders = mainDir.GetDirectories();

            Debug.Log(infoFolders.Length);


            if (infoFolders.Length == 0) _errorText.text = $" <color=red> похоже созданных файлов нет</color>";
            else
            {
                _errorText.text = string.Empty;
            }

            int counterXposition = 1;
            int counterYposition = 1;

            foreach (var infoFolder in infoFolders)
            {

                Debug.LogWarning($"Найдена папка <color=green> {infoFolder.Name}</color>");


                DirectoryInfo dir = new DirectoryInfo(Path.Combine(FileMetaData.Path, infoFolder.Name));

                FileInfo[] info = dir.GetFiles("*.*");

                 
                foreach (FileInfo f in info)
                {

                    if (f.Name[f.Name.Length - 1] == 'a') continue;
                    if (f.Name[f.Name.Length - 1] == 'n') continue;

                    Debug.Log($"Найден объект: <color=green>{f.Name}</color>");

                    var meshViewInstance = GameObject.Instantiate(_meshViewFab, _content);
                    var meshView = meshViewInstance.GetComponent<MeshView>();
                    meshView.FullPathToFile = f.FullName;
                    _meshes.Add(meshView);

                   
                    meshView.Init(f.Name, infoFolder.Name, counterXposition, counterYposition);

                    
                    
                }

                counterYposition++;
                if (counterYposition > 3)
                {
                    counterXposition++;
                    counterYposition = 1;
                }

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
                                FolderName = mesh.FolderName,
                                WorldPosition = mesh.WorldPosition,
                                MeshView = mesh
                            });

                        }

                    });


                    if (request.Chuncks.Count > 0)
                    {
                        _hidenOnLoad.ForEach(obj => obj.SetActive(false));
                        _shownOnLoad.ForEach(obj => obj.SetActive(true));

                        StartCoroutine(LoadChuncks(flyCam, chunckEditor, request));
                    }
                });


                _newNameFile.onValueChanged.AddListener(value =>
                {

                    //FileMetaData.NewChunckFileXmlName = value + ".xml";
                   // FileMetaData.NewChunckFileJsonName = value + ".json";

                    if (AdminView.Height == 0)
                        AdminView.Width = 128;
                    if (AdminView.Width == 0)
                        AdminView.Height = 30;
                });

                _unblockRemoving.onClick.AddListener(() =>
                {
                    foreach (var view in _meshes)
                    {
                        view.UnblockRemove();
                    }
                });

            }
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
            _newNameFile.onValueChanged.RemoveAllListeners();
            _unblockRemoving.onClick.RemoveAllListeners();
        }
    }
}