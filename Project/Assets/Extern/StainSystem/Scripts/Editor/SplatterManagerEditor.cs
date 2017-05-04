using UnityEngine;
using UnityEditor;

namespace SplatterSystem {	
	[CustomEditor(typeof(MeshSplatterManager))]
	public class SplatterManagerEditor : Editor {

		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultSettings"), true);

			MeshSplatterManager sm = target as MeshSplatterManager;

			if (sm.defaultSettings == null) {
				if (GUILayout.Button("Create new settings")) {
					SplatterSettings ss = CustomAssetMaker.CreateAsset<SplatterSettings>("New SplatterSettings");
					sm.defaultSettings = ss;
				}
			}

			sm.advancedSettings = EditorGUILayout.Toggle("Advanced settings", sm.advancedSettings);
			var psObject = sm.gameObject.transform.FindChild("Particle System");
			if (psObject != null) {
				if (sm.advancedSettings && psObject.hideFlags == HideFlags.HideInHierarchy) {
					psObject.hideFlags = HideFlags.None;
					EditorApplication.RepaintHierarchyWindow();
				}
				if (!sm.advancedSettings && psObject.hideFlags != HideFlags.HideInHierarchy) {
					psObject.hideFlags = HideFlags.HideInHierarchy;
					EditorApplication.RepaintHierarchyWindow();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

	}
}