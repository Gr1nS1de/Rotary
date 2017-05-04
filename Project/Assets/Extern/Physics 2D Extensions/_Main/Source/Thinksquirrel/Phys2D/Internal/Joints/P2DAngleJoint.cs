//
// P2DAngleJoint.cs
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
    sealed class P2DAngleJoint : P2DJoint
    {
        public float biasFactor { get; set; }
        public float maxImpulse { get; set; }
        public float softness { get; set; }
        public float targetAngle
        {
            get { return m_TargetAngle; }
            set
            {
                if (value != m_TargetAngle)
                {
                    m_TargetAngle = value;
                    WakeBodies();
                }
            }
        }
        float m_Bias;
        float m_JointError;
        float m_MassFactor;
        float m_TargetAngle;
        float m_Impulse;
        
        internal P2DAngleJoint()
        {
            _jointType = JointType.Angle;
        }

        internal P2DAngleJoint(Rigidbody2DExt bodyA, Rigidbody2DExt bodyB)
            : base(bodyA, bodyB)
        {
            _jointType = JointType.Angle;
            biasFactor = 0.2f;
            softness = 0.1f;
            maxImpulse = 1000.0f;
        }
        
        internal override Vector2 GetReactionForce(float invDt)
        {
            return Vector2.zero;
        }

        internal override float GetReactionTorque(float invDt)
        {
            return invDt * m_Impulse;
        }

        internal override void InitVelocityConstraints()
        {
            m_JointError = Mathf.DeltaAngle(Mathf.DeltaAngle(_bodyB.rotation, _bodyA.rotation), m_TargetAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;

            m_Bias = -biasFactor * (1.0f / Time.deltaTime) * m_JointError;
            m_MassFactor = (1.0f - softness) / (_bodyA._invI + _bodyB._invI);
        }

        internal override void SolveVelocityConstraints()
        {
            float wA = _bodyA.angularVelocity * Mathf.Deg2Rad;
            float wB = _bodyB.angularVelocity * Mathf.Deg2Rad;
            
            m_Impulse = (m_Bias - wB + wA) * m_MassFactor;
            
            wA -= _bodyA._invI * Mathf.Sign(m_Impulse) * Mathf.Min(Mathf.Abs(m_Impulse), maxImpulse);
            wB += _bodyB._invI * Mathf.Sign(m_Impulse) * Mathf.Min(Mathf.Abs(m_Impulse), maxImpulse);
            
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
