//
// WeldJoint2DInspector.cs
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
    [CustomEditor(typeof(WeldJoint2DExt))]
    [CanEditMultipleObjects]
    sealed class WeldJoint2DExtInspector : AnchoredJoint2DExtInspector 
    {
        public override sealed void OnInspectorGUI()
        {
            foreach (var targ in targets)
            {
                var anchoredJoint = (AnchoredJoint2DExt)targ;
                anchoredJoint.SetConnectedAnchorEditor();
            }
            Phys2DEditorHelpers.DrawDefaultInspector(serializedObject);
        }
    }
}
