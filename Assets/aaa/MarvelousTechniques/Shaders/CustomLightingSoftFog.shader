//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/CustomLightingSoftFog" {

	Properties {
		_MainTex ("Texture", 2D) = "white" {}
 		_FrontColor ("Front Color", Color) = (1,0.73,0.117,0)
 		_TopColor ("Top Color", Color) = (0.05,0.275,0.275,0)
 		_RightColor ("Right Color", Color) = (0,0,0,0)
 		_RimColor ("Rim Color", Color) = (0,0,0,0)
 		_RimPower ("Rim Power", Float) = 0.0
 		
 		_LightTint ("Light Tint", Color) = (1,1,1,0)
 		_AmbientColor ("Ambient Color", Color) = (0.5,0.1,0.2,0.0)
 		_AmbientPower ("Ambient Power", Float) = 0.0
 		[Toggle(LIGHTMAP)]_UseLightMap ("Lightmap Enabled", Float) = 0
		_LightmapColor ("Lightmap Tint", Color) = (0,0,0,0)
		_LightmapPower ("Lightmap Power", Float) = 1
		_ShadowPower ("Lightmap Light", Float) = 1
		
		_FogColor ("Fog color", Color) = (1,1,1,1)
		_FogYStartPos ("Fog Y-start pos", Float) = 0.1
		_FogHeight ("Fog Height", Float) = 0.1
		_FogAnimationHeight ("Fog Animation Height", Float) = 0.1
		_FogAnimationFreq ("Fog Animation Frequency", Float) = 0.1
		
		[Toggle(USE_DIST_FOG)]_UseFogDistance ("Distance Fog", Float) = 0
		_FogStart ("Distance Start", Float) = 0
 		_FogEnd ("Distance End", Float) = 50
 		_FogDensity ("Distance Density", Float) = 1
	}
	SubShader {
		Tags { "QUEUE"="Geometry" "RenderType"="Opaque" }
		LOD 200
		
		Pass {
		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Geometry" "RenderType"="Opaque" }
			CGPROGRAM
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma shader_feature LIGHTMAP
				#pragma shader_feature USE_DIST_FOG
				
				#define USE_MAIN_TEX;
				#pragma vertex vert
				#pragma fragment frag
				

				uniform half3 _RimColor;
				uniform half _RimPower;
				uniform half3 _RightColor;
				uniform half3 _TopColor;
				uniform half3 _FrontColor;
				uniform half3 _AmbientColor;
				uniform half _AmbientPower;
				uniform half _UseLightMap;
				uniform half _FogAnimationHeight;
				uniform half _FogAnimationFreq;

				uniform half _LightmapPower;
				uniform half3 _LightTint;
				uniform half3 _LightmapColor;
				uniform half _ShadowPower;
				
				uniform half3 _FogColor;
				uniform half _FogYStartPos;
				uniform half _FogHeight;
				
				uniform half _FogStart;
				uniform half _FogEnd;
				uniform half _FogDensity;
				
				#include "Marvelous.cginc"
								
				CL_OUT_WPOS_SOFT_FOG vert(CL_IN v) {
				#ifndef USE_DIST_FOG
					return customLightingSoftFogVert(v, _RimColor, _RimPower, _RightColor, _FrontColor, _TopColor, _AmbientColor, _AmbientPower,_FogYStartPos, _FogAnimationHeight, _FogAnimationFreq);
				#else
					CL_OUT_WPOS_SOFT_FOG o=customLightingSoftFogVert(v, _RimColor, _RimPower, _RightColor, _FrontColor, _TopColor, _AmbientColor, _AmbientPower,_FogYStartPos, _FogAnimationHeight, _FogAnimationFreq);
					float cameraVertDist = length(_WorldSpaceCameraPos - o.wpos)*_FogDensity; 
					o.fogPower = saturate((_FogEnd - cameraVertDist) / (_FogEnd - _FogStart));	
					return o;		
				#endif
				}
				
				fixed4 frag(CL_OUT_WPOS_SOFT_FOG v) : COLOR {
				#ifndef USE_DIST_FOG
					return customLightingSoftFogFrag(v, _FogColor, _FogHeight, _LightTint, _UseLightMap, _LightmapPower, _LightmapColor, _ShadowPower);
				#else
					fixed4 c = customLightingSoftFogFrag(v, _FogColor, _FogHeight, _LightTint, _UseLightMap, _LightmapPower, _LightmapColor, _ShadowPower);
					return lerp(half4(_FogColor,1),c,v.fogPower);
				#endif
				}
				
			ENDCG
		}
	}
	FallBack "Diffuse"
}
