//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/CustomLightingHardFog" {

	Properties {
		_MainTex ("Texture", 2D) = "white" {}
 		_FrontColor ("Front color", Color) = (1,0.73,0.117,0)
 		_TopColor ("Top Color", Color) = (0.05,0.275,0.275,0)
 		_RightColor ("Right Color", Color) = (0,0,0,0)
 		_RimColor ("Rim Color", Color) = (0,0,0,0)
 		_RimPower ("Rim Power", Color) = (0,0,0,0)
 		
 		_LightTint ("Light Tint", Color) = (1,1,1,0)
 		_AmbientColor ("Ambient Color", Color) = (0.5,0.1,0.2,0.0)
 		_AmbientPower ("Ambient Power", Float) = 0.0
		_LightmapColor ("Lightmap Tint", Color) = (0,0,0,0)
		_LightmapPower ("Lightmap Power", Float) = 1
		[Toggle(LIGHTMAP)]_UseLightMap ("Lightmap Enabled", Float) = 00
		_ShadowPower ("Shadow Light", Float) = 0
		
		_FogColor ("Fog color", Color) = (1,1,1,1)
		_FogYStartPos ("Fog Y-start pos", Float) = 0.1
	}
	SubShader {
		Tags { "QUEUE"="Geometry" "RenderType"="Opaque" }
		LOD 200
		
		Pass {
		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Geometry" "RenderType"="Opaque" }
			CGPROGRAM
				#pragma fragmentoption ARB_precision_hint_fastest
				#define USE_MAIN_TEX;
				#pragma shader_feature LIGHTMAP
				#pragma vertex vert
				#pragma fragment frag
				
				
				uniform half _RimPower;
				uniform half3 _RimColor;
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
				
				uniform half3 _FogColor;
				uniform half _FogYStartPos;
				
				#include "Marvelous.cginc"
				
				CL_OUT_WPOS vert(CL_IN v) {
					return customLightingWPosVert(v, _RimColor, _RimPower,_RightColor, _FrontColor, _TopColor, _AmbientColor, _AmbientPower);
				}
				
				fixed4 frag(CL_OUT_WPOS v) : COLOR {
					return customLightingHardFogFrag(v, _FogYStartPos, _FogColor, _LightTint, _UseLightMap, _LightmapPower, _LightmapColor, _ShadowPower);
				}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
