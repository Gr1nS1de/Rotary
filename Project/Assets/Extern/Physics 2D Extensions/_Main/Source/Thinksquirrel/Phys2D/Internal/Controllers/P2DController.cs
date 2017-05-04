//
// P2DController.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using Thinksquirrel.Phys2D;
using System.Collections.Generic;

namespace Thinksquirrel.Phys2D.Internal.Controllers
{
    abstract class P2DController : FilterData
    {
        public Transform xf { get; set; }
        public List<Rigidbody2D> bodies { get; set; }
        public virtual AABB container { get; set; }

        internal ControllerType _type;

        HashSet<Rigidbody2D> m_UniqueBodies = new HashSet<Rigidbody2D>();
        const int MaxQuerySize = 256;
        Collider2D[] m_QueryColliders = new Collider2D[MaxQuerySize];

        public P2DController(ControllerType controllerType, Transform xf, List<Rigidbody2D> bodies, AABB container)
        {
            _type = controllerType;
            this.xf = xf;
            this.bodies = bodies;
            this.container = container;
        }

        public HashSet<Rigidbody2D> FindUniqueBodies()
        {
            m_UniqueBodies.Clear();

            if (Mathf.Abs(container.Extents.SqrMagnitude() - 0) < 1.192092896e-07f)
            {
                foreach(var body in bodies)
                {
                    m_UniqueBodies.Add(body);
                }
                return m_UniqueBodies;
            }

            int queryCount = 
                Physics2D.OverlapAreaNonAlloc(
                    container.LowerBound, 
                    container.UpperBound,
                    m_QueryColliders,
                    enabledOnLayers);
            
            for(int i = 0; i < queryCount; ++i)
            {
                var body = m_QueryColliders[i].attachedRigidbody;

                if (!body)
                    continue;

                if (bodies.Contains(body) && IsActiveOn(body))
                {
                    m_UniqueBodies.Add(body);
                }
            }

            return m_UniqueBodies;
        }

        public override bool IsActiveOn(Rigidbody2D body)
        {
            if (!body)
                return false;

            if (!body.gameObject.activeInHierarchy)
                return false;

            ControllerFilter2DExt filter;

            if (!ControllerFilter2DExt._filterMap.TryGetValue(body, out filter))
            {
                filter = null;
            }

            if (!filter || !filter.enabled)
                return base.IsActiveOn(body);

            if (filter.IsControllerTypeIgnored(_type))
                return false;

            return base.IsActiveOn(body);
        }

        public abstract void Update(float dt);
    }
}