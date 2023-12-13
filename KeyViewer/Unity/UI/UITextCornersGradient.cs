using UnityEngine;
using UnityEngine.UI;
using KeyViewer.Utils;
using KeyViewer.Types;

namespace KeyViewer.Unity
{
    public class UITextCornersGradient : BaseMeshEffect
    {
        public Color topLeft = Color.white;
        public Color topRight = Color.white;
        public Color bottomRight = Color.white;
        public Color bottomLeft = Color.white;
        public override void ModifyMesh(VertexHelper vh)
        {
            if (enabled)
            {
                //Rect rect = graphic.rectTransform.rect;
                UIVertex vertex = default;
                for (int i = 0; i < vh.currentVertCount; i++)
                {
                    vh.PopulateUIVertex(ref vertex, i);
                    Vector2 normalizedPosition = GradientUtils.VerticePositions[i % 4];
                    vertex.color *= GradientUtils.Bilerp(bottomLeft, bottomRight, topLeft, topRight, normalizedPosition);
                    vh.SetUIVertex(vertex, i);
                }
            }
        }
    }
}
