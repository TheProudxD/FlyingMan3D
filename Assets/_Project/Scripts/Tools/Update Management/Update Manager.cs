using System;

namespace _Project.Scripts.Tools.Update_Management
{
    using UnityEngine;
    using System.Collections.Generic;

    public class UpdateManager : MonoBehaviour
    {
        private static UpdateManager _instance;

        private readonly HashSet<IUpdateObserver> _observers = new();
        private readonly HashSet<IUpdateObserver> _pendingAdd = new();
        private readonly HashSet<IUpdateObserver> _pendingRemove = new();

        private static bool _applicationIsQuitting = false;

        [RuntimeInitializeOnLoadMethod]
        private static void ResetState() => _applicationIsQuitting = false;

        public static UpdateManager Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    //Debug.LogWarning("UpdateManager is already destroyed. Application is quitting.");
                    return _instance;
                }
                
                if (_instance != null) return _instance;

                GameObject go = new GameObject("[Update Manager]");
                _instance = go.AddComponent<UpdateManager>();
                DontDestroyOnLoad(go);

                return _instance;
            }
        }

        public void AddObserver(IUpdateObserver observer)
        {
            if (this == null) return;

            if (!_observers.Contains(observer) && !_pendingAdd.Contains(observer))
                _pendingAdd.Add(observer);
        }

        public void RemoveObserver(IUpdateObserver observer)
        {
            if (this == null) return;

            if (_observers.Contains(observer) || _pendingAdd.Contains(observer))
                _pendingRemove.Add(observer);
        }

        private void Update()
        {
            foreach (IUpdateObserver observer in _observers)
                observer.OnUpdate(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            foreach (IUpdateObserver observer in _observers)
                observer.OnFixedUpdate(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            foreach (IUpdateObserver observer in _observers)
                observer.OnLateUpdate(Time.deltaTime);

            ProcessPending();
        }

        private void ProcessPending()
        {
            if (_pendingRemove.Count > 0)
            {
                _observers.ExceptWith(_pendingRemove);
                _pendingRemove.Clear();
            }

            if (_pendingAdd.Count > 0)
            {
                _observers.UnionWith(_pendingAdd);
                _pendingAdd.Clear();
            }
        }
        
        private void OnDestroy()
        {
            _observers.Clear();
            _pendingAdd.Clear();
            _pendingRemove.Clear();
        }
        
        private void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
            Destroy(gameObject);
        }
    }
}