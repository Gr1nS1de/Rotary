using UnityEngine;

namespace Thinksquirrel.Phys2D.Internal
{
    internal struct Mat33
    {
        public Vector3 ex, ey, ez;
        public Mat33(Vector3 c1, Vector3 c2, Vector3 c3)
        {
            ex = c1;
            ey = c2;
            ez = c3;
        }
        public void SetZero()
        {
            ex = Vector3.zero;
            ey = Vector3.zero;
            ez = Vector3.zero;
        }
        public Vector3 Solve33(Vector3 b)
        {
            var det = Vector3.Dot(ex, Vector3.Cross(ey, ez));
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (det != 0.0f)
            {
                det = 1.0f / det;
            }

            return new Vector3(det * Vector3.Dot(b, Vector3.Cross(ey, ez)),
                               det * Vector3.Dot(ex, Vector3.Cross(b, ez)),
                               det * Vector3.Dot(ex, Vector3.Cross(ey, b)));
        }        
        public Vector2 Solve22(Vector2 b)
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
        public void GetInverse22(ref Mat33 M)
        {
            var a = ex.x;
            var b = ey.x;
            var c = ex.y;
            var d = ey.y;
            var det = a * d - b * c;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (det != 0.0f)
            {
                det = 1.0f / det;
            }

            M.ex.x = det * d; M.ey.x = -det * b; M.ex.z = 0.0f;
            M.ex.y = -det * c; M.ey.y = det * a; M.ey.z = 0.0f;
            M.ez.x = 0.0f; M.ez.y = 0.0f; M.ez.z = 0.0f;
        }
        public void GetSymInverse33(ref Mat33 M)
        {
            var det = MathUtils.Dot(ex, MathUtils.Cross(ey, ez));
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (det != 0.0f)
            {
                det = 1.0f / det;
            }

            float a11 = ex.x, a12 = ey.x, a13 = ez.x;
            float a22 = ey.y, a23 = ez.y;
            var a33 = ez.z;

            M.ex.x = det * (a22 * a33 - a23 * a23);
            M.ex.y = det * (a13 * a23 - a12 * a33);
            M.ex.z = det * (a12 * a23 - a13 * a22);

            M.ey.x = M.ex.y;
            M.ey.y = det * (a11 * a33 - a13 * a13);
            M.ey.z = det * (a13 * a12 - a11 * a23);

            M.ez.x = M.ex.z;
            M.ez.y = M.ey.z;
            M.ez.z = det * (a11 * a22 - a12 * a12);
        }


    }
}