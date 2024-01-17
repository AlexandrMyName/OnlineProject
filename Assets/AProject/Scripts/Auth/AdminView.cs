using System.Collections;
using TadWhat.ACraft.Constructor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TadWhat.Auth
{

    public class AdminView : MonoBehaviour
    {

        [SerializeField] private Button _editMode,_back;
        
        [SerializeField] private FreeFlyCamera _flyCam;

        [SerializeField] private TMP_InputField _widthChunck, _heightChunck;
 
        [SerializeField] private EditChunckAndCreation _chunckEditor;

        [SerializeField] private TMP_Text _textInformation, _textInformation2;

        

        private int _width;
        private int _height;


        public void Init()
        {

            _textInformation.text = $"Вам доступен эдитор чанков -" +
                $" программа позволяющая создать текстовый меш и " +
                $"сохранить в файл с последующим использованием <color=green> введите высоту и ширину чанка  </color> \n " +
                $"рекомендуемые значения - 128 * 30, перегенирировать меш <color=red> нельзя </color> ";
            _textInformation2.text = $"Поставить блок - <color=red> правая кн.мыши </color>  \n" +
                                     $"Удалить блок - <color=red> левая кн.мыши </color>  \n" +
                                     $"Переключить блок - <color=red> кнопка P (окно) </color>  \n" +
                                     $"Сохранить проект - <color=red> LEFT ALT </color>  \n";

            _editMode.interactable = false;

            _widthChunck.onValueChanged.AddListener(width =>
            {

                if(int.TryParse(width, out var parseVal))
                {
                    if (parseVal != 0)
                    {
                        {_width = parseVal;}
                    }
                }
                 
            });

            _heightChunck.onValueChanged.AddListener(height =>
            {
                if (int.TryParse(height, out var parseVal))
                {
                    if (parseVal != 0)
                    {
                        { _height = parseVal; }
                    }
                }

            });

            _editMode.onClick.AddListener(() =>
            {
                if (Mathf.Abs(_width) == 0 || Mathf.Abs(_height) == 0) return;

                _textInformation.text = $" загрузка... Пожалуйста подождите";
                _textInformation2.text = $" загрузка... Пожалуйста подождите";
                _back.gameObject.SetActive(false);
                _editMode.gameObject.SetActive(false);
                _widthChunck.gameObject.SetActive(false);
                _heightChunck.gameObject.SetActive(false);
                 
                StartCoroutine(LoadEditor());
            });
        }


        private IEnumerator LoadEditor()
        {

            yield return new WaitForSeconds(2);
            _chunckEditor.InitEditor(_width, _height);
            _flyCam.enabled = true;
            this.gameObject.SetActive(false);
        }

        private void Update()
        {

            if(_width > 0 && _height > 0)
            {
                _editMode.interactable = true;
               
            }
        }
    }
}