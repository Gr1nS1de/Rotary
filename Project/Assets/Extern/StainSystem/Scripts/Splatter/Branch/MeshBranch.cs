using UnityEngine;

namespace SplatterSystem {
	public class MeshBranch : BaseBranch {
		private SplatterParticleProvider particles;
		
		override public void SetParticleProvider(MonoBehaviour particleProvider) {
			particles = (SplatterParticleProvider) particleProvider;
		}

        protected override void SpawnParticle(Vector3 position, float scale, Color color) {
            particles.SetPosition(position);
            particles.SetScale(scale);
            particles.SetColor(color);
            particles.MoveToNext();
			particles.Apply();
        }
    }
}
