using UnityEngine;

namespace SplatterSystem {
	
	public class ClickEmitter : MonoBehaviour {
		public AbstractSplatterManager splatter;
        public bool emitWhilePressed = false;
        public float emitPressedTimout = 0.1f;

        private float lastEmitTime;
		private float justClickedTime;

		void Start () {
            lastEmitTime = Time.time;
			justClickedTime = float.MaxValue;
		}
		
		void Update () {
			if (Input.GetMouseButtonDown(0)) {
				justClickedTime = Time.time;
			}

            bool justClicked = Input.GetMouseButtonDown(0);
            bool autoEmit = emitWhilePressed && Input.GetMouseButton(0) 
							&& (Time.time > (lastEmitTime + emitPressedTimout))
							&& (Time.time > (justClickedTime + 0.5f));

			if (justClicked || autoEmit) {
				lastEmitTime = Time.time;
				Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Collider2D collider = Physics2D.OverlapPoint(worldPos);
				if (collider != null) {
					//splatter.Spawn(worldPos);
				}
			}
		}
	}

}
