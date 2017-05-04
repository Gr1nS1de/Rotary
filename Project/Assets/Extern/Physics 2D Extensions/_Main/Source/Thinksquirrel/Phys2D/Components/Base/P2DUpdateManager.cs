//
// P2DUpdateManager.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using System.Collections.Generic;
using System.Collections;
using Thinksquirrel.Phys2D;
using UnityEngine;

namespace Thinksquirrel.Phys2D.Internal
{
    [AddComponentMenu("")]
    sealed class P2DUpdateManager : P2DBase
    {
        // Drag Control
        [System.NonSerialized] readonly HashSet<P2DPhysicsBase> m_Level0 = new HashSet<P2DPhysicsBase>();
        // Joints
        [System.NonSerialized] readonly HashSet<P2DPhysicsBase> m_Level1 = new HashSet<P2DPhysicsBase>();
        // Gear Joints
        [System.NonSerialized] readonly HashSet<P2DPhysicsBase> m_Level2 = new HashSet<P2DPhysicsBase>();
        // Controllers
        [System.NonSerialized] readonly HashSet<P2DPhysicsBase> m_Level3 = new HashSet<P2DPhysicsBase>();

        [System.NonSerialized] static P2DUpdateManager m_Instance;

        static void EnsureInstance()
        {
            if (!m_Instance)
            {
                var go = GameObject.Find("P2DUpdateManager");

                if (go)
                {
                    m_Instance = go.GetComponent<P2DUpdateManager>() ?? go.AddComponent<P2DUpdateManager>();
                }
            }
            if (!m_Instance)
            {
                var go = new GameObject("P2DUpdateManager");
                go.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                m_Instance = go.AddComponent<P2DUpdateManager>();
            }
        }

        internal static void Add(P2DPhysicsBase component, int executionLevel)
        {
            if (!Application.isPlaying)
                return;

            EnsureInstance();

            switch(executionLevel)
            {
                case 0:
                    m_Instance.m_Level0.Add(component);
                    break;
                case 1:
                    m_Instance.m_Level1.Add(component);
                    break;
                case 2:
                    m_Instance.m_Level2.Add(component);
                    break;
                case 3:
                    m_Instance.m_Level3.Add(component);
                    break;
                default:
                    Debug.LogWarning("[P2DInternal] Invalid execution level, defaulting to highest");
                    m_Instance.m_Level3.Add(component);
                    break;
            }
        }

        internal static void Remove(P2DPhysicsBase component)
        {
            if (!Application.isPlaying)
                return;

            m_Instance.m_Level0.Remove(component);
            m_Instance.m_Level1.Remove(component);
            m_Instance.m_Level2.Remove(component);
            m_Instance.m_Level3.Remove(component);
        }

        void Awake()
        {
            // Make the update manager persistent in case any objects are persistent
            Object.DontDestroyOnLoad(this);
        }

        void FixedUpdate()
        {
            for(var i = m_Level0.GetEnumerator(); i.MoveNext();)
            {
                var component = i.Current;

                if (component && component.enabled)
                    component.PhysicsUpdate();
            }
            for(var i = m_Level1.GetEnumerator(); i.MoveNext();)
            {
                var component = i.Current;

                if (component && component.enabled)
                    component.PhysicsUpdate();
            }
            for(var i = m_Level2.GetEnumerator(); i.MoveNext();)
            {
                var component = i.Current;

                if (component && component.enabled)
                    component.PhysicsUpdate();
            }
            for(var i = m_Level3.GetEnumerator(); i.MoveNext();)
            {
                var component = i.Current;

                if (component && component.enabled)
                    component.PhysicsUpdate();
            }
        }
    }
}
