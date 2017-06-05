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

			case ItemTypes.Magnet:
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
		data.ItemType = (ItemTypes)EditorGUILayout.EnumPopup("Platform Type", data.ItemType);  

		EditorGUILayout.PropertyField(_mainRenderer, true);

		switch (data.ItemType)
		{
			case ItemTypes.Coin:
				{
					data.DoubleCoinRenderer = (SpriteRenderer)EditorGUILayout.ObjectField("Double Coin Renderer", data.DoubleCoinRenderer, typeof(SpriteRenderer), true);
					data.CountRenderer = (tk2dTextMesh)EditorGUILayout.ObjectField ("Count Renderer", data.CountRenderer, typeof(tk2dTextMesh));
					break;
				}

			case ItemTypes.Crystal:
				{
					data.CrystalFractureCount = EditorGUILayout.IntField ("Crystal Fracture Count", data.CrystalFractureCount);
					data.CrystalDestroyTime = EditorGUILayout.FloatField ("Crystal Destroy Time", data.CrystalDestroyTime);
					data.CountRenderer = (tk2dTextMesh)EditorGUILayout.ObjectField ("Count Renderer", data.CountRenderer, typeof(tk2dTextMesh));
					break;
				}
			case ItemTypes.Magnet:
				{
					data.MagnetTrail = (WeaponTrail)EditorGUILayout.ObjectField ("Magnet Renderer", data.MagnetTrail, typeof(WeaponTrail));

					break;
				}
		}

		serializedObject.ApplyModifiedProperties();
	}

}

