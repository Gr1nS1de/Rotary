using UnityEngine;

namespace SplatterSystem.TopDown {
    [RequireComponent(typeof(LineRenderer))]
    public class MouseShooter : MonoBehaviour {
        [Space(10)]
        public bool shootWhilePressed = false;
        public float shootingTimout = 0.1f;
        public float hitDamage = 25f;

		[Space(10)]
        public Transform rotateTransform;
        public float lineLength = 10.0f;

		[Space(10)]
        public float recoilStrength = 1f;

        private LineRenderer lineRenderer;
        private Rigidbody2D rb;
        private float lastShotTime;

        private void Start() {
            lineRenderer = GetComponent<LineRenderer>();
            rb = GetComponent<Rigidbody2D>();

            lastShotTime = Time.time;
        }

        private void Update() {
            lineRenderer.SetPosition(0, transform.position);

            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            Vector2 direction = mouseWorldPosition - transform.position;
            direction.Normalize();

            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, direction);
            Vector2 pos2d = (Vector2) transform.position;
            if (raycastHit.collider != null && (raycastHit.point - pos2d).magnitude < lineLength) {
                lineRenderer.SetPosition(1, raycastHit.point);
            } else {
                lineRenderer.SetPosition(1, pos2d + direction * lineLength);
            }

            if (rotateTransform != null) {
                var d = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
                rotateTransform.rotation = Quaternion.AngleAxis(Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg, Vector3.forward);
            }

            bool justClicked = Input.GetMouseButtonDown(0) && !shootWhilePressed;
            bool autoShot = shootWhilePressed && Input.GetMouseButton(0) && Time.time > (lastShotTime + shootingTimout);

            if (justClicked || autoShot) {
                // Apply recoil.
                if (rb != null) {
                    rb.AddForce(recoilStrength * direction * -1f, ForceMode2D.Impulse);
                }
                
                lastShotTime = Time.time;

                if (raycastHit.collider != null) {
                    // Apply impulse to the hit object.
                    if (raycastHit.rigidbody != null) {
                        raycastHit.rigidbody.AddForce(recoilStrength * direction, ForceMode2D.Impulse);
                    }

                    // Damage the hit object.
                    var target = raycastHit.transform.GetComponent<Target>();
                    if (target != null) {
                        target.HandleHit(hitDamage, direction);
                    }
                }
            }
        }
    }
}