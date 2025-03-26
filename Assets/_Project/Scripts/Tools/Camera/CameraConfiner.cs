using UnityEngine;

namespace _Project.Scripts.Tools.Camera
{
    public class CameraConfiner : MonoBehaviour
    {
        [SerializeField] private Collider2D _confiner;

        private void Awake()
        {
            //FindObjectOfType<Cinemachine.CinemachineConfiner2D>().m_BoundingShape2D = _confiner;
        }
    }
}