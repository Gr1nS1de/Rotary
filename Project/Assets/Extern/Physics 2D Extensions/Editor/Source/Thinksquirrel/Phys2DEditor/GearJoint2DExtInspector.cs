//
// GearJoint2DInspector.cs
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
    [CustomEditor(typeof(GearJoint2DExt))]
    [CanEditMultipleObjects]
    sealed class GearJoint2DExtInspector : Joint2DExtInspector
    {
        GearJoint2DExt gearJoint2D;
        
        public override sealed void OnInspectorGUI()
        {
            Phys2DEditorHelpers.DrawDefaultInspector(serializedObject, "m_ConnectedRigidBody");
            foreach (var targ in targets)
            {
                var gearJoint = (GearJoint2DExt)targ;
                string error;
                if (!gearJoint.ValidateJoints(out error))
                {
                    EditorGUILayout.HelpBox(error, MessageType.Error);
                }
            }
        }

        public void OnSceneGUI()
        {
            gearJoint2D = (GearJoint2DExt)target;
            if (gearJoint2D.localJoint && gearJoint2D.connectedJoint)
            {
                Vector3 vector = gearJoint2D.localJoint.transform.position;
                Vector3 vector2 = gearJoint2D.connectedJoint.transform.position;
                Handles.color = Color.green;
                Handles.DrawAAPolyLine(new Vector3[] {
                    vector,
                    vector2
                });
            }
        }
    }
}
