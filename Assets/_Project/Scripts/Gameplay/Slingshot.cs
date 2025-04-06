using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class Slingshot : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody Capsule { get; private set; }
    }
}