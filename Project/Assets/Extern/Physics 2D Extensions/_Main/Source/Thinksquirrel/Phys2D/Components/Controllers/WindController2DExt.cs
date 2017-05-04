//
// WindController2DExt.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using Thinksquirrel.Phys2D.Internal.Controllers;
using Thinksquirrel.Phys2D.Internal;

namespace Thinksquirrel.Phys2D
{
    [AddComponentMenu("Physics 2D/Physics Controllers/Wind Controller 2D (P2D Extensions)")]
    public sealed class WindController2DExt : Controller2DExt
    {
        [SerializeField] WindType m_WindType = WindType.Curve;
        [SerializeField] ForceType m_ForceType = ForceType.Line;
        [SerializeField] Vector2 m_Position = Vector2.zero;
        [SerializeField] Vector2 m_Direction = Vector2.right;
        [SerializeField] bool m_IgnorePosition = true;
        [SerializeField] float m_Strength = 1.0f;
        [SerializeField] AnimationCurve m_StrengthCurve = new AnimationCurve();
        [SerializeField] DecayMode m_DecayMode = DecayMode.Curve;
        [SerializeField] AnimationCurve m_DecayCurve = new AnimationCurve();
        [SerializeField] float m_DecayStart = 2f;
        [SerializeField] float m_DecayEnd = 10f;
        [SerializeField] float m_Divergence = .2f;
        [SerializeField] float m_Variation = 0.8f;
        [SerializeField] float m_TimeScale = 1.0f;

        internal P2DSimpleWindForce _windForce;

        public Vector2 windForce
        {
            get { return _windForce == null ? Vector2.zero : _windForce._force; }
        }

        void Reset()
        {
            m_StrengthCurve.AddKey(new Keyframe(0, 0));
            m_StrengthCurve.AddKey(new Keyframe(0.1f, -4));
            m_StrengthCurve.AddKey(new Keyframe(0.2f, 5));
            m_StrengthCurve.AddKey(new Keyframe(0.4f, 5));
            m_StrengthCurve.AddKey(new Keyframe(0.5f, 0));
            m_StrengthCurve.AddKey(new Keyframe(0.65f, -3));
            m_StrengthCurve.AddKey(new Keyframe(0.8f, 0));
            m_StrengthCurve.postWrapMode = WrapMode.Loop;

            m_DecayCurve = AnimationCurve.EaseInOut(0, 1, 10, 0);
        }

        internal override sealed void PhysicsUpdate()
        {
            if (_internalController == null)
            {
                _internalController = _windForce = new P2DSimpleWindForce(transform, _rigidbodies, GetAABB(true));
            }

            _windForce.container = GetAABB(false);
            _windForce.DecayCurve = m_DecayCurve;
            _windForce.StrengthCurve = m_StrengthCurve;
            _windForce.forceType = m_ForceType;
            _windForce.Strength = m_Strength;
            _windForce.TimingMode = m_WindType;
            _windForce.Variation = m_Variation;
            _windForce.DecayMode = m_DecayMode;
            _windForce.DecayStart = m_DecayStart;
            _windForce.DecayEnd = m_DecayEnd;
            _windForce.Position = transform.GetRelativePoint(m_Position);
            _windForce.Direction = transform.GetRelativeVector(m_Direction);
            _windForce.Divergence = m_Divergence;
            _windForce.IgnorePosition = m_IgnorePosition;
            _windForce.TimeScale = m_TimeScale;

            _windForce.Update(Time.deltaTime);
        }

        public WindType windType { get { return m_WindType; } set { m_WindType = value; } }
        public ForceType forceType { get { return m_ForceType; } set { m_ForceType = value; } }
        public Vector2 position { get { return m_Position; } set { m_Position = value; } }
        public Vector2 direction { get { return m_Direction; } set { m_Direction = value; } }
        public bool ignorePosition { get { return m_IgnorePosition; } set { m_IgnorePosition = value; } }
        public float strength { get { return m_Strength; } set { m_Strength = value; } }
        public AnimationCurve strengthCurve { get { return m_StrengthCurve; } set { m_StrengthCurve = value; } }
        public DecayMode decayMode { get { return m_DecayMode; } set { m_DecayMode = value; } }
        public AnimationCurve decayCurve { get { return m_DecayCurve; } set { m_DecayCurve = value; } }
        public float decayStart { get { return m_DecayStart; } set { m_DecayStart = value; } }
        public float decayEnd { get { return m_DecayEnd; } set { m_DecayEnd = value; } }
        public float divergence { get { return m_Divergence; } set { m_Divergence = value; } }
        public float variation { get { return m_Variation; } set { m_Variation = value; } }
        public float timeScale { get { return m_TimeScale; } set { m_TimeScale = value; } }

    }

    #region enums
    public enum DecayMode
    {
        None,
        Step,
        Linear,
        InverseSquare,
        Curve
    }
    public enum ForceType
    {
        Point,
        Line
    }
    public enum WindType
    {
        Constant,
        Curve
    }

    #endregion

}
