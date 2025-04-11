using System;
using _Project.Scripts.Infrastructure.Observable;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class TargetArrowAnimation : MonoBehaviour
    {
        private float _speed = 220f;
        private readonly ObservableVariable<float> _angle = new();
        private bool _enabled;

        public IReadonlyObservableVariable<float> Angle => _angle;

        public void Enable() => _enabled = true;

        public void Disable() => _enabled = false;

        private void Update()
        {
            if (_enabled == false)
                return;

            _angle.Value += Time.unscaledDeltaTime * _speed;

            if (Mathf.Abs(_angle.Value) >= 90)
            {
                _speed *= -1;
                _angle.Value = MathF.Sign(_angle.Value) * 90;
            }

            transform.rotation = Quaternion.Euler(new Vector3(40, 0, _angle.Value));
        }
    }
}