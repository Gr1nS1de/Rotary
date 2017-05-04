//
// Attributes.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;

namespace Thinksquirrel.Phys2DEditor
{
    public class BitMaskAttribute : PropertyAttribute
    {
        public System.Type propertyType { get; private set; }

        public BitMaskAttribute(System.Type aType)
        {
            propertyType = aType;
        }
    }
}