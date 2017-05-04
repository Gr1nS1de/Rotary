//
// Controller2DExtInspector.cs
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
    [CustomEditor(typeof(Controller2DExt))]
    [CanEditMultipleObjects]
    abstract class Controller2DExtInspector : Editor
    {
        public virtual void OnSceneGUI()
        {
            var controller2D = (Controller2DExt)target;
            var position = controller2D.transform.position + new Vector3(controller2D.offset.x, controller2D.offset.y, 0.0f);
            var extents = new Vector3(controller2D.dimensions.x * 0.5f, controller2D.dimensions.y * 0.5f, 0.0f);

            Handles.DrawSolidRectangleWithOutline(
                new Vector3[]
                {
                    position - extents,
                    position - new Vector3(-extents.x, extents.y, 0.0f),
                    position + extents,
                    position + new Vector3(-extents.x, extents.y, 0.0f)
                },
                new Color32(214, 135, 30, 32),
                new Color32(214, 135, 30, 255));
        }

        protected static Vector3 GetRelativePoint(Transform t, Vector3 relativePoint)
        {
            var mat = Matrix4x4.TRS(t.position, t.rotation, Vector3.one);
            return mat.MultiplyPoint3x4(relativePoint);
        }
        protected static Vector3 GetRelativeVector(Transform t, Vector3 relativeVector)
        {
            var mat = Matrix4x4.TRS(t.position, t.rotation, Vector3.one);
            return mat.MultiplyVector(relativeVector);
        }
    }
}
