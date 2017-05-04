//
// P2DSimpleWindForce.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using System.Collections.Generic;
using Thinksquirrel.Phys2D;

namespace Thinksquirrel.Phys2D.Internal.Controllers
{
    /// <summary>
    /// Reference implementation for forces based on AbstractForceController
    /// It supports all features provided by the base class and illustrates proper
    /// usage as an easy to understand example.
    /// As a side-effect it is a nice and easy to use wind force for your projects
    /// </summary>
    sealed class P2DSimpleWindForce : P2DAbstractForceController
    {
        /// <summary>
        /// Direction of the windforce
        /// </summary>
        public Vector2 Direction { get; set; }

        /// <summary>
        /// The amount of Direction randomization. Allowed range is 0-1.
        /// </summary>
        public float Divergence { get; set; }

        /// <summary>
        /// Ignore the position and apply the force. If off only in the "front" (relative to position and direction)
        /// will be affected
        /// </summary>
        public bool IgnorePosition { get; set; }

        internal Vector2 _force;
    
        public P2DSimpleWindForce(Transform xf, List<Rigidbody2D> bodies, AABB container)
            : base(xf, bodies, container)
        {
        }


        public override void ApplyForce(float dt, float strength)
        {
            var uniqueBodies = FindUniqueBodies();
            var enumerator = uniqueBodies.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var body = enumerator.Current;

                if (!IsActiveOn(body))
                    continue;

                //TODO: Consider Force Type
                float decayMultiplier = GetDecayMultiplier(body);

                if (decayMultiplier != 0)
                {
                    Vector2 forceVector;

                    if (forceType == ForceType.Point)
                    {
                        forceVector = (Vector2)body.transform.position - Position;
                    }
                    else
                    {
                        Direction.Normalize();

                        forceVector = Direction;

                        if (forceVector.SqrMagnitude() == 0)
                            forceVector = new Vector2(0, 1);
                    }
                    forceVector += Random.insideUnitCircle * Divergence * (float)Randomize.NextDouble();

                    // Calculate random Variation
                    if (Variation != 0)
                    {
                        float strengthVariation = (float)Randomize.NextDouble() * Mathf.Clamp01(Variation);
                        forceVector.Normalize();
                        _force = forceVector * strength * decayMultiplier * strengthVariation;
                    }
                    else
                    {
                        forceVector.Normalize();
                        _force = forceVector * strength * decayMultiplier;
                    }
                    body.AddForce(_force);
                }
            }
        }
    }
}