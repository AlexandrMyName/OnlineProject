using Core.Models;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace TadWhat.ACraft.ChunckEditor
{

    public class ChunckEditorPauseView : MonoBehaviour
    {

        [SerializeField] private Button _backButton;
        [SerializeField] private Button _saveAllButton;
        [SerializeField] private Button _mainMenuButton;


        [SerializeField] private FreeFlyCamera _flyCamera; 


        public void Init(WorldChunckObjects worldChunckObjects)
        {

            
            _backButton.onClick.AddListener(() =>
            {
                _flyCamera.IsActive = true;
                this.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            });

            _mainMenuButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(0);
            });


            _saveAllButton.onClick.AddListener(() =>
            {
                int chunckCount = 0;

                foreach(var chunkData in worldChunckObjects.ChunckData)
                {
                    chunkData.Value.Render.SaveMeshToFile();
                    chunckCount++;
                }

                Debug.Log($"Сохранение ({chunckCount}) файлов");
            });
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveAllListeners();
            _saveAllButton.onClick.RemoveAllListeners();
            _mainMenuButton.onClick.RemoveAllListeners();
        }


        public void UpdatePauseView()
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.gameObject.SetActive(true);
                _flyCamera.IsActive = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}