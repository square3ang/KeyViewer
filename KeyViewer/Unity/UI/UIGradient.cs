using UnityEngine;
using UnityEngine.UI;
using KeyViewer.Utils;
using KeyViewer.Models;

namespace KeyViewer.Unity.UI
{
    public class UIGradient : BaseMeshEffect
    {
        public Color a = Color.white;
        public Color b = Color.white;
        [Range(-180f, 180f)]
        public float angle = 0f;
        public bool ignoreRatio = true;
        public override void ModifyMesh(VertexHelper vh)
        {
            if (enabled)
            {
                Rect rect = graphic.rectTransform.rect;
                Vector2 dir = GradientUtils.RotationDir(angle);
                if (!ignoreRatio)
                    dir = GradientUtils.CompensateAspectRatio(rect, dir);
                Matrix2x3 localPositionMatrix = GradientUtils.LocalPositionMatrix(rect, dir);
                UIVertex vertex = default;
                for (int i = 0; i < vh.currentVertCount; i++)
                {
                    vh.PopulateUIVertex(ref vertex, i);
                    Vector2 localPosition = localPositionMatrix * vertex.position;
                    vertex.color *= Color.Lerp(b, a, localPosition.y);
                    vh.SetUIVertex(vertex, i);
                }
            }
        }
    }
}
