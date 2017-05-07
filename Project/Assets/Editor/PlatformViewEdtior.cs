using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(PlatformView))]
public class PlatformViewEditor : Editor
{
	SerializedProperty _mainRenderer;
	PlatformView data;

	void OnEnable()
	{
		data = (PlatformView)target;

		switch (data.PlatformType)
		{
			case PlatformTypes.HORIZONTAL:
				{
					_mainRenderer = serializedObject.FindProperty ("HorizontalPlatformRenderer");
					break;
				}

			case PlatformTypes.VERTICAL:
				{
					_mainRenderer = serializedObject.FindProperty ("VerticalPlatformRenderers");
					break;
				}
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		data.PlatformType = (PlatformTypes)EditorGUILayout.EnumPopup("Platform Type", data.PlatformType);  

		switch (data.PlatformType)
		{
			case PlatformTypes.HORIZONTAL:
				{
					//EditorGUILayout.ObjectField("Main Platform Renderer", _mainRenderer.objectReferenceValue, typeof(SpriteRenderer), true);
					EditorGUILayout.PropertyField(_mainRenderer, true);
					break;
				}

			case PlatformTypes.VERTICAL:
				{
					EditorGUILayout.PropertyField(_mainRenderer, true);
					break;
				}
		}
				
		serializedObject.ApplyModifiedProperties();
	}

}

