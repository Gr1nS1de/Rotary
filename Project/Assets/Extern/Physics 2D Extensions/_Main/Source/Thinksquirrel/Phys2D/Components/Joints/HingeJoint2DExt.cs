//
// HingeJoint2DExt.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using Thinksquirrel.Phys2D.Internal;
using Thinksquirrel.Phys2D.Internal.Joints;

namespace Thinksquirrel.Phys2D
{
    // TODO: Update ref angle
    [AddComponentMenu("Physics 2D/Extended Joints/Hinge Joint 2D (P2D Extensions)")]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class HingeJoint2DExt : HingeOrSliderJoint2DExt
    {
        [SerializeField] bool m_UseMotor;
        [SerializeField] JointMotor2DExt m_Motor = new JointMotor2DExt();
        [SerializeField] bool m_UseLimits;
        [SerializeField] JointAngleLimits2DExt m_AngleLimits = new JointAngleLimits2DExt();

        public bool useMotor { get { return m_UseMotor; } set { m_UseMotor = value; } }
        public JointMotor2D motor { get { return m_Motor; } set { m_Motor.Set(value); } }
        public bool useLimits { get { return m_UseLimits; } set { m_UseLimits = value; } }
        public JointAngleLimits2D limits { get { return m_AngleLimits; } set { m_AngleLimits.Set(value); } }

        public float jointAngle
        {
            get
            {
                return m_InternalHingeJoint != null ? m_InternalHingeJoint.jointAngle * Mathf.Rad2Deg : 0.0f;
            }
        }
        public float jointSpeed
        {
            get
            {
                return m_InternalHingeJoint != null ? m_InternalHingeJoint.jointSpeed * Mathf.Rad2Deg : 0.0f;
            }
        }
        public JointLimitState2D limitState
        {
            get
            {
                return m_InternalHingeJoint != null ? m_InternalHingeJoint.limitState : JointLimitState2D.Inactive;
            }
        }
        public float referenceAngle
        {
            get
            {
                return m_InternalHingeJoint != null ? m_InternalHingeJoint.referenceAngle * Mathf.Rad2Deg : 0.0f;
            }
        }
        public float GetMotorTorque(float timeStep)
        {
            var invDt = timeStep == 0 ? 0 : 1.0f / timeStep;
            return m_InternalHingeJoint != null ? m_InternalHingeJoint.GetMotorTorque(invDt) : 0.0f;
        }
        public Vector2 GetReactionForce(float timeStep)
        {
            var invDt = timeStep == 0 ? 0 : 1.0f / timeStep;
            return _internalJoint != null ? _internalJoint.GetReactionForce(invDt) : Vector2.zero;
        }
        public float GetReactionTorque(float timeStep)
        {
            var invDt = timeStep == 0 ? 0 : 1.0f / timeStep;
            return _internalJoint != null ? _internalJoint.GetReactionTorque(invDt) : 0.0f;
        }

        P2DHingeJoint m_InternalHingeJoint;

        internal override sealed void PhysicsUpdate()
        {
            SetConnectedAnchor();

            if (_internalJoint == null)
            {
                _internalJoint = m_InternalHingeJoint = new P2DHingeJoint(
                    cachedRigidbody2D,
                    GetBodyB(),
                    anchor,
                    connectedAnchor);
                SubscribeToBreakEvent();
            }
            else
            {
                m_InternalHingeJoint._bodyB = GetBodyB();
                m_InternalHingeJoint._localAnchorA = anchor;
                m_InternalHingeJoint._localAnchorB = connectedAnchor;
            }

            m_InternalHingeJoint.motorEnabled = m_UseMotor;
            m_InternalHingeJoint.motorSpeed = m_Motor.motorSpeed * Mathf.Deg2Rad;
            m_InternalHingeJoint.maxMotorTorque = motor.maxMotorTorque;
            m_InternalHingeJoint.limitEnabled = m_UseLimits;
            m_InternalHingeJoint.SetLimits(m_AngleLimits.min * Mathf.Deg2Rad, m_AngleLimits.max * Mathf.Deg2Rad);

            Solve();
        }
    }
}
