//
// P2DSliderJoint.cs
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
    // d = p2 - p1 = x2 + r2 - x1 - r1
    // C = dot(perp, d)
    // Cdot = dot(d, cross(w1, perp)) + dot(perp, v2 + cross(w2, r2) - v1 - cross(w1, r1))
    //      = -dot(perp, v1) - dot(cross(d + r1, perp), w1) + dot(perp, v2) + dot(cross(r2, perp), v2)
    // J = [-perp, -cross(d + r1, perp), perp, cross(r2,perp)]
    //
    // Angular constraint
    // C = a2 - a1 + a_initial
    // Cdot = w2 - w1
    // J = [0 0 -1 0 0 1]
    //
    // K = J * invM * JT
    //
    // J = [-a -s1 a s2]
    //     [0  -1  0  1]
    // a = perp
    // s1 = cross(d + r1, a) = cross(p2 - x1, a)
    // s2 = cross(r2, a) = cross(p2 - x2, a)
    // Motor/Limit linear constraint
    // C = dot(ax1, d)
    // Cdot = = -dot(ax1, v1) - dot(cross(d + r1, ax1), w1) + dot(ax1, v2) + dot(cross(r2, ax1), v2)
    // J = [-ax1 -cross(d+r1,ax1) ax1 cross(r2,ax1)]
    // Block Solver
    // We develop a block solver that includes the joint limit. This makes the limit stiff (inelastic) even
    // when the mass has poor distribution (leading to large torques about the joint anchor points).
    //
    // The Jacobian has 3 rows:
    // J = [-uT -s1 uT s2] // linear
    //     [0   -1   0  1] // angular
    //     [-vT -a1 vT a2] // limit
    //
    // u = perp
    // v = axis
    // s1 = cross(d + r1, u), s2 = cross(r2, u)
    // a1 = cross(d + r1, v), a2 = cross(r2, v)
    // M * (v2 - v1) = JT * df
    // J * v2 = bias
    //
    // v2 = v1 + invM * JT * df
    // J * (v1 + invM * JT * df) = bias
    // K * df = bias - J * v1 = -Cdot
    // K = J * invM * JT
    // Cdot = J * v1 - bias
    //
    // Now solve for f2.
    // df = f2 - f1
    // K * (f2 - f1) = -Cdot
    // f2 = invK * (-Cdot) + f1
    //
    // Clamp accumulated limit impulse.
    // lower: f2(3) = max(f2(3), 0)
    // upper: f2(3) = min(f2(3), 0)
    //
    // Solve for correct f2(1:2)
    // K(1:2, 1:2) * f2(1:2) = -Cdot(1:2) - K(1:2,3) * f2(3) + K(1:2,1:3) * f1
    //                       = -Cdot(1:2) - K(1:2,3) * f2(3) + K(1:2,1:2) * f1(1:2) + K(1:2,3) * f1(3)
    // K(1:2, 1:2) * f2(1:2) = -Cdot(1:2) - K(1:2,3) * (f2(3) - f1(3)) + K(1:2,1:2) * f1(1:2)
    // f2(1:2) = invK(1:2,1:2) * (-Cdot(1:2) - K(1:2,3) * (f2(3) - f1(3))) + f1(1:2)
    //
    // Now compute impulse to be applied:
    // df = f2 - f1

    sealed class P2DSliderJoint : P2DJoint
    {
        public bool limitEnabled
        {
            get
            {
                return m_EnableLimit;
            }
            set
            {
                if (value != m_EnableLimit)
                {
                    WakeBodies();
                    m_EnableLimit = value;
                    m_Impulse.z = 0;
                }
            }
        }
        public void SetLimits(float lower, float upper)
        {
            if (upper != m_UpperTranslation || lower != m_LowerTranslation)
            {
                WakeBodies();
                m_UpperTranslation = upper;
                m_LowerTranslation = lower;
                m_Impulse.z = 0;
            }
        }
        public bool motorEnabled
        {
            get
            {
                return m_EnableMotor;
            }
            set
            {
                if (m_EnableMotor != value)
                {
                    WakeBodies();
                    m_EnableMotor = value;
                }
            }
        }
        public float motorSpeed
        {
            get
            {
                return m_MotorSpeed;
            }
            set
            {
                if (m_MotorSpeed != value)
                {
                    WakeBodies();
                    m_MotorSpeed = value;
                }
            }
        }
        public float maxMotorForce
        {
            get
            {
                return m_MaxMotorForce;
            }
            set
            {
                if (maxMotorForce != value)
                {
                    WakeBodies();
                    m_MaxMotorForce = value;
                }
            }
        }
        public float jointSpeed
        {
            get { return m_MotorImpulse; }
        }

        public void SetAxis(Vector2 axis)
        {
            localXAxisA = _bodyA.GetVector(axis);
            m_LocalYAxisA = MathUtils.Cross(1.0f, localXAxisA);
        }

        public float referenceAngle { get; set; }
        public Vector2 localXAxisA { get; set; }
        public JointLimitState2D limitState { get { return m_LimitState; } }
        public float jointTranslation { get { return m_Translation; } }

        #region Solver shared
        Vector2 m_LocalYAxisA;
        Vector3 m_Impulse;
        float m_MotorImpulse;
        float m_LowerTranslation;
        float m_UpperTranslation;
        float m_Translation;
        float m_MaxMotorForce;
        float m_MotorSpeed;
        bool m_EnableLimit;
        bool m_EnableMotor;
        JointLimitState2D m_LimitState;
        #endregion

        #region Solver temp
        Vector2 m_LocalCenterA;
        Vector2 m_LocalCenterB;
        float m_InvMassA;
        float m_InvMassB;
        float m_InvIA;
        float m_InvIB;
        Vector2 m_Axis;
        Vector2 m_Perp;
        float m_S1;
        float m_S2;
        float m_A1;
        float m_A2;
        Mat33 m_K;
        float m_MotorMass;
        #endregion

        internal P2DSliderJoint()
        {
            _jointType = JointType.Slider;
        }

        internal P2DSliderJoint(Rigidbody2DExt bodyA, Rigidbody2DExt bodyB, Vector2 localAnchorA, Vector2 localAnchorB, Vector2 axis)
            : base(bodyA, bodyB)
        {
            _jointType = JointType.Slider;

            _localAnchorA = localAnchorA;
            _localAnchorB = localAnchorB;

            SetAxis(axis);

            m_LocalYAxisA = MathUtils.Cross(1.0f, localXAxisA);
            referenceAngle = -Mathf.DeltaAngle(bodyB.rotation, bodyA.rotation) * Mathf.Deg2Rad;

            m_LimitState = JointLimitState2D.Inactive;
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

        internal float GetMotorForce(float invDt)
        {
            return invDt * m_MotorImpulse;
        }
        internal override Vector2 GetReactionForce(float invDt)
        {
            return invDt * (m_Impulse.x * m_Perp + (m_MotorImpulse + m_Impulse.z) * m_Axis);
        }

        internal override float GetReactionTorque(float invDt)
        {
            return invDt * m_Impulse.y;
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

            Vector2 rA = MathUtils.Mul(qA, _localAnchorA - m_LocalCenterA);
            Vector2 rB = MathUtils.Mul(qB, _localAnchorB - m_LocalCenterB);

            float mA = m_InvMassA, mB = m_InvMassB;
            float iA = m_InvIA, iB = m_InvIB;

            Vector2 d = (cB - cA) + rB - rA;

            // Compute motor Jacobian and effective mass.
            {
                m_Axis = MathUtils.Mul(qA, localXAxisA);
                m_A1 = MathUtils.Cross(d + rA, m_Axis);
                m_A2 = MathUtils.Cross(rB, m_Axis);

                m_MotorMass = mA + mB + iA * m_A1 * m_A1 + iB * m_A2 * m_A2;
                if (m_MotorMass > 0.0f)
                {
                    m_MotorMass = 1.0f / m_MotorMass;
                }
            }

            // Prismatic constraint.
            {
                m_Perp = MathUtils.Mul(qA, m_LocalYAxisA);

                m_S1 = MathUtils.Cross(d + rA, m_Perp);
                m_S2 = MathUtils.Cross(rB, m_Perp);

                float k11 = mA + mB + iA * m_S1 * m_S1 + iB * m_S2 * m_S2;
                float k12 = iA * m_S1 + iB * m_S2;
                float k13 = iA * m_S1 * m_A1 + iB * m_S2 * m_A2;
                float k22 = iA + iB;
                if (k22 == 0.0f)
                {
                    // For bodies with fixed rotation.
                    k22 = 1.0f;
                }
                float k23 = iA * m_A1 + iB * m_A2;
                float k33 = mA + mB + iA * m_A1 * m_A1 + iB * m_A2 * m_A2;

                m_K.ex = new Vector3(k11, k12, k13);
                m_K.ey = new Vector3(k12, k22, k23);
                m_K.ez = new Vector3(k13, k23, k33);
            }

            // Get joint translation
            m_Translation = Vector2.Dot(m_Axis, d);

            // Compute motor and limit terms.
            if (m_EnableLimit)
            {
                float jointTranslation = m_Translation;
                if (Mathf.Abs(m_UpperTranslation - m_LowerTranslation) < 2.0f * Constants.linearSlop)
                {
                    m_LimitState = JointLimitState2D.EqualLimits;
                }
                else if (jointTranslation <= m_LowerTranslation)
                {
                    if (m_LimitState != JointLimitState2D.LowerLimit)
                    {
                        m_LimitState = JointLimitState2D.LowerLimit;
                        m_Impulse.z = 0.0f;
                    }
                }
                else if (jointTranslation >= m_UpperTranslation)
                {
                    if (m_LimitState != JointLimitState2D.UpperLimit)
                    {
                        m_LimitState = JointLimitState2D.UpperLimit;
                        m_Impulse.z = 0.0f;
                    }
                }
                else
                {
                    m_LimitState = JointLimitState2D.Inactive;
                    m_Impulse.z = 0.0f;
                }
            }
            else
            {
                m_LimitState = JointLimitState2D.Inactive;
            }

            if (m_EnableMotor == false)
            {
                m_MotorImpulse = 0.0f;
            }

            // TODO: Account for variable time step.
            // m_Impulse *= data.step.dtRatio;
            // m_MotorImpulse *= data.step.dtRatio;

            Vector2 P = m_Impulse.x * m_Perp + (m_MotorImpulse + m_Impulse.z) * m_Axis;
            float LA = m_Impulse.x * m_S1 + m_Impulse.y + (m_MotorImpulse + m_Impulse.z) * m_A1;
            float LB = m_Impulse.x * m_S2 + m_Impulse.y + (m_MotorImpulse + m_Impulse.z) * m_A2;

            vA -= mA * P;
            wA -= iA * LA;

            vB += mB * P;
            wB += iB * LB;

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

            // Solve linear motor constraint.
            if (m_EnableMotor && m_LimitState != JointLimitState2D.EqualLimits)
            {
                float Cdot = Vector2.Dot(m_Axis, vB - vA) + m_A2 * wB - m_A1 * wA;
                float impulse = m_MotorMass * (m_MotorSpeed - Cdot);
                float oldImpulse = m_MotorImpulse;
                float maxImpulse = Time.deltaTime * m_MaxMotorForce;
                m_MotorImpulse = Mathf.Clamp(m_MotorImpulse + impulse, -maxImpulse, maxImpulse);
                impulse = m_MotorImpulse - oldImpulse;

                Vector2 P = impulse * m_Axis;
                float LA = impulse * m_A1;
                float LB = impulse * m_A2;

                vA -= mA * P;
                wA -= iA * LA;

                vB += mB * P;
                wB += iB * LB;
            }

            Vector2 Cdot1 = new Vector2();
            Cdot1.x = Vector2.Dot(m_Perp, vB - vA) + m_S2 * wB - m_S1 * wA;
            Cdot1.y = wB - wA;

            if (m_EnableLimit && m_LimitState != JointLimitState2D.Inactive)
            {
                // Solve slider and limit constraint in block form.
                float Cdot2;
                Cdot2 = Vector2.Dot(m_Axis, vB - vA) + m_A2 * wB - m_A1 * wA;
                Vector3 Cdot = new Vector3(Cdot1.x, Cdot1.y, Cdot2);

                Vector3 f1 = m_Impulse;
                Vector3 df = m_K.Solve33(-Cdot);
                m_Impulse += df;

                if (m_LimitState == JointLimitState2D.LowerLimit)
                {
                    m_Impulse.z = Mathf.Max(m_Impulse.z, 0.0f);
                }
                else if (m_LimitState == JointLimitState2D.UpperLimit)
                {
                    m_Impulse.z = Mathf.Min(m_Impulse.z, 0.0f);
                }

                // f2(1:2) = invK(1:2,1:2) * (-Cdot(1:2) - K(1:2,3) * (f2(3) - f1(3))) + f1(1:2)
                Vector2 b = -Cdot1 - (m_Impulse.z - f1.z) * new Vector2(m_K.ez.x, m_K.ez.y);
                Vector2 f2r = m_K.Solve22(b) + new Vector2(f1.x, f1.y);
                m_Impulse.x = f2r.x;
                m_Impulse.y = f2r.y;

                df = m_Impulse - f1;

                Vector2 P = df.x * m_Perp + df.z * m_Axis;
                float LA = df.x * m_S1 + df.y + df.z * m_A1;
                float LB = df.x * m_S2 + df.y + df.z * m_A2;

                vA -= mA * P;
                wA -= iA * LA;

                vB += mB * P;
                wB += iB * LB;
            }
            else
            {
                // Limit is inactive, just solve the slider constraint in block form.
                Vector2 df = m_K.Solve22(-Cdot1);
                m_Impulse.x += df.x;
                m_Impulse.y += df.y;

                Vector2 P = df.x * m_Perp;
                float LA = df.x * m_S1 + df.y;
                float LB = df.x * m_S2 + df.y;

                vA -= mA * P;
                wA -= iA * LA;

                vB += mB * P;
                wB += iB * LB;

                //FVector2 Cdot10 = Cdot1;

                Cdot1.x = Vector2.Dot(m_Perp, vB - vA) + m_S2 * wB - m_S1 * wA;
                Cdot1.y = wB - wA;

                if (Mathf.Abs(Cdot1.x) > 0.01f || Mathf.Abs(Cdot1.y) > 0.01f)
                {
                    //FVector2 test = MathUtils.Mul22(m_K, df);
                    Cdot1.x += 0.0f;
                }
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

            // Compute fresh Jacobians
            Vector2 rA = MathUtils.Mul(qA, _localAnchorA - m_LocalCenterA);
            Vector2 rB = MathUtils.Mul(qB, _localAnchorB - m_LocalCenterB);
            Vector2 d = cB + rB - cA - rA;

            Vector2 axis = MathUtils.Mul(qA, localXAxisA);
            float a1 = MathUtils.Cross(d + rA, axis);
            float a2 = MathUtils.Cross(rB, axis);
            Vector2 perp = MathUtils.Mul(qA, m_LocalYAxisA);

            float s1 = MathUtils.Cross(d + rA, perp);
            float s2 = MathUtils.Cross(rB, perp);

            Vector3 impulse;
            Vector2 C1 = new Vector2();
            C1.x = Vector2.Dot(perp, d);
            C1.y = aB - aA - referenceAngle;

            float linearError = Mathf.Abs(C1.x);
            float angularError = Mathf.Abs(C1.y);

            bool active = false;
            float C2 = 0.0f;
            if (m_EnableLimit)
            {
                float translation = Vector2.Dot(axis, d);
                m_Translation = translation;

                if (Mathf.Abs(m_UpperTranslation - m_LowerTranslation) < 2.0f * Constants.linearSlop)
                {
                    // Prevent large angular corrections
                    C2 = Mathf.Clamp(translation, -Constants.maxLinearCorrection, Constants.maxLinearCorrection);
                    linearError = Mathf.Max(linearError, Mathf.Abs(translation));
                    active = true;
                }
                else if (translation <= m_LowerTranslation)
                {
                    // Prevent large linear corrections and allow some slop.
                    C2 = Mathf.Clamp(translation - m_LowerTranslation + Constants.linearSlop,
                                         -Constants.maxLinearCorrection, 0.0f);
                    linearError = Mathf.Max(linearError, m_LowerTranslation - translation);
                    active = true;
                }
                else if (translation >= m_UpperTranslation)
                {
                    // Prevent large linear corrections and allow some slop.
                    C2 = Mathf.Clamp(translation - m_UpperTranslation - Constants.linearSlop, 0.0f,
                                         Constants.maxLinearCorrection);
                    linearError = Mathf.Max(linearError, translation - m_UpperTranslation);
                    active = true;
                }
            }

            if (active)
            {
                float k11 = mA + mB + iA * s1 * s1 + iB * s2 * s2;
                float k12 = iA * s1 + iB * s2;
                float k13 = iA * s1 * a1 + iB * s2 * a2;
                float k22 = iA + iB;
                if (k22 == 0.0f)
                {
                    // For fixed rotation
                    k22 = 1.0f;
                }
                float k23 = iA * a1 + iB * a2;
                float k33 = mA + mB + iA * a1 * a1 + iB * a2 * a2;

                Mat33 K = new Mat33();
                K.ex = new Vector3(k11, k12, k13);
                K.ey = new Vector3(k12, k22, k23);
                K.ez = new Vector3(k13, k23, k33);

                Vector3 C = new Vector3();
                C.x = C1.x;
                C.y = C1.y;
                C.z = C2;

                impulse = K.Solve33(-C);
            }
            else
            {
                float k11 = mA + mB + iA * s1 * s1 + iB * s2 * s2;
                float k12 = iA * s1 + iB * s2;
                float k22 = iA + iB;
                if (k22 == 0.0f)
                {
                    k22 = 1.0f;
                }

                Mat22 K = new Mat22();
                K.ex = new Vector2(k11, k12);
                K.ey = new Vector2(k12, k22);

                Vector2 impulse1 = K.Solve(-C1);
                impulse = new Vector3();
                impulse.x = impulse1.x;
                impulse.y = impulse1.y;
                impulse.z = 0.0f;
            }

            Vector2 P = impulse.x * perp + impulse.z * axis;
            float LA = impulse.x * s1 + impulse.y + impulse.z * a1;
            float LB = impulse.x * s2 + impulse.y + impulse.z * a2;

            cA -= mA * P;
            aA -= iA * LA;
            cB += mB * P;
            aB += iB * LB;

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
            return linearError < Constants.linearSlop && angularError < Constants.angularSlop;
        }
    }
}
