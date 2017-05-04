//
// P2DFrictionJoint.cs
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
    // Point-to-point constraint
    // Cdot = v2 - v1
    //      = v2 + cross(w2, r2) - v1 - cross(w1, r1)
    // J = [-I -r1_skew I r2_skew ]
    // Identity used:
    // w k % (rx i + ry j) = w * (-ry i + rx j)

    // Angle constraint
    // Cdot = w2 - w1
    // J = [0 0 -1 0 0 1]
    // K = invI1 + invI2

    /// <summary>
    /// Friction joint. This is used for top-down friction.
    /// It provides 2D translational friction and angular friction.
    /// </summary>
    sealed class P2DFrictionJoint : P2DJoint
    {
        public float maxForce { get; set; }
        public float maxTorque { get; set; }
        
        #region Solver shared
        Vector2 m_LinearImpulse;
        float m_AngularImpulse;
        #endregion

        #region Solver temp
        Vector2 m_RA;
        Vector2 m_RB;
        Vector2 m_LocalCenterA;
        Vector2 m_LocalCenterB;
        float m_InvMassA;
        float m_InvMassB;
        float m_InvIA;
        float m_InvIB;
        float m_AngularMass;
        Mat22 m_LinearMass;
        #endregion
        
        internal P2DFrictionJoint()
        {
            _jointType = JointType.Friction;
        }

        internal P2DFrictionJoint(Rigidbody2DExt bodyA, Rigidbody2DExt bodyB, Vector2 localAnchorA, Vector2 localAnchorB)
            : base(bodyA, bodyB)
        {
            _jointType = JointType.Friction;
            _localAnchorA = localAnchorA;
            _localAnchorB = localAnchorB;
            maxForce = 100f;
            maxTorque = 100f;
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
            return invDt * m_LinearImpulse;
        }

        internal override float GetReactionTorque(float invDt)
        {
            return invDt * m_AngularImpulse;
        }

        internal override void InitVelocityConstraints()
        {
            m_LocalCenterA = _bodyA.centerOfMass;
            m_LocalCenterB = _bodyB.centerOfMass;
            m_InvMassA = _bodyA._invMass;
            m_InvMassB = _bodyB._invMass;
            m_InvIA = _bodyA._invI;
            m_InvIB = _bodyB._invI;

            //Vector2 cA = _bodyA.GetRelativePoint(m_LocalCenterA);
            float aA = _bodyA.rotation * Mathf.Deg2Rad;
            Vector2 vA = _bodyA.velocity;
            float wA = _bodyA.angularVelocity * Mathf.Deg2Rad;

            //Vector2 cB = _bodyB.GetRelativePoint(m_LocalCenterB);
            float aB = _bodyB.rotation * Mathf.Deg2Rad;
            Vector2 vB = _bodyB.velocity;
            float wB = _bodyB.angularVelocity * Mathf.Deg2Rad;

            Rot qA = new Rot(aA), qB = new Rot(aB);

            m_RA = MathUtils.Mul(qA, _localAnchorA - m_LocalCenterA);
            m_RB = MathUtils.Mul(qB, _localAnchorB - m_LocalCenterB);
            
            // J = [-I -r1_skew I r2_skew]
            //     [ 0       -1 0       1]
            // r_skew = [-ry; rx]

            // Matlab
            // K = [ mA+r1y^2*iA+mB+r2y^2*iB,  -r1y*iA*r1x-r2y*iB*r2x,          -r1y*iA-r2y*iB]
            //     [  -r1y*iA*r1x-r2y*iB*r2x, mA+r1x^2*iA+mB+r2x^2*iB,           r1x*iA+r2x*iB]
            //     [          -r1y*iA-r2y*iB,           r1x*iA+r2x*iB,                   iA+iB]

            float mA = m_InvMassA, mB = m_InvMassB;
            float iA = m_InvIA, iB = m_InvIB;

            Mat22 K = new Mat22();
            K.ex.x = mA + mB + iA * m_RA.y * m_RA.y + iB * m_RB.y * m_RB.y;
            K.ex.y = -iA * m_RA.x * m_RA.y - iB * m_RB.x * m_RB.y;
            K.ey.x = K.ex.y;
            K.ey.y = mA + mB + iA * m_RA.x * m_RA.x + iB * m_RB.x * m_RB.x;

            m_LinearMass = K.inverse;

            m_AngularMass = iA + iB;
            if (m_AngularMass > 0.0f)
            {
                m_AngularMass = 1.0f / m_AngularMass;
            }

            // TODO: Scale impulses to support a variable time step.
            // m_LinearImpulse *= data.step.dtRatio;
            // m_AngularImpulse *= data.step.dtRatio;

            Vector2 P = new Vector2(m_LinearImpulse.x, m_LinearImpulse.y);

            vA -= mA * P;
            wA -= iA * (MathUtils.Cross(m_RA, P) + m_AngularImpulse);
            vB += mB * P;
            wB += iB * (MathUtils.Cross(m_RB, P) + m_AngularImpulse);
            
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

            float mA = m_InvMassA, mB = m_InvMassB;
            float iA = m_InvIA, iB = m_InvIB;
            
            float h = Time.deltaTime;
            
            // Solve angular friction
            {
                float Cdot = wB - wA;
                float impulse = -m_AngularMass * Cdot;

                float oldImpulse = m_AngularImpulse;
                float maxImpulse = h * maxTorque;
                m_AngularImpulse = Mathf.Clamp(m_AngularImpulse + impulse, -maxImpulse, maxImpulse);
                impulse = m_AngularImpulse - oldImpulse;

                wA -= iA * impulse;
                wB += iB * impulse;
            }

            // Solve linear friction
            {
                Vector2 Cdot = vB + MathUtils.Cross(wB, m_RB) - vA - MathUtils.Cross(wA, m_RA);

                Vector2 impulse = -MathUtils.Mul(ref m_LinearMass, Cdot);
                Vector2 oldImpulse = m_LinearImpulse;
                m_LinearImpulse += impulse;

                float maxImpulse = h * maxForce;

                if (m_LinearImpulse.sqrMagnitude > maxImpulse * maxImpulse)
                {
                    m_LinearImpulse.Normalize();
                    m_LinearImpulse *= maxImpulse;
                }

                impulse = m_LinearImpulse - oldImpulse;

                vA -= mA * impulse;
                wA -= iA * MathUtils.Cross(m_RA, impulse);

                vB += mB * impulse;
                wB += iB * MathUtils.Cross(m_RB, impulse);
            }
            
            if (!_bodyA.isKinematic)
            {
                if (!_bodyA.fixedAngle)
                {
                    _bodyA.angularVelocity = wA * Mathf.Rad2Deg;
                }
            }
            if (!_bodyB.isKinematic)
            {
                if (!_bodyB.fixedAngle)
                {
                    _bodyB.angularVelocity = wB * Mathf.Rad2Deg;
                }
            }
        }

        internal override bool SolvePositionConstraints()
        {
            return true;
        }
    }
}
