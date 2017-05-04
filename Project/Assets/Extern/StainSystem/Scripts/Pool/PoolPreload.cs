using UnityEngine;

namespace SplatterSystem {
    
    public class PoolPreload : MonoBehaviour {
        
        [System.Serializable]
        public struct PoolPreloadItem {
            public GameObject prefab;
            public int quantity;
        }
        
        public PoolPreloadItem[] poolPreload;
        
        void Awake() {
            // Preload pool items.
            foreach (var item in poolPreload) {
                SimplePool.Preload(item.prefab, item.quantity);
            }
        }
        
    }

}