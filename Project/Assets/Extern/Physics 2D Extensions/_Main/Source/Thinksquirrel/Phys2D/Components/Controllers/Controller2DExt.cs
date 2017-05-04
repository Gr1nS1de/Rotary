//
// Controller2DExt.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using Thinksquirrel.Phys2D.Internal.Controllers;
using System.Collections.Generic;
using Thinksquirrel.Phys2D.Internal;

namespace Thinksquirrel.Phys2D
{
    public abstract class Controller2DExt : P2DPhysicsBase
    {
        [SerializeField] LayerMask m_EnabledOnLayers = ~0;
        [SerializeField] internal List<Rigidbody2D> _rigidbodies = new List<Rigidbody2D>();
        [SerializeField] Vector2 m_Offset;
        [SerializeField] Vector2 m_Dimensions = new Vector2(5, 5);

        public LayerMask enabledOnLayers { get { return m_EnabledOnLayers; } set { m_EnabledOnLayers = value; } }
        public Vector2 offset { get { return m_Offset; } set { m_Offset = value; } }
        public Vector2 dimensions { get { return m_Dimensions; } set { m_Dimensions = value; } }

        public Rigidbody2D GetRigidbody(int index)
        {
            return _rigidbodies[index];
        }
        public int GetRigidbodyCount()
        {
            return _rigidbodies.Count;
        }
        public void AddRigidbody(Rigidbody2D body) { _rigidbodies.Add(body); }
        public bool RemoveRigidbody(Rigidbody2D body) { return _rigidbodies.Remove(body); }

        internal P2DController _internalController;

        Vector2 m_CachedCenter;
        Vector2 m_CachedSize;
        Vector3 m_CachedPosition;
        AABB m_CachedAABB;

        internal AABB GetAABB(bool force)
        {
            if (force || m_CachedCenter != m_Offset || m_CachedSize != m_Dimensions || transform.position != m_CachedPosition)
            {
                m_CachedCenter = m_Offset;
                m_CachedSize = m_Dimensions;
                m_CachedPosition = transform.position;
                Vector2 p = (Vector2)transform.position + m_Offset;
                Vector2 aa = new Vector2(p.x - m_Dimensions.x * .5f, p.y - m_Dimensions.y * .5f);
                Vector2 bb = new Vector2(p.x + m_Dimensions.x * .5f, p.y + m_Dimensions.y * .5f);

                m_CachedAABB = new AABB(aa, bb);
            }

            return m_CachedAABB;
        }

        internal void Solve()
        {
            _internalController.enabledOnLayers = m_EnabledOnLayers;
            _internalController.Update(Time.deltaTime);
        }

        public bool IsActiveOn(Rigidbody2D body)
        {
            return _internalController.IsActiveOn(body);
        }

        protected override sealed int executionLevel { get { return 3; } }
    }
}
