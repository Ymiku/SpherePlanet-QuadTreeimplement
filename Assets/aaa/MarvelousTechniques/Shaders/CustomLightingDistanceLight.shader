//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/CustomLightingDistanceLight" {

	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_LayoutTexture ("Layout Texture", 2D) = "white" {}
		_LightRampTexture ("Light Ramp Texture", 2D) = "white" {}
		_LayoutTexturePower ("Layout Texture Power", Float) = 0
		_LightMaxDistance ("Light Max Distance", Float) = 10
		_LightPos ("Light position", Vector) = (0,0,0,1)
		[Toggle(DIST_LIGHT_ADDITIVE)]_additiveBlend ("Additive blend", Float) = 0
 		_FrontColor ("Front Color", Color) = (1,0.73,0.117,0)
 		_TopColor ("Top Color", Color) = (0.05,0.275,0.275,0)
 		_RightColor ("Right Color", Color) = (0,0,0,0)
 		_RimColor ("Rim Color", Color) = (0,0,0,0)
 		_RimPower ("Rim Power", Float) = 0.0
 		
 		_LightTint ("Light Tint", Color) = (1,1,1,0)
 		_AmbientColor ("Ambient Color", Color) = (0.5,0.1,0.2,0.0)
 		_AmbientPower ("Ambient Power", Float) = 0.0
		_LightmapColor ("Lightmap Tint", Color) = (0,0,0,0)
		_LightmapPower ("Lightmap Power", Float) = 1
		_ShadowPower ("Shadow Light", Float) = 0
		[Toggle(LIGHTMAP)] _UseLightMap ("Lightmap Enabled", Float) = 0
	}
	SubShader {
		Tags { "QUEUE"="Geometry" "RenderType"="Opaque" }
		LOD 200
		
		Pass {

		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Geometry" "RenderType"="Opaque" }
			CGPROGRAM
				#pragma shader_feature LIGHTMAP
				#pragma shader_feature DIST_LIGHT_ADDITIVE
				#pragma fragmentoption ARB_precision_hint_fastest
				
				#define USE_DIST_LIGHT;
				#define USE_LAYOUT_TEXTURE;
				#define USE_MAIN_TEX;
				
				#pragma vertex vert
				#pragma fragment frag

				uniform half _UseFog;
				uniform half3 _RimColor;
				uniform half _RimPower;
				uniform half3 _RightColor;
				uniform half3 _TopColor;
				uniform half3 _FrontColor;
				uniform half3 _AmbientColor;
				uniform half _AmbientPower;
				uniform half _UseLightMap;

				uniform half _LightmapPower;
				uniform half3 _LightTint;
				uniform half3 _LightmapColor;
				uniform half _ShadowPower;
				
				uniform half _LightMaxDistance;
				uniform half3 _LightPos;
				uniform float _LayoutTexturePower;
				
				#include "Marvelous.cginc"
				
				CL_OUT_WPOS vert(CL_IN v) {
					return customLightingWPosVert(v, _RimColor, _RimPower, _RightColor, _FrontColor, _TopColor, _AmbientColor, _AmbientPower);
				}
				
				fixed4 frag(CL_OUT_WPOS v) : COLOR {
					return customLightingFragDistLight(v, _LightTint, _UseLightMap, _LightmapPower, _LightmapColor, _ShadowPower,_LightPos);
				}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
