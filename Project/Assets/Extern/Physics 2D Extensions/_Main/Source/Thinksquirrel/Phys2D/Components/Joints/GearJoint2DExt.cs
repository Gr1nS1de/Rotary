//
// GearJoint2DExt.cs
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
    [AddComponentMenu("Physics 2D/Extended Joints/Gear Joint 2D (P2D Extensions)")]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class GearJoint2DExt : Joint2DExt
    {
        [SerializeField] HingeOrSliderJoint2DExt m_LocalJoint;
        [SerializeField] HingeOrSliderJoint2DExt m_ConnectedJoint;
        [SerializeField] float m_GearRatio = 1f;

        public HingeOrSliderJoint2DExt localJoint { get { return m_LocalJoint; } set { m_LocalJoint = value; } }
        public HingeOrSliderJoint2DExt connectedJoint { get { return m_ConnectedJoint; } set { m_ConnectedJoint = value; } }
        public override sealed Rigidbody2D connectedBody
        {
			get { return m_ConnectedJoint != null ? m_ConnectedJoint.cachedRigidbody2D : null; }
            set { Debug.LogError("Cannot set the connected body for this joint type. Set the connectedJoint property instead."); }
        }
        public float gearRatio { get { return m_GearRatio; } set { m_GearRatio = value; } }

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

        P2DGearJoint m_InternalGearJoint;
        bool m_DisplayedError;

        void Reset()
        {
            m_LocalJoint = GetComponent<HingeJoint2DExt>();

            if (!m_LocalJoint) m_LocalJoint = GetComponent<SliderJoint2DExt>();
        }

        //! \cond PRIVATE
        public bool ValidateJoints(out string error)
        {
            if (!localJoint || !connectedJoint)
            {
                error = "Gear Joint must connect two valid joints.";
                return false;
            }

            if (localJoint.gameObject != gameObject)
            {
                error = "Local Joint must be on this game object.";
                return false;
            }

            if (localJoint is SliderJoint2DExt && connectedJoint is SliderJoint2DExt)
            {
                error = "Gear Joint cannot connect two slider joints.";
                return false;
            }

            error = null;
            return true;
        }
        //! \endcond

        internal override sealed void PhysicsUpdate()
        {
            // Check for valid joints
            string error;
            if (!ValidateJoints(out error))
            {
                if (!m_DisplayedError)
                {
                    Debug.LogWarning(string.Format("Gear Joint: {0}", error));
                    m_DisplayedError = true;
                }
                return;
            }

            m_DisplayedError = false;

            if (_internalJoint == null)
            {
                _internalJoint = m_InternalGearJoint = new P2DGearJoint(
                    localJoint,
                    connectedJoint,
                    m_GearRatio);
                SubscribeToBreakEvent();
            }
            else
            {
                m_InternalGearJoint.jointA = m_LocalJoint;
                m_InternalGearJoint.jointB = m_ConnectedJoint;
                m_InternalGearJoint.ratio = m_GearRatio;
            }

            Solve();
        }

        protected override sealed int executionLevel { get { return 2; } }
    }
}
