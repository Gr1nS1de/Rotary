using System;
using UnityEngine;

namespace SplatterSystem.Platformer {
        
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class UserCharacterController : MonoBehaviour {
        public float speed = 14f;
        public float acceleration = 6f;
        public float airAcceleration = 3f;
        public float jumpPower = 14f;
        public float wallPushPower = 0.75f;

        protected GroundState groundState;
        protected Rigidbody2D rb;
        protected SpriteRenderer sprite;
        protected Vector2 input;

        protected virtual void Awake() {
            rb = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            groundState = new GroundState(transform.gameObject);
        }

        protected virtual void Update() {
            input.x = 0;
            if (Input.GetKey(KeyCode.LeftArrow) || 
                    (Input.touchCount > 0 && Input.touches[0].position.x < Screen.width * 0.5f)) {
                input.x = -1;
            }
            if (Input.GetKey(KeyCode.RightArrow) ||
                    (Input.touchCount > 0 && Input.touches[0].position.x > Screen.width * 0.5f)) {
                input.x = 1;
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) {
                input.y = 1;
            }

            // Reverse player sprite if going different direction
            if (input.x != 0f) {
				#if UNITY_5_3_OR_NEWER	
                if (Math.Abs(input.x - 1) < float.Epsilon) {
                    sprite.flipX = false;
                } else {
                    sprite.flipX = true;
                }
				#endif
            }
        }

        void FixedUpdate() {
            // Add acceleration.
            float a = groundState.IsGround() ? acceleration : airAcceleration;
            rb.AddForce(new Vector2(((input.x*speed) - rb.velocity.x)*a, 0f));
            // Stop or move player.
            float vx = (Math.Abs(input.x) < float.Epsilon && groundState.IsGround()) ? 0 : rb.velocity.x;
            // Jump from ground.
            float vy = (Math.Abs(input.y - 1) < float.Epsilon && groundState.IsTouching()) ? jumpPower : rb.velocity.y;
            rb.velocity = new Vector2(vx, vy);

            // Wall jump.
            if (groundState.IsWall() && !groundState.IsGround() && Math.Abs(input.y - 1) < float.Epsilon) {
                //Add force negative to wall direction (with speed reduction).
                rb.velocity = new Vector2(-groundState.WallDirection()*speed*wallPushPower, rb.velocity.y);
            }

            input.y = 0;
        }
    }

}