using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TadWhat;
using TadWhat.LoginAccountView;

namespace TadWhat.EnterView
{

    public class EnterInGameView : MonoBehaviour
    {

        [SerializeField] private Button _createAccBut;
        [SerializeField] private Button _logINAccBut;

        [SerializeField] private GameObject _enterView;
        [SerializeField] private GameObject _logInView;
        [SerializeField] private GameObject _creationView;

        

        private void Start()
        {

            _logInView.GetComponent<LoginAccountWindow>().LogIN(); // Auto try

            _createAccBut.onClick.AddListener(() =>
            {
                ChangeView(TypeOfButton.Creation);
            });

            _logINAccBut.onClick.AddListener(() =>
            {
                ChangeView(TypeOfButton.LogIN);
            });
        }


        private void ChangeView(TypeOfButton buttonType = TypeOfButton.LogIN)
        {

            if (buttonType == TypeOfButton.LogIN)
            {
                _enterView.SetActive(false);
                _logInView.SetActive(true);
                _creationView.SetActive(false);
            }
            else if (buttonType == TypeOfButton.Creation)
            {
                _enterView.SetActive(false);
                _logInView.SetActive(false);
                _creationView.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            _createAccBut.onClick.RemoveAllListeners();
            _logINAccBut.onClick.RemoveAllListeners();

        }
    }
}