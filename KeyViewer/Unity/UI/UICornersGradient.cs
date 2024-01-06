using KeyViewer.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace KeyViewer.Unity.UI
{
    public class UICornersGradient : BaseMeshEffect
    {
        public Color topLeft = Color.white;
        public Color topRight = Color.white;
        public Color bottomRight = Color.white;
        public Color bottomLeft = Color.white;
        public override void ModifyMesh(VertexHelper vh)
        {
            if (enabled)
            {
                Rect rect = graphic.rectTransform.rect;
                Matrix2x3 localPositionMatrix = GradientUtils.LocalPositionMatrix(rect, Vector2.right);
                UIVertex vertex = default;
                for (int i = 0; i < vh.currentVertCount; i++)
                {
                    vh.PopulateUIVertex(ref vertex, i);
                    Vector2 normalizedPosition = localPositionMatrix * vertex.position;
                    vertex.color *= GradientUtils.Bilerp(bottomLeft, bottomRight, topLeft, topRight, normalizedPosition);
                    vh.SetUIVertex(vertex, i);
                }
            }
        }
    }
}
