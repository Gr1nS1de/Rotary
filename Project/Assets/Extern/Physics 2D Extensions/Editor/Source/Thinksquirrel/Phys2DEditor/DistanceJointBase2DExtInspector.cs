//
// DistanceJointBase2DExtInspector.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using UnityEditor;
using Thinksquirrel.Phys2D;
using Thinksquirrel.Phys2DEditor;

namespace Thinksquirrel.Phys2DEditor
{
    abstract class DistanceJointBase2DExtInspector : AnchoredJoint2DExtInspector
    {
        public override sealed void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (var targ in targets)
            {
                ((DistanceJointBase2DExt)targ).SetDistanceEditor();
            }

            Phys2DEditorHelpers.DrawDefaultInspector(serializedObject, "m_BreakTorque");
        }

        public override sealed void OnSceneGUI()
        {
            DistanceJointBase2DExt distanceJoint2D = (DistanceJointBase2DExt)this.target;
            if (!distanceJoint2D.enabled)
            {
                return;
            }
            Vector3 anchor = Joint2DExtInspector.TransformPoint(distanceJoint2D.transform, distanceJoint2D.anchor);
            Vector3 vector = distanceJoint2D.connectedAnchor;
            if (distanceJoint2D.connectedBody)
            {
                vector = Joint2DExtInspector.TransformPoint(distanceJoint2D.connectedBody.transform, vector);
            }
            Joint2DExtInspector.DrawDistanceGizmo(anchor, vector, distanceJoint2D.distance);
            bool autoConfigure = distanceJoint2D.autoConfigureConnectedAnchor;
            base.OnSceneGUI();
            if (autoConfigure && !distanceJoint2D.autoConfigureConnectedAnchor)
            {
                distanceJoint2D.autoConfigureDistance = false;
                EditorUtility.SetDirty(distanceJoint2D);
            }
        }
    }
}
