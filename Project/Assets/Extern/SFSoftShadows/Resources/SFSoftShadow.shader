// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/SFSoftShadow" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SoftHardMix ("Unshadowed/Shadowed Mix", Range(0.0, 1.0)) = 0.0
		_AmbientOnlyMix ("Lit/Ambient Mix", Range(0.0, 1.0)) = 0.0
		_Glow ("Self Illumination", Color) = (0.0, 0.0, 0.0, 0.0)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}
	
	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		
		Pass {
			Blend One OneMinusSrcAlpha
			Cull Off
			Lighting Off
			ZWrite Off
			
			Fog {
				Mode Off
			}
			
			CGPROGRAM

				#define DISABLE_PROJ_TEXTURE_READS 0

				#include "UnityCG.cginc"
				#pragma vertex VShader
				#pragma fragment FShader
				#pragma multi_compile _ PIXELSNAP_ON 
				#pragma multi_compile _ FIXEDSAMPLEPOINT_ON LINESAMPLE_ON

				float4x4 _SFProjection;
				
				sampler2D _MainTex;
				float4 _MainTex_ST;
				
				float4 _SFAmbientLight;
				sampler2D _SFLightMap;
				sampler2D _SFLightMapWithShadows;
				float _SFExposure;
				float2 _SamplePosition;

				float _SoftHardMix;
				float _AmbientOnlyMix;
				float4 _Glow;
				
				struct VertexInput {
					float3 position : POSITION;
					float2 texCoord : TEXCOORD0;
					float4 color : COLOR;
				};
				
				struct VertexOutput {
					float4 position : SV_POSITION;
					float2 texCoord : TEXCOORD0;
					#if DISABLE_PROJ_TEXTURE_READS
						float2 uv : TEXCOORD1;
					#else
						float4 lightCoord : TEXCOORD1;
					#endif
					float4 color : COLOR;
				};
				
				VertexOutput VShader(VertexInput v){
					float4 position = UnityObjectToClipPos(float4(v.position, 1.0));

					float3 samplePosition = v.position;
#if defined(LINESAMPLE_ON)
					samplePosition = float3(v.position.x, _SamplePosition.y, v.position.z);
#endif

#if defined(FIXEDSAMPLEPOINT_ON)
					samplePosition = float3(_SamplePosition.xy, v.position.z);
#endif

					// Unity does some magic crap to the projection matrix for DirectX
					// Can't trust it because you can't detect when it applies workarounds to it.
					float4 shadowCoord = mul(mul(_SFProjection, UNITY_MATRIX_MV), float4(samplePosition, 1.0));

#if defined(PIXELSNAP_ON)
					position = UnityPixelSnap(position);
					shadowCoord = UnityPixelSnap(shadowCoord);
#endif
					
					VertexOutput o = {
						position,
						TRANSFORM_TEX(v.texCoord, _MainTex),
					#if DISABLE_PROJ_TEXTURE_READS
						// uv:
						(0.5*(shadowCoord + shadowCoord.w)).xy,
					#else
						// lightCoords adjusted for perspective effects					
						0.5*(shadowCoord + shadowCoord.w),
					#endif
						v.color,
					};

					return o;
				}

				fixed4 FShader(VertexOutput v) : SV_Target {
					fixed4 color = v.color*tex2D(_MainTex, v.texCoord);

					#if DISABLE_PROJ_TEXTURE_READS
						// Cheaper version:
						fixed3 l0 = tex2D(_SFLightMap, v.uv).rgb;
						fixed3 l1 = tex2D(_SFLightMapWithShadows, v.uv).rgb;
					#else
						// lightCoords adjusted for perspective effects					
						fixed3 l0 = tex2Dproj(_SFLightMap, UNITY_PROJ_COORD(v.lightCoord)).rgb;
						fixed3 l1 = tex2Dproj(_SFLightMapWithShadows, UNITY_PROJ_COORD(v.lightCoord)).rgb;
					#endif


					fixed3 light = lerp(l0, l1, _SoftHardMix);

					// "light" already has ambient applied, _SFAmbientLight is only the ambient color.
					color.rgb *= (lerp(light, _SFAmbientLight, _AmbientOnlyMix) + _Glow) *_SFExposure*color.a;
	
					return color;
				}
			ENDCG
		}
	}
}
