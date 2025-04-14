using UnityEngine;

public class RingHolder : MonoBehaviour
{
    [field: SerializeField] public Renderer Renderers { get; private set; }
    [field: SerializeField] public Renderer TransRenderers { get; private set; }
}