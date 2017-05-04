//
// SpringJoint2DExt.cs
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
    [AddComponentMenu("Physics 2D/Extended Joints/Spring Joint 2D (P2D Extensions)")]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class SpringJoint2DExt : DistanceJointBase2DExt
    {
        [SerializeField] float m_DampingRatio = 0.0f;
        [SerializeField] float m_Frequency = 6.0f;

        public float dampingRatio { get { return m_DampingRatio; } set { m_DampingRatio = value; } }
        public float frequency { get { return m_Frequency; } set { m_Frequency = value; } }

        P2DDistanceJoint m_InternalDistanceJoint;

        internal override sealed void PhysicsUpdate()
        {
            SetConnectedAnchor();
            SetDistance();

            if (_internalJoint == null)
            {
                _internalJoint = m_InternalDistanceJoint = new P2DDistanceJoint(
                    cachedRigidbody2D,
                    GetBodyB(),
                    anchor,
                    connectedAnchor);
                SubscribeToBreakEvent();
            }
            else
            {
                m_InternalDistanceJoint._bodyB = GetBodyB();
                m_InternalDistanceJoint._localAnchorA = anchor;
                m_InternalDistanceJoint._localAnchorB = connectedAnchor;
            }

            m_InternalDistanceJoint._length = distance;
            m_InternalDistanceJoint._dampingRatio = m_DampingRatio;
            m_InternalDistanceJoint._frequency = m_Frequency;

            Solve();
        }
    }
}
