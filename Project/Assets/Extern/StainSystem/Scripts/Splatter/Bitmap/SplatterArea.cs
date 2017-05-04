using UnityEngine;

namespace SplatterSystem {
	#pragma warning disable 0414

	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(RectTransform))]
	[ExecuteInEditMode]
	public class SplatterArea : MonoBehaviour {
		public float pixelsPerUnit = 100f;
		public float scaleMultiplier = 1f;
		public bool debugDraw = false;

		public RectTransform rectTransform { get; private set; }

		private SpriteRenderer spriteRenderer;
		private Sprite sprite;
		private Texture2D texture;
		private bool isResizing = false;
		private bool isDirty = false;

		void OnEnable() {
			rectTransform = GetComponent<RectTransform>();
			spriteRenderer = GetComponent<SpriteRenderer>();

			if (Application.isPlaying) {
				debugDraw = false;
			}
			
			sprite = spriteRenderer.sprite;
			if (sprite == null) {
				ResetCanvasTexture();
				sprite = spriteRenderer.sprite;
			}
			texture = sprite.texture;
		}

		void LateUpdate() {
			if (isDirty) {
				texture.Apply();
				isDirty = false;
			}

			// Handle resizing.
	#if UNITY_EDITOR
			if (debugDraw && ((Color32)texture.GetPixel(0, 0)).a <= 1) {
				ResetCanvasTexture();
			}
			if (!debugDraw && ((Color32)texture.GetPixel(1, 1)).a > 1f) {
				ResetCanvasTexture();
			}
			if (!Application.isPlaying && isResizing) {
				if (Input.GetMouseButton(0)) {
					ResetSprite();
				}
				if (!Input.GetMouseButton(0)) {
					ResetCanvasTexture();
					isResizing = false;
				}
			}
	#endif
		}

		void OnRectTransformDimensionsChange() {
	#if UNITY_EDITOR
			isResizing = true;
			ResetSprite();
	#endif
		}

		public virtual void SpawnParticle(ParticleMode shape, Vector2 point, float scale, Color color) {
			Vector3 uv = WorldToTexturePosition(ref point);
			int particleSize = (int) (scaleMultiplier * scale * 40f);
			if (shape == ParticleMode.Square) {
				BitmapDraw.DrawFilledSquare(texture, (int)uv.x, (int)uv.y, particleSize, color);
			} else {
				BitmapDraw.DrawFilledCircle(texture, (int)uv.x, (int)uv.y, particleSize, color);
			}		
			isDirty = true;
		}

		public virtual void Apply() {
			isDirty = true;
		}

		public void ResetCanvasTexture() {
			if (rectTransform.rect.width <= 0 || rectTransform.rect.height <= 0) return;

			texture = new Texture2D((int)(rectTransform.rect.width * pixelsPerUnit), 
									(int)(rectTransform.rect.height * pixelsPerUnit), 
									TextureFormat.ARGB32, 
									false);
			texture.wrapMode = TextureWrapMode.Clamp;

			// Set alpha of all pixels to a minimum non-zero value to work around Unity not showing texture otherwise.
			var pixels = texture.GetPixels32();
			for (int i = 0; i < pixels.Length; i++) {
				pixels[i].a = (byte)(debugDraw? 100 : 1);
			}
			texture.SetPixels32(pixels);
			texture.Apply();

			var rect = new Rect(0, 0, texture.width, texture.height);
			sprite = Sprite.Create(texture, rect, rectTransform.pivot, pixelsPerUnit);
			spriteRenderer.sprite = sprite;
		}

		private Vector2 WorldToTexturePosition(ref Vector2 worldPos) {
			// TODO: Cache transform position.
			return (worldPos - (Vector2)rectTransform.position) * sprite.pixelsPerUnit + sprite.pivot;
		}

		private void ResetSprite() {
			if (spriteRenderer != null) {
				spriteRenderer.sprite = null;
			}
		}

	}

}
