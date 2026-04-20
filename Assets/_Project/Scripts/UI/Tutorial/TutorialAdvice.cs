using _Project.Scripts.Infrastructure.Services;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI.Windows
{
    public class TutorialAdvice : MonoBehaviour
    {
        [Inject] private InputReader _inputReader;

        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _adviceText;
        
        private void Update()
        {
            if (_inputReader == null)
                return;

            if (_inputReader.GetMouseButtonDown(0))
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
