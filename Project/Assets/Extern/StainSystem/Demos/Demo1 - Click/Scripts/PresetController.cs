using UnityEngine;

namespace SplatterSystem {
	
	public class PresetController : MonoBehaviour {
		public AbstractSplatterManager manager;
		public SplatterSettings[] presets;
		public int startAtElement = 0;

		private int currentIndex = 0;

		void Start () {
			// There can be only one.
			var pc = GameObject.FindObjectsOfType<PresetController>();
			if (pc.Length > 1) {
				Destroy(gameObject);
				return;
			} else {
				DontDestroyOnLoad(gameObject);
			}

			currentIndex = startAtElement;
		}
		
		public virtual void ApplyNextPreset() {
			currentIndex = (currentIndex + 1) % presets.Length;
			Apply();
		}

		public virtual void ApplyPreviousPreset() {
			currentIndex = (currentIndex + presets.Length - 1) % presets.Length;
			Apply();
		}

		private void Apply() {
			manager.SetDefaultSettings(presets[currentIndex]);
			manager.Clear();
		}
	}

}