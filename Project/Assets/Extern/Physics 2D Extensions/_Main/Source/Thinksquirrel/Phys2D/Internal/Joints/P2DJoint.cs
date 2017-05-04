//
// P2DJoint.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using Thinksquirrel.Phys2D;

namespace Thinksquirrel.Phys2D.Internal.Joints
{
    enum JointType
    {
        Angle,
        Hinge,
        Slider,
        Distance,
        Pulley,
        Gear,
        Wheel,
        Weld,
        Friction,
        FixedMouse
    }

    abstract class P2DJoint
    {
        internal float _breakForce = Mathf.Infinity;
        internal float _breakTorque = Mathf.Infinity;

        internal bool _enabled = true;

        protected P2DJoint()
        {
        }

        protected P2DJoint(Rigidbody2DExt body, Rigidbody2DExt bodyB)
        {
            _bodyA = body;
            _bodyB = bodyB;

            //Connected bodies should not collide by default
            collideConnected = false;
        }

        /// <summary>
        /// Constructor for fixed joint
        /// </summary>
        protected P2DJoint(Rigidbody2D body)
        {
            _bodyA = body;

            //Connected bodies should not collide by default
            collideConnected = false;
        }

        /// <summary>
        /// Gets or sets the type of the joint.
        /// </summary>
        /// <value>The type of the joint.</value>
        internal JointType _jointType;

        /// <summary>
        /// Get the first body attached to this joint.
        /// </summary>
        /// <value></value>
        internal Rigidbody2DExt _bodyA;

        /// <summary>
        /// Get the second body attached to this joint.
        /// </summary>
        /// <value></value>
        internal Rigidbody2DExt _bodyB;

        /// <summary>
        /// Get the anchor point on bodyA in world coordinates.
        /// </summary>
        /// <value></value>
        internal virtual Vector2 worldAnchorA { get { return _bodyA.position; } }

        /// <summary>
        /// Get the anchor point on bodyB in world coordinates.
        /// </summary>
        /// <value></value>
        internal virtual Vector2 worldAnchorB
        {
            get { return _bodyB.position; }
            set { Debug.LogError("You can't set the world anchor on this joint type."); }
        }

        /// <summary>
        /// Set this flag to true if the attached bodies should collide.
        /// </summary>
        internal bool collideConnected;

        /// <summary>
        /// Fires when the joint is broken.
        /// </summary>
        internal event System.Action<P2DJoint, float> OnJointBreak;

        /// <summary>
        /// Get the reaction force on bodyB at the joint anchor in Newtons.
        /// </summary>
        /// <param name="inv_dt">The inv_dt.</param>
        /// <returns></returns>
        internal abstract Vector2 GetReactionForce(float invDt);

        /// <summary>
        /// Get the reaction torque on bodyB in N*m.
        /// </summary>
        /// <param name="inv_dt">The inv_dt.</param>
        /// <returns></returns>
        internal abstract float GetReactionTorque(float invDt);

        protected void WakeBodies()
        {
            _bodyA.cachedRigidbody2D.WakeUp();
            if (_bodyB != null)
            {
                _bodyB.cachedRigidbody2D.WakeUp();
            }
        }

        internal abstract void InitVelocityConstraints();

        internal void Validate(float invDT)
        {
            if (!_enabled)
                return;

            float breakForce = GetReactionForce(invDT).magnitude;
            float breakTorque = GetReactionTorque(invDT);

            if ((float.IsNaN(breakForce) || Mathf.Abs(breakForce) <= _breakForce) && (float.IsNaN(breakTorque) || Mathf.Abs(breakTorque) <= _breakTorque))
                return;

            _enabled = false;

            if (OnJointBreak != null)
                OnJointBreak(this, Mathf.Max(breakForce, breakTorque));
        }

        internal abstract void SolveVelocityConstraints();

        /// <summary>
        /// Solves the position constraints.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>returns true if the position errors are within tolerance.</returns>
        internal abstract bool SolvePositionConstraints();
    }
}
