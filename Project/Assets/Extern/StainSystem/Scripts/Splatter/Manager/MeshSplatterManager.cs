using UnityEngine;

namespace SplatterSystem {
    
    [System.Serializable]
    public class MeshSplatterManager : AbstractSplatterManager {
        [SerializeField]
        public SplatterSettings defaultSettings;

        [SerializeField][HideInInspector]
        public bool advancedSettings = false;

        private SplatterParticleProvider particles;
        private GameObject splatterBranchPrefab;
        
        void Awake() {
            splatterBranchPrefab = (GameObject) Resources.Load("MeshSplatterBranch");
            if (splatterBranchPrefab == null) {
                Debug.LogError("[SPLATTER SYSTEM] Can't find SplatterBranch prefab");
                enabled = false;
                return;
            }
            
            particles = gameObject.GetComponentInChildren<SplatterParticleProvider>();
            if (particles == null) {
                Debug.LogError("[SPLATTER SYSTEM] Can't find SplatterParticleProvider");
                enabled = false;
                return;
            }

            if (defaultSettings != null) {
                particles.Configure(defaultSettings);
            }
        }

        override public void SetDefaultSettings(SplatterSettings settings) {
            defaultSettings = settings;
            particles.Configure(settings);
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
            SplatterUtils.SpawnBranch(splatterBranchPrefab, transform, particles, settings, position, direction, color);
        }

        override public void Clear() {
            particles.Clear();
        }

        public void FadeOut() {
            particles.FadeOut();
        }

    }
}