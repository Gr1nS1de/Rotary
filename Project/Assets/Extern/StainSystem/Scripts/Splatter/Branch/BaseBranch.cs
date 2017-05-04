
using UnityEngine;

namespace SplatterSystem {
	public abstract class BaseBranch : MonoBehaviour {
		protected SplatterSettings settings;
		private Vector3 position;
		private float startScale;
		private Vector3 moveDir;
		private Color color;

		private int numChildParticles;
        private float moveSpeed;
        private float progress;
        private float minScale = 0f;

		private bool isSpawning = false;
		private float nextSpawnTime;

		void Update () {
			if (isSpawning) {
				if (Time.time > nextSpawnTime) {
					SpawnChildParticle();
					if (progress > minScale) {
						nextSpawnTime += settings.stepDuration;
					} else {
						isSpawning = false;
						SimplePool.Despawn(gameObject);
					}
				}
			}
		}

		public abstract void SetParticleProvider(MonoBehaviour particleProvider);

		public virtual void ResetAndStart(SplatterSettings settings, Vector3 position, float startScale, 
										  Vector3 moveDir, Color color) {
			this.settings = settings;
			this.position = position;
			this.startScale = startScale;
			this.moveDir = moveDir;
			this.color = color;

			float numChildVar = Random.Range(-settings.branchChildrenVariance, settings.branchChildrenVariance);
			numChildParticles = Mathf.RoundToInt(Mathf.Max(settings.branchChildrenMean + numChildVar, 1));
			moveSpeed = settings.moveSpeedMean + 
						Random.Range(-settings.moveSpeedVariance, settings.moveSpeedVariance);
			progress = 1f - 1f / (numChildParticles + 1f);
			
			isSpawning = true;
			nextSpawnTime = Time.time;

			minScale = settings.scaleMin;
			
			enabled = true;
		}

        protected virtual void SpawnChildParticle() {
			SpawnParticle(position, progress * startScale, color);

			moveSpeed = Mathf.Lerp(moveSpeed, 0f, settings.damping);
			position += moveDir * moveSpeed;

			float deltaProgress = 1f / numChildParticles;
			progress -= deltaProgress;

			if (settings.fadeOutBranches) {
				color.a -= deltaProgress * Random.Range(0.7f, 1.3f);
			}
        }

        abstract protected void SpawnParticle(Vector3 position, float scale, Color color);
	}
}
