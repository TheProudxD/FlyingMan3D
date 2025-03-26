using TMPro;
using UnityEngine;

public class RingHolder : MonoBehaviour
{
    [field: SerializeField] public Renderer LeftRenderer { get; private set; }
    [field: SerializeField] public Renderer LeftTransRenderer { get; private set; }
    [field: SerializeField] public Renderer RightRenderer { get; private set; }
    [field: SerializeField] public Renderer RightTransRenderer { get; private set; }
}