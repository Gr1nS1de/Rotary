//
// P2DAbstractForceController.cs
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
    abstract class P2DAbstractForceController : P2DController
    {
        /// <summary>
        /// Curve to be used for Decay in Curve mode
        /// </summary>
        public AnimationCurve DecayCurve;

        /// <summary>
        /// The Forcetype of the instance
        /// </summary>
        public ForceType forceType;

        /// <summary>
        /// Provided for reuse to provide Variation functionality in 
        /// inheriting classes
        /// </summary>
        protected System.Random Randomize;

        /// <summary>
        /// Curve used by Curve Mode as an animated multiplier for the force 
        /// strength.
        /// Only positions between 0 and 1 are considered as that range is 
        /// stretched to have ImpulseLength.
        /// </summary>
        public AnimationCurve StrengthCurve;

        public float TimeScale;

        /// <summary>
        /// Constructor
        /// </summary>
        public P2DAbstractForceController(Transform xf, List<Rigidbody2D> bodies, AABB container)
            : base(ControllerType.WindController, xf, bodies, container)
        {
            Strength = 1.0f;
            Position = new Vector2(0, 0);
            MaximumSpeed = 100.0f;
            TimingMode = WindType.Constant;
            ImpulseTime = 0.0f;
            Variation = 0.0f;
            Randomize = new System.Random();
            DecayMode = DecayMode.None;

            DecayStart = 0.0f;
            DecayEnd = 0.0f;

            StrengthCurve = new AnimationCurve();
            StrengthCurve.AddKey(new Keyframe(0, 0));
            StrengthCurve.AddKey(new Keyframe(0.1f, -4));
            StrengthCurve.AddKey(new Keyframe(0.2f, 5));
            StrengthCurve.AddKey(new Keyframe(0.4f, 5));
            StrengthCurve.AddKey(new Keyframe(0.5f, 0));
            StrengthCurve.AddKey(new Keyframe(0.65f, -3));
            StrengthCurve.AddKey(new Keyframe(0.8f, 0));
            StrengthCurve.postWrapMode = WrapMode.Loop;

            DecayCurve = new AnimationCurve();
            DecayCurve = AnimationCurve.EaseInOut(0, 1, 10, 0);
        }

        /// <summary>
        /// Global Strength of the force to be applied
        /// </summary>
        public float Strength { get; set; }

        /// <summary>
        /// Position of the Force. Can be ignored (left at (0,0) for forces
        /// that are not position-dependent
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Maximum speed of the bodies. Bodies that are travelling faster are
        /// supposed to be ignored
        /// </summary>
        public float MaximumSpeed { get; set; }

        /// <summary>
        /// Maximum Force to be applied. As opposed to Maximum Speed this is 
        /// independent of the velocity of
        /// the affected body
        /// </summary>
        public float MaximumForce { get; set; }

        /// <summary>
        /// Timing Mode of the force instance
        /// </summary>
        public WindType TimingMode { get; set; }

        /// <summary>
        /// Time of the current impulse. Incremented in update till 
        /// ImpulseLength is reached
        /// </summary>
        public float ImpulseTime { get; private set; }

        /// <summary>
        /// Variation of the force applied to each body affected
        /// !! Must be used in inheriting classes properly !!
        /// </summary>
        public float Variation { get; set; }

        /// <summary>
        /// See DecayModes
        /// </summary>
        public DecayMode DecayMode { get; set; }

        /// <summary>
        /// Start of the distance based Decay. To set a non decaying area
        /// </summary>
        public float DecayStart { get; set; }

        /// <summary>
        /// Maximum distance a force should be applied
        /// </summary>
        public float DecayEnd { get; set; }

        /// <summary>
        /// Calculate the Decay for a given body. Meant to ease force 
        /// development and stick to the DRY principle and provide unified and 
        /// predictable decay math.
        /// </summary>
        /// <param name="body">The body to calculate decay for</param>
        /// <returns>A multiplier to multiply the force with to add decay 
        /// support in inheriting classes</returns>
        protected float GetDecayMultiplier(Rigidbody2D body)
        {
            //TODO: Consider ForceType in distance calculation!
            // TODO: transform.position to position
            float distance = ((Vector2)body.transform.position - Position).magnitude;
            switch (DecayMode)
            {
                case DecayMode.None:
                    {
                        return 1.0f;
                    }
                case DecayMode.Step:
                    {
                        if (distance < DecayEnd)
                            return 1.0f;
                        else
                            return 0.0f;
                    }
                case DecayMode.Linear:
                    {
                        if (distance < DecayStart)
                            return 1.0f;
                        if (distance > DecayEnd)
                            return 0.0f;
                        return (DecayEnd - DecayStart / distance - DecayStart);
                    }
                case DecayMode.InverseSquare:
                    {
                        if (distance < DecayStart)
                            return 1.0f;
                        else
                            return 1.0f / ((distance - DecayStart) * (distance - DecayStart));
                    }
                case DecayMode.Curve:
                    {
                        if (distance < DecayStart)
                            return 1.0f;
                        else
                            return DecayCurve.Evaluate(distance - DecayStart);
                    }
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Inherited from Controller
        /// Depending on the TimingMode perform timing logic and call ApplyForce()
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            switch (TimingMode)
            {
                case WindType.Constant:
                    {
                        ApplyForce(dt, Strength);
                        break;
                    }
                case WindType.Curve:
                    {
                        ApplyForce(dt, Strength * StrengthCurve.Evaluate(ImpulseTime * TimeScale));
                        ImpulseTime += dt;
                        break;
                    }
            }
        }

        /// <summary>
        /// Apply the force supplying strength (wich is modified in Update() 
        /// according to the TimingMode
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strength">The strength</param>
        public abstract void ApplyForce(float dt, float strength);
    }
}