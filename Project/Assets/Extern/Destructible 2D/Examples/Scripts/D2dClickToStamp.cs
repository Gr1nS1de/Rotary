using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dClickToStamp))]
	public class D2dClickToStamp_Editor : D2dEditor<D2dClickToStamp>
	{
		protected override void OnInspector()
		{
			DrawDefault("Requires");
			DrawDefault("Layers");
			DrawDefault("StampTex");
			DrawDefault("Size");
			DrawDefault("Angle");
			DrawDefault("Hardness");
		}
	}
}
#endif

namespace Destructible2D
{
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Click To Stamp")]
	public class D2dClickToStamp : MonoBehaviour
	{
		[Tooltip("The key you must hold down to stamp")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The GameObject layers this can stamp")]
		public LayerMask Layers = -1;
		
		[Tooltip("The shape of the stamp")]
		public Texture2D StampTex;
		
		[Tooltip("The size of the stamp")]
		public Vector2 Size = Vector2.one;
		
		[Tooltip("The angle of the stamp")]
		public float Angle;
		
		[Tooltip("The hardness of the stamp")]
		public float Hardness = 1.0f;
		
		protected virtual void Update()
		{
			// Required key is down?
			if (Input.GetKeyDown(Requires) == true)
			{
				// Main camera exists?
				var mainCamera = Camera.main;

				if (mainCamera != null)
				{
					// Get screen ray of mouse position
					var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

					// Project ray onto Z=0 plane and find the point it intersects
					var distance = D2dHelper.Divide(ray.origin.z, ray.direction.z);
					var point    = ray.origin - ray.direction * distance;
					
					// Stamp at that point
					D2dDestructible.StampAll(point, Size, Angle, StampTex, Hardness, Layers);
				}
			}
		}
	}
}