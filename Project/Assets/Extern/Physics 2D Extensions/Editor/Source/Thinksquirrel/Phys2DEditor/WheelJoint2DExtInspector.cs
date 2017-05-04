//
// WheelJoint2DExtInspector.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEditor;
using UnityEngine;
using Thinksquirrel.Phys2D;
using Thinksquirrel.Phys2DEditor;

namespace Thinksquirrel.Phys2DEditor
{
    [CustomEditor(typeof(WheelJoint2DExt))]
    [CanEditMultipleObjects]
    sealed class WheelJoint2DExtInspector : AnchoredJoint2DExtInspector
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

        public override sealed void OnSceneGUI()
        {
            var wheelJoint2D = (WheelJoint2DExt)this.target;
            if (!wheelJoint2D.enabled)
            {
                return;
            }
            Vector3 vector = Joint2DExtInspector.TransformPoint(wheelJoint2D.transform, wheelJoint2D.anchor);
            Vector3 vector2 = vector;
            Vector3 vector3 = vector;
            Vector3 vector4 = Joint2DExtInspector.RotateVector2(Vector3.right, -wheelJoint2D.suspension.angle);
            Handles.color = Color.green;
            vector4 *= HandleUtility.GetHandleSize(vector) * 0.3f;
            vector2 += vector4;
            vector3 -= vector4;
            Joint2DExtInspector.DrawAALine(vector2, vector3);
            base.OnSceneGUI();
        }
    }
}
