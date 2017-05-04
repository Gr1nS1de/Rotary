//
// VelocityLimitController2DExt.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using Thinksquirrel.Phys2D.Internal.Controllers;

namespace Thinksquirrel.Phys2D
{
    [AddComponentMenu("Physics 2D/Physics Controllers/Velocity Limit Controller 2D (P2D Extensions)")]
    public sealed class VelocityLimitController2DExt : Controller2DExt
    {
        [SerializeField] bool m_LimitLinearVelocity = true;
        [SerializeField] bool m_LimitAngularVelocity = true;
        [SerializeField] float m_MaxLinearVelocity = 10;
        [SerializeField] float m_MaxAngularVelocity = 10;

        public bool limitLinearVelocity { get { return m_LimitLinearVelocity; } set { m_LimitLinearVelocity = value; } }
        public bool limitAngularVelocity { get { return m_LimitAngularVelocity; } set { m_LimitAngularVelocity = value; } }
        public float maxLinearVelocity { get { return m_MaxLinearVelocity; } set { m_MaxLinearVelocity = value; } }
        public float maxAngularVelocity { get { return m_MaxAngularVelocity; } set { m_MaxAngularVelocity = value; } }

        P2DVelocityLimitController m_VelocityLimitController;

        internal override sealed void PhysicsUpdate()
        {
            if (_internalController == null)
            {
                _internalController = m_VelocityLimitController = new P2DVelocityLimitController(transform, _rigidbodies, GetAABB(true), m_MaxLinearVelocity, m_MaxAngularVelocity);
            }

            m_VelocityLimitController.container = GetAABB(false);
            m_VelocityLimitController.MaxLinearVelocity = m_MaxLinearVelocity;
            m_VelocityLimitController.MaxAngularVelocity = m_MaxAngularVelocity;
            m_VelocityLimitController.LimitLinearVelocity = m_LimitLinearVelocity;
            m_VelocityLimitController.LimitAngularVelocity = m_LimitAngularVelocity;

            m_VelocityLimitController.Update(Time.deltaTime);
        }
    }
}
