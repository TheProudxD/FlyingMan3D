using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows
{
    [RequireComponent(typeof(CameraBinder))]
    public abstract class WindowBase : UIContainer
    {
        [Inject] protected WindowService WindowService;
        [Inject] protected AudioService AudioService;

        [SerializeField] private Button _closeButton;

        private void Awake() => OnAwake();

        private void Start()
        {
            Initialize();
            SubscribeUpdates();
        }

        private void OnDestroy() => Cleanup();

        public override void Show()
        {
            AudioService.PlayWindowShowSound();
            gameObject.SetActive(true);
        }

        public override void Hide() => WindowService.Hide(this);

        protected virtual void OnAwake()
        {
            if (_closeButton != null)
                _closeButton.onClick.AddListener(() =>
                {
                    AudioService.PlayClickSound();
                    WindowService.Hide(this);
                });
        }

        protected virtual void Initialize() { }

        protected virtual void SubscribeUpdates() { }

        protected virtual void Cleanup() { }
    }
}