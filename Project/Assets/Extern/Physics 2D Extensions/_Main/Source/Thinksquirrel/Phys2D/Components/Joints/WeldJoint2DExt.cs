//
// WeldJoint2D.cs
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
    [AddComponentMenu("Physics 2D/Extended Joints/Weld Joint 2D (P2D Extensions)")]
    [RequireComponent(typeof(Rigidbody2D))]
    // TODO: Change reference angle
    public sealed class WeldJoint2DExt : AnchoredJoint2DExt
    {
        [SerializeField] float m_DampingRatio = 0.2f;
        [SerializeField] float m_Frequency = 0.2f;

        public float dampingRatio { get { return m_DampingRatio; } set { m_DampingRatio = value; } }
        public float frequency { get { return m_Frequency; } set { m_Frequency = value; } }

        public float referenceAngle
        {
            get
            {
                return m_InternalWeldJoint != null ? m_InternalWeldJoint.referenceAngle * Mathf.Rad2Deg : 0.0f;
            }
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

        P2DWeldJoint m_InternalWeldJoint;

        internal override sealed void PhysicsUpdate()
        {
            SetConnectedAnchor();

            if (_internalJoint == null)
            {
                var bodyA = (Rigidbody2DExt)cachedRigidbody2D;
                var bodyB = GetBodyB();
                _internalJoint = m_InternalWeldJoint = new P2DWeldJoint(
                    bodyA,
                    bodyB,
                    anchor,
                    connectedAnchor);
                SubscribeToBreakEvent();
            }
            else
            {
                m_InternalWeldJoint._bodyB = GetBodyB();
                m_InternalWeldJoint._localAnchorA = anchor;
                m_InternalWeldJoint._localAnchorB = connectedAnchor;
            }

            m_InternalWeldJoint.dampingRatio = dampingRatio;
            m_InternalWeldJoint.frequency = frequency;

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
