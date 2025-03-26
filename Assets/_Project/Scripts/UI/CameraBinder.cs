using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CameraBinder : MonoBehaviour
    {
        [Inject] private Camera _renderCamera;

        private void Awake() => GetComponent<Canvas>().worldCamera = _renderCamera;
    }
}