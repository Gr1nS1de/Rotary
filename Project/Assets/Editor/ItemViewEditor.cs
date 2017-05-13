using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ItemView))]
public class ItemViewEditor : Editor
{
	SerializedProperty _mainRenderer;
	ItemView data;

	void OnEnable()
	{
		data = (ItemView)target;

		switch (data.ItemType)
		{
			case ItemTypes.Coin:
				{
					_mainRenderer = serializedObject.FindProperty ("CoinRenderer");
					break;
				}

			case ItemTypes.Crystal:
				{
					_mainRenderer = serializedObject.FindProperty ("DimondRenderer");
					break;
				}
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		data.PoolingType = (PoolingObjectType)EditorGUILayout.EnumPopup("Pooling Type", data.PoolingType); 
		data.ItemType = (ItemTypes)EditorGUILayout.EnumPopup("Platform Type", data.ItemType);  

		switch (data.ItemType)
		{
			case ItemTypes.Coin:
				{
					//EditorGUILayout.ObjectField("Main Platform Renderer", _mainRenderer.objectReferenceValue, typeof(SpriteRenderer), true);
					EditorGUILayout.PropertyField(_mainRenderer, true);
					break;
				}

			case ItemTypes.Crystal:
				{
					EditorGUILayout.PropertyField(_mainRenderer, true);
					EditorGUILayout.FloatField ("Distruct Fracture Count", data.DistructFractureCount);
					break;
				}
		}

		serializedObject.ApplyModifiedProperties();
	}

}

