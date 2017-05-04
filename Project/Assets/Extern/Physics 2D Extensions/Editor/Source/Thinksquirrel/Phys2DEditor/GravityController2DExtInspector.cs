//
// GravityController2DExtInspector.cs
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
    [CustomEditor(typeof(GravityController2DExt))]
    [CanEditMultipleObjects]
    sealed class GravityController2DInspector : Controller2DExtInspector
    {
        public override sealed void OnInspectorGUI()
        {
            Phys2DEditorHelpers.DrawDefaultInspector(serializedObject);
        }

        public override void OnSceneGUI()
        {
            var gravityController2D = (GravityController2DExt)target;

            if (gravityController2D.minRadius > 0)
            {
                for(var i = 0; i < gravityController2D.GetRigidbodyCount(); ++i)
                {
                    var body = gravityController2D.GetRigidbody(i);

                    if (!body)
                        continue;

                    Gizmos.color = Color.yellow;
                    Handles.DrawWireDisc(body.transform.position, Vector3.forward, gravityController2D.minRadius);
                }

                foreach(var point in gravityController2D.points)
                {
                    Gizmos.color = Color.red;
                    Handles.DrawWireDisc(GetRelativePoint(gravityController2D.transform, point), Vector3.forward, gravityController2D.minRadius);
                }
            }

            base.OnSceneGUI();
        }
    }
}
