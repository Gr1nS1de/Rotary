//
// FrictionJoint2DExt.cs
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
    [AddComponentMenu("Physics 2D/Extended Joints/Friction Joint 2D (P2D Extensions)")]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class FrictionJoint2DExt : AnchoredJoint2DExt
    {
        [SerializeField] float m_MaxForce = 100f;
        [SerializeField] float m_MaxTorque = 100f;

        public float maxForce { get { return m_MaxForce; } set { m_MaxForce = value; } }
        public float maxTorque { get { return m_MaxTorque; } set { m_MaxTorque = value; } }

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

        P2DFrictionJoint m_InternalFrictionJoint;

        internal override sealed void PhysicsUpdate()
        {
            SetConnectedAnchor();

            if (_internalJoint == null)
            {
                _internalJoint = m_InternalFrictionJoint = new P2DFrictionJoint(
                    cachedRigidbody2D,
                    GetBodyB(),
                    anchor,
                    connectedAnchor);
                SubscribeToBreakEvent();
            }
            else
            {
                m_InternalFrictionJoint._bodyB = GetBodyB();
                m_InternalFrictionJoint._localAnchorA = anchor;
                m_InternalFrictionJoint._localAnchorB = connectedAnchor;
            }

            m_InternalFrictionJoint.maxForce = m_MaxForce;
            m_InternalFrictionJoint.maxTorque = m_MaxTorque;

            Solve();
        }

    }
}
