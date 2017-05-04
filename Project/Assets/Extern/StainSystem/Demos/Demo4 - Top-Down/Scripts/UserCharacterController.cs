using UnityEngine;

namespace SplatterSystem.TopDown {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class UserCharacterController : MonoBehaviour {
        public float speed = 14f;
        public float acceleration = 6f;

        protected Rigidbody2D rb;
        protected SpriteRenderer sprite;
        protected Vector2 input;

        protected virtual void Awake() {
            rb = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
        }

        protected virtual void Update() {
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
        }

        void FixedUpdate() {
            rb.AddForce(((input*speed) - rb.velocity) * acceleration);
        }
    }

}