//
// DragControl2DInspector.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEditor;
using Thinksquirrel.Phys2D;
using Thinksquirrel.Phys2DEditor;

namespace Thinksquirrel.Phys2DEditor
{
    [CustomEditor(typeof(DragControl2DExt))]
    [CanEditMultipleObjects]
    sealed class DragControl2DExtInspector : Editor
    {
        public override sealed void OnInspectorGUI()
        {
            Phys2DEditorHelpers.DrawDefaultInspector(serializedObject);
        }
    }
}
