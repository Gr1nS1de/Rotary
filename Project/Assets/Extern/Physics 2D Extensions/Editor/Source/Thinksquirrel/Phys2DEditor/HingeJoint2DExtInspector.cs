//
// HingeJoint2DInspector.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Thinksquirrel.Phys2D;
using Thinksquirrel.Phys2DEditor;

namespace Thinksquirrel.Phys2DEditor
{
    [CustomEditor(typeof(HingeJoint2DExt))]
    [CanEditMultipleObjects]
    sealed class HingeJoint2DExtInspector : AnchoredJoint2DExtInspector
    {
        public override sealed void OnInspectorGUI()
        {
            Phys2DEditorHelpers.DrawDefaultInspector(serializedObject);
        }

        void DrawTick(Vector3 center, float radius, float angle, Vector3 up, float length)
        {
            Vector3 a = Joint2DExtInspector.RotateVector2(up, angle).normalized;
            Vector3 vector = center + a * radius;
            Vector3 vector2 = vector + (center - vector).normalized * radius * length;
            var methodInfo = typeof(Handles).GetMethod("DrawAAPolyLine", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new System.Type[] {typeof(Color[]), typeof(Vector3[]) }, null);
            var args = new object[]
            {
                new Color[]
                {
                    new Color(0.0f, 1.0f, 0.0f, 0.7f),
                    new Color(0.0f, 1.0f, 0.0f, 0.0f)
                },
                new Vector3[] {
                    vector,
                    vector2
                }
            };
            methodInfo.Invoke(null, args);
        }

        public override sealed void OnSceneGUI()
        {
            HingeJoint2DExt hingeJoint2D = (HingeJoint2DExt)target;
            if (!hingeJoint2D.enabled)
            {
                return;
            }
            if (hingeJoint2D.useLimits)
            {
                Vector3 vector = Joint2DExtInspector.TransformPoint(hingeJoint2D.transform, hingeJoint2D.anchor);
                Vector3 vector2 = Vector2.up;
                if (Application.isPlaying)
                {
                    vector2 = Joint2DExtInspector.RotateVector2(Vector2.up, hingeJoint2D.referenceAngle + hingeJoint2D.limits.min);
                }
                else
                {
                    vector2 = Joint2DExtInspector.RotateVector2(hingeJoint2D.transform.up, hingeJoint2D.limits.min);
                }
                float angle = hingeJoint2D.limits.max - hingeJoint2D.limits.min;
                float num = HandleUtility.GetHandleSize(vector) * 0.8f;
                float d = HandleUtility.GetHandleSize(vector) * 0.1f;
                Vector3 vector3 = vector + hingeJoint2D.transform.up * num;
                Vector3 start = vector + (vector3 - vector).normalized * d;
                Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.7f);
                Joint2DExtInspector.DrawAALine(start, vector3);
                Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.03f);
                Handles.DrawSolidArc(vector, Vector3.back, vector2, angle, num);
                Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.7f);
                Handles.DrawWireArc(vector, Vector3.back, vector2, angle, num);
                DrawTick(vector, num, 0.0f, vector2, 1.0f);
                DrawTick(vector, num, angle, vector2, 1.0f);
            }
            base.OnSceneGUI();
        }
    }
}
