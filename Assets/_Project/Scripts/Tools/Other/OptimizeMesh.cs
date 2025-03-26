using UnityEngine;

namespace _Project.Scripts.Tools.Other
{
    public class OptimizeMesh : MonoBehaviour
    {
        private void Awake()
        {
            var materialPropertyBlock = new MaterialPropertyBlock();
            var meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}