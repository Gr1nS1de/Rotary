//
// P2DWheelJoint.cs
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
    // Linear constraint (point-to-line)
    // d = pB - pA = xB + rB - xA - rA
    // C = dot(ay, d)
    // Cdot = dot(d, cross(wA, ay)) + dot(ay, vB + cross(wB, rB) - vA - cross(wA, rA))
    //      = -dot(ay, vA) - dot(cross(d + rA, ay), wA) + dot(ay, vB) + dot(cross(rB, ay), vB)
    // J = [-ay, -cross(d + rA, ay), ay, cross(rB, ay)]

    // Spring linear constraint
    // C = dot(ax, d)
    // Cdot = = -dot(ax, vA) - dot(cross(d + rA, ax), wA) + dot(ax, vB) + dot(cross(rB, ax), vB)
    // J = [-ax -cross(d+rA, ax) ax cross(rB, ax)]

    // Motor rotational constraint
    // Cdot = wB - wA
    // J = [0 0 -1 0 0 1]

    /// <summary>
    /// A wheel joint. This joint provides two degrees of freedom: translation
    /// along an axis fixed in bodyA and rotation in the plane. You can use a
    /// joint limit to restrict the range of motion and a joint motor to drive
    /// the rotation or to model rotational friction.
    /// This joint is designed for vehicle suspensions.
    /// </summary>
    sealed class P2DWheelJoint : P2DJoint
    {
        #region Solver shared
        Vector2 m_LocalXaxisA;
        Vector2 m_LocalYaxisA;

        float m_Impulse;
        float m_MotorImpulse;
        float m_SpringImpulse;

        float m_MaxMotorTorque;
        float m_MotorSpeed;
        bool m_EnableMotor;
        #endregion

        #region Solver temp
        Vector2 m_LocalCenterA;
        Vector2 m_LocalCenterB;
        float m_InvMassA;
        float m_InvMassB;
        float m_InvIA;
        float m_InvIB;

        Vector2 m_Ax;
        Vector2 m_Ay;
        float m_SAx;
        float m_SBx;
        float m_SAy;
        float m_SBy;

        float m_Mass;
        float m_MotorMass;
        float m_SpringMass;

        float m_Bias;
        float m_Gamma;
        #endregion

        internal P2DWheelJoint()
        {
            _jointType = JointType.Wheel;
        }

        internal P2DWheelJoint(Rigidbody2DExt bodyA, Rigidbody2DExt bodyB, Vector2 localAnchorA, Vector2 localAnchorB, Vector2 axis)
            : base(bodyA, bodyB)
        {
            _jointType = JointType.Wheel;
            _localAnchorA = localAnchorA;
            _localAnchorB = localAnchorB;
            SetAxis(axis);
        }

        internal Vector2 _localAnchorA;
        internal Vector2 _localAnchorB;

        internal void SetAxis(Vector2 axis)
        {
            m_LocalXaxisA = _bodyA.GetVector(axis);
            m_LocalYaxisA = MathUtils.Cross(1.0f, m_LocalXaxisA);
        }

        internal override Vector2 worldAnchorA
        {
            get { return _bodyA.GetPoint(_localAnchorA); }
        }

        internal override Vector2 worldAnchorB
        {
            get { return _bodyB.GetPoint(_localAnchorB); }
            set { Debug.LogError("You can't set the world anchor on this joint type."); }
        }

        /// The desired motor speed in radians per second.
        internal float motorSpeed
        {
            get { return m_MotorSpeed; }
            set
            {
                if (m_MotorSpeed != value)
                {
                	WakeBodies();
                	m_MotorSpeed = value;
                }
            }
        }

        /// The maximum motor torque, usually in N-m.
        internal float maxMotorTorque
        {
            get { return m_MaxMotorTorque; }
            set
            {
                if (m_MaxMotorTorque != value)
                {
                	WakeBodies();
                	m_MaxMotorTorque = value;
                }
            }
        }

        /// Suspension frequency, zero indicates no suspension
        internal float _frequency;

        /// Suspension damping ratio, one indicates critical damping
        internal float _dampingRatio;

        internal override Vector2 GetReactionForce(float invDt)
        {
            return invDt * (m_Impulse * m_Ay + m_SpringImpulse * m_Ax);
        }

        internal override float GetReactionTorque(float invDt)
        {
            return invDt * m_MotorImpulse;
        }

        internal override void InitVelocityConstraints()
        {
            m_LocalCenterA = _bodyA.centerOfMass;
            m_LocalCenterB = _bodyB.centerOfMass;
            m_InvMassA = _bodyA._invMass;
            m_InvMassB = _bodyB._invMass;
            m_InvIA = _bodyA._invI;
            m_InvIB = _bodyB._invI;

            float mA = m_InvMassA, mB = m_InvMassB;
            float iA = m_InvIA, iB = m_InvIB;

            Vector2 cA = _bodyA.GetRelativePoint(m_LocalCenterA);
            float aA = _bodyA.rotation * Mathf.Deg2Rad;
            Vector2 vA = _bodyA.velocity;
            float wA = _bodyA.angularVelocity * Mathf.Deg2Rad;

            Vector2 cB = _bodyB.GetRelativePoint(m_LocalCenterB);
            float aB = _bodyB.rotation * Mathf.Deg2Rad;
            Vector2 vB = _bodyB.velocity;
            float wB = _bodyB.angularVelocity * Mathf.Deg2Rad;

            Rot qA = new Rot(aA), qB = new Rot(aB);

            // Compute the effective masses.
            Vector2 rA = MathUtils.Mul(qA, _localAnchorA - m_LocalCenterA);
            Vector2 rB = MathUtils.Mul(qB, _localAnchorB - m_LocalCenterB);
            Vector2 d1 = cB + rB - cA - rA;

            // Point to line constraint
            {
                m_Ay = MathUtils.Mul(qA, m_LocalYaxisA);
                m_SAy = MathUtils.Cross(d1 + rA, m_Ay);
                m_SBy = MathUtils.Cross(rB, m_Ay);

                m_Mass = mA + mB + iA * m_SAy * m_SAy + iB * m_SBy * m_SBy;

                if (m_Mass > 0.0f)
                {
                    m_Mass = 1.0f / m_Mass;
                }
            }

            // Spring constraint
            m_SpringMass = 0.0f;
            m_Bias = 0.0f;
            m_Gamma = 0.0f;
            if (_frequency > 0.0f)
            {
                m_Ax = MathUtils.Mul(qA, m_LocalXaxisA);
                m_SAx = MathUtils.Cross(d1 + rA, m_Ax);
                m_SBx = MathUtils.Cross(rB, m_Ax);

                float invMass = mA + mB + iA * m_SAx * m_SAx + iB * m_SBx * m_SBx;

                if (invMass > 0.0f)
                {
                    m_SpringMass = 1.0f / invMass;

                    float C = Vector2.Dot(d1, m_Ax);

                    // Frequency
                    float omega = 2.0f * Mathf.PI * _frequency;

                    // Damping coefficient
                    float d = 2.0f * m_SpringMass * _dampingRatio * omega;

                    // Spring stiffness
                    float k = m_SpringMass * omega * omega;

                    // magic formulas
                    float h = Time.deltaTime;
                    m_Gamma = h * (d + h * k);
                    if (m_Gamma > 0.0f)
                    {
                        m_Gamma = 1.0f / m_Gamma;
                    }

                    m_Bias = C * h * k * m_Gamma;

                    m_SpringMass = invMass + m_Gamma;
                    if (m_SpringMass > 0.0f)
                    {
                        m_SpringMass = 1.0f / m_SpringMass;
                    }
                }
            }
            else
            {
                m_SpringImpulse = 0.0f;
            }

            // Rotational motor
            if (m_EnableMotor)
            {
                m_MotorMass = iA + iB;
                if (m_MotorMass > 0.0f)
                {
                    m_MotorMass = 1.0f / m_MotorMass;
                }
            }
            else
            {
                m_MotorMass = 0.0f;
                m_MotorImpulse = 0.0f;
            }

            // TODO: Account for variable time step.
            // m_Impulse *= data.step.dtRatio;
            // m_SpringImpulse *= data.step.dtRatio;
            // m_MotorImpulse *= data.step.dtRatio;

            Vector2 P = m_Impulse * m_Ay + m_SpringImpulse * m_Ax;
            float LA = m_Impulse * m_SAy + m_SpringImpulse * m_SAx + m_MotorImpulse;
            float LB = m_Impulse * m_SBy + m_SpringImpulse * m_SBx + m_MotorImpulse;

            vA -= m_InvMassA * P;
            wA -= m_InvIA * LA;

            vB += m_InvMassB * P;
            wB += m_InvIB * LB;

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
            float mA = m_InvMassA, mB = m_InvMassB;
            float iA = m_InvIA, iB = m_InvIB;

            Vector2 vA = _bodyA.velocity;
            float wA = _bodyA.angularVelocity * Mathf.Deg2Rad;
            Vector2 vB = _bodyB.velocity;
            float wB = _bodyB.angularVelocity * Mathf.Deg2Rad;

            // Solve spring constraint
            {
                float Cdot = Vector2.Dot(m_Ax, vB - vA) + m_SBx * wB - m_SAx * wA;
                float impulse = -m_SpringMass * (Cdot + m_Bias + m_Gamma * m_SpringImpulse);
                m_SpringImpulse += impulse;

                Vector2 P = impulse * m_Ax;
                float LA = impulse * m_SAx;
                float LB = impulse * m_SBx;

                vA -= mA * P;
                wA -= iA * LA;

                vB += mB * P;
                wB += iB * LB;
            }

            // Solve rotational motor constraint
            {
                float Cdot = wB - wA - m_MotorSpeed;
                float impulse = -m_MotorMass * Cdot;

                float oldImpulse = m_MotorImpulse;
                float maxImpulse = Time.deltaTime * m_MaxMotorTorque;
                m_MotorImpulse = Mathf.Clamp(m_MotorImpulse + impulse, -maxImpulse, maxImpulse);
                impulse = m_MotorImpulse - oldImpulse;

                wA -= iA * impulse;
                wB += iB * impulse;
            }

            // Solve point to line constraint
            {
                float Cdot = Vector2.Dot(m_Ay, vB - vA) + m_SBy * wB - m_SAy * wA;
                float impulse = -m_Mass * Cdot;
                m_Impulse += impulse;

                Vector2 P = impulse * m_Ay;
                float LA = impulse * m_SAy;
                float LB = impulse * m_SBy;

                vA -= mA * P;
                wA -= iA * LA;

                vB += mB * P;
                wB += iB * LB;
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

            Vector2 rA = MathUtils.Mul(qA, _localAnchorA - m_LocalCenterA);
            Vector2 rB = MathUtils.Mul(qB, _localAnchorB - m_LocalCenterB);
            Vector2 d = (cB - cA) + rB - rA;

            Vector2 ay = MathUtils.Mul(qA, m_LocalYaxisA);

            float sAy = MathUtils.Cross(d + rA, ay);
            float sBy = MathUtils.Cross(rB, ay);

            float C = Vector2.Dot(d, ay);

            float k = m_InvMassA + m_InvMassB + m_InvIA * m_SAy * m_SAy + m_InvIB * m_SBy * m_SBy;

            float impulse;
            if (k != 0.0f)
            {
                impulse = -C / k;
            }
            else
            {
                impulse = 0.0f;
            }

            Vector2 P = impulse * ay;
            float LA = impulse * sAy;
            float LB = impulse * sBy;

            cA -= m_InvMassA * P;
            aA -= m_InvIA * LA;
            cB += m_InvMassB * P;
            aB += m_InvIB * LB;

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

            return Mathf.Abs(C) <= Constants.linearSlop;
        }

        internal float jointTranslation
        {
            get
            {
                var bA = _bodyA;
                var bB = _bodyB;

                Vector2 pA = bA.GetRelativePoint(_localAnchorA);
                Vector2 pB = bB.GetRelativePoint(_localAnchorB);
                Vector2 d = pB - pA;
                Vector2 axis = bA.GetRelativeVector(m_LocalXaxisA);

                float translation = Vector2.Dot(d, axis);
                return translation;
            }
        }

        internal float jointSpeed
        {
            get
            {
                float wA = _bodyA.angularVelocity * Mathf.Deg2Rad;
                float wB = _bodyB.angularVelocity * Mathf.Deg2Rad;
                return wB - wA;
            }
        }

        /// Enable/disable the joint motor.
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

        internal float GetMotorTorque(float invDt)
        {
            return invDt * m_MotorImpulse;
        }
    }
}
