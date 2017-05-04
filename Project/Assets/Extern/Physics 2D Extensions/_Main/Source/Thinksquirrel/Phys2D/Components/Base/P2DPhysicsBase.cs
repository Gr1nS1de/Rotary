//
// P2DPhysicsBase.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using Thinksquirrel.Phys2D.Internal;
using UnityEngine;

namespace Thinksquirrel.Phys2D
{
    public abstract class P2DPhysicsBase : P2DBase
    {
        protected abstract int executionLevel { get; }

        protected virtual void OnEnable()
        {
            P2DUpdateManager.Add(this, executionLevel);
        }

        protected virtual void OnDestroy()
        {
            P2DUpdateManager.Remove(this);
        }

        internal abstract void PhysicsUpdate();
    }
}
