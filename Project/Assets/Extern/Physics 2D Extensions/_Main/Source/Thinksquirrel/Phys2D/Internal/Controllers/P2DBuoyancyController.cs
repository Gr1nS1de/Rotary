//
// P2DBuoyancyController.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using System.Collections.Generic;
using Thinksquirrel.Phys2D;
using Thinksquirrel.Phys2D.Internal;

namespace Thinksquirrel.Phys2D.Internal.Controllers
{
    sealed class P2DBuoyancyController : P2DController
    {
        /// <summary>
        /// Controls the rotational drag that the fluid exerts on the bodies within it. Use higher values will simulate thick fluid, like honey, lower values to
        /// simulate water-like fluids. 
        /// </summary>
        public float AngularDragCoefficient;

        /// <summary>
        /// Density of the fluid. Higher values will make things more buoyant, lower values will cause things to sink.
        /// </summary>
        public float Density;

        /// <summary>
        /// Controls the linear drag that the fluid exerts on the bodies within it.  Use higher values will simulate thick fluid, like honey, lower values to
        /// simulate water-like fluids.
        /// </summary>
        public float LinearDragCoefficient;

        /// <summary>
        /// Acts like waterflow. Defaults to 0,0.
        /// </summary>
        public Vector2 Velocity;

        private Vector2 _gravity;
        private Vector2 _normal;
        private float _offset;
        private Rigidbody2DExt[] _bodyCache;
        private Dictionary<Rigidbody2DExt, MassData[]> _bodyMap = new Dictionary<Rigidbody2DExt, MassData[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BuoyancyController"/> class.
        /// </summary>
        /// <param name="container">Only bodies inside this Aabb will be influenced by the controller</param>
        /// <param name="density">Density of the fluid</param>
        /// <param name="linearDragCoefficient">Linear drag coefficient of the fluid</param>
        /// <param name="rotationalDragCoefficient">Rotational drag coefficient of the fluid</param>
        /// <param name="gravity">The direction gravity acts. Buoyancy force will act in opposite direction of gravity.</param>
        public P2DBuoyancyController(Transform xf, List<Rigidbody2D> bodies, 
                                     AABB container, 
                                     float density, float linearDragCoefficient,
                                     float rotationalDragCoefficient, Vector2 gravity)
            : base(ControllerType.BuoyancyController, xf, bodies, container)
        {
            _normal = -gravity;
            _normal.Normalize();
            _offset = base.container.UpperBound.y;
            Density = density;
            LinearDragCoefficient = linearDragCoefficient;
            AngularDragCoefficient = rotationalDragCoefficient;
            _gravity = gravity;
        }

        public override AABB container
        {
            get
            {
                return base.container;
            }
            set
            {
                if (value.LowerBound != base.container.LowerBound || value.UpperBound != base.container.UpperBound)
                {
                    base.container = value;
                    _offset = base.container.UpperBound.y; // TODO: Better offset calculation
                }
            }
        }

        public Vector2 Gravity
		{
			get { return _gravity; }
			set
			{
				_gravity = value;
			}
		}

        public void RecalculateMassData(Rigidbody2D body)
        {
            int ind = bodies.IndexOf(body);

            if (ind < 0)
                return;

            UpdateBody(body, ind);
        }

        public void RecalculateMassDataAll()
        {
            for(int i = 0; i < bodies.Count; ++i)
                UpdateBody(bodies[i], i);
        }

        void UpdateBody(Rigidbody2DExt body, int index)
        {
            if (!body)
                return;

            _bodyCache[index] = body;

            MassData[] massData;
            body.CalculateMassData(out massData);

            if (_bodyMap.ContainsKey(body))
            {
                _bodyMap[body] = massData;
            }
            else
            {
                _bodyMap.Add(body, massData);
            }
        }

        public override void Update(float dt)
        {
            System.Array.Resize(ref _bodyCache, bodies.Count);

            for(int i = 0; i < bodies.Count; ++i)
            {
                if (bodies[i] != _bodyCache[i])
                {
                    UpdateBody(bodies[i], i);
                }
            }

            var uniqueBodies = FindUniqueBodies();
            var enumerator = uniqueBodies.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var body = (Rigidbody2DExt)enumerator.Current;
                var massData = _bodyMap[body];
               
                Vector2 areac = Vector2.zero;
                Vector2 massc = Vector2.zero;
                float area = 0;
                float mass = 0;

                for (int j = 0; j < massData.Length; j++)
                {
                    var md = massData[j];
                    var shape = md.shape;

                    if (shape is EdgeCollider2D)
                        continue;

                    Vector2 sc;
                    float sarea = md.ComputeSubmergedArea(_normal, _offset, out sc);
                    area += sarea;
                    areac.x += sarea * sc.x;
                    areac.y += sarea * sc.y;

                    mass += sarea;
                    massc.x += sarea * sc.x;
                    massc.y += sarea * sc.y;
                }

                areac.x /= area;
                areac.y /= area;
                massc.x /= mass;
                massc.y /= mass;

                if (area < 1.192092896e-07f)
                    continue;

                //Buoyancy
                Vector2 buoyancyForce = -Density * area * _gravity;

                body.AddForceAtPosition(buoyancyForce, massc);

                //Linear drag
                Vector2 dragForce = body.GetLinearVelocityFromWorldPoint(areac) - Velocity;
                dragForce *= -LinearDragCoefficient * area;
                body.AddForceAtPosition(dragForce, areac);

                //Angular drag
                body.AddTorque(-body.inertia / body.mass * area * body.angularVelocity * AngularDragCoefficient);
            }
        }
    }
}