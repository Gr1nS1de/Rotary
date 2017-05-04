using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace SplatterSystem.Platformer {
	public class LevelReset : MonoBehaviour {
		public Transform target;
		public float minTargetY = -10f;
		public KeyCode restartKey = KeyCode.Escape;

		void Update () {
			bool resetByKey = Input.GetKeyDown(restartKey);
			bool resetByTarget = target != null && target.position.y < minTargetY;
			if (resetByKey || resetByTarget) {
				#if UNITY_5_3_OR_NEWER
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				#endif
			}
		}
	}
}
