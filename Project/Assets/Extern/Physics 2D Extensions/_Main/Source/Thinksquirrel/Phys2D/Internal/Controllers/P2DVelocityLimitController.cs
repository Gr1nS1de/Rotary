//
// P2DVelocityLimitController.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using System.Collections.Generic;
using Thinksquirrel.Phys2D;
using UnityEngine;

namespace Thinksquirrel.Phys2D.Internal.Controllers
{
    /// <summary>
    /// Put a limit on the linear (translation - the movespeed) and angular (rotation) velocity
    /// of bodies added to this controller.
    /// </summary>
    sealed class P2DVelocityLimitController : P2DController
    {
        public bool LimitAngularVelocity = true;
        public bool LimitLinearVelocity = true;
        private float _maxAngularSqared;
        private float _maxAngularVelocity;
        private float _maxLinearSqared;
        private float _maxLinearVelocity;

        /// <summary>
        /// Initializes a new instance of the <see cref="VelocityLimitController"/> class.
        /// Pass in 0 or float.MaxValue to disable the limit.
        /// maxAngularVelocity = 0 will disable the angular velocity limit.
        /// </summary>
        /// <param name="maxLinearVelocity">The max linear velocity.</param>
        /// <param name="maxAngularVelocity">The max angular velocity.</param>
        public P2DVelocityLimitController(Transform xf, List<Rigidbody2D> bodies, AABB container, float maxLinearVelocity, float maxAngularVelocity)
            : base(ControllerType.VelocityLimitController, xf, bodies, container)
        {
            if (maxLinearVelocity == 0 || maxLinearVelocity == float.MaxValue)
                LimitLinearVelocity = false;

            if (maxAngularVelocity == 0 || maxAngularVelocity == float.MaxValue)
                LimitAngularVelocity = false;

            MaxLinearVelocity = maxLinearVelocity;
            MaxAngularVelocity = maxAngularVelocity;
        }

        /// <summary>
        /// Gets or sets the max angular velocity.
        /// </summary>
        /// <value>The max angular velocity.</value>
        public float MaxAngularVelocity
        {
            get { return _maxAngularVelocity; }
            set
            {
                if (_maxAngularVelocity != value)
                {
                    _maxAngularVelocity = value;
                    _maxAngularSqared = _maxAngularVelocity * _maxAngularVelocity;
                }
            }
        }

        /// <summary>
        /// Gets or sets the max linear velocity.
        /// </summary>
        /// <value>The max linear velocity.</value>
        public float MaxLinearVelocity
        {
            get { return _maxLinearVelocity; }
            set
            {
                if (_maxLinearVelocity != value)
                {
                    _maxLinearVelocity = value;
                    _maxLinearSqared = _maxLinearVelocity * _maxLinearVelocity;
                }
            }
        }

        public override void Update(float dt)
        {
            var uniqueBodies = FindUniqueBodies();
            var enumerator = uniqueBodies.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var body = enumerator.Current;

                if (!IsActiveOn(body))
                    continue;

                if (LimitLinearVelocity)
                {
                    //Translation
                    // Check for large velocities.
                    float translationX = dt * body.velocity.x;
                    float translationY = dt * body.velocity.y;
                    float result = translationX * translationX + translationY * translationY;

                    if (result > dt * _maxLinearSqared)
                    {
                        float sq = Mathf.Sqrt(result);

                        float ratio = _maxLinearVelocity / sq;
                        Vector2 v = body.velocity;
                        v.x *= ratio;
                        v.y *= ratio;
                        body.velocity = v * dt;
                    }
                }

                if (LimitAngularVelocity)
                {
                    //Rotation
                    float rotation = dt * body.angularVelocity * Mathf.Deg2Rad;
                    if (rotation * rotation > _maxAngularSqared)
                    {
                        float ratio = _maxAngularVelocity / Mathf.Abs(rotation);
                        body.angularVelocity *= ratio * Mathf.Rad2Deg * dt;
                    }
                }
            }
        }
    }
}