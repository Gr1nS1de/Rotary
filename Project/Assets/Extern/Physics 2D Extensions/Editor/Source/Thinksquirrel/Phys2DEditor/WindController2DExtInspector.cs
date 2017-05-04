//
// WindController2DExtInspector.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEditor;
using UnityEngine;
using Thinksquirrel.Phys2D;
using Thinksquirrel.Phys2D.Internal;
using Thinksquirrel.Phys2DEditor;

namespace Thinksquirrel.Phys2DEditor
{
    [CustomEditor(typeof(WindController2DExt))]
    [CanEditMultipleObjects]
    sealed class WindController2DInspector : Controller2DExtInspector
    {
        public override sealed void OnInspectorGUI()
        {
            Phys2DEditorHelpers.DrawDefaultInspector(serializedObject);
        }

        public override void OnSceneGUI()
        {
            var windController2D = (WindController2DExt)target;
            var transform = windController2D.transform;
            var offset = windController2D.offset;
            var direction = windController2D.direction;
            var strength = windController2D.strength;
            var forceType = windController2D.forceType;

            // TODO - arrow with colors, incomplete
            var start = GetRelativePoint(transform, offset);
            var dir = GetRelativeVector(transform, direction);
            var dir2 = windController2D.windForce;

            var c = Handles.color;
            Handles.color = Color.red;

            if (forceType == ForceType.Point)
            {
                Handles.DrawWireDisc(start, Vector3.forward, strength);

                float dp = Vector3.Dot(dir, dir2);
                Handles.color = dp >= 0 ? Color.yellow : Color.green;

                Handles.DrawWireDisc(start, Vector3.forward, dir2.magnitude);
            }
            else
            {
                DrawArrow(start, dir * strength, Camera.current.GetPixelWidth(start) * 10, 35f);

                float dp = Vector3.Dot(dir, dir2);
                Handles.color = dp >= 0 ? Color.yellow : Color.green;

                DrawArrow(start, dir2, Camera.current.GetPixelWidth(start) * 5, 35f);
            }

            Handles.color = c;

            base.OnSceneGUI();
        }

        void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength, float arrowHeadAngle)
        {
            var pos2 = pos + direction;

            Handles.DrawLine(pos, pos2);

            if (direction.sqrMagnitude > Vector3.kEpsilon)
            {
                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(180+arrowHeadAngle,0,0) * new Vector3(0,0,1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(180-arrowHeadAngle,0,0) * new Vector3(0,0,1);
                Handles.DrawLine(pos2, pos2 + (right * arrowHeadLength));
                Handles.DrawLine(pos2, pos2 + (left * arrowHeadLength));
            }
        }
    }
}
