using System.Collections;
using UnityEngine;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.UI;
using Reflex.Attributes;

namespace _Project.Scripts.Gameplay
{
    /// <summary>
    /// Handles player launch sequence, animation, and physics application.
    /// Manages the catapult launch mechanics and UI transitions.
    /// </summary>
    public class PlayerLaunchController : MonoBehaviour
    {
        [Inject] private UIFactory _uiFactory;
        [Inject] private GameFactory _gameFactory;
        [Inject] private AudioService _audioService;

        [SerializeField] private float _launchAnimationDuration = 0.8f;
        [SerializeField] private float _retractAnimationDuration = 0.1f;

        private Rigidbody[] _bodies;
        private Transform _capsule;
        private Vector3 _initialPos;
        private float _maxLaunchSpeed;

        public void Initialize(
            Rigidbody[] bodies,
            Transform capsule,
            Vector3 initialPos)
        {
            _bodies = bodies;
            _capsule = capsule;
            _initialPos = initialPos;
        }

        public void UpdateMaxLaunchSpeed(float speed) => _maxLaunchSpeed = speed;

        public IEnumerator ApplyLaunchForce(float factor)
        {
            Hud hud = _uiFactory?.GetHUD();
            if (hud != null)
            {
                hud.Show();
                hud.DeactivateStartText();
            }

            if (_audioService != null)
            {
                _audioService.PlayLaunchSound();
            }

            Vector3 targetPos = _initialPos + new Vector3(0f, -1f, -4f) * factor;

            // Launch animation - move capsule back
            float time = 0f;
            while (time <= _launchAnimationDuration)
            {
                if (_capsule == null)
                    yield break;

                _capsule.position = Vector3.Lerp(_initialPos, targetPos, time / _launchAnimationDuration);
                time += Time.deltaTime;
                yield return null;
            }

            // Retract animation - move capsule forward
            time = 0f;
            while (time <= _retractAnimationDuration)
            {
                if (_capsule == null)
                    yield break;

                _capsule.position = Vector3.Lerp(targetPos, _initialPos, time / (_retractAnimationDuration * 2f));
                time += Time.deltaTime;
                yield return null;
            }

            // Apply launch force to all bodies
            Vector3 forceVector = new Vector3(0, factor, factor * 2f) * _maxLaunchSpeed;

            foreach (Rigidbody rb in _bodies)
            {
                if (rb == null)
                    continue;

                rb.linearVelocity = forceVector;
                rb.AddTorque(Vector3.forward);
            }

            // Spawn objects if launch force is significant
            Spawner spawner = _gameFactory?.GetSpawner();
            if (factor > 0.1f && spawner != null && !spawner.HasSpawnedLevelObjects)
            {
                spawner.SpawnObjects(_bodies[0]?.linearVelocity ?? Vector3.zero);
            }
        }
    }
}
