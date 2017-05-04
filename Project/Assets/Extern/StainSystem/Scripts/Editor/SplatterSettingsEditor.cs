using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace SplatterSystem {
	[CustomEditor(typeof(SplatterSettings))]
	public class SplatterSettingsEditor : Editor {
		private static bool showHints = false;
		private static bool showIntencityAdvanced = false;
		private static bool showMovementAdvanced = false;
		private static bool showColorsAdvanced = false;

		void OnEnable() {
			showHints = EditorPrefs.GetBool("SplatterSystem.showHints", false);
			showIntencityAdvanced = EditorPrefs.GetBool("SplatterSystem.showIntencityAdvanced", false);
			showMovementAdvanced = EditorPrefs.GetBool("SplatterSystem.showMovementAdvanced", false);
			showColorsAdvanced = EditorPrefs.GetBool("SplatterSystem.showColorsAdvanced", false);
		}

		void OnDisable() {
			EditorPrefs.SetBool("SplatterSystem.showHints", showHints);
			EditorPrefs.SetBool("SplatterSystem.showIntencityAdvanced", showIntencityAdvanced);
			EditorPrefs.SetBool("SplatterSystem.showMovementAdvanced", showMovementAdvanced);
			EditorPrefs.SetBool("SplatterSystem.showColorsAdvanced", showColorsAdvanced);
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			SplatterSettings s = target as SplatterSettings;
			Undo.RecordObject(s, "SplatterSettings_test1");
			EditorGUI.BeginChangeCheck();

			// Settings mode
			EditorGUILayout.LabelField("Splatter mode", EditorStyles.boldLabel);
			ShowHint("Bitmap and Mesh modes have different parameters, this shows which parameters are in use.");
			var style = new GUIStyle(EditorStyles.popup);
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 12;
			style.stretchHeight = true;
			style.fixedHeight = 18;
			style.margin.bottom = 10;
			s.mode = (SettingsMode) EditorGUILayout.EnumPopup(s.mode, style);

			// Movement
			EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			ShowHint("Splatter direction can be defined in different ways. Click through options for more details.");
			s.directionMode = (DirectionMode) EditorGUILayout.EnumPopup("Mode", s.directionMode);
			EditorGUI.indentLevel++;
			if (s.directionMode != DirectionMode.AllDirections) {
				ShowHint("Radial angle of the splatter in degrees. A value of 180 means only one half of the space " +
						 "will be covered in splatter. Value of 360 means splatter goes in all directions.");
				s.spanAngle = EditorGUILayout.FloatField("Span angle", s.spanAngle);

				if (s.directionMode == DirectionMode.Transform) {
					ShowHint("Splatter particles will go in the direction of the reference Transform.");
					s.directionReference = EditorGUILayout.ObjectField("Direction reference", s.directionReference, 
																	   typeof(Transform), true) as Transform;
				}

				if (s.directionMode == DirectionMode.Vector) {
					ShowHint("Constant direction of splatter in world coordinates.");
					s.direction = EditorGUILayout.Vector3Field("Direction", s.direction);
				}
			} else {
				ShowHint("Splatter is done in all 360 degrees.");
			}
			EditorGUI.indentLevel--;

			ShowHint("How random starting positions of branches are.");
			s.centerPositionRange = EditorGUILayout.Slider("Position variance", s.centerPositionRange, 0f, 1f);
			
			ShowHint("Distance between particles in a branch. Particles are placed on fixed timesteps, so this is " + 
					 "also the mean speed of particles. This value is affected by the 'Step duration' setting.");
			s.moveSpeedMean = EditorGUILayout.Slider("Speed", s.moveSpeedMean, 0f, 2f);

			ShowHint("Time in seconds between spawning two particles in a branch.");
			s.stepDuration = EditorGUILayout.Slider("Step duration", s.stepDuration, 0.001f, 0.25f);

			showMovementAdvanced = EditorGUILayout.Foldout(showMovementAdvanced, "Advanced");
			if (showMovementAdvanced) {
				EditorGUI.indentLevel++;

				ShowHint("Damping to be applied to particle speed. Damping is a factor that gradually changes a the " + 
						"speed towards a zero over time. A value of zero here means the particle will move with " +
						"constant speed. Higher damping looks cooler.");
				s.damping = EditorGUILayout.Slider("Damping", s.damping, 0f, 1f);

				ShowHint("Maximum possible deviation of move speed from the mean value above.");
				s.moveSpeedVariance = EditorGUILayout.Slider("Speed variance", s.moveSpeedVariance, 0f, 1f);

				ShowHint("Whether each particle should have a random angle when spawned.");
				s.randomRotation = EditorGUILayout.Toggle("Random rotation", s.randomRotation);

				EditorGUI.indentLevel--;
			}

			EditorGUI.indentLevel--;

			// Orientation
			if (s.mode == SettingsMode.Mesh) {
				EditorGUILayout.LabelField("Orientation", EditorStyles.boldLabel);
				ShowHint("The direction which the splatter faces. Vertical is suitable for splatter on walls and for 2D, " +
						"whereas horizontal is suitable for floor splatter in 3D games.");
				EditorGUI.indentLevel++;
				s.orientation = (OrientationMode) EditorGUILayout.EnumPopup("Mode", s.orientation);
				EditorGUI.indentLevel--;
			}

			// Intensity
			EditorGUILayout.LabelField("Intensity", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;

			ShowHint("Splatters consist of multiple branches (long lines of particles). This value is the mean " +
					 "number of branches in each spllatter.");
			s.numBranchesMean = EditorGUILayout.Slider("Branches", s.numBranchesMean, 1f, 20f);
			
			ShowHint("Mean number of particles in each branch.");
			s.branchChildrenMean = EditorGUILayout.Slider("Branch children", s.branchChildrenMean, 1f, 20f);

			ShowHint("Mean scale of particles at which each branch starts. Scale of particles in each branch is " +
					 "gradually reduced to zero.");
			s.scaleMean = EditorGUILayout.Slider("Branch scale", s.scaleMean, 0f, 3f);

			showIntencityAdvanced = EditorGUILayout.Foldout(showIntencityAdvanced, "Advanced");
			if (showIntencityAdvanced) {
				EditorGUI.indentLevel++;

				ShowHint("Maximum possible deviation of branch number from the mean value above.");
				EditorGUILayout.LabelField("Branches variance");
				s.numBranchesVariance = EditorGUILayout.Slider(s.numBranchesVariance, 0f, 5f);
				
				ShowHint("Maximum possible deviation of branch children from the mean value above.");
				EditorGUILayout.LabelField("Branch children variance");
				s.branchChildrenVariance = EditorGUILayout.Slider(s.branchChildrenVariance, 0f, 10f);
				
				ShowHint("Maximum possible deviation of children scale from the mean value above.");
				EditorGUILayout.LabelField("Branch scale variance");
				s.scaleVariance = EditorGUILayout.Slider(s.scaleVariance, 0f, 1f);

				ShowHint("Minimum size of particles at which branches stop spawning.");
				EditorGUILayout.LabelField("Minimum particle size");
				s.scaleMin = EditorGUILayout.Slider(s.scaleMin, 0f, 1f);
				
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;

			// Colors
			EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;

			ShowHint("The way colors of branches are picked.\n" +
					 "Continuous: aka Ranbow mode. In this mode colors are calculated from the HSV space, where hue " +
					 "continuously moves around the circle.\n" +
					 "List: colors for each branch are picked randomly from a list.");
			s.colorMode = (ColorMode) EditorGUILayout.EnumPopup("Mode", s.colorMode);
			EditorGUI.indentLevel++;
			switch (s.colorMode) {
				case ColorMode.Continuous:
					ShowHint("Color that will be used for the first branch spawned.");
					s.startColor = EditorGUILayout.ColorField("Start color", s.startColor);
					break;
				case ColorMode.List:
					EditorList.Show(serializedObject.FindProperty("colors"), 
									EditorListOption.Buttons | EditorListOption.ListLabel);
					break;
			}
			EditorGUI.indentLevel--;

			if (s.mode == SettingsMode.Mesh) {
				ShowHint("Multiply colors of overlapping colors instead of replacing them.");
				s.multiplyColor = EditorGUILayout.Toggle("Multily blending", s.multiplyColor);
			}

			showColorsAdvanced = EditorGUILayout.Foldout(showColorsAdvanced, "Advanced");
			if (showColorsAdvanced) {
				EditorGUI.indentLevel++;

				ShowHint("Maximum random value that will be added to brighness of branch color in HSV space.");
				EditorGUILayout.LabelField("Brightness variation");
				s.brightnessVariation = EditorGUILayout.Slider(s.brightnessVariation, 0f, 0.5f);

				ShowHint("Maximum random value that will be added to hue of branch color in HSV space.");
				EditorGUILayout.LabelField("Hue variation");
				s.hueVariation = EditorGUILayout.Slider(s.hueVariation, 0f, 0.5f);

				ShowHint("Maximum random value that will be added to saturation of branch color in HSV space.");
				EditorGUILayout.LabelField("Saturation variation");
				s.saturationVariation = EditorGUILayout.Slider(s.saturationVariation, 0f, 0.5f);

				EditorGUILayout.Separator();

				ShowHint("Whether particles in each branch should become transparent towards end.");
				s.fadeOutBranches = EditorGUILayout.Toggle("Fade out branches", s.fadeOutBranches);
				EditorGUI.indentLevel--;
			}

			EditorGUI.indentLevel--;

			// Particles
			EditorGUILayout.LabelField("Particles", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;

			ShowHint("Shape of small particles that splatter consists of.");
			s.particleMode = (ParticleMode) EditorGUILayout.EnumPopup("Shape", s.particleMode);

			if (s.mode == SettingsMode.Bitmap && s.particleMode == ParticleMode.CustomTexture) {
				EditorGUILayout.HelpBox("Custom textures can't be used in Bitmap mode, please select other shape or " +
						"switch to Mesh mode", MessageType.Warning);
			}

			if (s.particleMode == ParticleMode.CustomMaterial) {
				s.particleMaterial = 
						(Material) EditorGUILayout.ObjectField(s.particleMaterial, typeof(Material), false);
			}
			
			if (s.mode == SettingsMode.Mesh) {
				if (s.particleMode == ParticleMode.CustomTexture) {
					s.particleTexture = 
							(Texture2D) EditorGUILayout.ObjectField(s.particleTexture, typeof(Texture2D), false);
				}
				
				ShowHint("The maximum number of particles that can exist at the same time. When this number is " +
						 "exeeded the oldest particles are removed. This setting is very important for performance. " + 
						 "Lower number means faster execution.");
				s.maxParticles = EditorGUILayout.IntSlider("Max quantity", s.maxParticles, 100, 20000);
				
				ShowHint("Whether the particles should be removed after some time.");
				s.removeParticles = EditorGUILayout.Toggle("Fade out", s.removeParticles);

				if (s.removeParticles) {
					EditorGUI.indentLevel++;

					ShowHint("Time in seconds between spawning and fading out a particle.");
					s.particleLifetime = EditorGUILayout.FloatField("Lifetime", s.particleLifetime);

					EditorGUI.indentLevel--;
				}

				ShowHint("The Sorting Layer that the splatter should draw to.");
				var sortingLayerNames = SortingLayer.layers.Select(l => l.name).ToArray();
				int currentIndex = Array.IndexOf(sortingLayerNames, s.sortingLayerName);
				int newLayerIndex = EditorGUILayout.Popup("Sorting Layer", currentIndex, sortingLayerNames);
				if (newLayerIndex < sortingLayerNames.Length && newLayerIndex >= 0) {
					s.sortingLayerName = sortingLayerNames[newLayerIndex];
				}
			}
			
			EditorGUI.indentLevel--;

			// Utility
			EditorGUILayout.LabelField("Utility", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;

			showHints = EditorGUILayout.Toggle("Show explanations", showHints);

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(16);
			if (GUILayout.Button(EditorGUIUtility.IconContent("_Help"), 
					GUILayout.MinWidth(40), GUILayout.MinHeight(20))) {
				Application.OpenURL("http://dustyroom.com/splatter-system/manual/");
			}
			EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel--;

			if (EditorGUI.EndChangeCheck()) {
				EditorUtility.SetDirty(target);
				serializedObject.ApplyModifiedProperties();
			}
		}
		
		[MenuItem("Assets/Create/SplatterSettings")]
		public static void CreateSplatterSettings() {
			CustomAssetMaker.CreateAsset<SplatterSettings>("New SplatterSettings");		
		}

		private void ShowHint(string s) {
			if (SplatterSettingsEditor.showHints) {
				EditorGUILayout.HelpBox(s, MessageType.None);
			}
		}
	}

	//[CustomPropertyDrawer(typeof(SplatterSettings))]
	public class SplatterSettingsDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
			EditorGUILayout.HelpBox("SplatterSettings object defines splatter parameters.",	MessageType.None);
			EditorGUILayout.PropertyField(prop, true);
		}
	}

}