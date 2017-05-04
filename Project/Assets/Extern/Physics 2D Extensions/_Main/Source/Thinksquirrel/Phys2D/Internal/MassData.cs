using UnityEngine;
using System;

namespace Thinksquirrel.Phys2D.Internal
{
    internal struct MassData : IEquatable<MassData>
    {
        public Collider2D shape;
        public Vector2[] points;
        public float[] depths;
        public Transform xf;

        public float area;
        public Vector2 centroid;
        public float inertia;
        public float mass;

        public float i;
        public Vector2 center;

        public bool Equals(MassData other)
        {
            return this == other;
        }
        public static bool operator ==(MassData left, MassData right)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return left.area == right.area && left.mass == right.mass && left.centroid == right.centroid && left.inertia == right.inertia;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public static bool operator !=(MassData left, MassData right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MassData && Equals((MassData)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                var result = area.GetHashCode();
                result = (result * 397) ^ centroid.GetHashCode();
                result = (result * 397) ^ inertia.GetHashCode();
                result = (result * 397) ^ mass.GetHashCode();
                return result;
                // ReSharper restore NonReadonlyMemberInGetHashCode
            }
        }
    }
}