using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dCameraShake))]
	public class D2dCameraShake_Editor : D2dEditor<D2dCameraShake>
	{
		protected override void OnInspector()
		{
			DrawDefault("ShakeScale");
			DrawDefault("ShakeDampening");
			DrawDefault("ShakeSpeed");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component allows you to make the objects shake
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Camera Shake")]
	public class D2dCameraShake : MonoBehaviour
	{
		// Global shake amount that gets reduced every frame
		public static float Shake;
		
		// The amount this camera shakes relative to the Shake value
		public float ShakeScale = 1.0f;
		
		// The speed at which the Shake value gets reduced
		public float ShakeDampening = 10.0f;
		
		// The freqncy of the camera shake
		public float ShakeSpeed = 10.0f;
		
		private float offsetX;
		
		private float offsetY;
		
		protected virtual void Awake()
		{
			offsetX = Random.Range(-1000.0f, 1000.0f);
			offsetY = Random.Range(-1000.0f, 1000.0f);
		}
		
		protected virtual void LateUpdate()
		{
			Shake = D2dHelper.Dampen(Shake, 0.0f, ShakeDampening, Time.deltaTime);
			
			var shakeStrength = Shake * ShakeScale;
			var shakeTime     = Time.time * ShakeSpeed;
			var offset        = Vector2.zero;
			
			offset.x = Mathf.PerlinNoise(offsetX, shakeTime) * shakeStrength;
			offset.y = Mathf.PerlinNoise(offsetY, shakeTime) * shakeStrength;
			
			transform.localPosition = offset;
		}
	}
}