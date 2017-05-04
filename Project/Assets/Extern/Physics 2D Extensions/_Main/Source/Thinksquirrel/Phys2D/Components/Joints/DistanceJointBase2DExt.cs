//
// DistanceJointBase2DExt.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using Thinksquirrel.Phys2D.Internal;

namespace Thinksquirrel.Phys2D
{
    public abstract class DistanceJointBase2DExt : AnchoredJoint2DExt
    {
        [SerializeField] float m_Distance = 1.0f;
        [SerializeField] bool m_AutoConfigureDistance = true;

        public float distance { get { return m_Distance; } set { m_Distance = value; } }
        public bool autoConfigureDistance { get { return m_AutoConfigureDistance; } set { m_AutoConfigureDistance = value; } }

        public override sealed float breakTorque
        {
            get { return Mathf.Infinity; }
            set { Debug.LogError("Break torque cannot be set for this joint type."); }
        }
        public Vector2 GetReactionForce(float timeStep)
        {
            var invDt = timeStep == 0 ? 0 : 1.0f / timeStep;
            return _internalJoint != null ? _internalJoint.GetReactionForce(invDt) : Vector2.zero;
        }

        [System.NonSerialized] protected bool m_DistanceWasAutoConfigured;

        internal void SetDistance()
        {
            if (!m_AutoConfigureDistance)
            {
                m_DistanceWasAutoConfigured = false;
                return;
            }

            if (m_DistanceWasAutoConfigured)
                return;

            var bodyA = (Rigidbody2DExt)cachedRigidbody2D;
            var bodyB = GetBodyB();

            var pointA = bodyA.GetRelativePoint(anchor);
            var pointB = bodyB.GetRelativePoint(connectedAnchor);

            m_Distance = Vector2.Distance(pointA, pointB);

            m_DistanceWasAutoConfigured = true;
        }

        //! \cond PRIVATE
        public void SetDistanceEditor()
        {
            if (!m_AutoConfigureDistance || Application.isPlaying)
                return;

            var bodyBTransform = connectedBody ? connectedBody.transform : null;

            var pointA = transform.GetRelativePoint(anchor);
            var pointB = bodyBTransform ? bodyBTransform.GetRelativePoint(connectedAnchor) : Vector2.zero;

            m_Distance = Vector2.Distance(pointA, pointB);
        }
        //! \endcond
    }
}
