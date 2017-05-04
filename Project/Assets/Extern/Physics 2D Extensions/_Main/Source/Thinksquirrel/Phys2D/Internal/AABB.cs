using UnityEngine;

namespace Thinksquirrel.Phys2D.Internal
{
    internal struct AABB
    {
        /// <summary>
        /// The lower vertex
        /// </summary>
        public Vector2 LowerBound;

        /// <summary>
        /// The upper vertex
        /// </summary>
        public Vector2 UpperBound;

        public AABB(Vector2 min, Vector2 max)
            : this(ref min, ref max)
        {
        }

        public AABB(ref Vector2 min, ref Vector2 max)
        {
            LowerBound = min;
            UpperBound = max;
        }

        public AABB(Vector2 center, float width, float height)
        {
            LowerBound = center - new Vector2(width / 2, height / 2);
            UpperBound = center + new Vector2(width / 2, height / 2);
        }


        /// <summary>
        /// Get the center of the Aabb.
        /// </summary>
        /// <value></value>
        public Vector2 Center
        {
            get { return 0.5f * (LowerBound + UpperBound); }
        }

        /// <summary>
        /// Get the extents of the Aabb (half-widths).
        /// </summary>
        /// <value></value>
        public Vector2 Extents
        {
            get { return 0.5f * (UpperBound - LowerBound); }
        }

        /// <summary>
        /// Get the perimeter length
        /// </summary>
        /// <value></value>
        public float Perimeter
        {
            get
            {
                float wx = UpperBound.x - LowerBound.x;
                float wy = UpperBound.y - LowerBound.y;
                return 2.0f * (wx + wy);
            }
        }


        /// <summary>
        /// first quadrant
        /// </summary>
        public AABB Q1
        {
            get { return new AABB(Center, UpperBound); }
        }

        public AABB Q2
        {
            get
            {
                return new AABB(new Vector2(LowerBound.x, Center.y), new Vector2(Center.x, UpperBound.y));
                ;
            }
        }

        public AABB Q3
        {
            get { return new AABB(LowerBound, Center); }
        }

        public AABB Q4
        {
            get { return new AABB(new Vector2(Center.x, LowerBound.y), new Vector2(UpperBound.x, Center.y)); }
        }

        public Vector2[] GetVertices()
        {
            Vector2 p1 = UpperBound;
            Vector2 p2 = new Vector2(UpperBound.x, LowerBound.y);
            Vector2 p3 = LowerBound;
            Vector2 p4 = new Vector2(LowerBound.x, UpperBound.y);
            return new[] { p1, p2, p3, p4 };
        }

        /// <summary>
        /// Combine an Aabb into this one.
        /// </summary>
        /// <param name="aabb">The aabb.</param>
        public void Combine(ref AABB aabb)
        {
            LowerBound = Vector2.Min(LowerBound, aabb.LowerBound);
            UpperBound = Vector2.Max(UpperBound, aabb.UpperBound);
        }

        /// <summary>
        /// Combine two AABBs into this one.
        /// </summary>
        /// <param name="aabb1">The aabb1.</param>
        /// <param name="aabb2">The aabb2.</param>
        public void Combine(ref AABB aabb1, ref AABB aabb2)
        {
            LowerBound = Vector2.Min(aabb1.LowerBound, aabb2.LowerBound);
            UpperBound = Vector2.Max(aabb1.UpperBound, aabb2.UpperBound);
        }

        /// <summary>
        /// Does this aabb contain the provided Aabb.
        /// </summary>
        /// <param name="aabb">The aabb.</param>
        /// <returns>
        ///     <c>true</c> if it contains the specified aabb; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ref AABB aabb)
        {
            bool result = true;
            result = result && LowerBound.x <= aabb.LowerBound.x;
            result = result && LowerBound.y <= aabb.LowerBound.y;
            result = result && aabb.UpperBound.x <= UpperBound.x;
            result = result && aabb.UpperBound.y <= UpperBound.y;
            return result;
        }

        /// <summary>
        /// Determines whether the AAABB contains the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if it contains the specified point; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ref Vector2 point)
        {
            //using epsilon to try and gaurd against float rounding errors.
            if ((point.x > (LowerBound.x + 1.192092896e-07f) && point.x < (UpperBound.x - 1.192092896e-07f) &&
                 (point.y > (LowerBound.y + 1.192092896e-07f) && point.y < (UpperBound.y - 1.192092896e-07f))))
            {
                return true;
            }
            return false;
        }

        public static bool TestOverlap(AABB a, AABB b)
        {
            return TestOverlap(ref a, ref b);
        }

        public static bool TestOverlap(ref AABB a, ref AABB b)
        {
            Vector2 d1 = b.LowerBound - a.UpperBound;
            Vector2 d2 = a.LowerBound - b.UpperBound;

            if (d1.x > 0.0f || d1.y > 0.0f)
                return false;

            if (d2.x > 0.0f || d2.y > 0.0f)
                return false;

            return true;
        }
    }
}