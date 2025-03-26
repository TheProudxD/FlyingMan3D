using System;
using System.Collections;
using _Project.Scripts.Tools.Coroutine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Infrastructure.Services
{
    public class SceneLoader : IService
    {
        public void Load(string sceneName, Action onLoaded = null)
        {
            Coroutines.StartRoutine(LoadScene(sceneName, onLoaded));
        }

        private IEnumerator LoadScene(string sceneName, Action onLoaded = null)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                onLoaded?.Invoke();
                yield break;
            }

            AsyncOperation loadSceneAsync =
                SceneManager.LoadSceneAsync(sceneName, new LoadSceneParameters(LoadSceneMode.Single));

            yield return loadSceneAsync;

            onLoaded?.Invoke();
        }
    }
}