using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Tools.Other
{
    public abstract class Singleton<T> : MonoBehaviour
        where T : Component
    {
        public bool AutoUnparentOnAwake = true;
        public static bool HasInstance => _instance != null;
        public float InitializationTime { get; private set; }

        public static T TryGetInstance() => HasInstance ? _instance : null;

        private static bool _applicationIsQuitting;

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null || _applicationIsQuitting)
                    return _instance;

                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    _instance = new GameObject("_" + typeof(T).Name).AddComponent<T>();
                    _instance.hideFlags = HideFlags.HideAndDontSave;
                }

                DontDestroyOnLoad(_instance);

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (!Application.isPlaying) return;

            InitializationTime = Time.time;

            if (AutoUnparentOnAwake)
                transform.SetParent(null);

            /*foreach (T old in FindObjectsByType<T>(FindObjectsSortMode.None).Where(old => old.GetComponent<Singleton<T>>().InitializationTime < InitializationTime))
            {
                Destroy(old.gameObject);
            }*/

            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (_instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _instance = null;
            Destroy(gameObject);
            _applicationIsQuitting = true;
        }
    }
}