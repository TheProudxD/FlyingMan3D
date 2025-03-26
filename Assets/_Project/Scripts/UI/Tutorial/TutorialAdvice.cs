using System;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI.Windows
{
    public class TutorialAdvice : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _adviceText;
        
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                gameObject.SetActive(false);
            }
        }

        public void SetInfo(string title, string advice)
        {
            _titleText.text = title;
            _adviceText.text = advice;
        }
    }
}