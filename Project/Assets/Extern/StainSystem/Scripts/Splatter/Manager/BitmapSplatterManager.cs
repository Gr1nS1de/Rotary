using UnityEngine;

namespace SplatterSystem {
    
    [System.Serializable]
	public class BitmapSplatterManager : AbstractSplatterManager {
		public SplatterSettings defaultSettings;
		private SplatterArea[] areas;
        private GameObject splatterBranchPrefab;

		void Start () {
			areas = FindObjectsOfType<SplatterArea>();

            splatterBranchPrefab = (GameObject) Resources.Load("BitmapSplatterBranch");
            if (splatterBranchPrefab == null) {
                Debug.LogError("[SPLATTER SYSTEM] Can't find SplatterBranch prefab");
                enabled = false;
                return;
            }
		}

        override public void SetDefaultSettings(SplatterSettings settings) {
            defaultSettings = settings;
        }
		
        override public void Spawn(Vector3 position) {
            Spawn(position, null, null);
        }

        override public void Spawn(Vector3 position, Vector3 direction) {
            Spawn(position, direction, null);
        }

        override public void Spawn(Vector3 position, Color color) {
            Spawn(position, null, color);
        }

        override public void Spawn(Vector3 position, Vector3? direction, Color? color) {
            if (defaultSettings == null) {
                Debug.LogError("[SPLATTER SYSTEM] No default settings is assigned in SplatterManager.");
                return;
            }

            Spawn(defaultSettings, position, direction, color);
        }

        override public void Spawn(SplatterSettings settings, Vector3 position, Vector3? direction, Color? color) {
			// Find SplatterArea at position.
			SplatterArea area = null;
			foreach (var a in areas) {
				if (a.rectTransform.rect.Contains(position - a.rectTransform.position)) {
					area = a;
					break;
				}
			}
			// If no area found, return.
			if (area == null) {
				return;
			}

            SplatterUtils.SpawnBranch(splatterBranchPrefab, transform, area, settings, position, direction, color);
        }

        override 
        public void Clear() {
            foreach (var area in areas) {
                area.ResetCanvasTexture();
            }
        }

	}

}