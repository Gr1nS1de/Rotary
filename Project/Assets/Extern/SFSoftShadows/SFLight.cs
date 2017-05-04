// Super Fast Soft Lighting. Copyright 2015 Howling Moon Software, LLP

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class SFLight : MonoBehaviour, _SFCullable {
	[Tooltip("The radius of the light source. Larger lights cast softer shadows.")]
	public float _radius = 0.5f;

	[Tooltip("The brightness of the light. (Ignored when using non-linear light blending.) Allows for colors brighter than 1.0 in HDR lighting situations.")]
	public float _intensity = 1.0f;

	[Tooltip("The color of the light.")]
	public Color _color = Color.white;

	[Tooltip("The shape of the light.")]
	public Texture2D _cookieTexture;
	
	[Tooltip("Which layers cast shadows.")]
	public LayerMask _shadowLayers = ~0;

	public float radius {get {return _radius;} set {_radius = value;}}
	public float intensity {get {return _intensity;} set {_intensity = value;}}
	public Color color {get {return _color;} set {_color = value;}}
	public Texture2D cookieTexture {get {return _cookieTexture;} set {_cookieTexture = value;}}
	public LayerMask shadowLayers {get {return _shadowLayers;} set {_shadowLayers = value;}}

	private RectTransform _rt;

	public static List<SFLight> _lights = new List<SFLight>();
	private void OnEnable(){_lights.Add(this);}
	private void OnDisable(){_lights.Remove(this);}

	public Vector2 _position {
		get {
			if (!_rt) _rt = GetComponent<RectTransform>();
			return _rt.position;
		}
	}

	public Matrix4x4 _lightMatrix {
		get {
			if (!_rt) _rt = GetComponent<RectTransform> ();
			
			Vector2 r = _rt.sizeDelta / 2.0f;
			Vector2 p = Vector2.one - 2.0f*_rt.pivot;
			return Matrix4x4.TRS(new Vector3(p.x*r.x, p.y*r.y, 0.0f), Quaternion.identity, new Vector3(r.x, r.y, 1.0f));
		}
	}

	private Rect _worldBounds;
	public Rect _GetWorldBounds(){return _worldBounds;}

	public void _CacheWorldBounds(){
		var matrix = this._lightMatrix;
		_worldBounds = SFRenderer._TransformRect(_rt.localToWorldMatrix*matrix, new Rect(-1, -1, 2, 2));
	}

	private class VertexArray {
		public int capacity, size;
		public Vector3[] verts;
		public Vector4[] tangents;
		public Vector2[] uvs;
		public int[] tris;

		public VertexArray(int segments){
			capacity = segments;
			size = 0;

			verts = new Vector3[segments*4];
			tangents = new Vector4[segments*4];
			uvs = new Vector2[segments*4];
			tris = new int[segments*6];
		}
	}

	private static int GROWTH_NUM = 4;
	private static int GROWTH_DENOM = 3;

	// Should provide a quarter million verts per light in the worst case.
	private static VertexArray[] vertexArrays = new VertexArray[40];
	private VertexArray GetVertexArray(int segments){
		int i = 0, size = 4;
		while(segments > size){
			size = size*GROWTH_NUM/GROWTH_DENOM;
			i++;
		}

		if(i >= vertexArrays.Length){
			Debug.LogError("SFSS: Maximum vertexes per light exceeded. (" + size + ")");
			return null;
		}

		if(vertexArrays[i] == null){
			vertexArrays[i] = new VertexArray(size);
		}

		return vertexArrays[i];
	}

    public bool _BuildShadowMesh(Mesh _mesh, List<SFPolygon> polys, float minLightPenetration){
		var segments = 0;
		for(int i = 0; i < polys.Count; i++){
			var poly = polys[i];
			segments += poly.verts.Length - (poly.looped ? 0 : 1);
		}

		VertexArray arr = GetVertexArray(segments);
		Vector3[] verts = arr.verts;
		Vector4[] tangents = arr.tangents;
		Vector2[] uvs = arr.uvs;
		int[] tris = arr.tris;

		// Skip rendering for empty meshes.
		if(segments == 0 || arr.capacity == 0) return false;

		if (!_rt) _rt = GetComponent<RectTransform> ();
		var toLight = _rt.worldToLocalMatrix;

		var j = 0;
		for(int c = 0; c < polys.Count; c++){
			var poly = polys[c];

			var fromPoly = poly._GetMatrix();
			var t = toLight*fromPoly;

			// Reverse the vertex order if the transform is flipped to keep the correct winding.
			var flipped = (Det2x3(fromPoly) < 0.0f);

			var lightPenetration = Mathf.Max(poly._lightPenetration, minLightPenetration);
			var properties = new Vector3(_radius, lightPenetration, poly._opacity);

			/* foreach path? */{
				var path = poly.verts;

				int startIndex;
				Vector2 a;

				if(poly.looped){
					startIndex = 0;
					a = Transform(t, path[path.Length - 1]);
				} else {
					startIndex = 1;
					a = Transform(t, path[0]);
				}

				for(int i = startIndex; i < path.Length; i++){
					var b = Transform(t, path[i]);

					var ab = (flipped ? new Vector4(b.x, b.y, a.x, a.y) : new Vector4(a.x, a.y, b.x, b.y));
					verts[j*4 + 0] = properties; tangents[j*4 + 0] = ab; uvs[j*4 + 0] = new Vector2(0.0f, 0.0f);
					verts[j*4 + 1] = properties; tangents[j*4 + 1] = ab; uvs[j*4 + 1] = new Vector2(1.0f, 0.0f);
					verts[j*4 + 2] = properties; tangents[j*4 + 2] = ab; uvs[j*4 + 2] = new Vector2(0.0f, 1.0f);
					verts[j*4 + 3] = properties; tangents[j*4 + 3] = ab; uvs[j*4 + 3] = new Vector2(1.0f, 1.0f);

					tris[j*6 + 0] = j*4 + 0; tris[j*6 + 1] = j*4 + 1; tris[j*6 + 2] = j*4 + 2;
					tris[j*6 + 3] = j*4 + 1; tris[j*6 + 4] = j*4 + 3; tris[j*6 + 5] = j*4 + 2;
					
					j++;
					a = b;
				}
			}
		}

		// Clear remaining triangle indices.
		for(int i = 6*segments; i < 6*arr.size; i++){
			arr.tris[i] = 0;
		}

		_mesh.vertices = verts;
		_mesh.tangents = tangents;
		_mesh.uv = uvs;
		_mesh.triangles = tris;

		// Set the amount of the array has been used so it can be zeroed next time it's used.
		arr.size = segments;

		return true;
	}

	private static float Det2x3(Matrix4x4 m){
		return m[0]*m[5] - m[1]*m[4];
	}

	private static Vector2 Transform(Matrix4x4 m, Vector2 p){
		return new Vector2(p.x*m[0] + p.y*m[4] + m[12], p.x*m[1] + p.y*m[5] + m[13]);
	}

}
