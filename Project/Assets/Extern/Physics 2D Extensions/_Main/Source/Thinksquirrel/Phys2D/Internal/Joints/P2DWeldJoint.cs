//
// P2DWeldJoint.cs
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
    // C = p2 - p1
    // Cdot = v2 - v1
    //      = v2 + cross(w2, r2) - v1 - cross(w1, r1)
    // J = [-I -r1_skew I r2_skew ]
    // Identity used:
    // w k % (rx i + ry j) = w * (-ry i + rx j)

    // Angle constraint
    // C = angle2 - angle1 - referenceAngle
    // Cdot = w2 - w1
    // J = [0 0 -1 0 0 1]
    // K = invI1 + invI2

    /// <summary>
    /// A weld joint essentially glues two bodies together. A weld joint may
    /// distort somewhat because the island constraint solver is approximate.
    /// </summary>
    sealed class P2DWeldJoint : P2DJoint
    {
        public float frequency { get; set; }
        public float dampingRatio { get; set; }
        public float referenceAngle { get; private set; }

        #region Solver shared
        Vector3 m_Impulse;
        float m_Gamma;
        float m_Bias;
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
        Mat33 m_Mass;
        #endregion

        internal Vector2 _localAnchorA;
        internal Vector2 _localAnchorB;

        internal P2DWeldJoint()
        {
            _jointType = JointType.Weld;
        }

        /// <summary>
        /// You need to specify a local anchor point
        /// where they are attached and the relative body angle. The position
        /// of the anchor point is important for computing the reaction torque.
        /// You can change the anchor points relative to bodyA or bodyB by changing LocalAnchorA
        /// and/or LocalAnchorB.
        /// </summary>
        /// <param name="bodyA">The first body</param>
        /// <param name="bodyB">The second body</param>
        /// <param name="localAnchorA">The first body anchor.</param>
        /// <param name="localAnchorB">The second body anchor.</param>
        internal P2DWeldJoint(Rigidbody2DExt bodyA, Rigidbody2DExt bodyB, Vector2 localAnchorA, Vector2 localAnchorB)
            : base(bodyA, bodyB)
        {
            _jointType = JointType.Weld;

            this._localAnchorA = localAnchorA;
            this._localAnchorB = localAnchorB;
            referenceAngle = Mathf.DeltaAngle(bodyB.rotation, bodyA.rotation) * Mathf.Deg2Rad;
        }

        internal override Vector2 worldAnchorA
        {
            get { return _bodyA.GetRelativePoint(_localAnchorA); }
        }

        internal override Vector2 worldAnchorB
        {
            get { return _bodyB.GetRelativePoint(_localAnchorB); }
            set { Debug.LogError("You can't set the world anchor on this joint type."); }
        }


        internal override Vector2 GetReactionForce(float inv_dt)
        {
            return inv_dt * new Vector2(m_Impulse.x, m_Impulse.y);
        }

        internal override float GetReactionTorque(float inv_dt)
        {
            return inv_dt * m_Impulse.x;
        }

        internal override void InitVelocityConstraints()
        {
            m_LocalCenterA = _bodyA.centerOfMass;
            m_LocalCenterB = _bodyB.centerOfMass;
            m_InvMassA = _bodyA._invMass;
            m_InvMassB = _bodyB._invMass;
            m_InvIA = _bodyA._invI;
            m_InvIB = _bodyB._invI;

            float aA = _bodyA.rotation * Mathf.Deg2Rad;
            Vector2 vA = _bodyA.velocity;
            float wA = _bodyA.angularVelocity * Mathf.Deg2Rad;

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

            Mat33 K = new Mat33();
            K.ex.x = mA + mB + m_RA.y * m_RA.y * iA + m_RB.y * m_RB.y * iB;
            K.ey.x = -m_RA.y * m_RA.x * iA - m_RB.y * m_RB.x * iB;
            K.ez.x = -m_RA.y * iA - m_RB.y * iB;
            K.ex.y = K.ey.x;
            K.ey.y = mA + mB + m_RA.x * m_RA.x * iA + m_RB.x * m_RB.x * iB;
            K.ez.y = m_RA.x * iA + m_RB.x * iB;
            K.ex.z = K.ez.x;
            K.ey.z = K.ez.y;
            K.ez.z = iA + iB;

            if (frequency > 0.0f)
            {
                K.GetInverse22(ref m_Mass);

                float invM = iA + iB;
                float m = invM > 0.0f ? 1.0f / invM : 0.0f;

                float C = aB - aA - referenceAngle;

                // Frequency
                float omega = 2.0f * Mathf.PI * frequency;

                // Damping coefficient
                float d = 2.0f * m * dampingRatio * omega;

                // Spring stiffness
                float k = m * omega * omega;

                // magic formulas
                float h = Time.deltaTime;
                m_Gamma = h * (d + h * k);
                m_Gamma = m_Gamma != 0.0f ? 1.0f / m_Gamma : 0.0f;
                m_Bias = C * h * k * m_Gamma;

                invM += m_Gamma;
                m_Mass.ez.z = invM != 0.0f ? 1.0f / invM : 0.0f;
            }
            else
            {
                K.GetSymInverse33(ref m_Mass);
                m_Gamma = 0.0f;
                m_Bias = 0.0f;
            }

            // TODO: Scale impulses to support a variable time step.
            // m_Impulse *= data.step.dtRatio;

            Vector2 P = new Vector2(m_Impulse.x, m_Impulse.y);

            vA -= mA * P;
            wA -= iA * (MathUtils.Cross(m_RA, P) + m_Impulse.z);

            vB += mB * P;
            wB += iB * (MathUtils.Cross(m_RB, P) + m_Impulse.z);

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

            if (frequency > 0.0f)
            {
                float Cdot2 = wB - wA;

                float impulse2 = -m_Mass.ez.z * (Cdot2 + m_Bias + m_Gamma * m_Impulse.z);
                m_Impulse.z += impulse2;

                wA -= iA * impulse2;
                wB += iB * impulse2;

                Vector2 Cdot1 = vB + MathUtils.Cross(wB, m_RB) - vA - MathUtils.Cross(wA, m_RA);

                Vector2 impulse1 = -MathUtils.Mul22(m_Mass, Cdot1);
                m_Impulse.x += impulse1.x;
                m_Impulse.y += impulse1.y;

                Vector2 P = impulse1;

                vA -= mA * P;
                wA -= iA * MathUtils.Cross(m_RA, P);

                vB += mB * P;
                wB += iB * MathUtils.Cross(m_RB, P);
            }
            else
            {
                Vector2 Cdot1 = vB + MathUtils.Cross(wB, m_RB) - vA - MathUtils.Cross(wA, m_RA);
                float Cdot2 = wB - wA;
                Vector3 Cdot = new Vector3(Cdot1.x, Cdot1.y, Cdot2);

                Vector3 impulse = -MathUtils.Mul(m_Mass, Cdot);
                m_Impulse += impulse;

                Vector2 P = new Vector2(impulse.x, impulse.y);

                vA -= mA * P;
                wA -= iA * (MathUtils.Cross(m_RA, P) + impulse.z);

                vB += mB * P;
                wB += iB * (MathUtils.Cross(m_RB, P) + impulse.z);
            }

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

            float mA = m_InvMassA, mB = m_InvMassB;
            float iA = m_InvIA, iB = m_InvIB;

            Vector2 rA = MathUtils.Mul(qA, _localAnchorA - m_LocalCenterA);
            Vector2 rB = MathUtils.Mul(qB, _localAnchorB - m_LocalCenterB);

            float positionError, angularError;

            Mat33 K = new Mat33();
            K.ex.x = mA + mB + rA.y * rA.y * iA + rB.y * rB.y * iB;
            K.ey.x = -rA.y * rA.x * iA - rB.y * rB.x * iB;
            K.ez.x = -rA.y * iA - rB.y * iB;
            K.ex.y = K.ey.x;
            K.ey.y = mA + mB + rA.x * rA.x * iA + rB.x * rB.x * iB;
            K.ez.y = rA.x * iA + rB.x * iB;
            K.ex.z = K.ez.x;
            K.ey.z = K.ez.y;
            K.ez.z = iA + iB;

            if (frequency > 0.0f)
            {
                Vector2 C1 = cB + rB - cA - rA;

                positionError = C1.magnitude;
                angularError = 0.0f;

                Vector2 P = -K.Solve22(C1);

                cA -= mA * P;
                aA -= iA * MathUtils.Cross(rA, P);

                cB += mB * P;
                aB += iB * MathUtils.Cross(rB, P);
            }
            else
            {
                Vector2 C1 = cB + rB - cA - rA;
                float C2 = aB - aA - referenceAngle;

                positionError = C1.magnitude;
                angularError = Mathf.Abs(C2);

                Vector3 C = new Vector3(C1.x, C1.y, C2);

                Vector3 impulse = -K.Solve33(C);
                Vector2 P = new Vector2(impulse.x, impulse.y);

                cA -= mA * P;
                aA -= iA * (MathUtils.Cross(rA, P) + impulse.z);

                cB += mB * P;
                aB += iB * (MathUtils.Cross(rB, P) + impulse.z);
            }

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

            return positionError <= Constants.linearSlop && angularError <= Constants.angularSlop;
        }
    }
}
