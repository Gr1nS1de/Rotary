using UnityEngine;

namespace SplatterSystem {
	public class BitmapBranch : BaseBranch {
		private SplatterArea area;

		override public void SetParticleProvider(MonoBehaviour particleProvider) {
			area = (SplatterArea) particleProvider;
		}

        override protected void SpawnParticle(Vector3 position, float scale, Color color) {
			area.SpawnParticle(settings.particleMode, position, scale, color);
        }
    }
}
