using System.Collections.Generic;
using UnityEngine;

namespace SplatterSystem.Platformer {
	[RequireComponent(typeof(Collider2D))]
	public class Pickup : MonoBehaviour {
		public AbstractSplatterManager splatterManager;
		public Color splatterColor;

		private static Shaker screenShake;
		private ParticleSystem particles;

		#if UNITY_5_4_OR_NEWER
		private List<ParticleCollisionEvent> collisionEvents;
		#else
		private ParticleCollisionEvent[] collisionEvents;
		#endif

		void Start() {
			particles = GetComponent<ParticleSystem>();

			#if UNITY_5_4_OR_NEWER
			collisionEvents = new List<ParticleCollisionEvent>(16);
			#else
			collisionEvents = new ParticleCollisionEvent[16];
			#endif


			if (screenShake == null) {
				var shakers = FindObjectsOfType<Shaker>();
				foreach (var shaker in shakers) {
					if (shaker.targetCamera) {
						screenShake = shaker;
						break;
					}
				}
			}
		}
		
		void OnTriggerEnter2D(Collider2D collider) {
			if (string.Equals(collider.tag, "Player")) {
				HandlePickup();
			}
		}

		private void HandlePickup() {
			var renderer = GetComponent<Renderer>();
			renderer.enabled = false;

			var collider = GetComponent<Collider2D>();
			collider.enabled = false;

			var particles = GetComponent<ParticleSystem>();
			if (particles != null) {
				particles.Play();
			}
		}

		void OnParticleCollision(GameObject other) {
			int safeLength = particles.GetSafeCollisionEventSize();

			#if UNITY_5_4_OR_NEWER
			if (collisionEvents.Count < safeLength) {
				collisionEvents = new List<ParticleCollisionEvent>(safeLength);
			}
			#else
			if (collisionEvents.Length < safeLength) {
				collisionEvents = new ParticleCollisionEvent[safeLength];
			}
			#endif
			
			int numCollisionEvents = particles.GetCollisionEvents(other, collisionEvents);
			for (int i = 0; i < numCollisionEvents; i++) {
				Vector3 position = collisionEvents[i].intersection;
				Vector3 velocity = collisionEvents[i].velocity;
				HandleParticleCollision(position, velocity);
			}
		}

        private void HandleParticleCollision(Vector3 position, Vector3 velocity) {
            if (splatterManager != null) {
				splatterManager.Spawn(position, velocity.normalized, splatterColor);
			}

            // Screen shake on landing.
			if (screenShake != null) {
				screenShake.Shake();
			}
        }
    }	
}
