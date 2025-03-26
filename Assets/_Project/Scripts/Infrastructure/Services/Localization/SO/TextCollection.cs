using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Localization.SO
{
    [Serializable]
    public class TextCollection : ScriptableObject
    {
        [NaughtyAttributes.ReadOnly] [SerializeField] private List<string> _keys;
        [NaughtyAttributes.ReadOnly] [SerializeField] private List<string> _values;

        public string Localize(string key)
        {
            if (_keys == null || _keys.Count == 0) return null;

            if (!_keys.Contains(key))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                UnityEngine.Debug.LogWarning(key + " not found");
#endif
                return null;
            }

            return _values[_keys.IndexOf(key)];
        }

#if UNITY_EDITOR

        public void Add(string key, string value)
        {
            _keys ??= new List<string>();

            if (_keys.Contains(key))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                UnityEngine.Debug.LogWarning(key + " already exists with value " + value);
#endif
                return;
            }

            _keys.Add(key);

            _values ??= new List<string>();

            _values.Add(value);
        }
#endif
    }
}