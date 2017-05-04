//
// P2DGravityController.cs
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
    sealed class P2DGravityController : P2DController
    {
        public List<Vector2> Points;

        public P2DGravityController(Transform xf, List<Rigidbody2D> bodies, AABB container, List<Vector2> pointList, float strength, float maxRadius, float minRadius)
            : base(ControllerType.GravityController, xf, bodies, container)
        {
            Points = pointList;
            MinRadius = minRadius;
            MaxRadius = maxRadius;
            Strength = strength;
        }

        public float MinRadius { get; set; }
        public float MaxRadius { get; set; }
        public float Strength { get; set; }
        public GravityType GravityType { get; set; }

        public override void Update(float dt)
        {
            Vector2 f = Vector2.zero;

            var uniqueBodies = FindUniqueBodies();
            var bodiesEnumeratorA = uniqueBodies.GetEnumerator();
            var bodiesEnumeratorB = bodiesEnumeratorA;

            while(bodiesEnumeratorA.MoveNext())
            {
                var body1 = (Rigidbody2DExt)bodiesEnumeratorA.Current;

                while(bodiesEnumeratorB.MoveNext())
                {
                    var body2 = (Rigidbody2DExt)bodiesEnumeratorB.Current;

                    var d = body2.worldCenterOfMass - body1.worldCenterOfMass;
                    float r2 = d.SqrMagnitude();

                    if (r2 < 1.192092896e-07f)
                        continue;

                    float r = Mathf.Sqrt(r2);

                    if (r >= MaxRadius || r <= MinRadius)
                        continue;

                    switch (GravityType)
                    {
                        case GravityType.DistanceSquared:
                            f = Strength / r2 / r * body1.mass * body2.mass * d;
                            break;
                        case GravityType.Linear:
                            f = Strength / r2 * body1.mass * body2.mass * d;
                            break;
                    }

                    body1.AddForce(f);
                    body2.AddForce(-f);
                }

                for (int k = 0; k < Points.Count; k++)
                {
                    var point = Points[k];
                  
                    var d = xf.GetRelativePoint(point) - body1.worldCenterOfMass;
                    float r2 = d.SqrMagnitude();

                    if (r2 < 1.192092896e-07f)
                        continue;

                    float r = d.magnitude;

                    if (r >= MaxRadius || r <= MinRadius)
                        continue;

                    switch (GravityType)
                    {
                        case GravityType.DistanceSquared:
                            f = Strength / r2 / r * body1.mass * d;
                            break;
                        case GravityType.Linear:
                            f = Strength / r2 * body1.mass * d;
                            break;
                    }

                    body1.AddForce(f);
                }
            }
        }
    }
}