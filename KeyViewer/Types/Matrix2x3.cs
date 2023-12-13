using UnityEngine;

namespace KeyViewer.Types
{
    public struct Matrix2x3
    {
        public float m00, m01, m02, m10, m11, m12;
        public Matrix2x3(float m00, float m01, float m02, float m10, float m11, float m12)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;
            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;
        }
        public static Vector2 operator *(Matrix2x3 m, Vector2 v)
        {
            float x = (m.m00 * v.x) - (m.m01 * v.y) + m.m02;
            float y = (m.m10 * v.x) + (m.m11 * v.y) + m.m12;
            return new Vector2(x, y);
        }
    }
}
