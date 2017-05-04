using UnityEngine;
using System.Collections;

namespace SplatterSystem {
	
	// Based on https://gist.github.com/brettmjohnson/02cfcec45b5aa50add17825455c29af2
	// Which is based on http://unitytipsandtricks.blogspot.com/2013/05/camera-shake.html
	public class Shaker : MonoBehaviour {	
		public float duration = 2f;
		public float speed = 20f;
		public float magnitude = 2f;
		public AnimationCurve damper = new AnimationCurve(
			new Keyframe(0f, 1f), new Keyframe(0.9f, .33f, -2f, -2f), new Keyframe(1f, 0f, -5.65f, -5.65f));
		public float minStepSize = 0f;
		public bool targetCamera = true;
		public bool useCameraProjection = false;
		public bool testOnPlay = false;

		private Vector3 originalPos;
		private Vector2 seed;

		void OnEnable()	{
			seed = new Vector2(Random.value * 1000f, Random.value * 1000f);
			if (targetCamera) {
				originalPos = Camera.main.transform.localPosition;
			} else {
				originalPos = transform.localPosition;
			}
		}	
		

		void Update() {
			if (testOnPlay) {
				testOnPlay = false;
				Shake();
			}
		}

		virtual public void Shake() {
			StopAllCoroutines();
			if (targetCamera) {
				if (useCameraProjection) {
					StartCoroutine(ShakeCamera(Camera.main, duration, speed, magnitude, damper));
				} else {
					StartCoroutine(Shake(Camera.main.transform, originalPos, duration, speed, magnitude, damper));
				}
			} else {
				StartCoroutine(Shake(transform, originalPos, duration, speed, magnitude, damper));
			}
		}

		virtual public void Stop() {
			StopAllCoroutines();
		}

		IEnumerator Shake(Transform transform, Vector3 originalPosition, float duration, float speed, float magnitude, 
						AnimationCurve damper = null) {
			float elapsed = 0f;
			while (elapsed < duration) 
			{
				elapsed += Time.deltaTime;			
				float damperedMag = (damper != null) ? (damper.Evaluate(elapsed / duration) * magnitude) : magnitude;
				float x = (Mathf.PerlinNoise(Time.time * speed + seed.x, 0f) * damperedMag) - (damperedMag / 2f);
				float y = (Mathf.PerlinNoise(0f, Time.time * speed + seed.y) * damperedMag) - (damperedMag / 2f);
				if (minStepSize > 0f) {
					x = x - x % minStepSize;
					y = y - y % minStepSize;
				}
				transform.localPosition = new Vector3(
					originalPosition.x + x, originalPosition.y + y, originalPosition.z);
				yield return null;
			}
			transform.localPosition = originalPosition;
		}


		IEnumerator ShakeCamera(Camera camera, float duration, float speed, float magnitude, 
								AnimationCurve damper = null)
		{
			float elapsed = 0f;
			while (elapsed < duration) 
			{
				elapsed += Time.deltaTime;			
				float damperedMag = (damper != null) ? (damper.Evaluate(elapsed / duration) * magnitude) : magnitude;
				float x = (Mathf.PerlinNoise(Time.time * speed, 0f) * damperedMag) - (damperedMag / 2f);
				float y = (Mathf.PerlinNoise(0f, Time.time * speed) * damperedMag) - (damperedMag / 2f);
				// offset camera obliqueness - 
				// http://answers.unity3d.com/questions/774164/is-it-possible-to-shake-the-screen-rather-than-sha.html
				float frustrumHeight = 2 * camera.nearClipPlane * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
				float frustrumWidth = frustrumHeight * camera.aspect;
				Matrix4x4 mat = camera.projectionMatrix;
				mat[0, 2] = 2 * x / frustrumWidth;
				mat[1, 2] = 2 * y / frustrumHeight;
				camera.projectionMatrix = mat;
				yield return null;
			}
			camera.ResetProjectionMatrix();
		}

	}

}