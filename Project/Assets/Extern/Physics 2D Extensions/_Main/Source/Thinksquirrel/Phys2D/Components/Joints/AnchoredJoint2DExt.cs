//
// AnchoredJoint2DExt.cs
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
    public abstract class AnchoredJoint2DExt : Joint2DExt
    {
        [SerializeField] Vector2 m_Anchor;
        [SerializeField] Vector2 m_ConnectedAnchor;
        [SerializeField] bool m_AutoConfigureConnectedAnchor = true;

        public Vector2 anchor { get { return m_Anchor; } set { m_Anchor = value; } }
        public Vector2 connectedAnchor { get { return m_ConnectedAnchor; } set { m_ConnectedAnchor = value; } }
        public bool autoConfigureConnectedAnchor { get { return m_AutoConfigureConnectedAnchor; } set { m_AutoConfigureConnectedAnchor = value; } }

        [System.NonSerialized] protected bool m_AnchorWasAutoConfigured;

        internal virtual void SetConnectedAnchor()
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

            connectedAnchor = connectedBody ? Vector2.zero : bodyB.GetPoint(bodyA.GetRelativePoint(anchor)) + Physics2D.gravity.normalized * Constants.epsilon;

            m_AnchorWasAutoConfigured = true;
        }

        //! \cond PRIVATE
        public virtual void SetConnectedAnchorEditor()
        {
            if (!autoConfigureConnectedAnchor || Application.isPlaying)
                return;

            connectedAnchor = connectedBody ? Vector2.zero : new Vector2(-transform.position.x, -transform.position.y) + Physics2D.gravity.normalized * Constants.epsilon;
        }
        //! \endcond
    }
}
