using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

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
			case PlatformTypes.Horizontal:
				{
					_mainRenderer = serializedObject.FindProperty ("HorizontalPlatformRenderer");
					break;
				}

			case PlatformTypes.Vertical_Moving:
			case PlatformTypes.Vertical:
				{
					_mainRenderer = serializedObject.FindProperty ("VerticalPlatformRenderers");
					break;
				}
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		data.PoolingType = (PoolingObjectType)EditorGUILayout.EnumPopup("Pooling Type", data.PoolingType); 
		data.PlatformType = (PlatformTypes)EditorGUILayout.EnumPopup("Platform Type", data.PlatformType);  

		EditorGUILayout.PropertyField(_mainRenderer, true);

		/*
		switch (data.PlatformType)
		{
			case PlatformTypes.Horizontal:
				{
					//EditorGUILayout.ObjectField("Main Platform Renderer", _mainRenderer.objectReferenceValue, typeof(SpriteRenderer), true);
					EditorGUILayout.PropertyField(_mainRenderer, true);
					break;
				}

			case PlatformTypes.Vertical:
				{
					EditorGUILayout.PropertyField(_mainRenderer, true);
					break;
				}
		}*/
				
		serializedObject.ApplyModifiedProperties();
	}

}

