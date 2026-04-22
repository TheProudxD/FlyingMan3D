using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public abstract class RingBase : MonoBehaviour
    {
        protected PlayerFactory PlayerFactory;
        protected AudioService AudioService;

        protected abstract string Key { get; }
        public int Effect { get; set; }
        public Vector3 MovementAxis { get; set; }
        public float Speed { get; set; }

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private bool _movingToTarget = true;

        public void Construct(PlayerFactory playerFactory, AudioService audioService)
        {
            PlayerFactory = playerFactory;
            AudioService = audioService;
        }

        private void Start()
        {
            _startPosition = transform.position;
            _targetPosition = _startPosition + MovementAxis;

            var text = GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.SetText(Key + Effect);
        }

        private void Update()
        {
            if (MovementAxis == Vector3.zero)
                return;

            Vector3 currentTarget = _movingToTarget ? _targetPosition : _startPosition;

            transform.position = Vector3.MoveTowards(transform.position, currentTarget, Speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
            {
                _movingToTarget = !_movingToTarget;
            }
        }
    }
}
