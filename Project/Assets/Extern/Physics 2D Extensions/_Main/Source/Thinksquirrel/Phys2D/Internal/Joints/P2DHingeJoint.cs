//
// P2DHingeJoint.cs
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
    sealed class P2DHingeJoint : P2DJoint
    {
        public JointLimitState2D limitState { get; private set; }
        public float referenceAngle { get; internal set; }

        #region Solver shared
        Vector3 m_Impulse;
        float m_MotorImpulse;
        bool m_EnableMotor;
        float m_MaxMotorTorque;
        float m_MotorSpeed;
        bool m_EnableLimit;
        float m_LowerAngle;
        float m_UpperAngle;
        #endregion

        #region Solver temp
        Vector2 m_LocalCenterA;
        Vector2 m_LocalCenterB;
        float m_InvMassA;
        float m_InvMassB;
        float m_InvIA;
        float m_InvIB;
        Vector2 m_RA;
        Vector2 m_RB;
        Mat33 m_Mass;
        float m_MotorMass;
        #endregion

        internal P2DHingeJoint()
        {
            _jointType = JointType.Hinge;
        }

        internal P2DHingeJoint(Rigidbody2DExt bodyA, Rigidbody2DExt bodyB, Vector2 localAnchorA, Vector2 localAnchorB)
            : base(bodyA, bodyB)
        {
            _jointType = JointType.Hinge;
            _localAnchorA = localAnchorA;
            _localAnchorB = localAnchorB;
            referenceAngle = Mathf.DeltaAngle(bodyB.rotation, bodyA.rotation) * Mathf.Deg2Rad;
            m_Impulse = Vector3.zero;
            limitState = JointLimitState2D.Inactive;
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

        internal bool limitEnabled
        {
            get { return m_EnableLimit; }
            set
            {
                if (m_EnableLimit != value)
                {
                    WakeBodies();
                    m_EnableLimit = value;
                    m_Impulse.z = 0.0f;
                }
            }
        }

        internal void SetLimits(float lower, float upper)
        {
            if (lower != m_LowerAngle || upper != m_UpperAngle)
            {
                WakeBodies();
                m_UpperAngle = upper;
                m_LowerAngle = lower;
                m_Impulse.z = 0.0f;
            }
        }

        internal float jointAngle
        {
            get { return _bodyB.rotation - _bodyA.rotation - referenceAngle; }
        }

        internal float jointSpeed
        {
            get { return _bodyB.angularVelocity - _bodyA.angularVelocity; }
        }

        internal bool motorEnabled
        {
            get { return m_EnableMotor; }
            set
            {
                if (m_EnableMotor != value)
                {
                    WakeBodies();
                    m_EnableMotor = value;
                }
            }
        }

        internal float motorSpeed
        {
            set
            {
                if (m_MotorSpeed != value)
                {
                    WakeBodies();
                    m_MotorSpeed = value;
                }
            }
            get { return m_MotorSpeed; }
        }

        internal float maxMotorTorque
        {
            set
            {
                if (m_MaxMotorTorque != value)
                {
                    WakeBodies();
                    m_MaxMotorTorque = value;
                }
            }
            get { return m_MaxMotorTorque; }
        }

        internal float GetMotorTorque(float invDt)
        {
            return invDt * m_MotorImpulse;
        }

        internal override Vector2 GetReactionForce(float invDt)
        {
            Vector2 P = new Vector2(m_Impulse.x, m_Impulse.y);
            return invDt * P;
        }

        internal override float GetReactionTorque(float invDt)
        {
            return invDt * m_Impulse.z;
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

            bool fixedRotation = (iA + iB == 0.0f);

            m_Mass.ex.x = mA + mB + m_RA.y * m_RA.y * iA + m_RB.y * m_RB.y * iB;
            m_Mass.ey.x = -m_RA.y * m_RA.x * iA - m_RB.y * m_RB.x * iB;
            m_Mass.ez.x = -m_RA.y * iA - m_RB.y * iB;
            m_Mass.ex.y = m_Mass.ey.x;
            m_Mass.ey.y = mA + mB + m_RA.x * m_RA.x * iA + m_RB.x * m_RB.x * iB;
            m_Mass.ez.y = m_RA.x * iA + m_RB.x * iB;
            m_Mass.ex.z = m_Mass.ez.x;
            m_Mass.ey.z = m_Mass.ez.y;
            m_Mass.ez.z = iA + iB;

            m_MotorMass = iA + iB;

            if (m_MotorMass > 0.0f)
            {
                m_MotorMass = 1.0f / m_MotorMass;
            }

            if (m_EnableMotor == false || fixedRotation)
            {
                m_MotorImpulse = 0.0f;
            }
            if (m_EnableLimit && fixedRotation == false)
            {
                float jointAngle = aB - aA - referenceAngle;
                if (Mathf.Abs(m_UpperAngle - m_LowerAngle) < 2.0f * Constants.angularSlop)
                {
                    limitState = JointLimitState2D.EqualLimits;
                }
                else if (jointAngle <= m_LowerAngle)
                {
                    if (limitState != JointLimitState2D.LowerLimit)
                    {
                        m_Impulse.z = 0.0f;
                    }
                    limitState = JointLimitState2D.LowerLimit;
                }
                else if (jointAngle >= m_UpperAngle)
                {
                    if (limitState != JointLimitState2D.UpperLimit)
                    {
                        m_Impulse.z = 0.0f;
                    }
                    limitState = JointLimitState2D.UpperLimit;
                }
                else
                {
                    limitState = JointLimitState2D.Inactive;
                    m_Impulse.z = 0.0f;
                }
            }
            else
            {
                limitState = JointLimitState2D.Inactive;
            }

            // TODO: Scale impulses to support a variable time step.
            // m_Impulse *= data.step.dtRatio;
            // m_MotorImpulse *= data.step.dtRatio;

            Vector2 P = new Vector2(m_Impulse.x, m_Impulse.y);

            vA -= mA * P;
            wA -= iA * (MathUtils.Cross(m_RA, P) + m_MotorImpulse + m_Impulse.z);

            vB += mB * P;
            wB += iB * (MathUtils.Cross(m_RB, P) + m_MotorImpulse + m_Impulse.z);

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

            bool fixedRotation = (iA + iB == 0.0f);

            // Solve motor constraint.
            if (m_EnableMotor && limitState != JointLimitState2D.EqualLimits && fixedRotation == false)
            {
                float Cdot = wB - wA - m_MotorSpeed;
                float impulse = m_MotorMass * (-Cdot);
                float oldImpulse = m_MotorImpulse;
                float maxImpulse = Time.deltaTime * m_MaxMotorTorque;
                m_MotorImpulse = Mathf.Clamp(m_MotorImpulse + impulse, -maxImpulse, maxImpulse);
                impulse = m_MotorImpulse - oldImpulse;

                wA -= iA * impulse;
                wB += iB * impulse;
            }

            // Solve limit constraint.
            if (m_EnableLimit && limitState != JointLimitState2D.Inactive && fixedRotation == false)
            {
                Vector2 Cdot1 = vB + MathUtils.Cross(wB, m_RB) - vA - MathUtils.Cross(wA, m_RA);
                float Cdot2 = wB - wA;
                Vector3 Cdot = new Vector3(Cdot1.x, Cdot1.y, Cdot2);

                Vector3 impulse = -m_Mass.Solve33(Cdot);

                if (limitState == JointLimitState2D.EqualLimits)
                {
                    m_Impulse += impulse;
                }
                else if (limitState == JointLimitState2D.LowerLimit)
                {
                    float newImpulse = m_Impulse.z + impulse.z;
                    if (newImpulse < 0.0f)
                    {
                        Vector2 rhs = -Cdot1 + m_Impulse.z * new Vector2(m_Mass.ez.x, m_Mass.ez.y);
                        Vector2 reduced = m_Mass.Solve22(rhs);
                        impulse.x = reduced.x;
                        impulse.y = reduced.y;
                        impulse.z = -m_Impulse.z;
                        m_Impulse.x += reduced.x;
                        m_Impulse.y += reduced.y;
                        m_Impulse.z = 0.0f;
                    }
                    else
                    {
                        m_Impulse += impulse;
                    }
                }
                else if (limitState == JointLimitState2D.UpperLimit)
                {
                    float newImpulse = m_Impulse.z + impulse.z;
                    if (newImpulse > 0.0f)
                    {
                        Vector2 rhs = -Cdot1 + m_Impulse.z * new Vector2(m_Mass.ez.x, m_Mass.ez.y);
                        Vector2 reduced = m_Mass.Solve22(rhs);
                        impulse.x = reduced.x;
                        impulse.y = reduced.y;
                        impulse.z = -m_Impulse.z;
                        m_Impulse.x += reduced.x;
                        m_Impulse.y += reduced.y;
                        m_Impulse.z = 0.0f;
                    }
                    else
                    {
                        m_Impulse += impulse;
                    }
                }

                Vector2 P = new Vector2(impulse.x, impulse.y);

                vA -= mA * P;
                wA -= iA * (MathUtils.Cross(m_RA, P) + impulse.z);

                vB += mB * P;
                wB += iB * (MathUtils.Cross(m_RB, P) + impulse.z);
            }
            else
            {
                // Solve point-to-point constraint
                Vector2 Cdot = vB + MathUtils.Cross(wB, m_RB) - vA - MathUtils.Cross(wA, m_RA);
                Vector2 impulse = m_Mass.Solve22(-Cdot);

                m_Impulse.x += impulse.x;
                m_Impulse.y += impulse.y;

                vA -= mA * impulse;
                wA -= iA * MathUtils.Cross(m_RA, impulse);

                vB += mB * impulse;
                wB += iB * MathUtils.Cross(m_RB, impulse);
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

            float angularError = 0.0f;
            float positionError;

            bool fixedRotation = (m_InvIA + m_InvIB == 0.0f);

            // Solve angular limit constraint.
            if (m_EnableLimit && limitState != JointLimitState2D.Inactive && fixedRotation == false)
            {
                float angle = aB - aA - referenceAngle;
                float limitImpulse = 0.0f;

                if (limitState == JointLimitState2D.EqualLimits)
                {
                    // Prevent large angular corrections
                    float C = Mathf.Clamp(angle - m_LowerAngle, -Constants.maxAngularCorrection, Constants.maxAngularCorrection);
                    limitImpulse = -m_MotorMass * C;
                    angularError = Mathf.Abs(C);
                }
                else if (limitState == JointLimitState2D.LowerLimit)
                {
                    float C = angle - m_LowerAngle;
                    angularError = -C;

                    // Prevent large angular corrections and allow some slop.
                    C = Mathf.Clamp(C + Constants.angularSlop, -Constants.maxAngularCorrection, 0.0f);
                    limitImpulse = -m_MotorMass * C;
                }
                else if (limitState == JointLimitState2D.UpperLimit)
                {
                    float C = angle - m_UpperAngle;
                    angularError = C;

                    // Prevent large angular corrections and allow some slop.
                    C = Mathf.Clamp(C - Constants.angularSlop, 0.0f, Constants.maxAngularCorrection);
                    limitImpulse = -m_MotorMass * C;
                }

                aA -= m_InvIA * limitImpulse;
                aB += m_InvIB * limitImpulse;
            }

            // Solve point-to-point constraint.
            {
                qA.Set(aA);
                qB.Set(aB);
                Vector2 rA = MathUtils.Mul(qA, _localAnchorA - m_LocalCenterA);
                Vector2 rB = MathUtils.Mul(qB, _localAnchorB - m_LocalCenterB);

                Vector2 C = cB + rB - cA - rA;
                positionError = C.magnitude;

                float mA = m_InvMassA, mB = m_InvMassB;
                float iA = m_InvIA, iB = m_InvIB;

                Mat22 K = new Mat22();
                K.ex.x = mA + mB + iA * rA.y * rA.y + iB * rB.y * rB.y;
                K.ex.y = -iA * rA.x * rA.y - iB * rB.x * rB.y;
                K.ey.x = K.ex.y;
                K.ey.y = mA + mB + iA * rA.x * rA.x + iB * rB.x * rB.x;

                Vector2 impulse = -K.Solve(C);

                cA -= mA * impulse;
                aA -= iA * MathUtils.Cross(rA, impulse);

                cB += mB * impulse;
                aB += iB * MathUtils.Cross(rB, impulse);
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
