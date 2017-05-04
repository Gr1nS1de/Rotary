//
// P2DGearJoint.cs
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
    // Gear Joint:
    // C0 = (coordinate1 + ratio * coordinate2)_initial
    // C = (coordinate1 + ratio * coordinate2) - C0 = 0
    // J = [J1 ratio * J2]
    // K = J * invM * JT
    //   = J1 * invM1 * J1T + ratio * ratio * J2 * invM2 * J2T
    //
    // Hinge:
    // coordinate = rotation
    // Cdot = angularVelocity
    // J = [0 0 1]
    // K = J * invM * JT = invI
    //
    // Slider:
    // coordinate = dot(p - pg, ug)
    // Cdot = dot(v + cross(w, r), ug)
    // J = [ug cross(r, ug)]
    // K = J * invM * JT = invMass + invI * cross(r, ug)^2

    /// <summary>
    /// A gear joint is used to connect two joints together. Either joint
    /// can be a hinge or slider joint. You specify a gear ratio
    /// to bind the motions together:
    /// coordinate1 + ratio * coordinate2 = ant
    /// The ratio can be negative or positive. If one joint is a hinge joint
    /// and the other joint is a slider joint, then the ratio will have units
    /// of length or units of 1/length.
    /// @warning You have to manually destroy the gear joint if joint1 or joint2
    /// is destroyed.
    /// </summary>
    sealed class P2DGearJoint : P2DJoint
    {
        public Joint2DExt jointA { get; set; }
        public Joint2DExt jointB { get; set; }
        public float ratio { get; set; }

        internal Rigidbody2DExt _bodyC;
        internal Rigidbody2DExt _bodyD;

        #region Solver shared
        Vector2 m_LocalAxisC;
        Vector2 m_LocalAxisD;
        float m_Constant;
        float m_Impulse;
        #endregion

        #region Solver temp
        Vector2 m_LocalCenterA;
        Vector2 m_LocalCenterB;
        Vector2 m_LocalCenterC;
        Vector2 m_LocalCenterD;
        float m_ReferenceAngleA;
        float m_ReferenceAngleB;
        float m_InvMassA;
        float m_InvMassB;
        float m_InvMassC;
        float m_InvMassD;
        float m_InvIA;
        float m_InvIB;
        float m_InvIC;
        float m_InvID;
        Vector2 m_JvAC;
        Vector2 m_JvBD;
        float m_JwA;
        float m_JwB;
        float m_JwC;
        float m_JwD;
        float m_Mass;
        #endregion

        internal P2DGearJoint()
        {
            _jointType = JointType.Gear;
        }

        internal P2DGearJoint(Joint2DExt jointA, Joint2DExt jointB, float ratio)
            : base(jointA._internalJoint._bodyA, jointA._internalJoint._bodyB)
        {
            _jointType = JointType.Gear;
            this.jointA = jointA;
            this.jointB = jointB;
            this.ratio = ratio;
        }

        internal Vector2 _localAnchorA;
        internal Vector2 _localAnchorB;
        internal Vector2 _localAnchorC;
        internal Vector2 _localAnchorD;

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
            return invDt * (m_Impulse * m_JvAC);
        }

        internal override float GetReactionTorque(float invDt)
        {
            return invDt * (m_Impulse * m_JwA);
        }

        internal override void InitVelocityConstraints()
        {
            _bodyA = jointA._internalJoint._bodyB;
            _bodyB = jointB._internalJoint._bodyB;
            _bodyC = jointA._internalJoint._bodyA;
            _bodyD = jointB._internalJoint._bodyA;

            m_LocalCenterA = _bodyA.centerOfMass;
            m_LocalCenterB = _bodyB.centerOfMass;
            m_LocalCenterC = _bodyC.centerOfMass;
            m_LocalCenterD = _bodyD.centerOfMass;
            m_InvMassA = _bodyA._invMass;
            m_InvMassB = _bodyB._invMass;
            m_InvMassC = _bodyC._invMass;
            m_InvMassD = _bodyD._invMass;
            m_InvIA = _bodyA._invI;
            m_InvIB = _bodyB._invI;
            m_InvIC = _bodyC._invI;
            m_InvID = _bodyD._invI;

            //Vector2 cA = _bodyA.GetRelativePoint(m_LocalCenterA);
            float aA = _bodyA.rotation * Mathf.Deg2Rad;
            Vector2 vA = _bodyA.velocity;
            float wA = _bodyA.angularVelocity * Mathf.Deg2Rad;

            //Vector2 cB = _bodyB.GetRelativePoint(m_LocalCenterB);
            float aB = _bodyB.rotation * Mathf.Deg2Rad;
            Vector2 vB = _bodyB.velocity;
            float wB = _bodyB.angularVelocity * Mathf.Deg2Rad;

            //Vector2 cC = _bodyB.GetRelativePoint(m_LocalCenterC);
            float aC = _bodyC.rotation * Mathf.Deg2Rad;
            Vector2 vC = _bodyC.velocity;
            float wC = _bodyC.angularVelocity * Mathf.Deg2Rad;

            //Vector2 cD = _bodyB.GetRelativePoint(m_LocalCenterD);
            float aD = _bodyD.rotation * Mathf.Deg2Rad;
            Vector2 vD = _bodyD.velocity;
            float wD = _bodyD.angularVelocity * Mathf.Deg2Rad;

            Rot qA = new Rot(aA), qB = new Rot(aB), qC = new Rot(aC), qD = new Rot(aD);

            m_Mass = 0.0f;

            float coordinateA = 0.0f, coordinateB = 0.0f;

            if (jointA._internalJoint._jointType == JointType.Hinge)
            {
                var hinge = (P2DHingeJoint)jointA._internalJoint;
                _localAnchorC = hinge._localAnchorA;
                _localAnchorA = hinge._localAnchorB;
                m_LocalAxisC = Vector2.zero;
                m_ReferenceAngleA = hinge.referenceAngle;

                coordinateA = aA - aC - m_ReferenceAngleA;

                m_JvAC = Vector2.zero;
                m_JwA = 1.0f;
                m_JwC = 1.0f;
                m_Mass += m_InvIA + m_InvIC;
            }
            else
            {
                var slider = (P2DSliderJoint)jointA._internalJoint;
                _localAnchorC = slider._localAnchorA;
                _localAnchorA = slider._localAnchorB;
                m_LocalAxisC = slider.localXAxisA;

                var bpA = _bodyA.position;
                var bpC = _bodyC.position;

                Vector2 pC = _localAnchorC;
                Vector2 pA = MathUtils.MulT(qC, MathUtils.Mul(qA, _localAnchorA) + (bpA - bpC));
                coordinateA = Vector2.Dot(pA - pC, m_LocalAxisC);

                Vector2 u = MathUtils.Mul(qC, m_LocalAxisC);
                Vector2 rC = MathUtils.Mul(qC, _localAnchorC - m_LocalCenterC);
                Vector2 rA = MathUtils.Mul(qA, _localAnchorA - m_LocalCenterA);
                m_JvAC = u;
                m_JwC = MathUtils.Cross(rC, u);
                m_JwA = MathUtils.Cross(rA, u);
                m_Mass += m_InvMassC + m_InvMassA + m_InvIC * m_JwC * m_JwC + m_InvIA * m_JwA * m_JwA;
            }

            if (jointB._internalJoint._jointType == JointType.Hinge)
            {
                var hinge = (P2DHingeJoint)jointB._internalJoint;
                _localAnchorD = hinge._localAnchorA;
                _localAnchorB = hinge._localAnchorB;
                m_LocalAxisD = Vector2.zero;
                m_ReferenceAngleB = hinge.referenceAngle;

                coordinateB = aB - aD - m_ReferenceAngleB;

                m_JvBD = Vector2.zero;
                m_JwB = ratio;
                m_JwD = ratio;
                m_Mass += ratio * ratio * (m_InvIB + m_InvID);
            }
            else
            {
                var slider = (P2DSliderJoint)jointB._internalJoint;
                _localAnchorD = slider._localAnchorA;
                _localAnchorB = slider._localAnchorB;
                m_LocalAxisD = slider.localXAxisA;

                var bPB = _bodyB.position;
                var bPD = _bodyD.position;

                Vector2 pD = _localAnchorD;
                Vector2 pB = MathUtils.MulT(qD, MathUtils.Mul(qB, _localAnchorB) + (bPB - bPD));
                coordinateB = Vector2.Dot(pB - pD, m_LocalAxisD);

                Vector2 u = MathUtils.Mul(qD, m_LocalAxisD);
                Vector2 rD = MathUtils.Mul(qD, _localAnchorD - m_LocalCenterD);
                Vector2 rB = MathUtils.Mul(qB, _localAnchorB - m_LocalCenterB);
                m_JvBD = ratio * u;
                m_JwD = ratio * MathUtils.Cross(rD, u);
                m_JwB = ratio * MathUtils.Cross(rB, u);
                m_Mass += ratio * ratio * (m_InvMassD + m_InvMassB) + m_InvID * m_JwD * m_JwD + m_InvIB * m_JwB * m_JwB;
            }

            m_Constant = coordinateA + ratio * coordinateB;

            // Compute effective mass.
            m_Mass = m_Mass > 0.0f ? 1.0f / m_Mass : 0.0f;

            // TODO: Scale impulse to support variable timestep

            vA += (m_InvMassA * m_Impulse) * m_JvAC;
            wA += m_InvIA * m_Impulse * m_JwA;
            vB += (m_InvMassB * m_Impulse) * m_JvBD;
            wB += m_InvIB * m_Impulse * m_JwB;
            vC -= (m_InvMassC * m_Impulse) * m_JvAC;
            wC -= m_InvIC * m_Impulse * m_JwC;
            vD -= (m_InvMassD * m_Impulse) * m_JvBD;
            wD -= m_InvID * m_Impulse * m_JwD;

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
            if (!_bodyC.isKinematic)
            {
                _bodyC.velocity = vC;
                if (!_bodyC.fixedAngle)
                {
                    _bodyC.angularVelocity = wC * Mathf.Rad2Deg;
                }
            }
            if (!_bodyD.isKinematic)
            {
                _bodyD.velocity = vD;
                if (!_bodyD.fixedAngle)
                {
                    _bodyD.angularVelocity = wD * Mathf.Rad2Deg;
                }
            }
        }

        internal override void SolveVelocityConstraints()
        {
            Vector2 vA = _bodyA.velocity;
            float wA = _bodyA.angularVelocity * Mathf.Deg2Rad;
            Vector2 vB = _bodyB.velocity;
            float wB = _bodyB.angularVelocity * Mathf.Deg2Rad;
            Vector2 vC = _bodyC.velocity;
            float wC = _bodyC.angularVelocity * Mathf.Deg2Rad;
            Vector2 vD = _bodyD.velocity;
            float wD = _bodyD.angularVelocity * Mathf.Deg2Rad;

            float Cdot = Vector2.Dot(m_JvAC, vA - vC) + Vector2.Dot(m_JvBD, vB - vD);
            Cdot += (m_JwA * wA - m_JwC * wC) + (m_JwB * wB - m_JwD * wD);

            float impulse = -m_Mass * Cdot;
            m_Impulse += impulse;

            vA += (m_InvMassA * impulse) * m_JvAC;
            wA += m_InvIA * impulse * m_JwA;
            vB += (m_InvMassB * impulse) * m_JvBD;
            wB += m_InvIB * impulse * m_JwB;
            vC -= (m_InvMassC * impulse) * m_JvAC;
            wC -= m_InvIC * impulse * m_JwC;
            vD -= (m_InvMassD * impulse) * m_JvBD;
            wD -= m_InvID * impulse * m_JwD;

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
            if (!_bodyC.isKinematic)
            {
                _bodyC.velocity = vC;
                if (!_bodyC.fixedAngle)
                {
                    _bodyC.angularVelocity = wC * Mathf.Rad2Deg;
                }
            }
            if (!_bodyD.isKinematic)
            {
                _bodyD.velocity = vD;
                if (!_bodyD.fixedAngle)
                {
                    _bodyD.angularVelocity = wD * Mathf.Rad2Deg;
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
            Vector2 cC = _bodyC.GetRelativePoint(m_LocalCenterC);
            Vector2 cC0 = cC;
            float aC = _bodyC.rotation * Mathf.Deg2Rad;
            Vector2 cD = _bodyD.GetRelativePoint(m_LocalCenterD);
            Vector2 cD0 = cD;
            float aD = _bodyD.rotation * Mathf.Deg2Rad;

            Rot qA = new Rot(aA), qB = new Rot(aB), qC = new Rot(aC), qD = new Rot(aD);

            float coordinateA, coordinateB;

            Vector2 JvAC, JvBD;
            float JwA, JwB, JwC, JwD;
            float mass = 0.0f;

            if (jointA._internalJoint._jointType == JointType.Hinge)
            {
                JvAC = Vector2.zero;
                JwA = 1.0f;
                JwC = 1.0f;
                mass += m_InvIA + m_InvIC;

                coordinateA = aA - aC - m_ReferenceAngleA;
            }
            else
            {
                Vector2 u = MathUtils.Mul(qC, m_LocalAxisC);
                Vector2 rC = MathUtils.Mul(qC, _localAnchorC - m_LocalCenterC);
                Vector2 rA = MathUtils.Mul(qA, _localAnchorA - m_LocalCenterA);
                JvAC = u;
                JwC = MathUtils.Cross(rC, u);
                JwA = MathUtils.Cross(rA, u);
                mass += m_InvMassC + m_InvMassA + m_InvIC * JwC * JwC + m_InvIA * JwA * JwA;

                Vector2 pC = _localAnchorC - m_LocalCenterC;
                Vector2 pA = MathUtils.MulT(qC, rA + (cA - cC));
                coordinateA = Vector2.Dot(pA - pC, m_LocalAxisC);
            }

            if (jointB._internalJoint._jointType == JointType.Hinge)
            {
                JvBD = Vector2.zero;
                JwB = ratio;
                JwD = ratio;
                mass += ratio * ratio * (m_InvIB + m_InvID);

                coordinateB = aB - aD - m_ReferenceAngleB;
            }
            else
            {
                Vector2 u = MathUtils.Mul(qD, m_LocalAxisD);
                Vector2 rD = MathUtils.Mul(qD, _localAnchorD - m_LocalCenterD);
                Vector2 rB = MathUtils.Mul(qB, _localAnchorB - m_LocalCenterB);
                JvBD = ratio * u;
                JwD = ratio * MathUtils.Cross(rD, u);
                JwB = ratio * MathUtils.Cross(rB, u);
                mass += ratio * ratio * (m_InvMassD + m_InvMassB) + m_InvID * JwD * JwD + m_InvIB * JwB * JwB;

                Vector2 pD = _localAnchorD - m_LocalCenterD;
                Vector2 pB = MathUtils.MulT(qD, rB + (cB - cD));
                coordinateB = Vector2.Dot(pB - pD, m_LocalAxisD);
            }

            float C = (coordinateA + ratio * coordinateB) - m_Constant;

            float impulse = 0.0f;
            if (mass > 0.0f)
            {
                impulse = -C / mass;
            }

            cA += m_InvMassA * impulse * JvAC;
            aA += m_InvIA * impulse * JwA;
            cB += m_InvMassB * impulse * JvBD;
            aB += m_InvIB * impulse * JwB;
            cC -= m_InvMassC * impulse * JvAC;
            aC -= m_InvIC * impulse * JwC;
            cD -= m_InvMassD * impulse * JvBD;
            aD -= m_InvID * impulse * JwD;

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
            if (!_bodyC.isKinematic)
            {
                var dcC = cC0 - cC;
                _bodyC.position -= dcC;
                if (!_bodyC.fixedAngle)
                {
                    _bodyC.rotation = aC * Mathf.Rad2Deg;
                }
            }
            if (!_bodyD.isKinematic)
            {
                var dcD = cD0 - cD;
                _bodyD.position -= dcD;
                if (!_bodyD.fixedAngle)
                {
                    _bodyD.rotation = aD * Mathf.Rad2Deg;
                }
            }

            // Not yet implemented
            return false;
        }
    }
}
