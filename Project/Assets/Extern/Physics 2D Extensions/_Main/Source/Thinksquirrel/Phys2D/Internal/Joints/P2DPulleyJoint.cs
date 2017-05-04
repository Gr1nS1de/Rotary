//
// P2DPulleyJoint.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using Thinksquirrel.Phys2D;

namespace Thinksquirrel.Phys2D.Internal.Joints
{
    // Pulley:
    // length1 = norm(p1 - s1)
    // length2 = norm(p2 - s2)
    // C0 = (length1 + ratio * length2)_initial
    // C = C0 - (length1 + ratio * length2)
    // u1 = (p1 - s1) / norm(p1 - s1)
    // u2 = (p2 - s2) / norm(p2 - s2)
    // Cdot = -dot(u1, v1 + cross(w1, r1)) - ratio * dot(u2, v2 + cross(w2, r2))
    // J = -[u1 cross(r1, u1) ratio * u2  ratio * cross(r2, u2)]
    // K = J * invM * JT
    //   = invMass1 + invI1 * cross(r1, u1)^2 + ratio^2 * (invMass2 + invI2 * cross(r2, u2)^2)

    /// <summary>
    /// The pulley joint is connected to two bodies and two fixed ground points.
    /// The pulley supports a ratio such that:
    /// length1 + ratio * length2 <= constant
    /// Yes, the force transmitted is scaled by the ratio.
    /// Warning: the pulley joint can get a bit squirrelly by itself. They often
    /// work better when combined with prismatic joints. You should also cover the
    /// the anchor points with static shapes to prevent one side from going to
    /// zero length.
    /// </summary>
    sealed class P2DPulleyJoint : P2DJoint
    {
        public Transform groundAnchorA { get; set; }
        public Transform groundAnchorB { get; set; }
        public float ratio { get; set; }

        #region Solver shared
        float m_Impulse;
        float m_Constant;
        Vector2 m_GroundPositionA;
        Vector2 m_GroundPositionB;
        float m_LengthA;
        float m_LengthB;
        #endregion

        #region Solver temp
        Vector2 m_LocalCenterA;
        Vector2 m_LocalCenterB;
        Vector2 m_UA;
        Vector2 m_UB;
        Vector2 m_RA;
        Vector2 m_RB;
        float m_InvMassA;
        float m_InvMassB;
        float m_InvIA;
        float m_InvIB;
        float m_Mass;
        #endregion

        internal P2DPulleyJoint()
        {
            _jointType = JointType.Pulley;
        }

        internal P2DPulleyJoint(Rigidbody2DExt bodyA, Rigidbody2DExt bodyB, Vector2 localAnchorA, Vector2 localAnchorB, Transform groundAnchorA, Transform groundAnchorB, float ratio)
            : base(bodyA, bodyB)
        {
            _jointType = JointType.Pulley;
            _localAnchorA = localAnchorA;
            _localAnchorB = localAnchorB;
            this.groundAnchorA = groundAnchorA;
            this.groundAnchorB = groundAnchorB;

            if (m_GroundPositionA != (Vector2)groundAnchorA.position)
            {
                m_GroundPositionA = groundAnchorA.position;
                var dA = _bodyA.GetRelativePoint(_localAnchorA) - m_GroundPositionA;
                m_LengthA = dA.magnitude;
            }

            if (m_GroundPositionB != (Vector2)groundAnchorB.position)
            {
                m_GroundPositionB = groundAnchorB.position;
                var dB = _bodyB.GetRelativePoint(_localAnchorB) - m_GroundPositionB;
                m_LengthB = dB.magnitude;
            }

            this.ratio = ratio;
        }

        internal Vector2 _localAnchorA;
        internal Vector2 _localAnchorB;

        internal override sealed Vector2 worldAnchorA
        {
            get { return _bodyA.GetRelativePoint(_localAnchorA); }
        }

        internal override sealed Vector2 worldAnchorB
        {
            get { return _bodyB.GetRelativePoint(_localAnchorB); }
            set { Debug.LogError("You can't set the world anchor on this joint type."); }
        }

        internal override Vector2 GetReactionForce(float invDt)
        {
            return invDt * (m_Impulse * m_UB);
        }

        internal override float GetReactionTorque(float invDt)
        {
            return 0.0f;
        }

        internal override void InitVelocityConstraints()
        {
            m_LocalCenterA = _bodyA.centerOfMass;
            m_LocalCenterB = _bodyB.centerOfMass;
            m_InvMassA = _bodyA._invMass;
            m_InvMassB = _bodyB._invMass;
            m_InvIA = _bodyA._invI;
            m_InvIB = _bodyB._invI;

            Vector2 cA = _bodyA.GetRelativePoint(m_LocalCenterA);
            float aA = _bodyA.rotation * Mathf.Deg2Rad;
            Vector2 vA = _bodyA.velocity;
            float wA = _bodyA.angularVelocity * Mathf.Deg2Rad;

            Vector2 cB = _bodyB.GetRelativePoint(m_LocalCenterB);
            float aB = _bodyB.rotation * Mathf.Deg2Rad;
            Vector2 vB = _bodyB.velocity;
            float wB = _bodyB.angularVelocity * Mathf.Deg2Rad;

            Rot qA = new Rot(aA), qB = new Rot(aB);

            m_RA = MathUtils.Mul(qA, _localAnchorA - m_LocalCenterA);
            m_RB = MathUtils.Mul(qB, _localAnchorB - m_LocalCenterB);

            if (m_GroundPositionA != (Vector2)groundAnchorA.position)
            {
                m_GroundPositionA = groundAnchorA.position;
                var dA = _bodyA.GetRelativePoint(_localAnchorA) - m_GroundPositionA;
                m_LengthA = dA.magnitude;
            }

            if (m_GroundPositionB != (Vector2)groundAnchorB.position)
            {
                m_GroundPositionB = groundAnchorB.position;
                var dB = _bodyB.GetRelativePoint(_localAnchorB) - m_GroundPositionB;
                m_LengthB = dB.magnitude;
            }

            m_Constant = m_LengthA + ratio * m_LengthB;

            // Get the pulley axes.
            m_UA = cA + m_RA - (Vector2)groundAnchorA.position;
            m_UB = cB + m_RB - (Vector2)groundAnchorB.position;

            float lengthA = m_UA.magnitude;
            float lengthB = m_UB.magnitude;

            if (lengthA > 10.0f * Constants.linearSlop)
            {
                m_UA *= 1.0f / lengthA;
            }
            else
            {
                m_UA = Vector2.zero;
            }

            if (lengthB > 10.0f * Constants.linearSlop)
            {
                m_UB *= 1.0f / lengthB;
            }
            else
            {
                m_UB = Vector2.zero;
            }

            // Compute effective mass.
            float ruA = MathUtils.Cross(m_RA, m_UA);
            float ruB = MathUtils.Cross(m_RB, m_UB);

            float mA = m_InvMassA + m_InvIA * ruA * ruA;
            float mB = m_InvMassB + m_InvIB * ruB * ruB;

            m_Mass = mA + ratio * ratio * mB;

            if (m_Mass > 0.0f)
            {
                m_Mass = 1.0f / m_Mass;
            }

            // TODO: Scale impulses to support variable time steps.
            // m_Impulse *= data.step.dtratio;

            // Warm starting.
            Vector2 PA = -(m_Impulse) * m_UA;
            Vector2 PB = (-ratio * m_Impulse) * m_UB;
            vA += m_InvMassA * PA;
            wA += m_InvIA * MathUtils.Cross(m_RA, PA);
            vB += m_InvMassB * PB;
            wB += m_InvIB * MathUtils.Cross(m_RB, PB);

            if (!_bodyA.isKinematic)
            {
                _bodyA.velocity = vA;
                if (!_bodyA.fixedAngle)
                {
                    _bodyA.angularVelocity = wA * Mathf.Rad2Deg;
                }
            }
            if (!_bodyB.isKinematic)
            {
                _bodyB.velocity = vB;
                if (!_bodyB.fixedAngle)
                {
                    _bodyB.angularVelocity = wB * Mathf.Rad2Deg;
                }
            }
        }

        internal override void SolveVelocityConstraints()
        {
            Vector2 vA = _bodyA.velocity;
            float wA = _bodyA.angularVelocity * Mathf.Deg2Rad;
            Vector2 vB = _bodyB.velocity;
            float wB = _bodyB.angularVelocity * Mathf.Deg2Rad;

            Vector2 vpA = vA + MathUtils.Cross(wA, m_RA);
            Vector2 vpB = vB + MathUtils.Cross(wB, m_RB);

            float Cdot = -Vector2.Dot(m_UA, vpA) - ratio * Vector2.Dot(m_UB, vpB);
            float impulse = -m_Mass * Cdot;
            m_Impulse += impulse;

            Vector2 PA = -impulse * m_UA;
            Vector2 PB = -ratio * impulse * m_UB;
            vA += m_InvMassA * PA;
            wA += m_InvIA * MathUtils.Cross(m_RA, PA);
            vB += m_InvMassB * PB;
            wB += m_InvIB * MathUtils.Cross(m_RB, PB);

            if (!_bodyA.isKinematic)
            {
                _bodyA.velocity = vA;
                if (!_bodyA.fixedAngle)
                {
                    _bodyA.angularVelocity = wA * Mathf.Rad2Deg;
                }
            }
            if (!_bodyB.isKinematic)
            {
                _bodyB.velocity = vB;
                if (!_bodyB.fixedAngle)
                {
                    _bodyB.angularVelocity = wB * Mathf.Rad2Deg;
                }
            }
        }

        internal override bool SolvePositionConstraints()
        {
            Vector2 cA = _bodyA.GetRelativePoint(m_LocalCenterA);
            Vector2 cA0 = cA;
            float aA = _bodyA.rotation * Mathf.Deg2Rad;
            Vector2 cB = _bodyB.GetRelativePoint(m_LocalCenterB);
            Vector2 cB0 = cB;
            float aB = _bodyB.rotation * Mathf.Deg2Rad;

            Rot qA = new Rot(aA), qB = new Rot(aB);

            Vector2 rA = MathUtils.Mul(qA, _localAnchorA - m_LocalCenterA);
            Vector2 rB = MathUtils.Mul(qB, _localAnchorB - m_LocalCenterB);

            // Get the pulley axes.
            Vector2 uA = cA + rA - m_GroundPositionA;
            Vector2 uB = cB + rB - m_GroundPositionB;

            float lengthA = uA.magnitude;
            float lengthB = uB.magnitude;

            if (lengthA > 10.0f * Constants.linearSlop)
            {
                uA *= 1.0f / lengthA;
            }
            else
            {
                uA = Vector2.zero;
            }

            if (lengthB > 10.0f * Constants.linearSlop)
            {
                uB *= 1.0f / lengthB;
            }
            else
            {
                uB = Vector2.zero;
            }

            // Compute effective mass.
            float ruA = MathUtils.Cross(rA, uA);
            float ruB = MathUtils.Cross(rB, uB);

            float mA = m_InvMassA + m_InvIA * ruA * ruA;
            float mB = m_InvMassB + m_InvIB * ruB * ruB;

            float mass = mA + ratio * ratio * mB;

            if (mass > 0.0f)
            {
                mass = 1.0f / mass;
            }

            float C = m_Constant - lengthA - ratio * lengthB;
            float linearError = Mathf.Abs(C);

            float impulse = -mass * C;

            Vector2 PA = -impulse * uA;
            Vector2 PB = -ratio * impulse * uB;

            cA += m_InvMassA * PA;
            aA += m_InvIA * MathUtils.Cross(rA, PA);
            cB += m_InvMassB * PB;
            aB += m_InvIB * MathUtils.Cross(rB, PB);

            if (!_bodyA.isKinematic)
            {
                var dcA = cA0 - cA;
                _bodyA.position -= dcA;
                if (!_bodyA.fixedAngle)
                {
                    _bodyA.rotation = aA * Mathf.Rad2Deg;
                }
            }
            if (!_bodyB.isKinematic)
            {
                var dcB = cB0 - cB;
                _bodyB.position -= dcB;
                if (!_bodyB.fixedAngle)
                {
                    _bodyB.rotation = aB * Mathf.Rad2Deg;
                }
            }

            return linearError < Constants.linearSlop;
        }
    }
}
