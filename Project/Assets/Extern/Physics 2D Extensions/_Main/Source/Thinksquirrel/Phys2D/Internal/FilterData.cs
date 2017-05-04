//
// FilterData.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;

namespace Thinksquirrel.Phys2D.Internal
{
    /// <summary>
    /// Contains filter data that can determine whether an object should be processed or not.
    /// </summary>
    abstract class FilterData
    {
        public LayerMask enabledOnLayers = ~0;

        public virtual bool IsActiveOn(Rigidbody2D body)
        {
            if (body == null || !body.gameObject.activeInHierarchy)
                return false;

            //Enable
            if ((enabledOnLayers.value & 1 << body.gameObject.layer) != 0)
                return true;
                        
            return false;
        }
    }
}
