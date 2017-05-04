using UnityEngine;

namespace Thinksquirrel.Phys2D.Internal
{
    internal struct Rot
    {
        public float s;
        public float c;

        public Rot(float angle)
        {
            MathUtils.SineCos(angle, out s, out c);
        }    
        public void Set(float angle)
        {
            MathUtils.SineCos(angle, out s, out c);
        }
        public void SetIdentity()
        {
            s = 0.0f;
            c = 1.0f;
        }
        public float GetAngle()
        {
            return Mathf.Atan2(s, c);
        }
        public Vector2 GetXAxis()
        {
            return new Vector2(c, s);
        }
        public Vector2 GetYAxis()
        {
            return new Vector2(-s, c);
        }        
    }
}