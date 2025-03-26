namespace Ilumisoft.BubblePop
{
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("UI/Effects/Vertical Gradient")]
    public class VerticalGradient : BaseMeshEffect
    {
        [SerializeField]
        Color topColor = Color.white;

        [SerializeField] [Range(0,1)]
        float offset = 1f;

        [SerializeField]
        Color bottomColor = Color.white;

        [SerializeField] bool revers;

        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if (enabled)
            {
                UIVertex vertex = default;

                var t = new float[4] { 0, offset, offset, 0 };

                for (int i = 0; i < vertexHelper.currentVertCount; i++)
                {
                    vertexHelper.PopulateUIVertex(ref vertex, i);
                    vertex.color *= Color.Lerp(bottomColor, topColor, t[i]);
                    vertexHelper.SetUIVertex(vertex, i);
                }
            }
        }
    }
}