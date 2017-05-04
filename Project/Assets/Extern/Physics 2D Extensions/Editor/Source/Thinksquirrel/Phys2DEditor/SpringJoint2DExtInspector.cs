//
// SpringJoint2DInspector.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEditor;
using Thinksquirrel.Phys2D;
using Thinksquirrel.Phys2DEditor;

namespace Thinksquirrel.Phys2DEditor
{
    [CustomEditor(typeof(SpringJoint2DExt))]
    [CanEditMultipleObjects]
    sealed class SpringJoint2DExtInspector : DistanceJointBase2DExtInspector
    {
    }
}
