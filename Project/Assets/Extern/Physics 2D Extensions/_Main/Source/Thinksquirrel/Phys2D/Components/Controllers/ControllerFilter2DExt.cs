//
// ControllerFilter2DExt.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using System.Collections.Generic;
using Thinksquirrel.Phys2DEditor;

namespace Thinksquirrel.Phys2D
{
    [AddComponentMenu("Physics 2D/Physics Controllers/Controller Filter (P2D Extensions)")]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class ControllerFilter2DExt : P2DBase
    {
        [SerializeField] [BitMask(typeof(ControllerType))] ControllerType m_IgnoredControllers;

        internal static Dictionary<Rigidbody2D, ControllerFilter2DExt> _filterMap = new Dictionary<Rigidbody2D, ControllerFilter2DExt>();

        public ControllerType ignoredControllers { get { return m_IgnoredControllers; } set { m_IgnoredControllers = value; } }

        public void IgnoreControllerType(ControllerType controllerType)
        {
            ignoredControllers |= controllerType;
        }
        public void IgnoreControllerType(Controller2DExt controller)
        {
            IgnoreControllerType(controller._internalController._type);
        }
        public void RestoreControllerType(ControllerType controllerType)
        {
            ignoredControllers &= ~controllerType;
        }
        public void RestoreControllerType(Controller2DExt controller)
        {
            RestoreControllerType(controller._internalController._type);
        }
        public bool IsControllerTypeIgnored(Controller2DExt controller)
        {
            return IsControllerTypeIgnored(controller._internalController._type);
        }
        public bool IsControllerTypeIgnored(ControllerType controllerType)
        {
            return (ignoredControllers & controllerType) == controllerType;
        }

        void OnEnable()
        {
            _filterMap.Add(cachedRigidbody2D, this);
        }

        void OnDestroy()
        {
            _filterMap.Remove(cachedRigidbody2D);
        }

    }

    [System.Flags]
    public enum ControllerType
    {
        GravityController = (1 << 0),
        VelocityLimitController = (1 << 1),
        WindController = (1 << 2),
        BuoyancyController = (1 << 3),
    }
}
