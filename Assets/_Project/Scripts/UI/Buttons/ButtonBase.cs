using _Project.Scripts.Infrastructure.Services.Audio;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonBase : MonoBehaviour
    {
        [Inject] protected AudioService AudioService;

        private Button _button;

        private void Awake() => _button = GetComponent<Button>();
        
        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
            _button.onClick.AddListener(AudioService.PlayClickSound);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
            _button.onClick.RemoveListener(AudioService.PlayClickSound);
        }
        
        protected abstract void OnClick(); 

        public void Add(UnityAction action) => _button.onClick.AddListener(action);

        public void Remove(UnityAction action) => _button.onClick.RemoveListener(action);
        
        public void RemoveAll() => _button.onClick.RemoveAllListeners();

        public void MakeInteractive() => _button.interactable = true;

        public void MakeNonInteractive() => _button.interactable = false;

        public void Click() => _button?.onClick?.Invoke();
    }
}