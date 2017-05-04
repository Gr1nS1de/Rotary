//
// P2DFixedMouseJoint.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;

namespace Thinksquirrel.Phys2D.Internal.Joints
{
    // p = attached point, m = mouse point
    // C = p - m
    // Cdot = v
    //      = v + cross(w, r)
    // J = [I r_skew]
    // Identity used:
    // w k % (rx i + ry j) = w * (-ry i + rx j)

    sealed class P2DFixedMouseJoint : P2DJoint
    {
        Vector2 m_TargetA;
        float m_Frequency;
        float m_DampingRatio;
        float m_Beta;

        #region Solver shared
        Vector2 m_Impulse;
        internal float _maxForce;
        float m_Gamma;
        #endregion

        #region Solver temp
        Vector2 m_RB;
        Vector2 m_LocalCenterA;
        float m_InvMassA;
        float m_InvIA;
        Mat22 m_Mass;
        Vector2 m_C;
        #endregion

        internal P2DFixedMouseJoint(Rigidbody2D body, Vector2 worldAnchor)
            : base(body)
        {
            _jointType = JointType.FixedMouse;
            m_Frequency = 5.0f;
            m_DampingRatio = 0.7f;
            _maxForce = 1000 * body.mass;

            InitializeLocalAnchor(worldAnchor);
        }

        internal void InitializeLocalAnchor(Vector2 worldAnchor)
        {
            m_TargetA = worldAnchor;
            localAnchorB = MathUtils.MulT(_bodyA.transform, worldAnchor);
        }

        internal Vector2 localAnchorB { get; set; }

        internal override Vector2 worldAnchorA
        {
            get { return _bodyA.GetRelativePoint(localAnchorB); }
        }

        internal override Vector2 worldAnchorB
        {
            get { return m_TargetA; }
            set
            {
                if (m_TargetA != value)
                {
                    _bodyA.cachedRigidbody2D.WakeUp();
                    m_TargetA = value;
                }
            }
        }

        internal override Vector2 GetReactionForce(float invDt)
        {
            return invDt * m_Impulse;
        }

        internal override float GetReactionTorque(float invDt)
        {
            return 0.0f;
        }

        internal override void InitVelocityConstraints()
        {
            m_LocalCenterA = _bodyA.centerOfMass;
            m_InvMassA = _bodyA._invMass;
            m_InvIA = _bodyA._invI;

            Vector2 cA = _bodyA.transform.position;
            float aA = _bodyA.rotation * Mathf.Deg2Rad;
            Vector2 vA = _bodyA.velocity;
            float wA = _bodyA.angularVelocity * Mathf.Deg2Rad;

            Rot qB = new Rot(aA);

            float mass = _bodyA.mass;

            // Frequency
            float omega = 2.0f * Mathf.PI * m_Frequency;

            // Damping coefficient
            float d = 2.0f * mass * m_DampingRatio * omega;

            // Spring stiffness
            float k = mass * (omega * omega);

            // magic formulas
            // gamma has units of inverse mass.
            // beta has units of inverse time.
            float h = Time.deltaTime;

            m_Gamma = h * (d + h * k);
            if (m_Gamma != 0.0f)
            {
                m_Gamma = 1.0f / m_Gamma;
            }

            m_Beta = h * k * m_Gamma;

            // Compute the effective mass matrix.
            m_RB = MathUtils.Mul(qB, localAnchorB - m_LocalCenterA);
            // K    = [(1/m1 + 1/m2) * eye(2) - skew(r1) * invI1 * skew(r1) - skew(r2) * invI2 * skew(r2)]
            //      = [1/m1+1/m2     0    ] + invI1 * [r1.Y*r1.Y -r1.X*r1.Y] + invI2 * [r1.Y*r1.Y -r1.X*r1.Y]
            //        [    0     1/m1+1/m2]           [-r1.X*r1.Y r1.X*r1.X]           [-r1.X*r1.Y r1.X*r1.X]
            Mat22 K = new Mat22();
            K.ex.x = m_InvMassA + m_InvIA * m_RB.y * m_RB.y + m_Gamma;
            K.ex.y = -m_InvIA * m_RB.x * m_RB.y;
            K.ey.x = K.ex.y;
            K.ey.y = m_InvMassA + m_InvIA * m_RB.x * m_RB.x + m_Gamma;

            m_Mass = K.inverse;

            m_C = cA + m_RB - m_TargetA;
            m_C *= m_Beta;

            // Cheat with some damping
            wA *= 0.98f;

            // TODO: Scale impulse
            //m_Impulse *= data.step.dtRatio;
            vA += m_InvMassA * m_Impulse;
            wA += m_InvIA * MathUtils.Cross(m_RB, m_Impulse);

            if (!_bodyA.isKinematic)
            {
                _bodyA.velocity = vA;
                if (!_bodyA.fixedAngle)
                {
                    _bodyA.angularVelocity = wA * Mathf.Rad2Deg;
                }
            }
        }

        internal override void SolveVelocityConstraints()
        {
            Vector2 vA = _bodyA.velocity;
            float wA = _bodyA.angularVelocity * Mathf.Deg2Rad;

            // Cdot = v + cross(w, r)
            Vector2 Cdot = vA + MathUtils.Cross(wA, m_RB);
            Vector2 impulse = MathUtils.Mul(ref m_Mass, -(Cdot + m_C + m_Gamma * m_Impulse));

            Vector2 oldImpulse = m_Impulse;
            m_Impulse += impulse;
            float maxImpulse = Time.deltaTime * _maxForce;
            if (m_Impulse.SqrMagnitude() > maxImpulse * maxImpulse)
            {
                m_Impulse *= maxImpulse / m_Impulse.magnitude;
            }
            impulse = m_Impulse - oldImpulse;

            vA += m_InvMassA * impulse;
            wA += m_InvIA * MathUtils.Cross(m_RB, impulse);

            if (!_bodyA.isKinematic)
            {
                _bodyA.velocity = vA;
                if (!_bodyA.fixedAngle)
                {
                    _bodyA.angularVelocity = wA * Mathf.Rad2Deg;
                }
            }
        }

        internal override bool SolvePositionConstraints()
        {
            return true;
        }
    }
}
