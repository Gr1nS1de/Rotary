//
// DistanceJoint2DExt.cs
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
    [AddComponentMenu("Physics 2D/Extended Joints/Distance Joint 2D (P2D Extensions)")]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class DistanceJoint2DExt : DistanceJointBase2DExt
    {
        [SerializeField] bool m_MaxDistanceOnly;

        public bool maxDistanceOnly { get { return m_MaxDistanceOnly; } set { m_MaxDistanceOnly = value; } }

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
            m_InternalDistanceJoint._isElastic = m_MaxDistanceOnly;

            Solve();
        }
    }
}
