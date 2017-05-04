//
// AnchoredJoint2DExtInspector.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using UnityEditor;
using Thinksquirrel.Phys2D;

namespace Thinksquirrel.Phys2DEditor
{
    abstract class AnchoredJoint2DExtInspector : Joint2DExtInspector
    {
        protected const float k_SnapDistance = 0.13f;
        protected AnchoredJoint2DExt anchorJoint2D;

        public override void OnInspectorGUI()
        {
            foreach(var targ in targets)
            {
                ((AnchoredJoint2DExt)targ).SetConnectedAnchorEditor();
            }
        }

        public virtual void OnSceneGUI()
        {
            anchorJoint2D = (AnchoredJoint2DExt)target;
            anchorJoint2D.SetConnectedAnchorEditor();
            Vector3 vector = Joint2DExtInspector.TransformPoint(anchorJoint2D.transform, anchorJoint2D.anchor);
            Vector3 vector2 = anchorJoint2D.connectedAnchor;
            bool hideConnectedAnchor = false;
            if (anchorJoint2D.connectedBody)
            {
                vector2 = Joint2DExtInspector.TransformPoint(anchorJoint2D.connectedBody.transform, vector2);
            }
            else
            {
                hideConnectedAnchor = anchorJoint2D.autoConfigureConnectedAnchor;
            }
            Vector3 vector3 = vector + (vector2 - vector).normalized * HandleUtility.GetHandleSize(vector) * 0.1f;
            Handles.color = Color.green;
            if (!hideConnectedAnchor)
            {
                Handles.DrawAAPolyLine(new Vector3[] {
                    vector3,
                    vector2
                });
            }
            if (!hideConnectedAnchor)
            {
                if (HandleAnchor(ref vector2, true))
                {
                    vector2 = SnapToSprites(vector2);
                    vector2 = Joint2DExtInspector.SnapToPoint(vector2, vector, k_SnapDistance);
                    if (anchorJoint2D.connectedBody)
                    {
                        vector2 = Joint2DExtInspector.InverseTransformPoint(anchorJoint2D.connectedBody.transform, vector2);
                    }
                    anchorJoint2D.autoConfigureConnectedAnchor = false;
                    anchorJoint2D.connectedAnchor = vector2;
                    EditorUtility.SetDirty(anchorJoint2D);
                }
            }
            if (HandleAnchor(ref vector, false))
            {
                vector = SnapToSprites(vector);
                vector = Joint2DExtInspector.SnapToPoint(vector, vector2, k_SnapDistance);
                anchorJoint2D.anchor = Joint2DExtInspector.InverseTransformPoint(anchorJoint2D.transform, vector);
                EditorUtility.SetDirty(anchorJoint2D);
            }
        }

        protected Vector3 SnapToSprites(Vector3 position)
        {
            SpriteRenderer component = anchorJoint2D.GetComponent<SpriteRenderer>();
            position = Joint2DExtInspector.SnapToSprite(component, position, k_SnapDistance);
            if (anchorJoint2D.connectedBody)
            {
                component = anchorJoint2D.connectedBody.GetComponent<SpriteRenderer>();
                position = Joint2DExtInspector.SnapToSprite(component, position, k_SnapDistance);
            }
            return position;
        }
    }
}
