using System.Collections;
using System.Collections.Generic;
using TadWhat.ACraft.ChunckEditor;
using TadWhat.ACraft.Constructor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TadWhat.Auth
{

    public class AdminView : MonoBehaviour
    {

        [SerializeField] private Button _createNew, _myMeshes, _back;
        
        [SerializeField] private FreeFlyCamera _flyCam;

        [SerializeField] private TMP_InputField _widthChunck, _heightChunck, _fileName;
 
        [SerializeField] private EditChunckAndCreation _chunckEditor;

        [SerializeField] private TMP_Text _textInformation, _textInformation2;

        [SerializeField] private MyMeshesView _meshesView;


        public static string NewChunckFileName;
        public static int Width;
        public static int Height;

        private int _width;
        private int _height;


        public void Init()
        {

            _textInformation.text = $"��� �������� ������ ������ -" +
                $" ��������� ����������� ������� ��������� ��� � " +
                $"��������� � ���� � ����������� �������������� <color=green> ������� ������ � ������ �����  </color> \n " +
                $"������������� �������� - 128 * 30, ���������������� ��� <color=red> ������ </color> ";
            _textInformation2.text = $"��������� ���� - <color=red> ������ ��.���� </color>  \n" +
                                     $"������� ���� - <color=red> ����� ��.���� </color>  \n" +
                                     $"����������� ���� - <color=red> ������ P (����) </color>  \n" +
                                     $"��������� ������ - <color=red> LEFT ALT </color>  \n";

            _createNew.interactable = false;

            _fileName.onValueChanged.AddListener(value =>
            {
                AdminView.NewChunckFileName = value + ".xml";
            });

            _myMeshes.onClick.AddListener(() =>
            {

                this.gameObject.SetActive(false);

                _meshesView.gameObject.SetActive(true);
                _meshesView.InitView(_flyCam, _chunckEditor);
            });

           
            _widthChunck.onValueChanged.AddListener(width =>
            {

                if(int.TryParse(width, out var parseVal))
                {
                    if (parseVal != 0)
                    {
                        _width = parseVal;
                        AdminView.Width = parseVal;
                       
                    }
                }
                 
            });

            _heightChunck.onValueChanged.AddListener(height =>
            {
                if (int.TryParse(height, out var parseVal))
                {
                    if (parseVal != 0)
                    {
                          _height = parseVal;
                           AdminView.Height = parseVal;
                    }
                }

            });

            _createNew.onClick.AddListener(() =>
            {
                if (Mathf.Abs(_width) == 0 || Mathf.Abs(_height) == 0) return;

                _textInformation.text = $" ��������... ���������� ���������";
                _textInformation2.text = $" ��������... ���������� ���������";
                _back.gameObject.SetActive(false);
                _createNew.gameObject.SetActive(false);
                _widthChunck.gameObject.SetActive(false);
                _heightChunck.gameObject.SetActive(false);
                 
                StartCoroutine(CreateNewChunckEditor());
            });
        }


        private IEnumerator CreateNewChunckEditor()
        {

            yield return new WaitForSeconds(2);
            _chunckEditor.CreateNewChunck(_width, _height,_fileName.text);
            _flyCam.enabled = true;
            this.gameObject.SetActive(false);
        }

        private void Update()
        {

            if(_width > 0 && _height > 0)
            {
                _createNew.interactable = true;
               
            }
        }
    }


    public class ChunckRequest
    {
  
        public MeshView MeshView { get; set; }
        public string FileName { get; set; }

        public Vector3  WorldPosition { get; set; }
    }


    public class LoadChuncksRequest
    {

       
        public List<ChunckRequest> Chuncks { get; set; }
    }
     
}