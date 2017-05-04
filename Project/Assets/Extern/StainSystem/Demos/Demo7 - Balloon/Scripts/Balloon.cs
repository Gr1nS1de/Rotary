using System.Collections;
using UnityEngine;

namespace SplatterSystem.Demos {
	
	public class Balloon : MonoBehaviour {
		public RectTransform hitBox;
		public AbstractSplatterManager splatter;
		public Shaker screenShake;

		void Start() {
			if (splatter == null) {
				splatter = FindObjectOfType<AbstractSplatterManager>();
			}
		}

		void Update () {
			bool justClicked = Input.GetMouseButtonDown(0);

			if (justClicked) {
				Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if (hitBox.rect.Contains(worldPos - (Vector2)hitBox.transform.position)) {
					StartCoroutine(HandleBaloonPop(worldPos));
				}
			}
		}

		private IEnumerator HandleBaloonPop(Vector3 pos) {
			gameObject.SetActive(false);
			splatter.Spawn(pos);
			if (screenShake != null) {
				screenShake.Shake();
			}

			yield return 0;
		}
	}
}
