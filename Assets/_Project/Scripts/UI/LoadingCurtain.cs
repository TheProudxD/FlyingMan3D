using System.Collections;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class LoadingCurtain : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _curtain;

        private readonly float _speed = 2.35f;

        public bool IsLoading => _curtain.alpha != 0;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Show()
        {
            // gameObject.SetActive(true);
            _curtain.alpha = 1;
        }

        public void Hide()
        {
            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            yield return null;

            while (_curtain.alpha > 0)
            {
                _curtain.alpha -= Time.deltaTime * _speed;
                yield return null;
            }

            _curtain.alpha = 0;
            // gameObject.SetActive(false);
        }
    }
}