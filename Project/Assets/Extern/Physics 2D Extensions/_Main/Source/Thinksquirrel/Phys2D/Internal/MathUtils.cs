//
// MathUtils.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using System.Collections.Generic;
using Thinksquirrel.Phys2D.Internal.ConvexDecomposition;
using UnityEngine;

namespace Thinksquirrel.Phys2D.Internal
{
    internal static class MathUtils
    {
        public static Vector2 GetLinearVelocityFromWorldPoint(this Rigidbody2DExt body, Vector2 worldPoint)
        {
            return body.velocity +
                new Vector2(-body.angularVelocity * Mathf.Deg2Rad * (worldPoint.y - body.position.y),
                            body.angularVelocity * Mathf.Deg2Rad * (worldPoint.x - body.position.x));
        }
        public static float ComputeSubmergedArea(this MassData data, Vector2 normal, float offset, out Vector2 sc)
        {
            var shape = data.shape;
            sc = Vector2.zero;

            if (!shape)
                return 0;

            if (shape is CircleCollider2D)
            {
                var circle = shape as CircleCollider2D;

                var p = Mul(data.xf, circle.offset);
                var l = -(Vector2.Dot(normal, p) - offset);
                if (l < -circle.radius + 1.192092896e-07f)
                {
                    return 0;
                }
                if (l > circle.radius)
                {
                    sc = p;
                    return Mathf.PI * circle.radius * circle.radius;
                }

                var r2 = circle.radius * circle.radius;
                var l2 = l * l;
                var area = r2 * (Mathf.Asin(l / circle.radius) + Mathf.PI / 2 + l * Mathf.Sqrt(r2 - l2));
                var com = -2.0f / 3.0f * Mathf.Pow(r2 - l2, 1.5f) / area;

                sc.x = p.x + normal.x * com;
                sc.y = p.y + normal.y * com;

                return area;
            }

            if (shape is PolygonCollider2D || shape is BoxCollider2D)
            {
                return ComputeSubmergedAreaPoly(data, data.points, data.xf, normal, offset, out sc);
            }

            return 0;
        }
        static float ComputeSubmergedAreaPoly(MassData data, Vector2[] points, Transform xf, Vector2 normal, float offset, out Vector2 sc)
        {
            sc = Vector2.zero;

            // Transform into shape coordinates
            var normalL = MulT(new Rot(xf.GetAngle() * Mathf.Deg2Rad), normal);
            var offsetL = offset - Vector2.Dot(normal, xf.position);

            var l = points.Length;
            var depths = data.depths;
            var diveCount = 0;
            var intoIndex = -1;
            var outoIndex = -1;

            var lastSubmerged = false;
            int i;
            for (i = 0; i < l; i++)
            {
                depths[i] = Vector2.Dot(normalL, points[i]) - offsetL;
                var isSubmerged = depths[i] < -1.192092896e-07f;
                if (i > 0)
                {
                    if (isSubmerged)
                    {
                        if (!lastSubmerged)
                        {
                            intoIndex = i - 1;
                            diveCount++;
                        }
                    }
                    else
                    {
                        if (lastSubmerged)
                        {
                            outoIndex = i - 1;
                            diveCount++;
                        }
                    }
                }
                lastSubmerged = isSubmerged;
            }
            if (diveCount == 0)
            {
                if (lastSubmerged)
                {
                    // We're fully submerged
                    sc = Mul(xf, data.centroid);
                    return data.mass;
                }
                // We're not submerged
                return 0;
            }
            if (diveCount == 1)
            {
                if (intoIndex == -1)
                {
                    intoIndex = l - 1;
                }
                else
                {
                    outoIndex = l - 1;
                }
            }
            var intoIndex2 = (intoIndex + 1) % l;
            var outoIndex2 = (outoIndex + 1) % l;

            var intoLambda = (0 - depths[intoIndex]) / (depths[intoIndex2] - depths[intoIndex]);
            var outoLambda = (0 - depths[outoIndex]) / (depths[outoIndex2] - depths[outoIndex]);

            var intoVec = new Vector2(
                points[intoIndex].x * (1 - intoLambda) + points[intoIndex2].x * intoLambda,
                points[intoIndex].y * (1 - intoLambda) + points[intoIndex2].y * intoLambda);
            var outoVec = new Vector2(
                points[outoIndex].x * (1 - outoLambda) + points[outoIndex2].x * outoLambda,
                points[outoIndex].y * (1 - outoLambda) + points[outoIndex2].y * outoLambda);

            // Initialize accumulator
            float area = 0;
            var center = Vector2.zero;
            var p2 = points[intoIndex2];

            var k_inv3 = 1.0f / 3.0f;

            //An awkward loop from intoIndex2+1 to outIndex2
            i = intoIndex2;
            while (i != outoIndex2)
            {
                i = (i + 1) % l;
                var p3 = i == outoIndex2 ? outoVec : points[i];
                //Add the triangle formed by intoVec,p2,p3
                {
                    var e1 = p2 - intoVec;
                    var e2 = p3 - intoVec;

                    var D = Cross(e1, e2);

                    var triangleArea = 0.5f * D;

                    area += triangleArea;

                    // Area weighted centroid
                    center += triangleArea * k_inv3 * (intoVec + p2 + p3);
                }
                //
                p2 = p3;
            }

            //Normalize and transform centroid
            center *= 1.0f / area;

            sc = Mul(xf, center);

            return area;
        }
        static float GetAngle(this Transform xf)
        {
            return xf.eulerAngles.z;
        }
        public static bool IsCounterClockwise(this IList<Vector2> vertices)
        {
            if (vertices.Count < 3) return true;
            return vertices.GetSignedArea() > 0.0f;
        }
        public static float GetSignedArea(this IList<Vector2> vertices)
        {
            int i;
            float area = 0;

            for (i = 0; i < vertices.Count; i++)
            {
                var j = (i + 1) % vertices.Count;
                area += vertices[i].x * vertices[j].y;
                area -= vertices[i].y * vertices[j].x;
            }
            area /= 2.0f;
            return area;
        }
        public static bool IsConvex(this IList<Vector2> vertices)
        {
            for (var i = 0; i < vertices.Count; ++i)
            {
                var i1 = i;
                var i2 = i + 1 < vertices.Count ? i + 1 : 0;
                var edge = vertices[i2] - vertices[i1];

                for (var j = 0; j < vertices.Count; ++j)
                {
                    if (j == i1 || j == i2)
                    {
                        continue;
                    }

                    var r = vertices[j] - vertices[i1];

                    var s = edge.x * r.y - edge.y * r.x;

                    if (s <= 0.0f)
                        return false;
                }
            }
            return true;
        }
        public static void CollinearSimplify(this IList<Vector2> vertices, float collinearityTolerance)
        {
            if (vertices.Count < 3) return;

            for (var i = 0; i < vertices.Count; i++)
            {
                var prevId = vertices.PreviousIndex(i);
                var nextId = vertices.NextIndex(i);

                var prev = vertices[prevId];
                var current = vertices[i];
                var next = vertices[nextId];

                if (FloatInRange(Area(ref prev, ref current, ref next), -collinearityTolerance, collinearityTolerance))
                {
                    vertices.Remove(current);
                }
            }
        }
        static int NextIndex(this ICollection<Vector2> vertices, int index)
        {
            return index == vertices.Count - 1 ? 0 : index + 1;
        }
        static int PreviousIndex(this ICollection<Vector2> vertices, int index)
        {
            return index == 0 ? vertices.Count - 1 : index - 1;
        }
        static bool FloatInRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }
        static float Area(ref Vector2 a, ref Vector2 b, ref Vector2 c)
        {
            return a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y);
        }

        public static void ComputeProperties(this Rigidbody2D body, IList<Collider2D> shapes, ref MassData[] massData)
        {
            var totalMass = body.mass;
            var totalArea = 0.0f;

            // Calculate initial properties
            for (var i = 0; i < massData.Length; i++)
            {
                var shape = shapes[i];
                massData[i].shape = shape;

                var xf = shape.transform;
                massData[i].xf = xf;

                Vector2 scale = xf.lossyScale;
                scale.Set(Mathf.Abs(scale.x), Mathf.Abs(scale.y));

                if (!shape.enabled)
                {
                    massData[i].area = 0;
                    continue;
                }

                if (shape is CircleCollider2D)
                {
                    var circle = shape as CircleCollider2D;
                    var rad = circle.radius * Mathf.Max(scale.x, scale.y);
                    float area = Mathf.PI * rad * rad;
                    massData[i].area = area;
                    massData[i].centroid = circle.offset;
                    totalArea += area;
                }
                else if (shape is BoxCollider2D)
                {
                    var box = shape as BoxCollider2D;

                    var area = 0f;
                    var I = 0.0f;
                    var center = Vector2.zero;
                    var s = Vector2.zero;
                    const int l = 4;

                    var points = new Vector2[4];
                    var ex = Vector2.Scale(box.size, scale) / 2f;
                    points[0] = box.offset + ex;
                    points[1] = box.offset + new Vector2(-ex.x, ex.y);
                    points[2] = box.offset - ex;
                    points[3] = box.offset + new Vector2(ex.x, -ex.y);

                    massData[i].points = points;
                    massData[i].depths = new float[4];

                    // This code would put the reference point inside the polygon
                    for (var j = 0; j < l; ++j)
                    {
                        s += points[i];
                    }
                    s *= 1.0f / l;

                    const float k_inv3 = 1.0f / 3.0f;

                    for (var j = 0; j < l; ++j)
                    {
                        // Triangle vertices
                        var e1 = points[j] - s;
                        var e2 = j + 1 < l ? points[j + 1] - s : points[0] - s;

                        var D = Cross(e1, e2);

                        float triangleArea = 0.5f * D;
                        area += triangleArea;

                        // Area weighted centroid
                        center += triangleArea * k_inv3 * (e1 + e2);

                        float ex1 = e1.x, ey1 = e1.y;
                        float ex2 = e2.x, ey2 = e2.y;

                        var intx2 = ex1 * ex1 + ex2 * ex1 + ex2 * ex2;
                        var inty2 = ey1 * ey1 + ey2 * ey1 + ey2 * ey2;

                        I += 0.25f * k_inv3 * D * (intx2 + inty2);
                    }

                    // Area
                    massData[i].area = area;
                    totalArea += area;

                    // Center of mass
                    center *= 1.0f / area;
                    massData[i].centroid = center + s;

                    // For inertia calc
                    massData[i].center = center;
                    massData[i].i = I;
                }
                else if (shape is EdgeCollider2D)
                {
                    // Edges have no area, therefore they don't have mass or
                    // contribute to center of mass
                    massData[i].area = 0;
                }
                else if (shape is PolygonCollider2D)
                {
                    var poly = shape as PolygonCollider2D;

                    var pathL = poly.pathCount;

                    for(int k = 0; k < pathL; ++k)
                    {
                        var decomposed = EarclipDecomposer.ConvexPartition(poly.GetPath(k));

                        var l = decomposed.Count;

                        // Resize array
                        System.Array.Resize(ref massData, massData.Length + (l - 1));

                        // Compute each path as a seperate set of mass data
                        for(var j = 0; j < l; ++j)
                        {
                            if (decomposed[j].Count < 3)
                            {
                                // Degenerate case
                                continue;
                            }
                            ComputePropertiesPoly(poly, decomposed[j], massData, scale, j, i, ref totalArea);
                        }

                        // Increase iterator
                        i+= l - 1;
                    }
                }
            }

            for (var i = 0; i < massData.Length; i++)
            {
                var shape = massData[i].shape;

                if (!shape || !shape.enabled)
                {
                    massData[i].mass = 0;
                    massData[i].inertia = 0;
                    continue;
                }

                massData[i].mass = (massData[i].area * totalMass) / totalArea;
                
                if (shape is CircleCollider2D)
                {
                    var circle = shape as CircleCollider2D;
                    // inertia about the local origin
                    massData[i].inertia = massData[i].mass * (0.5f * circle.radius * circle.radius + Vector2.Dot(circle.offset, circle.offset));
                }
                else if (shape is BoxCollider2D || shape is PolygonCollider2D)
                {
                    var density = massData[i].mass / massData[i].area;

                    // Inertia tensor relative to the local origin (point s).
                    massData[i].inertia = density * massData[i].i;

                    // Shift to center of mass then to original body origin.
                    massData[i].inertia += massData[i].mass * (Vector2.Dot(massData[i].centroid, massData[i].centroid) - Vector2.Dot(massData[i].center, massData[i].center));
                }
            }            
        }
        static void ComputePropertiesPoly(Collider2D poly, List<Vector2> verts, MassData[] massData, Vector2 scale, int pathIndex, int iter, ref float totalArea)
        {
            var center = Vector2.zero;
            var area = 0.0f;
            var I = 0.0f;

            var s = Vector2.zero;
            var path = verts;
            var l = path.Count;
            var i = iter + pathIndex;

            massData[i].shape = poly;
            massData[i].xf = poly.transform;

            if (l < 3)
            {
                massData[i].area = 0;
                return;
            }

            massData[i].points = new Vector2[l];
            massData[i].depths = new float[l];

            // This code would put the reference point inside the polygon
            for (var j = 0; j < l; ++j)
            {
                var point = Vector2.Scale(path[j], scale);
                massData[i].points[j] = point;
                s += point;
            }
            s *= 1.0f / l;

            const float k_inv3 = 1.0f / 3.0f;

            for (var j = 0; j < l; ++j)
            {
                var point = massData[i].points[j];
                var nextPoint = j + 1 < l ? massData[i].points[j + 1] : massData[i].points[0];

                // Triangle vertices
                var e1 = point - s;
                var e2 = nextPoint - s;

                var D = Cross(e1, e2);

                var triangleArea = 0.5f * D;
                area += triangleArea;

                // Area weighted centroid
                center += triangleArea * k_inv3 * (e1 + e2);

                float ex1 = e1.x, ey1 = e1.y;
                float ex2 = e2.x, ey2 = e2.y;

                var intx2 = ex1 * ex1 + ex2 * ex1 + ex2 * ex2;
                var inty2 = ey1 * ey1 + ey2 * ey1 + ey2 * ey2;

                I += (0.25f * k_inv3 * D) * (intx2 + inty2);
            }

            // Area
            massData[i].area = area;
            totalArea += area;

            // Center of mass
            center *= 1.0f / area;
            massData[i].centroid = center + s;

            // For inertia calc
            massData[i].center = center;
            massData[i].i = I;
        }
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
        public static Vector2 Cross(float s, Vector2 a)
        {
            return new Vector2(-s * a.y, s * a.x);
        }
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }
        public static void Cross(ref Vector2 a, ref Vector2 b, out float c)
        {
            c = a.x * b.y - a.y * b.x;
        }
        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }
        internal static Vector2 Mul(ref Mat22 A, Vector2 v)
        {
            return Mul(ref A, ref v);
        }
        internal static Vector2 Mul(ref Mat22 A, ref Vector2 v)
        {
            return new Vector2(A.ex.x * v.x + A.ey.x * v.y, A.ex.y * v.x + A.ey.y * v.y);
        }
        internal static Vector3 Mul(Mat33 A, Vector3 v)
        {
            return v.x * A.ex + v.y * A.ey + v.z * A.ez;
        }
        internal static Vector2 Mul22(Mat33 A, Vector2 v)
        {
            return new Vector2(A.ex.x * v.x + A.ey.x * v.y, A.ex.y * v.x + A.ey.y * v.y);
        }
        internal static Vector2 Mul(Rot q, Vector2 v)
        {
            return new Vector2(q.c * v.x - q.s * v.y, q.s * v.x + q.c * v.y);
        }
        internal static Vector2 MulT(Rot q, Vector2 v)
        {
            return new Vector2(q.c * v.x + q.s * v.y, -q.s * v.x + q.c * v.y);
        }
        internal static Vector2 Mul(Transform T, Vector2 v)
        {
            return Mul(ref T, ref v);
        }
        public static Vector2 Mul(ref Transform T, ref Vector2 v)
        {
            var q = new Rot(T.GetAngle() * Mathf.Deg2Rad);
            var x = (q.c * v.x - q.s * v.y) + T.position.x;
            var y = (q.s * v.x + q.c * v.y) + T.position.y;

            return new Vector2(x, y);
        }
        public static Vector2 MulT(Transform T, Vector2 v)
        {
            return MulT(ref T, ref v);
        }
        public static Vector2 MulT(ref Transform T, Vector2 v)
        {
            return MulT(ref T, ref v);
        }
        public static Vector2 MulT(ref Transform T, ref Vector2 v)
        {
            var q = new Rot(T.GetAngle() * Mathf.Deg2Rad);

            var px = v.x - T.position.x;
            var py = v.y - T.position.y;
            var x = q.c * px + q.s * py;
            var y = -q.s * px + q.c * py;

            return new Vector2(x, y);
        }
        public static void SineCos(float x, out float sin, out float cos)
        {
            // TODO: Optimize sincos calc
            sin = Mathf.Sin(x);
            cos = Mathf.Cos(x);
        }
    }
}
