#define TK2D_SUPPORT_ENABLED

#if TK2D_SUPPORT_ENABLED
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(tk2dJellySprite))]
class tk2dJellySpriteEditor : JellySpriteEditor
{	
	protected override void DisplayInspectorGUI()
	{
		tk2dJellySprite targetObject = this.target as tk2dJellySprite;
		tk2dSpriteGuiUtility.SpriteSelector(targetObject.m_Tk2DCollection, targetObject.m_SpriteId, new tk2dSpriteGuiUtility.SpriteChangedCallback( SpriteChangedCallbackImpl ), null);

		if (tk2dPreferences.inst.displayTextureThumbs && targetObject.m_Tk2DCollection != null) 
		{
			tk2dSpriteDefinition def = targetObject.GetCurrentSpriteDef();
			if (targetObject.m_Tk2DCollection.version < 1 || def.texelSize == Vector2.zero)
			{
				string message = "";
				
				message = "No thumbnail data.";
				if (targetObject.m_Tk2DCollection.version < 1)
					message += "\nPlease rebuild Sprite Collection.";
				
				tk2dGuiUtility.InfoBox(message, tk2dGuiUtility.WarningLevel.Info);
			}
			else
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(" ");
				
				int tileSize = 128;
				Rect r = GUILayoutUtility.GetRect(tileSize, tileSize, GUILayout.ExpandWidth(false));
				tk2dGrid.Draw(r);
				tk2dSpriteThumbnailCache.DrawSpriteTextureInRect(r, def, Color.white);
				
				GUILayout.EndHorizontal();
			}
		}

		base.DisplayInspectorGUI();
	}

	void SpriteChangedCallbackImpl(tk2dSpriteCollectionData spriteCollection, int spriteId, object data)
	{
		tk2dJellySprite targetObject = this.target as tk2dJellySprite;        
		targetObject.m_SpriteId = spriteId;
		targetObject.m_Tk2DCollection = spriteCollection;
        Bounds bounds = targetObject.GetCurrentSpriteDef().GetUntrimmedBounds();
        float pivotX = -bounds.center.x / bounds.extents.x;
        float pivotY = -bounds.center.y / bounds.extents.y;
        targetObject.m_CentralBodyOffset = targetObject.m_SoftBodyOffset = new Vector3(pivotX * bounds.extents.x * targetObject.m_SpriteScale.x, pivotY * bounds.extents.y * targetObject.m_SpriteScale.y, 0.0f);
		targetObject.RefreshMesh();
	}

	void OnSceneGUI ()
	{
		UpdateHandles();
	}
}
#endif