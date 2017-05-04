
using UnityEngine;

namespace SplatterSystem.Platformer {

    public class SplatterUserCharacterController : UserCharacterController {
        public bool usePaint = true; 
        public float paintTimeout = 0.05f;
        public AbstractSplatterManager splatter;
        public float paintPositionOffset = 0;

        private float lastSplatterTime;

        override protected void Awake() {
            base.Awake();
            lastSplatterTime = Time.time;
        }

        override protected void Update() {
            base.Update();
            
            // Paint.
            if (usePaint && (Time.time - lastSplatterTime > paintTimeout) && groundState.IsTouching() && 
                    input != Vector2.zero) {
                lastSplatterTime = Time.time;
                
                if (groundState.raycastDown) {
                    splatter.Spawn(groundState.raycastDown.point + Vector2.down * paintPositionOffset, Vector3.down);
                }
                if (groundState.raycastUp) {
                    splatter.Spawn(groundState.raycastUp.point + Vector2.up * paintPositionOffset, Vector3.up);
                }
                if (groundState.raycastLeft) {
                    splatter.Spawn(groundState.raycastLeft.point + Vector2.left * paintPositionOffset, Vector3.left);
                }
                if (groundState.raycastRight) {
                    splatter.Spawn(groundState.raycastRight.point + Vector2.right * paintPositionOffset, Vector3.right);
                }
                
            }
        }
    }
    
}