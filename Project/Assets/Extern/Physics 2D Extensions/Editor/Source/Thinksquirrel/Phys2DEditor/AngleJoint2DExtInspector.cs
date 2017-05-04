//
// AngleJoint2DInspector.cs
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
    [CustomEditor(typeof(AngleJoint2DExt))]
    [CanEditMultipleObjects]
    sealed class AngleJoint2DExtInspector : Joint2DExtInspector
    {
        public override sealed void OnInspectorGUI()
        {
            Phys2DEditorHelpers.DrawDefaultInspector(serializedObject, "m_BreakForce");
            foreach (var targ in targets)
            {
                var angleJoint = (AngleJoint2DExt)targ;
                string error;
                if (!angleJoint.ValidateAngleJoint(out error))
                {
                    EditorGUILayout.HelpBox(error, MessageType.Error);
                }
            }
        }

        public void OnSceneGUI()
        {
            var angleJoint2D = (AngleJoint2DExt)target;
            if (!angleJoint2D.enabled)
            {
                return;
            }
            Vector3 vector = angleJoint2D.transform.position;
            Vector3 vector2 = vector;
            Vector3 vector3 = vector;
            Vector3 vector4 = Joint2DExtInspector.RotateVector2(Vector3.right, -angleJoint2D.targetAngle);
            Handles.color = Color.green;
            vector4 *= HandleUtility.GetHandleSize(vector) * 0.6f;
            vector2 += vector4;
            Joint2DExtInspector.DrawAALine(vector2, vector3);
        }
    }
}
