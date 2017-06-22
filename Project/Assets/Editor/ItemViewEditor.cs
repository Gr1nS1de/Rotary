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
			case ItemType.Coin:
				{
					_mainRenderer = serializedObject.FindProperty ("CoinRenderer");
					break;
				}

			case ItemType.Crystal:
				{
					_mainRenderer = serializedObject.FindProperty ("CrystalRenderer");
					break;
				}

			case ItemType.Magnet:
				{
					_mainRenderer = serializedObject.FindProperty ("MagnetRenderer");
					break;
				}

		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		data.PoolingType = (PoolingObjectType)EditorGUILayout.EnumPopup("Pooling Type", data.PoolingType); 
		data.ItemType = (ItemType)EditorGUILayout.EnumPopup("Platform Type", data.ItemType);  

		EditorGUILayout.PropertyField(_mainRenderer, true);

		switch (data.ItemType)
		{
			case ItemType.Coin:
				{
					data.DoubleCoinRenderer = (SpriteRenderer)EditorGUILayout.ObjectField("Double Coin Renderer", data.DoubleCoinRenderer, typeof(SpriteRenderer), true);
					//data.CountRenderers = (tk2dTextMesh[])EditorGUILayout.ObjectField ("Count Renderers", data.CountRenderers, typeof(tk2dTextMesh[]));

					EditorGUILayout.PropertyField(serializedObject.FindProperty("CountRenderers"), true); //(tk2dTextMesh[])EditorGUILayout.ObjectField ("Count Renderers", data.CountRenderers, typeof(tk2dTextMesh[]));

					break;
				}

			case ItemType.Crystal:
				{
					data.CrystalFractureCount = EditorGUILayout.IntField ("Crystal Fracture Count", data.CrystalFractureCount);
					data.CrystalDestroyTime = EditorGUILayout.FloatField ("Crystal Destroy Time", data.CrystalDestroyTime);
					//data.CountRenderers = 
					EditorGUILayout.PropertyField(serializedObject.FindProperty("CountRenderers"), true); //(tk2dTextMesh[])EditorGUILayout.ObjectField ("Count Renderers", data.CountRenderers, typeof(tk2dTextMesh[]));
					break;
				}
			case ItemType.Magnet:
				{
					data.MagnetTrail = (WeaponTrail)EditorGUILayout.ObjectField ("Magnet Renderer", data.MagnetTrail, typeof(WeaponTrail));

					break;
				}
		}

		serializedObject.ApplyModifiedProperties();
	}

}

