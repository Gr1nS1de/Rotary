using UnityEngine;

namespace SplatterSystem {

	public enum SettingsMode {
		Bitmap,
		Mesh
	}

	public enum DirectionMode {
		AllDirections,
		Transform,
		Vector
	}

	public enum ColorMode {
		List,
		Continuous
	}

	public enum OrientationMode {
		Vertical,
		Horizontal
	}

	public enum ParticleMode {
		Circle,
		Square,
		CustomTexture,
		CustomMaterial
	}

	[System.Serializable]
	public class SplatterSettings : ScriptableObject {
		public SettingsMode mode = SettingsMode.Bitmap;
		public DirectionMode directionMode = DirectionMode.AllDirections;
		public float spanAngle = 180f;
		public Transform directionReference = null;
		public Vector3 direction = Vector3.zero;
		public OrientationMode orientation = OrientationMode.Vertical;
		public float numBranchesMean = 8.5f;
		public float numBranchesVariance = 1.5f;
		public float branchChildrenMean = 7.5f;
		public float branchChildrenVariance = 2.5f;
		public float scaleMean = 0.5f;
		public float scaleVariance = 0.1f;
		public float scaleMin = 0.1f;
		public float centerPositionRange = 0.4f;
		public bool randomRotation = false;
		public float moveSpeedMean = 0.5f;
		public float moveSpeedVariance = 0.3f;
		public float stepDuration = 0.01f;
		public float damping = 0.3f;
		public ColorMode colorMode = ColorMode.Continuous;
		public Color startColor = Color.red;	//  Used in the RandomRange color mode.
		public Color[] colors = new Color[0];	//  Used in the List color mode.
		public bool multiplyColor = false;
		public bool fadeOutBranches = false;
		public float brightnessVariation = 0.1f;
		public float hueVariation = 0f;
		public float saturationVariation = 0f;
		public ParticleMode particleMode = ParticleMode.Circle;
		public Texture2D particleTexture = null;
		public Material particleMaterial = null;
		public int maxParticles = 10000;
		public bool removeParticles = false;
		public float particleLifetime = 10f;
		public string sortingLayerName = "Default";

        public Color[] colorsHSV { get; private set; }

        public static float currentHue { get; set; }
        public static Color startColorHSV { get; set; }

		void OnEnable() {
			colorsHSV = new Color[colors.Length];
			for (int i = 0; i < colors.Length; ++i) {
				colorsHSV[i] = new Color();
#if UNITY_5_3_OR_NEWER
				Color.RGBToHSV(colors[i], out colorsHSV[i].r, out colorsHSV[i].g, out colorsHSV[i].b);
#else
				// TODO: Convert to HSV.
				colorsHSV[i] = colors[i];
#endif
			}

			Color startHSV = new Color();
#if UNITY_5_3_OR_NEWER
			Color.RGBToHSV(startColor, out startHSV.r, out startHSV.g, out startHSV.b);
#else
			// TODO: Convert to HSV.
			startHSV = startColor;
#endif
			currentHue = startHSV.r;
			startColorHSV = startHSV;
		}
	}
}