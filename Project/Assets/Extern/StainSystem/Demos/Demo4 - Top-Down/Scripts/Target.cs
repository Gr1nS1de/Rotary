using System.Collections;
using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace SplatterSystem.TopDown {
		
	public class Target : MonoBehaviour {
		public MeshSplatterManager splatter;
		public float splatOffset = 0f;
		public Shaker screenShake;
		public SplatterSettings hitSplatterSettings;
		public Color hitSplatterColor = Color.red;
		public SplatterSettings dieSplatterSettins;
		public Color dieSplatterColor = Color.red;

		[Space(10)]
		public float healthPoints = 100f;

		private static int numTargets = 0;
		private float shakeMagnitude;
		private float shakeDuration;
		private bool isDead = false;

		void Start() {
            if (splatter == null) {
                Debug.LogError("[SPLATTER SYSTEM] No splatter manager attached to target.");
                return;
            }

			if (screenShake != null) {
				shakeMagnitude = screenShake.magnitude * healthPoints / 300f;
				shakeDuration = screenShake.duration * healthPoints / 300f;
			}

			if (numTargets == 0) {
				numTargets = FindObjectsOfType<Target>().Length;
			}
		}

		public void HandleHit(float damage, Vector2 direction) {
			healthPoints = Mathf.Max(healthPoints - damage, 0f);
			if (healthPoints <= 0f) {
				HandleDeath();
			} else {
				Vector2 hitPos = (Vector2) transform.position + splatOffset * direction;
				splatter.Spawn(hitSplatterSettings, hitPos, direction, hitSplatterColor);
			}
		}

		private void HandleDeath() {
			if (isDead) return;
			isDead = true;

			splatter.Spawn(dieSplatterSettins, transform.position, null, dieSplatterColor);
			
			if (screenShake != null) {
				screenShake.magnitude = shakeMagnitude;
				screenShake.duration = shakeDuration;
				screenShake.Shake();
			}
			
			// If this is last target - restart.
			numTargets--;
			if (numTargets <= 0) {
				StartCoroutine(HandleGameCompleted());
			} else {
				gameObject.SetActive(false);
			}
		}

		private IEnumerator HandleGameCompleted() {
			var renderer = GetComponent<Renderer>();
			renderer.enabled = false;

			yield return new WaitForSeconds(1.0f);
			splatter.FadeOut();

			yield return new WaitForSeconds(2.0f);
			#if UNITY_5_3_OR_NEWER
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			#endif
		}
	}

}