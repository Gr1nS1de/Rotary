//
// GravityController2DExt.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using System.Collections.Generic;
using Thinksquirrel.Phys2D.Internal;
using Thinksquirrel.Phys2D.Internal.Controllers;

namespace Thinksquirrel.Phys2D
{
    [AddComponentMenu("Physics 2D/Physics Controllers/Gravity Controller 2D (P2D Extensions)")]
    public sealed class GravityController2DExt : Controller2DExt
    {
        // Analysis disable FieldCanBeMadeReadOnly.Local
        [SerializeField] GravityType m_GravityType = GravityType.DistanceSquared;
        [SerializeField] float m_Strength = 1f;
        [SerializeField] float m_MinRadius;
        [SerializeField] float m_MaxRadius = float.MaxValue;
        [SerializeField] List<Vector2> m_Points = new List<Vector2>();
        // Analysis restore FieldCanBeMadeReadOnly.Local

        public GravityType gravityType { get { return m_GravityType; } set { m_GravityType = value; } }
        public float strength { get { return m_Strength; } set { m_Strength = value; } }
        public float minRadius { get { return m_MinRadius; } set { m_MinRadius = value; } }
        public float maxRadius { get { return m_MaxRadius; } set { m_MaxRadius = value; } }
        public List<Vector2> points { get { return m_Points; } }

        P2DGravityController m_GravityController;

        internal override sealed void PhysicsUpdate()
        {
            if (_internalController == null)
            {
                _internalController = m_GravityController = new P2DGravityController(transform, _rigidbodies, GetAABB(true), m_Points, m_Strength, m_MaxRadius, m_MinRadius);
            }

            m_GravityController.container = GetAABB(false);
            m_GravityController.GravityType = m_GravityType;
            m_GravityController.Strength = m_Strength;
            m_GravityController.MinRadius = m_MinRadius;
            m_GravityController.MaxRadius = m_MaxRadius;

            Solve();
        }
    }

    public enum GravityType
    {
        Linear,
        DistanceSquared
    }
}
