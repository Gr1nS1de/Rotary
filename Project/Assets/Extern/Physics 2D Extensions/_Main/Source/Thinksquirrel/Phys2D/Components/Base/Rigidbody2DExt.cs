//
// Rigidbody2DExt.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
#define UNITY_5_3_PLUS
#endif
#pragma warning disable 169
using System;
using UnityEngine;
using System.Collections.Generic;
using Thinksquirrel.Phys2D.Internal;

namespace Thinksquirrel.Phys2D
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class Rigidbody2DExt : P2DBase
    {
        [NonSerialized] readonly List<Collider2D> m_Colliders = new List<Collider2D>();

#region Properties
        public Vector2 position
        {
            get
            {
                return cachedRigidbody2D.position;
            }
            set
            {
                cachedRigidbody2D.position = value;
            }
        }
        public float rotation
        {
            get
            {
                return cachedRigidbody2D.rotation;
            }
            set
            {
                cachedRigidbody2D.rotation = value;
            }
        }
        public float angularDrag
        {
            get
            {
                return cachedRigidbody2D.angularDrag;
            }
            set
            {
                cachedRigidbody2D.angularDrag = value;
            }
        }
        public float angularVelocity
        {
            get
            {
                return cachedRigidbody2D.angularVelocity;
            }
            set
            {
                cachedRigidbody2D.angularVelocity = value;
            }
        }
        public CollisionDetectionMode2D collisionDetectionMode
        {
            get
            {
                return cachedRigidbody2D.collisionDetectionMode;
            }
            set
            {
                cachedRigidbody2D.collisionDetectionMode = value;
            }
        }
        public float drag
        {
            get
            {
                return cachedRigidbody2D.drag;
            }
            set
            {
                cachedRigidbody2D.drag = value;
            }
        }
        public bool fixedAngle
        {
            get
            {
                #if UNITY_5_3_PLUS
                return (cachedRigidbody2D.constraints & RigidbodyConstraints2D.FreezeRotation) != 0;
                #else
                return cachedRigidbody2D.fixedAngle;
                #endif
            }
            set
            {
                #if UNITY_5_3_PLUS
                if (value)
                {
                    cachedRigidbody2D.constraints |= RigidbodyConstraints2D.FreezeRotation;
                }
                else
                {
                    cachedRigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
                }
                #else
                cachedRigidbody2D.fixedAngle = value;
                #endif
            }
        }
        public float gravityScale
        {
            get
            {
                return cachedRigidbody2D.gravityScale;
            }
            set
            {
                cachedRigidbody2D.gravityScale = value;
            }
        }
        public RigidbodyInterpolation2D interpolation
        {
            get
            {
                return cachedRigidbody2D.interpolation;
            }
            set
            {
                cachedRigidbody2D.interpolation = value;
            }
        }
        public bool isKinematic
        {
            get
            {
                return cachedRigidbody2D.isKinematic;
            }
            set
            {
                cachedRigidbody2D.isKinematic = value;
            }
        }
        public float mass
        {
            get
            {
                return cachedRigidbody2D.mass;
            }
            set
            {
                cachedRigidbody2D.mass = value;
            }
        }
        public RigidbodySleepMode2D sleepMode
        {
            get
            {
                return cachedRigidbody2D.sleepMode;
            }
            set
            {
                cachedRigidbody2D.sleepMode = value;
            }
        }
        public Vector2 velocity
        {
            get
            {
                return cachedRigidbody2D.velocity;
            }
            set
            {
                cachedRigidbody2D.velocity = value;
            }
        }
        public Vector2 centerOfMass
        {
            get
            {
                return cachedRigidbody2D.centerOfMass;
            }
            set
            {
                cachedRigidbody2D.centerOfMass = value;
            }
        }
        public Vector2 worldCenterOfMass
        {
            get
            {
                return transform.GetRelativePoint(centerOfMass);                
            }
        }
        public float inertia
        {
            get
            {
                return cachedRigidbody2D.inertia;
            }
            set
            {
                cachedRigidbody2D.inertia = value;
            }
        }
        public bool simulated
        {
            get
            {
                return cachedRigidbody2D.simulated;
            }
            set
            {
                cachedRigidbody2D.simulated = value;
            }
        }
#endregion

#region Internal
        internal float _invMass
        {
            get
            {
                return Mathf.Approximately(cachedRigidbody2D.mass, 0.0f) ? 0.0f : 1.0f / cachedRigidbody2D.mass;                
            }
        }
        internal float _invI
        {
            get
            {
                return Mathf.Approximately(inertia, 0.0f) || fixedAngle ? 0.0f : 1.0f / inertia;
            }
        }
#endregion

#region Methods
        public void AddForce(Vector2 force)
        {
            cachedRigidbody2D.AddForce(force);
        }
        public void AddForce(Vector2 force, ForceMode2D mode)
        {
            cachedRigidbody2D.AddForce(force, mode);
        }
        public void AddForceAtPosition(Vector2 force, Vector2 position)
        {
            cachedRigidbody2D.AddForceAtPosition(force, position);
        }
        public void AddForceAtPosition(Vector2 force, Vector2 position, ForceMode2D mode)
        {
            cachedRigidbody2D.AddForceAtPosition(force, position, mode);
        }
        public void AddRelativeForce(Vector2 relativeForce)
        {
            cachedRigidbody2D.AddRelativeForce(relativeForce);
        }
        public void AddRelativeForce(Vector2 relativeForce, ForceMode2D mode)
        {
            cachedRigidbody2D.AddRelativeForce(relativeForce, mode);
        }
        public void AddTorque(float torque, ForceMode2D mode)
        {
            cachedRigidbody2D.AddTorque(torque, mode);
        }
        public void AddTorque(float torque)
        {
            cachedRigidbody2D.AddTorque(torque);
        }
        public Vector2 GetPoint(Vector2 point)
        {
            return cachedRigidbody2D.GetPoint(point);
        }
        internal Vector2 GetPointNoRotation(Vector2 point)
        {
            return transform.GetPointNoRotation(point);
        }
        public Vector2 GetPointVelocity(Vector2 worldPoint)
        {
            return cachedRigidbody2D.GetPointVelocity(worldPoint);
        }
        public Vector2 GetRelativePoint(Vector2 relativePoint)
        {
            return cachedRigidbody2D.GetRelativePoint(relativePoint);
        }
        internal Vector2 GetRelativePointNoRotation(Vector2 relativePoint)
        {
            return transform.GetRelativePointNoRotation(relativePoint);
        }
        public Vector2 GetRelativePointVelocity(Vector2 relativePoint)
        {
            return cachedRigidbody2D.GetRelativePointVelocity(relativePoint);
        }
        public Vector2 GetRelativeVector(Vector2 relativeVector)
        {
            return cachedRigidbody2D.GetRelativeVector(relativeVector);
        }
        public Vector2 GetVector(Vector2 vector)
        {
            return cachedRigidbody2D.GetVector(vector);
        }
        public bool IsAwake()
        {
            return cachedRigidbody2D.IsAwake();
        }
        public bool IsSleeping()
        {
            return cachedRigidbody2D.IsSleeping();
        }
        public void MovePosition(Vector2 position)
        {
            cachedRigidbody2D.MovePosition(position);
        }
        public void MoveRotation(float angle)
        {
            cachedRigidbody2D.MoveRotation(angle);
        }
        public void Sleep()
        {
            cachedRigidbody2D.Sleep();
        }
        public void WakeUp()
        {
            cachedRigidbody2D.WakeUp();
        }
#endregion

#region Calculate mass data
        internal IList<Collider2D> GetColliders()
        {
            m_Colliders.Clear();
            GetCollidersRecursive(cachedRigidbody2D, transform);

            return m_Colliders;
        }
        void GetCollidersRecursive(Rigidbody2D body, Transform root)
        {
            // Don't get colliders if we hit another rigidbody
            if (root.GetComponent<Rigidbody2D>() != body)
                return;

            var colliders = root.GetComponents<Collider2D>();

            if (colliders.Length > 0)
                m_Colliders.AddRange(colliders);

            for(int i = 0; i < root.childCount; ++i)
            {
                var child = root.GetChild(i);
                GetCollidersRecursive(body, child);
            }
        }
        internal void CalculateMassData(out MassData[] massData)
        {
            m_Colliders.Clear();
            GetCollidersRecursive(cachedRigidbody2D, transform);

            massData = new MassData[m_Colliders.Count];
            MathUtils.ComputeProperties(cachedRigidbody2D, m_Colliders, ref massData);
        }
#endregion
#region Operators
        public static implicit operator Rigidbody2DExt(Rigidbody2D rb2D)
        {
            if (!rb2D)
                return null;

            var rb2DExt = rb2D.GetComponent<Rigidbody2DExt>();

            if (!rb2DExt)
            {
                rb2DExt = rb2D.gameObject.AddComponent<Rigidbody2DExt>();
                rb2DExt.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
            }

            return rb2DExt;
        }

        public static implicit operator Rigidbody2D(Rigidbody2DExt rb2DExt)
        {
            if (!rb2DExt)
                return null;

            return rb2DExt.cachedRigidbody2D;
        }
#endregion
    }
}