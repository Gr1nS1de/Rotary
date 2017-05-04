//
// PulleyJoint2DInspector.cs
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
    [CustomEditor(typeof(PulleyJoint2DExt))]
    [CanEditMultipleObjects]
    sealed class PulleyJoint2DExtInspector : AnchoredJoint2DExtInspector
    {
        public override sealed void OnInspectorGUI()
        {
            Phys2DEditorHelpers.DrawDefaultInspector(serializedObject, "m_BreakTorque");
            foreach (var targ in targets)
            {
                var pulleyJoint = (PulleyJoint2DExt)targ;
                string error;
                if (!pulleyJoint.ValidatePulley(out error))
                {
                    EditorGUILayout.HelpBox(error, MessageType.Error);
                }
            }
        }

        public override void OnSceneGUI()
        {
            var pulleyJoint2D = (PulleyJoint2DExt)target;
            anchorJoint2D = pulleyJoint2D;
            pulleyJoint2D.SetConnectedAnchorEditor();
            Vector3 vector = Joint2DExtInspector.TransformPoint(pulleyJoint2D.transform, pulleyJoint2D.anchor);
            Vector3 vector2 = pulleyJoint2D.groundAnchorA ? pulleyJoint2D.groundAnchorA.position : Vector3.zero;
            Vector3 vector3 = pulleyJoint2D.groundAnchorB ? pulleyJoint2D.groundAnchorB.position : Vector3.zero;
            Vector3 vector4 = pulleyJoint2D.connectedAnchor;
            bool twoPoints = !pulleyJoint2D.groundAnchorA || !pulleyJoint2D.groundAnchorB;
            if (pulleyJoint2D.connectedBody)
            {
                vector4 = Joint2DExtInspector.TransformPoint(pulleyJoint2D.connectedBody.transform, vector4);
            }
            Vector3 vector5 = vector + (vector2 - vector).normalized * HandleUtility.GetHandleSize(vector) * 0.1f;
            Handles.color = Color.green;
            if (twoPoints)
            {
                Handles.DrawAAPolyLine(new Vector3[] {
                    vector5,
                    vector4
                });
            }
            else
            {
                Handles.DrawAAPolyLine(new Vector3[] {
                    vector5,
                    vector2,
                    vector3,
                    vector4
                });
            }
            if (HandleAnchor(ref vector4, true))
            {
                vector4 = SnapToSprites(vector4);
                vector4 = Joint2DExtInspector.SnapToPoint(vector4, vector, k_SnapDistance);
                if (pulleyJoint2D.connectedBody)
                {
                    vector4 = Joint2DExtInspector.InverseTransformPoint(pulleyJoint2D.connectedBody.transform, vector4);
                }
                pulleyJoint2D.autoConfigureConnectedAnchor = false;
                pulleyJoint2D.connectedAnchor = vector4;
                EditorUtility.SetDirty(pulleyJoint2D);
            }
            if (HandleAnchor(ref vector, false))
            {
                vector = SnapToSprites(vector);
                vector = Joint2DExtInspector.SnapToPoint(vector, vector4, k_SnapDistance);
                pulleyJoint2D.anchor = Joint2DExtInspector.InverseTransformPoint(pulleyJoint2D.transform, vector);
                EditorUtility.SetDirty(pulleyJoint2D);
            }
        }
    }
}
