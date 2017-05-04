//
// P2DDistanceJoint.cs
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
    // 1-D rained system
    // m (v2 - v1) = lambda
    // v2 + (beta/h) * x1 + gamma * lambda = 0, gamma has units of inverse mass.
    // x2 = x1 + h * v2

    // 1-D mass-damper-spring system
    // m (v2 - v1) + h * d * v2 + h * k *

    // C = norm(p2 - p1) - L
    // u = (p2 - p1) / norm(p2 - p1)
    // Cdot = dot(u, v2 + cross(w2, r2) - v1 - cross(w1, r1))
    // J = [-u -cross(r1, u) u cross(r2, u)]
    // K = J * invM * JT
    //   = invMass1 + invI1 * cross(r1, u)^2 + invMass2 + invI2 * cross(r2, u)^2

    /// <summary>
    /// A distance joint rains two points on two bodies
    /// to remain at a fixed distance from each other. You can view
    /// this as a massless, rigid rod.
    /// </summary>
    sealed class P2DDistanceJoint : P2DJoint
    {
        #region Solver shared
        float m_Bias;
        float m_Gamma;
        float m_Impulse;
        float m_CurrentLength;
        #endregion

        #region Solver temp
        Vector2 m_LocalCenterA;
        Vector2 m_LocalCenterB;
        float m_InvMassA;
        float m_InvMassB;
        float m_InvIA;
        float m_InvIB;
        float m_Mass;
        Vector2 m_RA;
        Vector2 m_RB;
        Vector2 m_U;
        #endregion

        internal P2DDistanceJoint()
        {
            _jointType = JointType.Distance;
        }

        internal P2DDistanceJoint(Rigidbody2DExt bodyA, Rigidbody2DExt bodyB, Vector2 localAnchorA, Vector2 localAnchorB)
            : base(bodyA, bodyB)
        {
            _jointType = JointType.Distance;
            _localAnchorA = localAnchorA;
            _localAnchorB = localAnchorB;
        }

        internal float _length;
        internal float _frequency;
        internal float _dampingRatio;
        internal bool _isElastic;

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
            return (invDt * m_Impulse) * m_U;
        }

        internal override float GetReactionTorque(float invDt)
        {
            return 0f;
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
            m_U = cB + m_RB - cA - m_RA;

            m_CurrentLength = m_U.magnitude;

            if (m_CurrentLength > Constants.linearSlop)
            {
                m_U *= 1.0f / m_CurrentLength;
            }
            else
            {
                m_U = Vector2.zero;
            }

            float crA = MathUtils.Cross(m_RA, m_U);
            float crB = MathUtils.Cross(m_RB, m_U);
            float invMass = m_InvMassA + m_InvIA * crA * crA + m_InvMassB + m_InvIB * crB * crB;

            m_Mass = invMass != 0.0f ? 1.0f / invMass : 0.0f;

            if (_frequency > 0.0f)
            {
                float C = m_CurrentLength - _length;

                // Frequency
                float omega = 2.0f * Mathf.PI * _frequency;

                // Damping coefficient
                float d = 2.0f * m_Mass * _dampingRatio * omega;

                // Spring stiffness
                float k = m_Mass * omega * omega;

                // magic formulas
                float h = Time.deltaTime;
                m_Gamma = h * (d + h * k);
                m_Gamma = m_Gamma != 0.0f ? 1.0f / m_Gamma : 0.0f;
                m_Bias = C * h * k * m_Gamma;

                invMass += m_Gamma;
                m_Mass = invMass != 0.0f ? 1.0f / invMass : 0.0f;
            }
            else
            {
                m_Gamma = 0.0f;
                m_Bias = 0.0f;
            }

            // Scale the impulse to support a variable time step.
            //m_Impulse *= data.step.dtRatio; TODO

            Vector2 P = m_Impulse * m_U;
            vA -= m_InvMassA * P;
            wA -= m_InvIA * MathUtils.Cross(m_RA, P);
            vB += m_InvMassB * P;
            wB += m_InvIB * MathUtils.Cross(m_RB, P);

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

            // Cdot = dot(u, v + cross(w, r))
            Vector2 vpA = vA + MathUtils.Cross(wA, m_RA);
            Vector2 vpB = vB + MathUtils.Cross(wB, m_RB);
            float C = m_CurrentLength - _length;
            float Cdot = Vector2.Dot(m_U, vpB - vpA);

            float impulse;

            if (_isElastic)
            {
                // Predictive constraint.
                if (C > 0.0f)
                {
                    Cdot += (1.0f / Time.deltaTime) * C;
                }

                impulse = -m_Mass * (Cdot + m_Bias + m_Gamma * m_Impulse);
                float oldImpulse = m_Impulse;
                m_Impulse = Mathf.Min(0.0f, m_Impulse + impulse);
                impulse = m_Impulse - oldImpulse;
            }
            else
            {
                impulse = -m_Mass * (Cdot + m_Bias + m_Gamma * m_Impulse);
                m_Impulse += impulse;
            }

            Vector2 P = impulse * m_U;
            vA -= m_InvMassA * P;
            wA -= m_InvIA * MathUtils.Cross(m_RA, P);
            vB += m_InvMassB * P;
            wB += m_InvIB * MathUtils.Cross(m_RB, P);

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
            if (_frequency > 0.0f)
            {
                // There is no position correction for soft distance constraints.
                return true;
            }
            Vector2 cA = _bodyA.GetRelativePoint(m_LocalCenterA);
            Vector2 cA0 = cA;
            float aA = _bodyA.rotation * Mathf.Deg2Rad;
            Vector2 cB = _bodyB.GetRelativePoint(m_LocalCenterB);
            Vector2 cB0 = cB;
            float aB = _bodyB.rotation * Mathf.Deg2Rad;

            Rot qA = new Rot(aA), qB = new Rot(aB);

            Vector2 rA = MathUtils.Mul(qA, _localAnchorA - m_LocalCenterA);
            Vector2 rB = MathUtils.Mul(qB, _localAnchorB - m_LocalCenterB);
            Vector2 u = cB + rB - cA - rA;

            float length = u.magnitude;
            u.Normalize();

            float C = length - _length;

            if (_isElastic)
            {
                C = Mathf.Clamp(C, 0.0f, Constants.maxLinearCorrection);
            }
            else
            {
                C = Mathf.Clamp(C, -Constants.maxLinearCorrection, Constants.maxLinearCorrection);
            }
            float impulse = -m_Mass * C;
            Vector2 P = impulse * u;

            cA -= m_InvMassA * P;
            aA -= m_InvIA * MathUtils.Cross(rA, P);
            cB += m_InvMassB * P;
            aB += m_InvIB * MathUtils.Cross(rB, P);

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
            return _isElastic ? (length - _length) < Constants.linearSlop : Mathf.Abs(C) < Constants.linearSlop;
        }
    }
}
