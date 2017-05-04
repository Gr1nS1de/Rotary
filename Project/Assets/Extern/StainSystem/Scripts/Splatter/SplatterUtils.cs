using UnityEngine;
using Random = UnityEngine.Random;

namespace SplatterSystem {
    public static class SplatterUtils {

        public static Vector3 GetRandomDirection(SplatterSettings settings, Vector3? avgDir) {
            Vector3 moveDir = Vector3.zero;
            float random;
            DirectionMode currentDirectionMode = avgDir.HasValue? DirectionMode.Vector : settings.directionMode;
            if (!avgDir.HasValue) {
                avgDir = settings.direction;
            }
            switch (currentDirectionMode) {
                case DirectionMode.Vector:
                    random = Random.Range(-settings.spanAngle * 0.5f * Mathf.Deg2Rad, 
                                           settings.spanAngle * 0.5f * Mathf.Deg2Rad);
                    float movAngle = Mathf.Atan2(avgDir.Value.y, avgDir.Value.x);
                    movAngle += random;
                    if (settings.orientation == OrientationMode.Vertical) {
                        moveDir = new Vector3(Mathf.Cos(movAngle), Mathf.Sin(movAngle), 0f);
                    } else {
                        moveDir = new Vector3(Mathf.Cos(movAngle), 0f, Mathf.Sin(movAngle));
                    }
                    break;
                case DirectionMode.Transform:
                    if (settings.directionReference == null) {
                        Debug.LogError("[SPLATTER SYSTEM] Orientation reference is not set.");
                        return Vector3.zero;
                    }
                    var angles = settings.directionReference.rotation.eulerAngles * Mathf.Deg2Rad;
                    random = Random.Range(-settings.spanAngle * 0.5f * Mathf.Deg2Rad, 
                                           settings.spanAngle * 0.5f * Mathf.Deg2Rad);
                    angles.z += Mathf.Sin(random);
                    if (settings.orientation == OrientationMode.Vertical) {
                        moveDir = new Vector3(Mathf.Cos(angles.z), Mathf.Sin(angles.z), 0f);
                    } else {
                        moveDir = new Vector3(Mathf.Cos(angles.z), 0f, Mathf.Sin(angles.z));
                    }
                    break;
                default:
                    moveDir = Random.onUnitSphere;
                    if (settings.orientation == OrientationMode.Vertical) {
                        moveDir.z = 0;
                    } else {
                        moveDir.y = 0;
                    }
                    break;
            }
            return moveDir;
        }

        public static Color GetRandomColor(SplatterSettings settings, Color? color) {
            Color result = Color.white;
            if (!color.HasValue) {
                // Set color in HSV depending on mode.
                switch (settings.colorMode) {
                    case ColorMode.Continuous:
                        result = new Color(SplatterSettings.currentHue, SplatterSettings.startColorHSV.g,
                                SplatterSettings.startColorHSV.b);
                        const float hueSpeed = 0.005f;
                        SplatterSettings.currentHue = (SplatterSettings.currentHue + hueSpeed) % 1f;
                        break;
                    default:
                    case ColorMode.List:
                        if (settings.colors.Length == 0) {
                            Debug.LogError("[SPLATTER SYSTEM] No colors are defined in the colors list.");
                            return result;
                        }
                        int index = Random.Range(0, settings.colors.Length);
                        result = settings.colorsHSV[index];
                        break;
                }
            } else {
#if UNITY_5_3_OR_NEWER
                Color.RGBToHSV(color.Value, out result.r, out result.g, out result.b);
#else
				// TODO: convert to HSV.
				color = result;
#endif
            }

            if (settings.hueVariation > 0f) {
                float random = Random.Range(-settings.hueVariation, settings.hueVariation);
                result.r += random; // The color is in HSV here, so this is hue.
            }
            if (settings.saturationVariation > 0f) {
                float random = Random.Range(-settings.saturationVariation, settings.saturationVariation);
                result.g += random; // The color is in HSV here, so this is saturation.
            }
            if (settings.brightnessVariation > 0f) {
                float random = Random.Range(-settings.brightnessVariation, settings.brightnessVariation);
                result.b += random; // The color is in HSV here, so this is brightness.
            }

            // Color is defined, convert HSV to RGB.
#if UNITY_5_3_OR_NEWER
            return Color.HSVToRGB(result.r, result.g, result.b);
#else
			// TODO: convert to RGB.
			return result;
#endif
        }

        public static void SpawnBranch(GameObject splatterBranchPrefab, Transform branchParent, 
                                       MonoBehaviour particleProvider, SplatterSettings settings, Vector3 position, 
                                       Vector3? direction, Color? color) {
            float nbVar = Random.Range(-settings.numBranchesVariance, settings.numBranchesVariance);
            int numBranches = Mathf.RoundToInt(settings.numBranchesMean + nbVar);
            for (int i = 0; i < numBranches; i++) {
                Color c = SplatterUtils.GetRandomColor(settings, color);
                Vector3 dir = SplatterUtils.GetRandomDirection(settings, direction);
                float centerDisplacement = Random.value * settings.centerPositionRange;
                Vector2 centerAngle = Random.onUnitSphere; 
                position.x += centerAngle.x * centerDisplacement;
                if (settings.orientation == OrientationMode.Vertical) {
                    position.y += centerAngle.y * centerDisplacement;
                } else {
                    position.z += centerAngle.y * centerDisplacement;
                } 
                float scale = settings.scaleMean + Random.Range(-settings.scaleVariance, settings.scaleVariance);
                if (scale < 0f) {
                    scale = 0f;
                }

                var branchPrefab = SimplePool.Spawn(splatterBranchPrefab, Vector3.zero, Quaternion.identity);
                branchPrefab.transform.parent = branchParent;
                var branch = branchPrefab.GetComponent<BaseBranch>();
                branch.SetParticleProvider(particleProvider);
                branch.ResetAndStart(settings, position, scale, dir, c);
            }
        }
    }
}