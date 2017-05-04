//
// Joint2DExt.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
#define UNITY_5_3_PLUS
#endif
using System.Collections.Generic;
using UnityEngine;
using Thinksquirrel.Phys2D.Internal.Joints;

namespace Thinksquirrel.Phys2D
{
    public abstract class Joint2DExt : P2DPhysicsBase
    {
        [SerializeField] bool m_CollideConnected;
        [SerializeField] Rigidbody2D m_ConnectedRigidBody;
        [SerializeField] float m_BreakForce = Mathf.Infinity;
        [SerializeField] float m_BreakTorque = Mathf.Infinity;

        /// <summary>
        /// If true, the bodies attached to this joint will collide.
        /// </summary>
        public bool collideConnected
        {
            get
            {
                return m_CollideConnected;
            }
            set
            {
                m_CollideConnected = value;
                SetCollideConnected();
            }
        }
        /// <summary>
        /// Gets or sets the connected body.
        /// </summary>
        /// <remarks>
        /// Changing the connected value at runtime will automatically recalculate mass data.
        /// If this is set to null, the body will be anchored to the world.
        /// </remarks>
        public virtual Rigidbody2D connectedBody { get { return m_ConnectedRigidBody; } set { m_ConnectedRigidBody = value; } }
        /// <summary>
        /// The force that needs to be applied for this joint to break.
        /// </summary>
        /// <remarks>
        /// When a joint breaks, the OnJointBreak message is sent to the joint's game object, with an optional breakForce float parameter.
        /// </remarks>
        public virtual float breakForce { get { return m_BreakForce; } set { m_BreakForce = value; } }
        /// <summary>
        /// The torque that needs to be applied for this joint to break.
        /// </summary>
        /// <remarks>
        /// When a joint breaks, the OnJointBreak message is sent to the joint's game object, with an optional breakForce float parameter.
        /// </remarks>
        public virtual float breakTorque { get { return m_BreakTorque; } set { m_BreakTorque = value; } }

        [System.NonSerialized] bool m_CollideConnectedCached;
        [System.NonSerialized] Rigidbody2D m_ConnectedBodyCached;
        [System.NonSerialized] Rigidbody2DExt m_WorldAnchor;
        [System.NonSerialized] bool m_DebugMessageSent;
        [System.NonSerialized] internal P2DJoint _internalJoint;
        [System.NonSerialized] readonly List<Collider2DPair> m_OldCollisionStates = new List<Collider2DPair>();

        struct Collider2DPair
        {
            public Collider2D colliderA;
            public Collider2D colliderB;
            public bool ignore;
        }
        protected override sealed void OnEnable()
        {
            base.OnEnable();

            if (connectedBody)
                SetCollideConnected();
        }

        void SetCollideConnected()
        {
            if (!Application.isPlaying)
                return;

            for (int i = 0, l = m_OldCollisionStates.Count; i < l; ++i)
            {
                var pair = m_OldCollisionStates [i];
                Physics2D.IgnoreCollision(pair.colliderA, pair.colliderB, pair.ignore);
            }

            m_OldCollisionStates.Clear();

            var collidersA = ((Rigidbody2DExt)cachedRigidbody2D).GetColliders();
            var collidersB = ((Rigidbody2DExt)connectedBody).GetColliders();

            for (int i = 0, lI = collidersA.Count; i < lI; ++i)
            {
                var colliderA = collidersA [i] as Collider2D;

                for(int j = 0, lJ = collidersB.Count; j < lJ; ++j)
                {
                    var colliderB = collidersB [j] as Collider2D;

                    m_OldCollisionStates.Add(new Collider2DPair
                    {
                        colliderA = colliderA,
                        colliderB = colliderB,
                        ignore = Physics2D.GetIgnoreCollision(colliderA, colliderB)
                    });
                    Physics2D.IgnoreCollision(colliderA, colliderB, !m_CollideConnected);
                }
            }

            m_CollideConnectedCached = m_CollideConnected;

            m_ConnectedBodyCached = connectedBody;
        }
        internal Rigidbody2DExt GetBodyB()
        {
            if (connectedBody)
            {
                return connectedBody;
            }
            if (!m_WorldAnchor)
            {
                var worldAnchorObject = new GameObject("Physics2D World Anchor");
                worldAnchorObject.hideFlags = HideFlags.HideAndDontSave;

                var worldAnchorTransform = worldAnchorObject.transform;
                worldAnchorTransform.position = Vector3.zero;
                worldAnchorTransform.rotation = Quaternion.identity;

                var worldAnchorBody = worldAnchorObject.AddComponent<Rigidbody2D>();
                worldAnchorBody.isKinematic = true;
                #if UNITY_5_3_PLUS
                worldAnchorBody.constraints = RigidbodyConstraints2D.FreezeAll;
                #else
                worldAnchorBody.fixedAngle = true;
                #endif

                m_WorldAnchor = worldAnchorBody;
            }
            return m_WorldAnchor;
        }

        internal void SubscribeToBreakEvent()
        {
            _internalJoint.OnJointBreak += OnP2DJointBreak;
        }
        internal void Solve()
        {
            if (!VerifyJoint())
                return;

            _internalJoint._bodyA = cachedRigidbody2D;
            _internalJoint._bodyB = GetBodyB();
            _internalJoint._breakForce = m_BreakForce;
            _internalJoint._breakTorque = m_BreakTorque;

            if (!_internalJoint._bodyA.simulated || !_internalJoint._bodyB.simulated)
                return;

            if (connectedBody && (m_CollideConnectedCached != m_CollideConnected || m_ConnectedBodyCached != connectedBody))
                SetCollideConnected();
            
            _internalJoint.InitVelocityConstraints();

            for(int i = 0; i < Physics2D.velocityIterations; ++i)
            {
                _internalJoint.SolveVelocityConstraints();
            }
            for(int i = 0; i < Physics2D.positionIterations; ++i)
            {
                if (_internalJoint.SolvePositionConstraints())
                    break;
            }

            // For breaking joints
            _internalJoint.Validate(Time.deltaTime);
        }

        void OnP2DJointBreak(P2DJoint internalJoint, float breakForce)
        {
            SendMessage("OnJointBreak", breakForce, SendMessageOptions.DontRequireReceiver);
            Object.Destroy(this);
        }

        bool VerifyJoint()
        {
            if (cachedRigidbody2D == connectedBody)
            {
                if (!m_DebugMessageSent)
                {
                    Debug.LogWarning(string.Format("Cannot create 2D joint on '{0}' as it connects to itself.", name));
                    m_DebugMessageSent = true;
                }
                return false;
            }

            m_DebugMessageSent = false;
            return _internalJoint._enabled;
        }

        protected override int executionLevel { get { return 1; } }
    }

    [System.Serializable]
    public sealed class JointSuspension2DExt
    {
        [SerializeField] float m_Angle = 90;
        [SerializeField] float m_DampingRatio = 0.7f;
        [SerializeField] float m_Frequency = 2.0f;

        public float angle
        {
            get
            {
                return m_Angle;
            }
            set
            {
                m_Angle = value;
            }
        }
        public float dampingRatio
        {
            get
            {
                return m_DampingRatio;
            }
            set
            {
                m_DampingRatio = value;
            }
        }
        public float frequency
        {
            get
            {
                return m_Frequency;
            }
            set
            {
                m_Frequency = value;
            }
        }

        internal void Set(JointSuspension2D s)
        {
            angle = s.angle;
            dampingRatio = s.dampingRatio;
            frequency = s.frequency;
        }

        public static implicit operator JointSuspension2D(JointSuspension2DExt s)
        {
            return new JointSuspension2D
            {
                angle = s.angle,
                dampingRatio = s.dampingRatio,
                frequency = s.frequency
            };
        }
    }
    [System.Serializable]
    public sealed class JointMotor2DExt
    {
        [SerializeField] float m_MotorSpeed;
        [SerializeField] float m_MaximumMotorForce = 10000.0f;

        public float maxMotorTorque
        {
            get
            {
                return m_MaximumMotorForce;
            }
            set
            {
                m_MaximumMotorForce = value;
            }
        }

        public float motorSpeed
        {
            get
            {
                return m_MotorSpeed;
            }
            set
            {
                m_MotorSpeed = value;
            }
        }

        internal void Set(JointMotor2D m)
        {
            maxMotorTorque = m.maxMotorTorque;
            motorSpeed = m.motorSpeed;
        }

        public static implicit operator JointMotor2D(JointMotor2DExt m)
        {
            return new JointMotor2D
            {
                maxMotorTorque = m.maxMotorTorque,
                motorSpeed = m.motorSpeed
            };
        }
    }
    [System.Serializable]
    public sealed class JointAngleLimits2DExt
    {
        [SerializeField] float m_LowerAngle;
        [SerializeField] float m_UpperAngle = 359;

        public float min { get { return m_LowerAngle; } set { m_LowerAngle = value; } }
        public float max { get { return m_UpperAngle; } set { m_UpperAngle = value; } }

        internal void Set(JointAngleLimits2D l)
        {
            min = l.min;
            max = l.max;
        }

        public static implicit operator JointAngleLimits2D(JointAngleLimits2DExt l)
        {
            return new JointAngleLimits2D
            {
                min = l.min,
                max = l.max
            };
        }
    }
    [System.Serializable]
    public sealed class JointTranslationLimits2DExt
    {
        [SerializeField] float m_LowerTranslation;
        [SerializeField] float m_UpperTranslation;

        public float min { get { return m_LowerTranslation; } set { m_LowerTranslation = value; } }
        public float max { get { return m_UpperTranslation; } set { m_UpperTranslation = value; } }

        internal void Set(JointTranslationLimits2D l)
        {
            min = l.min;
            max = l.max;
        }

        public static implicit operator JointTranslationLimits2D(JointTranslationLimits2DExt l)
        {
            return new JointTranslationLimits2D
            {
                min = l.min,
                max = l.max
            };
        }
    }    
}
