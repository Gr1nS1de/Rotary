//
// AngleJoint2DExt.cs
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
    [AddComponentMenu("Physics 2D/Extended Joints/Angle Joint 2D (P2D Extensions)")]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class AngleJoint2DExt : Joint2DExt
    {
        [SerializeField] float m_TargetAngle;
        [SerializeField] float m_BiasFactor = 0.2f;
        [SerializeField] float m_Softness = 0.1f;
        [SerializeField] float m_MaxImpulse = 1000.0f;

        public float targetAngle { get { return m_TargetAngle; } set { m_TargetAngle = value; } }
        public float biasFactor { get { return m_BiasFactor; } set { m_BiasFactor = value; } }
        public float softness { get { return m_Softness; } set { m_Softness = value; } }
        public float maxImpulse { get { return m_MaxImpulse; } set { m_MaxImpulse = value; } }

        public override sealed float breakForce
        {
            get { return Mathf.Infinity; }
            set { Debug.LogError("Break force cannot be set for this joint type."); }
        }
        public float GetReactionTorque(float timeStep)
        {
            var invDt = timeStep == 0 ? 0 : 1.0f / timeStep;
            return _internalJoint != null ? _internalJoint.GetReactionTorque(invDt) : 0.0f;
        }

        P2DAngleJoint m_InternalAngleJoint;
        bool m_DisplayedError;

        //! \cond PRIVATE
        public bool ValidateAngleJoint(out string error)
        {
            if (m_BiasFactor < 0 || m_BiasFactor > 1)
            {
                error = "Bias factor must be between 0 and 1.";
                return false;
            }
            if (m_Softness < 0)
            {
                error = "Softness must be greater than or equal to 0.";
                return false;
            }

            error = null;
            return true;
        }
        //! \endcond

        internal override sealed void PhysicsUpdate()
        {
            // Check for valid anchors and ratio
            string error;
            if (!ValidateAngleJoint(out error))
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
                _internalJoint = m_InternalAngleJoint = new P2DAngleJoint(
                    cachedRigidbody2D,
                    GetBodyB());
                SubscribeToBreakEvent();
            }
            else
            {
                m_InternalAngleJoint._bodyB = GetBodyB();
            }

            m_InternalAngleJoint.targetAngle = m_TargetAngle * Mathf.Deg2Rad;
            m_InternalAngleJoint.biasFactor = m_BiasFactor;
            m_InternalAngleJoint.softness = m_Softness;
            m_InternalAngleJoint.maxImpulse = m_MaxImpulse;

            Solve();
        }
    }
}
