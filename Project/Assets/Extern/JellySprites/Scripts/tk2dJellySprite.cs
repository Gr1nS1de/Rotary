#define TK2D_SUPPORT_ENABLED

#if TK2D_SUPPORT_ENABLED
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("Jelly Sprite/Toolkit 2D Jelly Sprite")]
public class tk2dJellySprite : JellySprite
{
	public tk2dSpriteCollectionData m_Tk2DCollection;	
	public int m_SpriteId;
	
	/// <summary>
	/// Get the bounds of the sprite
	/// </summary>
	protected override Bounds GetSpriteBounds()
	{
		return GetCurrentSpriteDef().GetBounds();
	}

	/// <summary>
	/// Check if the sprite is valid
	/// </summary>
	protected override bool IsSpriteValid()
	{
		return GetCurrentSpriteDef() != null;
	}

	/// <summary>
	/// Gets the current sprite definition.
	/// </summary>
	public tk2dSpriteDefinition GetCurrentSpriteDef()
	{
		return (m_Tk2DCollection == null || m_SpriteId >= m_Tk2DCollection.inst.spriteDefinitions.Length) ? null : m_Tk2DCollection.inst.spriteDefinitions[m_SpriteId];
	}

	/// <summary>
	/// Check if the source sprite is rotated
	/// </summary>
	protected override bool IsSourceSpriteRotated()
	{
		if (IsSpriteValid())
		{
			return GetCurrentSpriteDef().flipped != tk2dSpriteDefinition.FlipMode.None;
		}

		return false;
	}

	/// <summary>
	/// Initialise the render material
	/// </summary>
	protected override void InitMaterial ()
	{
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		Material material = GetCurrentSpriteDef().material;
		meshRenderer.sharedMaterial = material;
	}

	/// <summary>
	/// Gets the minimum max texture rect.
	/// </summary>
	protected override void GetMinMaxTextureRect(out Vector2 min, out Vector2 max)
	{
		tk2dSpriteDefinition spriteDef = GetCurrentSpriteDef();
		min = spriteDef.uvs[0];
		max = spriteDef.uvs[spriteDef.uvs.Length - 1];
	}

#if UNITY_EDITOR
	/// <summary>
	/// Create a new jellysprite object.
	/// </summary>
	[MenuItem("GameObject/Create Other/Jelly Sprite/Toolkit 2D Jelly Sprite", false, 12951)]
	static void DoCreateSpriteObject()
	{
		GameObject gameObject = new GameObject("JellySprite");
		gameObject.AddComponent<tk2dJellySprite>();
		Selection.activeGameObject = gameObject;
		Undo.RegisterCreatedObjectUndo(gameObject, "Create Toolkit 2D Jelly Sprite");
	}
#endif
}
#endif