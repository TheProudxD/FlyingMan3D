using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Tools.Coroutine
{
    public sealed class Coroutines : MonoBehaviour, ICoroutineRunner
    {
        private static Coroutines _instance;

        private static Coroutines Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("[COROUTINES MANAGER]");
                    _instance = go.AddComponent<Coroutines>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        public static UnityEngine.Coroutine StartRoutine(IEnumerator enumerator) => Instance.StartCoroutine(enumerator);

        public static bool StopRoutine(UnityEngine.Coroutine coroutine)
        {
            if (coroutine == null)
                return false;

            Instance.StopCoroutine(coroutine);
            return true;
        }
    }
}