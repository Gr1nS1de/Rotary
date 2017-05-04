//
// WheelJoint2DExt.cs
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
    [AddComponentMenu("Physics 2D/Extended Joints/Wheel Joint 2D (P2D Extensions)")]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class WheelJoint2DExt : AnchoredJoint2DExt
    {
        [SerializeField] JointSuspension2DExt m_Suspension = new JointSuspension2DExt();
    	[SerializeField] bool m_UseMotor;
        [SerializeField] JointMotor2DExt m_Motor = new JointMotor2DExt();

        public JointSuspension2D suspension { get { return m_Suspension; } set { m_Suspension.Set(value); } }
        public bool useMotor { get { return m_UseMotor; } set { m_UseMotor = value; } }
        public JointMotor2D motor { get { return m_Motor; } set { m_Motor.Set(value); } }

        public float jointSpeed
        {
            get
            {
                return m_InternalWheelJoint != null ? m_InternalWheelJoint.jointSpeed * Mathf.Rad2Deg : 0.0f;
            }
        }
        public float jointTranslation
        {
            get
            {
                return m_InternalWheelJoint != null ? m_InternalWheelJoint.jointTranslation : 0.0f;
            }
        }
        public float GetMotorTorque(float timeStep)
        {
            var invDt = timeStep == 0 ? 0 : 1.0f / timeStep;
            return m_InternalWheelJoint != null ? m_InternalWheelJoint.GetMotorTorque(invDt) : 0.0f;
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

        P2DWheelJoint m_InternalWheelJoint;
        float m_CachedAngle;

        internal override sealed void PhysicsUpdate()
        {
            SetConnectedAnchor();

            if (_internalJoint == null)
            {
                var suspensionAxis = MathUtils.MulT(new Rot(-m_Suspension.angle * Mathf.Deg2Rad), Vector2.right);
                m_CachedAngle = m_Suspension.angle;

                _internalJoint = m_InternalWheelJoint = new P2DWheelJoint(
                    cachedRigidbody2D,
                    GetBodyB(),
                    anchor,
                    connectedAnchor,
                    suspensionAxis);

                m_CachedAngle = m_Suspension.angle;
                SubscribeToBreakEvent();
            }
            else
            {
                m_InternalWheelJoint._bodyB = GetBodyB();
                m_InternalWheelJoint._localAnchorA = anchor;
                m_InternalWheelJoint._localAnchorB = connectedAnchor;

                // Analysis disable CompareOfFloatsByEqualityOperator
                if (m_CachedAngle != m_Suspension.angle)
                // Analysis restore CompareOfFloatsByEqualityOperator
                {
                    var suspensionAxis = MathUtils.MulT(new Rot(-m_Suspension.angle * Mathf.Deg2Rad), Vector2.right);

                    m_InternalWheelJoint.SetAxis(suspensionAxis);
                    m_CachedAngle = m_Suspension.angle;
                }
            }

            m_InternalWheelJoint._frequency = m_Suspension.frequency;
            m_InternalWheelJoint._dampingRatio = m_Suspension.dampingRatio;
            m_InternalWheelJoint.motorEnabled = m_UseMotor;
            m_InternalWheelJoint.motorSpeed = m_Motor.motorSpeed * Mathf.Deg2Rad;
            m_InternalWheelJoint.maxMotorTorque = m_Motor.maxMotorTorque;

            Solve();
        }

        internal override sealed void SetConnectedAnchor()
        {
            if (!autoConfigureConnectedAnchor)
            {
                m_AnchorWasAutoConfigured = false;
                return;
            }

            if (m_AnchorWasAutoConfigured)
                return;

            var bodyA = (Rigidbody2DExt)cachedRigidbody2D;
            var bodyB = GetBodyB();

            connectedAnchor = bodyB.GetPoint(bodyA.GetRelativePoint(anchor));

            m_AnchorWasAutoConfigured = true;
        }

        //! \cond PRIVATE
        public override sealed void SetConnectedAnchorEditor()
        {
            if (!autoConfigureConnectedAnchor || Application.isPlaying)
                return;

            var bodyBTransform = connectedBody ? connectedBody.transform : null;

            connectedAnchor = bodyBTransform ? bodyBTransform.GetPoint(transform.GetRelativePoint(anchor)) : new Vector2(-transform.position.x, -transform.position.y);
        }
        //! \endcond
    }
}
