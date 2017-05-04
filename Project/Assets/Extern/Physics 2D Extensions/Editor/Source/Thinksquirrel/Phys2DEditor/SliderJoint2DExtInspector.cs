//
// SliderJoint2DInspector.cs
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
    [CustomEditor(typeof(SliderJoint2DExt))]
    [CanEditMultipleObjects]
    sealed class SliderJoint2DExtInspector : AnchoredJoint2DExtInspector
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
            var sliderJoint2D = (SliderJoint2DExt)target;
            if (!sliderJoint2D.enabled)
            {
                return;
            }
            Vector3 vector = Joint2DExtInspector.TransformPoint(sliderJoint2D.transform, sliderJoint2D.anchor);
            Vector3 vector2 = vector;
            Vector3 vector3 = vector;
            Vector3 vector4 = Joint2DExtInspector.RotateVector2(Vector3.right, sliderJoint2D.transform.eulerAngles.z);
            vector4 = Joint2DExtInspector.RotateVector2(vector4, -sliderJoint2D.angle);
            Handles.color = Color.green;
            if (sliderJoint2D.useLimits)
            {
                vector2 = vector + vector4 * sliderJoint2D.limits.max;
                vector3 = vector + vector4 * sliderJoint2D.limits.min;
                Vector3 a = Vector3.Cross(vector4, Vector3.forward);
                float d = HandleUtility.GetHandleSize(vector2) * 0.16f;
                float d2 = HandleUtility.GetHandleSize(vector3) * 0.16f;
                Joint2DExtInspector.DrawAALine(vector2 + a * d, vector2 - a * d);
                Joint2DExtInspector.DrawAALine(vector3 + a * d2, vector3 - a * d2);
            }
            else
            {
                vector4 *= HandleUtility.GetHandleSize(vector) * 0.3f;
                vector2 += vector4;
                vector3 -= vector4;
            }
            Joint2DExtInspector.DrawAALine(vector2, vector3);
            base.OnSceneGUI();
        }
    }
}
