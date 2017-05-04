//
// BuoyancyController2DExt.cs
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
    [AddComponentMenu("Physics 2D/Physics Controllers/Buoyancy Controller 2D (P2D Extensions)")]
    public sealed class BuoyancyController2DExt : Controller2DExt
    {
        [SerializeField] Vector2 m_Velocity = Vector2.zero;
        [SerializeField] float m_Density = 0.5f;
        [SerializeField] float m_LinearDragCoefficient = 0.987f;
        [SerializeField] float m_AngularDragCoefficient =  0.987f;

        public Vector2 velocity { get { return m_Velocity; } set { m_Velocity = value; } }
        public float density { get { return m_Density; } set { m_Density = value; } }
        public float linearDragCoefficient { get { return m_LinearDragCoefficient; } set { m_LinearDragCoefficient = value; } }
        public float angularDragCoefficient { get { return m_AngularDragCoefficient; } set { m_AngularDragCoefficient = value; } }

        P2DBuoyancyController m_BuoyancyController;

        public void RecalculateMassData(Rigidbody2D body)
        {
            if (m_BuoyancyController != null) m_BuoyancyController.RecalculateMassData(body);
        }

        public void RecalculateMassDataAll()
        {
            if (m_BuoyancyController != null) m_BuoyancyController.RecalculateMassDataAll();
        }

        internal override sealed void PhysicsUpdate()
        {
            if (_internalController == null)
            {
                _internalController = m_BuoyancyController = new P2DBuoyancyController(
                  transform,
                  _rigidbodies,
                  GetAABB(true),
                  m_Density,
                  m_LinearDragCoefficient,
                  m_AngularDragCoefficient,
                  Physics2D.gravity);
            }

            m_BuoyancyController.container = GetAABB(false);
            m_BuoyancyController.Velocity = m_Velocity;
            m_BuoyancyController.Density = m_Density;
            m_BuoyancyController.LinearDragCoefficient = m_LinearDragCoefficient;
            m_BuoyancyController.AngularDragCoefficient = m_AngularDragCoefficient;
            m_BuoyancyController.Gravity = Physics2D.gravity;

            Solve();
        }
    }
}
