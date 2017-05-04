using UnityEngine;

namespace Thinksquirrel.Phys2D.Internal
{
    internal struct Mat22
    {
        public Vector2 ex, ey;

        public Mat22(Vector2 c1, Vector2 c2)
        {
            ex = c1;
            ey = c2;
        }
        public Mat22(float a11, float a12, float a21, float a22)
        {
            ex = new Vector2(a11, a21);
            ey = new Vector2(a12, a22);
        }
        public Mat22 inverse
        {
            get
            {
                float a = ex.x, b = ey.x, c = ex.y, d = ey.y;
                var det = a * d - b * c;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (det != 0.0f)
                {
                    det = 1.0f / det;
                }

                var result = new Mat22
                {
                    ex =
                    {
                        x = det*d,
                        y = -det*c
                    },
                    ey =
                    {
                        x = -det*b,
                        y = det*a
                    }
                };

                return result;
            }
        }
        public void Set(Vector2 c1, Vector2 c2)
        {
            ex = c1;
            ey = c2;
        }        
        public void SetIdentity()
        {
            ex.x = 1.0f;
            ey.x = 0.0f;
            ex.y = 0.0f;
            ey.y = 1.0f;
        }        
        public void SetZero()
        {
            ex.x = 0.0f;
            ey.x = 0.0f;
            ex.y = 0.0f;
            ey.y = 0.0f;
        }
        public Vector2 Solve(Vector2 b)
        {
            var a11 = ex.x;
            var a12 = ey.x;
            var a21 = ex.y;
            var a22 = ey.y;
            var det = a11 * a22 - a12 * a21;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (det != 0.0f)
            {
                det = 1.0f / det;
            }

            return new Vector2(det * (a22 * b.x - a12 * b.y), det * (a11 * b.y - a21 * b.x));
        }

        public static void Add(ref Mat22 A, ref Mat22 B, out Mat22 R)
        {
            R.ex = A.ex + B.ex;
            R.ey = A.ey + B.ey;
        }
    }
}