//
// PulleyJoint2DExt.cs
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
    [AddComponentMenu("Physics 2D/Extended Joints/Pulley Joint 2D (P2D Extensions)")]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PulleyJoint2DExt : AnchoredJoint2DExt
    {
        [SerializeField] Transform m_GroundAnchorA;
        [SerializeField] Transform m_GroundAnchorB;
        [SerializeField] float m_Ratio = 1.0f;

        public Transform groundAnchorA { get { return m_GroundAnchorA; } set { m_GroundAnchorA = value; } }
        public Transform groundAnchorB { get { return m_GroundAnchorB; } set { m_GroundAnchorB = value; } }
        public float ratio { get { return m_Ratio; } set { m_Ratio = value; } }

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

        P2DPulleyJoint m_InternalPulleyJoint;
        bool m_DisplayedError;

        //! \cond PRIVATE
        public bool ValidatePulley(out string error)
        {
            if (!m_GroundAnchorA || !m_GroundAnchorB)
            {
                error = "Pulley joint must have two valid ground anchors.";
                return false;
            }

            if (!connectedBody)
            {
                error = "Pulley joint must have a valid connected body.";
                return false;
            }

            if (m_Ratio <= Constants.epsilon)
            {
                error = string.Format("Pulley ratio must be greater than {0}.", Constants.epsilon);
                return false;
            }

            error = null;
            return true;
        }
        //! \endcond

        internal override sealed void PhysicsUpdate()
        {
            SetConnectedAnchor();

            // Check for valid anchors and ratio
            string error;
            if (!ValidatePulley(out error))
            {
                if (!m_DisplayedError)
                {
                    Debug.LogError(string.Format("Pulley Joint: {0}", error));
                    m_DisplayedError = true;
                }
                return;
            }

            m_DisplayedError = false;

            if (_internalJoint == null)
            {
                _internalJoint = m_InternalPulleyJoint = new P2DPulleyJoint(
                    cachedRigidbody2D,
                    GetBodyB(),
                    anchor,
                    connectedAnchor,
                    m_GroundAnchorA,
                    m_GroundAnchorB,
                    m_Ratio);
                SubscribeToBreakEvent();
            }
            else
            {
                m_InternalPulleyJoint._bodyB = GetBodyB();
                m_InternalPulleyJoint._localAnchorA = anchor;
                m_InternalPulleyJoint._localAnchorB = connectedAnchor;
                m_InternalPulleyJoint.groundAnchorA = m_GroundAnchorA;
                m_InternalPulleyJoint.groundAnchorB = m_GroundAnchorB;
                m_InternalPulleyJoint.ratio = m_Ratio;
            }

            Solve();
        }
    }
}
