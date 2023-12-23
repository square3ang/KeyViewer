using UnityEngine;
using UnityEngine.UI;
using KeyViewer.Utils;
using KeyViewer.Models;

namespace KeyViewer.Unity.UI
{
    public class UITextGradient : BaseMeshEffect
    {
        public Color a = Color.white;
        public Color b = Color.white;
        [Range(-180f, 180f)]
        public float angle = 0f;
        public override void ModifyMesh(VertexHelper vh)
        {
            if (enabled)
            {
                //Rect rect = graphic.rectTransform.rect;
                Vector2 dir = GradientUtils.RotationDir(angle);
                Matrix2x3 localPositionMatrix = GradientUtils.LocalPositionMatrix(new Rect(0f, 0f, 1f, 1f), dir);
                UIVertex vertex = default;
                for (int i = 0; i < vh.currentVertCount; i++)
                {
                    vh.PopulateUIVertex(ref vertex, i);
                    Vector2 position = GradientUtils.VerticePositions[i % 4];
                    Vector2 localPosition = localPositionMatrix * position;
                    vertex.color *= Color.Lerp(b, a, localPosition.y);
                    vh.SetUIVertex(vertex, i);
                }
            }
        }
    }
}
