using UnityEngine;

namespace SplatterSystem.Isometric {

    public class SplatterUserCharacterController : SplatterSystem.Isometric.UserCharacterController {
        [Space(10)]
        public bool useSplatter = true; 
        public float paintTimeout = 0.05f;
        public AbstractSplatterManager splatter;
        public Vector3 paintPositionOffset = Vector3.zero;

        private float lastSplatterTime;

        override protected void Awake() {
            base.Awake();
            lastSplatterTime = Time.time;
        }

        override protected void Update() {
            base.Update();
            
            if (useSplatter && (Time.time - lastSplatterTime) > paintTimeout && input != Vector3.zero) {
                lastSplatterTime = Time.time;
                splatter.Spawn(transform.position + paintPositionOffset);   
            }
        }
    }
    
}