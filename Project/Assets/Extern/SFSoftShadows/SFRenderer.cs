// Super Fast Soft Lighting. Copyright 2015 Howling Moon Software, LLP

using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Linq;
using System.Collections.Generic;

public interface _SFCullable {
	void _CacheWorldBounds();
	Rect _GetWorldBounds();
}

[ExecuteInEditMode]
public class SFRenderer : MonoBehaviour {
	public bool _renderInSceneView = true;

	private void ScenePreRender(Camera camera){
		if(_renderInSceneView && camera.cameraType == CameraType.SceneView){
			OnPreRender();
		}
	}

	private void ScenePostRender(Camera camera){
		if(_renderInSceneView && camera.cameraType == CameraType.SceneView){
			OnPostRender();
		}
	}

	private Mesh _mesh;
	private void OnDestroy(){
		if(_mesh) DestroyImmediate(_mesh);
	}

	private void OnEnable(){
		if(Application.isEditor){
			Camera.onPreRender += ScenePreRender;
			Camera.onPostRender += ScenePostRender;
		}
	}

	private void OnDisable(){
		if(Application.isEditor){
			Camera.onPreRender -= ScenePreRender;
			Camera.onPostRender -= ScenePostRender;
		}
	}

	private RenderTexture _lightMap;
	private RenderTexture _ShadowMap;

	[Tooltip("Blend the lights in linear space rather than gamma space. Nonlinear blending prevents oversaturation, but can cause draw order artifacts.")]
	public bool _linearLightBlending = true;
	public bool _shadows = true;
	[Tooltip("The global ambient light color- the ambient light is used to light your scene when no lights are affecting part of it. A darker grey, blue, or yellow is often a good place to start. Alpha unused. ")]
	public Color _ambientLight = Color.black;

	[Tooltip("Exposure is a multiplier applied to all lights in this renderer. Use to adjust all your lights at once. Particularly useful if you're using HDR lighting, otherwise it can be used to cause oversaturation.")]
	[FormerlySerializedAs("_globalDynamicRange")]
	public float _exposure = 1.0f;

	[Tooltip("Scale of the render texture for the colored lights. Larger numbers will give you blockier lights, but will run faster. Since lighting tends to " +
		"be pretty diffuse, high numbers like 8 usually look good here. Recommended values are between 8 - 32.")]
	public float _lightMapScale = 8;
	[Tooltip("Scale of the render texture for the colored lights. Larger numbers will give you blockier shadows," +
		" but will run faster. Blocky shadows tend to look worse than blocky lights, so this should usually be lower than the light map scale. Recommended values are between 2 - 8. Less if you have a lot of sharp shadows.")]
	public float _shadowMapScale = 4;

	[Tooltip("How far will light penetrate into each shadow casting object. Makes it look like objects that are casting shadows are illuminated by the lights.")]
	public float _minLightPenetration = 0.2f;

	[Tooltip("The color of the fog color. The alpha controls the fog's strength.")]
	public Color _fogColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
	[Tooltip("The scatter color is the color that the fog will glow when it is lit. Alpha is unused. Black disables illumination effects on the fog.")]
	public Color _scatterColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
	[Tooltip("What percentage of unshadowed/shadowed light should apply to the fog. At 1.0, your shadows will be fully applied to the scattered light in your fog.")]
	public float _softHardMix = 0.0f;

	public bool linearLightBlending {get {return _linearLightBlending;} set {_linearLightBlending = value;}}
	public bool shadows {get {return _shadows;} set {_shadows = value;}}
	public Color ambientLight {get {return _ambientLight;} set {_ambientLight = value;}}
	public float exposure {get {return _exposure;} set {_exposure = value;}}
	public float lightMapScale {get {return _lightMapScale;} set {_lightMapScale = value;}}
	public float shadowMapScale {get {return _shadowMapScale;} set {_shadowMapScale = value;}}
	public float minLightPenetration {get {return _minLightPenetration;} set {_minLightPenetration = value;}}
	public Color fogColor {get {return _fogColor;} set {_fogColor = value;}}
	public Color scatterColor {get {return _scatterColor;} set {_scatterColor = value;}}
	public float softHardMix {get {return _softHardMix;} set {_softHardMix = value;}}

	[Obsolete("Please use SFRenderer.exposure instead.")]
	public float globalIlluminationScale {get {return _exposure;} set {_exposure = value;}}
	[Obsolete("Please use SFRenderer.exposure instead.")]
	public float globalDynamicRange {get {return _exposure;} set {_exposure = value;}}

	private Material _shadowMaskMaterial;
	private Material shadowMaskMaterial {
		get {
			if(_shadowMaskMaterial == null){
				_shadowMaskMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/ShadowMask"));
				_shadowMaskMaterial.hideFlags = HideFlags.HideAndDontSave;
			}

			return _shadowMaskMaterial;
		}
	}
	
	private Material _linearLightMaterial;
	private Material _softLightMaterial;
	private Material lightMaterial {
		get {
			if(_linearLightMaterial == null){
				_linearLightMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/LightBlendLinear"));
				_linearLightMaterial.hideFlags = HideFlags.HideAndDontSave;

				_softLightMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/LightBlendSoft"));
				_softLightMaterial.hideFlags = HideFlags.HideAndDontSave;
			}

			return (_linearLightBlending ? _linearLightMaterial : _softLightMaterial);
		}
	}
	
	private Material _HDRClampMaterial;
	private Material HDRClampMaterial {
		get {
			if(_HDRClampMaterial == null){
				_HDRClampMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/HDRClamp"));
				_HDRClampMaterial.hideFlags = HideFlags.HideAndDontSave;
			}

			return _HDRClampMaterial;
		}
	}

	private Material _fogMaterial;
	private Material fogMaterial {
		get {
			if(_fogMaterial == null){
				_fogMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/FogLayer"));
				_fogMaterial.hideFlags = HideFlags.HideAndDontSave;
			}

			return _fogMaterial;
		}
	}

	private bool UV_STARTS_AT_TOP;
	private static Matrix4x4 TEXTURE_FLIP_MATRIX = Matrix4x4.Scale(new Vector3(1.0f, -1.0f, 1.0f));

	private void Start(){
		var type = SystemInfo.graphicsDeviceType;

		// Consoles or new platforms may need to be added here too.
		UV_STARTS_AT_TOP = (
			type == UnityEngine.Rendering.GraphicsDeviceType.Direct3D9 ||
			type == UnityEngine.Rendering.GraphicsDeviceType.Direct3D11 ||
			type == UnityEngine.Rendering.GraphicsDeviceType.Direct3D12 ||
			type == UnityEngine.Rendering.GraphicsDeviceType.Metal
		);
	}

	private static RenderTexture GetTexture(float downscale){
		downscale = Mathf.Max(1.0f, downscale);
		
		var camera = Camera.current;
		var width = (int)(camera.pixelWidth/downscale);
		var height = (int)(camera.pixelHeight/downscale);

		RenderTextureFormat format = (camera.hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);
		return RenderTexture.GetTemporary(width, height, 0, format);
	}
	
	public static Rect _TransformRect(Matrix4x4 m, Rect r){
		Vector4 c = m.MultiplyPoint3x4(new Vector4(r.x + 0.5f*r.width, r.y + 0.5f*r.height, 0.0f, 1.0f));
		float hw = 0.5f*Mathf.Max(Mathf.Abs(r.width*m[0] + r.height*m[4]), Mathf.Abs(r.width*m[0] - r.height*m[4]));
		float hh = 0.5f*Mathf.Max(Mathf.Abs(r.width*m[1] + r.height*m[5]), Mathf.Abs(r.width*m[1] - r.height*m[5]));
		return new Rect(c.x - hw, c.y - hh, 2.0f*hw, 2.0f*hh);
	}
	
	private static Rect Intersection(Rect r1, Rect r2){
		return Rect.MinMaxRect(
			Mathf.Max(r1.xMin, r2.xMin),
			Mathf.Max(r1.yMin, r2.yMin),
			Mathf.Min(r1.xMax, r2.xMax),
			Mathf.Min(r1.yMax, r2.yMax)
		);
	}
	
	private static Rect Union(Rect r1, Rect r2){
		return Rect.MinMaxRect(
			Mathf.Min(r1.xMin, r2.xMin),
			Mathf.Min(r1.yMin, r2.yMin),
			Mathf.Max(r1.xMax, r2.xMax),
			Mathf.Max(r1.yMax, r2.yMax)
		);
	}
	
	private static Rect ConvertToPixelRect(Rect rect, float w, float h){
		var l = Mathf.Max(0.0f, Mathf.Floor((0.5f*rect.xMin + 0.5f)*w));
		var b = Mathf.Max(0.0f, Mathf.Floor((0.5f*rect.yMin + 0.5f)*h));
		var r = Mathf.Min(w, Mathf.Ceil((0.5f*rect.xMax + 0.5f)*w));
		var t = Mathf.Min(h, Mathf.Ceil((0.5f*rect.yMax + 0.5f)*h));
		return new Rect(l, b, r - l, t - b);
	}
	
	// Calculate a clip matrix given the viewport pixel rect and 2/width, 2/height
	private static Matrix4x4 ClipMatrix(Rect r, float dw, float dh){
		float x = r.x*dw - 1.0f;
		float y = r.y*dh - 1.0f;
		return Matrix4x4.Ortho(x, x + r.width*dw, y, y + r.height*dh, -1.0f, 1.0f);
	}
	
	private List<SFLight> _culledLights = new List<SFLight>();
	private List<SFPolygon> _culledPolygons = new List<SFPolygon>();
	private List<SFPolygon> _perLightCulledPolygons = new List<SFPolygon>();


	private List<T> CullObjects<T>(List<T> objs, List<T> cache, Rect cullBounds, bool cacheBounds = true) where T : MonoBehaviour, _SFCullable {
		for(var i = 0; i < objs.Count; i++){
			var obj = objs[i];

			if(cacheBounds) obj._CacheWorldBounds();
			if(cullBounds.Overlaps(obj._GetWorldBounds())) cache.Add(obj);
		}
		
		return cache;
	}

	private Rect PolyCullingBounds(List<SFLight> lights, Rect viewBounds){
		var l = viewBounds.xMin;
		var b = viewBounds.yMin;
		var r = viewBounds.xMax;
		var t = viewBounds.yMax;

		for(int i = 0; i < lights.Count; i++){
			var p = lights[i]._position;
			l = Mathf.Min(l, p.x);
			b = Mathf.Min(b, p.y);
			r = Mathf.Max(r, p.x);
			t = Mathf.Max(t, p.y);
        }
        
        return Rect.MinMaxRect(l, b, r, t);
    }

	private void RenderLightMap(RenderTexture target, bool shadows, List<SFLight> lights, List<SFPolygon> polys, Rect viewBounds, Color ambient){
		var cam = Camera.current;
		var projection = cam.projectionMatrix;
		
		var LIGHT_RECT = new Rect(-1, -1, 2, 2);
		var SRC_RECT = new Rect(0, 0, 1, 1);
		
		Graphics.SetRenderTarget(target);
		GL.Clear(false, true, ambient);
		
		if(_mesh == null){
			_mesh = new Mesh();
			_mesh.MarkDynamic();
			_mesh.hideFlags = HideFlags.HideAndDontSave;
		}

		for(int i = 0; i < lights.Count; i++){
			var light = lights[i];
			if(!light.enabled) continue;
			
			var transf = light.transform.localToWorldMatrix;
			var boundsTransform = transf*light._lightMatrix;
			var modelView = cam.worldToCameraMatrix*boundsTransform;
			
			float w = target.width;
			float h = target.height;
			var clipBounds = _TransformRect(projection*modelView, LIGHT_RECT);
			var viewport = ConvertToPixelRect(clipBounds, w, h);
			
			GL.Viewport(viewport);
			var clippedProjection = ClipMatrix(viewport, 2.0f/w, 2.0f/h)*projection;
			GL.LoadProjectionMatrix(clippedProjection);
			
			// Draw shadow mask
			if(shadows && light._shadowLayers != 0){
				var radius = light.radius;
				var areaBounds = _TransformRect(transf, new Rect(-radius, -radius, 2.0f*radius, 2.0f*radius));
				var rangeBounds = _TransformRect(boundsTransform, LIGHT_RECT);

				var lightBounds = Union(areaBounds, Intersection(rangeBounds, viewBounds));
				var lightPolys = CullObjects(polys, _perLightCulledPolygons, lightBounds, true);

				if(light._BuildShadowMesh(_mesh, lightPolys, _minLightPenetration)){
					// Note: DrawMesh apparently not affected by the "GL" transform.
					this.shadowMaskMaterial.SetPass(0);
					Graphics.DrawMeshNow(_mesh, transf);
					_mesh.Clear();
				}

				lightPolys.Clear();
			}

			var textureMatrix = (clippedProjection*modelView).inverse;

			if(UV_STARTS_AT_TOP){
				// Viewport and texture coordinates are flipped on DirectX.
				// Need to flip the projection going in, then flip the texture coordinates coming out.
				textureMatrix = TEXTURE_FLIP_MATRIX*textureMatrix*TEXTURE_FLIP_MATRIX;
			}
			
			var cookie = light._cookieTexture;
			var material = this.lightMaterial;
			if(_linearLightBlending) material.SetFloat("_intensity", light._intensity);

			// Clamp the shadow mask to 0 when rendering to an HDR lightmap.
			if(cam.hdr) Graphics.DrawTexture(LIGHT_RECT, Texture2D.blackTexture, this.HDRClampMaterial);

			// Composite the light.
			// Draw a fullscreen quad with the light's texture on it.
			// The vertex shader doesn't use any transforms at all.
			// Abuse the projection matrix since there isn't really a better way to pass the texture's transform.
			GL.LoadProjectionMatrix(textureMatrix);
			Graphics.DrawTexture(LIGHT_RECT, cookie ? cookie : Texture2D.whiteTexture, SRC_RECT, 0, 0, 0, 0, light._color, material);
		}

		if(shadows) _culledPolygons.Clear();
	}
		
	private void OnPreRender(){
		RenderBuffer savedColorBuffer = Graphics.activeColorBuffer;
		RenderBuffer savedDepthBuffer = Graphics.activeDepthBuffer;
		var currentCam = Camera.current;
		var vp = currentCam.projectionMatrix*currentCam.worldToCameraMatrix;
		var viewBounds = _TransformRect(vp.inverse, new Rect(-1.0f, -1.0f, 2.0f, 2.0f));

		var lights = SFLight._lights;
		var polys = SFPolygon._polygons;

		// Extra slow fallback for editor mode to find and init the objects.
		if(!Application.isPlaying){
			lights = new List<SFLight>(Component.FindObjectsOfType<SFLight>().Where((o) => o.isActiveAndEnabled));
			polys = new List<SFPolygon>(Component.FindObjectsOfType<SFPolygon>().Where((o) => o.isActiveAndEnabled));

			// Force polys to calculate their initial bounds.
			foreach(var p in polys) p._UpdateBounds();
		}

		Color ambient = _ambientLight; ambient.a = 1f;
		var culledLights = CullObjects(lights, _culledLights, viewBounds);
		
		GL.PushMatrix();
		_lightMap = GetTexture(_lightMapScale);
		RenderLightMap(_lightMap, false, culledLights, null, viewBounds, ambient);
		
		if(_shadows){
			_ShadowMap = GetTexture(_shadowMapScale);
			var culledPolys = CullObjects(polys, _culledPolygons, PolyCullingBounds(culledLights, viewBounds));
			RenderLightMap(_ShadowMap, true, culledLights, culledPolys, viewBounds, ambient);
		}
		GL.PopMatrix();
		
		// Lights is a cached list. Clear it now since we are done with the contents.
		culledLights.Clear();
		
		Graphics.SetRenderTarget(null);

		GL.Viewport(currentCam.pixelRect);
		
		Shader.SetGlobalMatrix("_SFProjection", Camera.current.projectionMatrix);
		Shader.SetGlobalColor("_SFAmbientLight", ambient);
		Shader.SetGlobalFloat("_SFExposure", _exposure);
		Shader.SetGlobalTexture("_SFLightMap", _lightMap);
		Shader.SetGlobalTexture("_SFLightMapWithShadows", _shadows ? _ShadowMap : _lightMap);

		Graphics.SetRenderTarget (savedColorBuffer, savedDepthBuffer);
	}
	
	private void OnPostRender(){

//		Debug.Log ("OnPostRender Camera "+Camera.current.cameraType+ " is in scene view in Unity: " + Application.unityVersion);

		var fogCheck = _fogColor.a + _scatterColor.r + _scatterColor.g + _scatterColor.b;
		if(fogCheck > 0.0f){
			GL.PushMatrix(); {
				// Unity may or may not apply DirectX magic fixes to the identity matrix.
				// Need to use that to flip (or not flip...) the rendering in some cases.
				GL.LoadProjectionMatrix(Matrix4x4.identity);

				var scatter = _scatterColor;
				scatter.a = _softHardMix;
				
				var mat = this.fogMaterial;
				mat.SetColor("_FogColor", _fogColor);
				mat.SetColor("_Scatter", scatter);
				mat.SetPass(0);
				
				GL.Begin(GL.QUADS); {
					GL.MultiTexCoord2(0, 0.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 0.0f);
					GL.MultiTexCoord2(0, 0.0f, 1.0f); GL.Vertex3(-1.0f,  1.0f, 0.0f);
					GL.MultiTexCoord2(0, 1.0f, 1.0f); GL.Vertex3( 1.0f,  1.0f, 0.0f);
					GL.MultiTexCoord2(0, 1.0f, 0.0f); GL.Vertex3( 1.0f, -1.0f, 0.0f);
				} GL.End();
			} GL.PopMatrix();
		}

		Shader.SetGlobalColor("_SFAmbientLight", Color.white);
		Shader.SetGlobalFloat("_SFExposure", 1.0f);
		Shader.SetGlobalTexture("_SFLightMap", Texture2D.whiteTexture);
		Shader.SetGlobalTexture("_SFLightMapWithShadows", Texture2D.whiteTexture);
		RenderTexture.ReleaseTemporary(_lightMap); _lightMap = null;
		RenderTexture.ReleaseTemporary(_ShadowMap); _ShadowMap = null;
	}
}
