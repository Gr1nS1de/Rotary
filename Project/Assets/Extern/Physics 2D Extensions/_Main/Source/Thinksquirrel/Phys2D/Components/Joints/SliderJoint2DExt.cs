//
// SliderJoint2DExt.cs
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
    [AddComponentMenu("Physics 2D/Extended Joints/Slider Joint 2D (P2D Extensions)")]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class SliderJoint2DExt : HingeOrSliderJoint2DExt
    {
        [SerializeField] float m_Angle;
        [SerializeField] bool m_UseMotor;
        [SerializeField] JointMotor2DExt m_Motor = new JointMotor2DExt();
        [SerializeField] bool m_UseLimits;
        [SerializeField] JointTranslationLimits2DExt m_TranslationLimits = new JointTranslationLimits2DExt();

        public float angle { get { return m_Angle; } set { m_Angle = value; } }
        public bool useMotor { get { return m_UseMotor; } set { m_UseMotor = value; } }
        public JointMotor2D motor { get { return m_Motor; } set { m_Motor.Set(value); } }
        public bool useLimits { get { return m_UseLimits; } set { m_UseLimits = value; } }
        public JointTranslationLimits2D limits { get { return m_TranslationLimits; } set { m_TranslationLimits.Set(value); } }

        public float jointSpeed
        {
            get
            {
                return m_InternalSliderJoint != null ? m_InternalSliderJoint.jointSpeed : 0.0f;
            }
        }
        public float jointTranslation
        {
            get
            {
                return m_InternalSliderJoint != null ? m_InternalSliderJoint.jointTranslation : 0.0f;
            }
        }
        public JointLimitState2D limitState
        {
            get
            {
                return m_InternalSliderJoint != null ? m_InternalSliderJoint.limitState : JointLimitState2D.Inactive;
            }
        }
        public float referenceAngle
        {
            get
            {
                return m_InternalSliderJoint != null ? m_InternalSliderJoint.referenceAngle * Mathf.Rad2Deg : 0.0f;
            }
        }
        public float GetMotorForce(float timeStep)
        {
            var invDt = timeStep == 0 ? 0 : 1.0f / timeStep;
            return m_InternalSliderJoint != null ? m_InternalSliderJoint.GetMotorForce(invDt) : 0.0f;
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

        P2DSliderJoint m_InternalSliderJoint;
        float m_CachedAngle;

        internal override sealed void PhysicsUpdate()
        {
            SetConnectedAnchor();

            if (_internalJoint == null)
            {
                var axis = MathUtils.MulT(new Rot(-Mathf.DeltaAngle(m_Angle, ((Rigidbody2DExt)cachedRigidbody2D).rotation) * Mathf.Deg2Rad), Vector2.right);
                m_CachedAngle = m_Angle;

                _internalJoint = m_InternalSliderJoint = new P2DSliderJoint(
                    cachedRigidbody2D,
                    GetBodyB(),
                    anchor,
                    connectedAnchor,
                    axis);
                SubscribeToBreakEvent();
            }
            else
            {
                m_InternalSliderJoint._bodyB = GetBodyB();
                m_InternalSliderJoint._localAnchorA = anchor;
                m_InternalSliderJoint._localAnchorB = connectedAnchor;

                // Analysis disable CompareOfFloatsByEqualityOperator
                if (m_CachedAngle != m_Angle)
                // Analysis restore CompareOfFloatsByEqualityOperator
                {
                    var axis = MathUtils.MulT(new Rot(-Mathf.DeltaAngle(m_Angle, ((Rigidbody2DExt)cachedRigidbody2D).rotation) * Mathf.Deg2Rad), Vector2.right);

                    m_InternalSliderJoint.SetAxis(axis);
                    m_CachedAngle = m_Angle;
                }
            }

            m_InternalSliderJoint.motorEnabled = m_UseMotor;
            m_InternalSliderJoint.motorSpeed = m_Motor.motorSpeed;
            m_InternalSliderJoint.maxMotorForce = motor.maxMotorTorque;
            m_InternalSliderJoint.limitEnabled = m_UseLimits;
            m_InternalSliderJoint.SetLimits(m_TranslationLimits.min, m_TranslationLimits.max);

            Solve();
        }
    }
}
