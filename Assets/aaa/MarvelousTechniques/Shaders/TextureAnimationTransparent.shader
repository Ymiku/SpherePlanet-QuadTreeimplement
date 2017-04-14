//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/TextureAnimationTransparent" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_USpeed("U Speed", Float) = 1
		_VSpeed("V Speed", Float) = 1
		
		_WaveMagnitude ("Wave Magnitude", Float) = 1
 		_WaveFrequency ("Wave Frequency", Float) = 1
 		_WaveLength ("Wave Length", Float) = 10

 		_FrontColor ("Front color", Color) = (1,0.73,0.117,0)
 		_TopColor ("Top Color", Color) = (0.05,0.275,0.275,0)
 		_RightColor ("Right Color", Color) = (0,0,0,0)
 		_RimColor ("Rim colour", Color) = (0,0,0,0)
 		_RimPower ("Rim Power", Float) = 0.0
 		
 		_LightTint ("Light Multiplier", Color) = (1,1,1,0)
 		_AmbientColor ("Ambient Color", Color) = (0.5,0.1,0.2,0.0)
 		_AmbientPower ("Ambient Power", Float) = 0.0
	}
	SubShader {
		Tags { "QUEUE"="Geometry" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
		
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
		CGPROGRAM
		#pragma fragmentoption ARB_precision_hint_fastest
		#define USE_MAIN_TEX;
		#pragma vertex vert
		#pragma fragment frag
		#include "Marvelous.cginc"
		
		struct IN{
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
			float3 normal : NORMAL;
		};
		
		struct OUT {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 uv_MainTex2 : TEXCOORD2;
			float wposY: TEXCOORD3;
		};
						
		uniform float _USpeed;
		uniform float _VSpeed;
		
		uniform float _WaveLength;
		uniform float _WaveFrequency;
		uniform float _WaveMagnitude;
		
		uniform half4 _RimColor;
		uniform half _RimPower;
		uniform half4 _RightColor;
		uniform half4 _TopColor;
		uniform half4 _FrontColor;
		uniform half4 _AmbientColor;
		uniform float _AmbientPower;
		uniform float4 _LightTint;

		CL_OUT_WPOS vert(CL_IN v) {
			CL_OUT_WPOS o = customLightingWPosVert(v, _RimColor, _RimPower, _RightColor, _FrontColor, _TopColor, _AmbientColor, _AmbientPower);
			o.pos.x+=(sin((((_WaveFrequency * _Time) * 20.0) + (o.pos.y  * _WaveLength)) ) * _WaveMagnitude/100.0);

			float2 offset=_MainTex_ST.zw;
			offset.x*=_USpeed*_Time;
			offset.y*=_VSpeed* _Time;
  			o.main_uv += offset;
  			
			return o;
		}
		
		fixed4 frag(CL_OUT_WPOS v) : COLOR {
			return customLightingFrag(v, _LightTint, half3(0,0,0), 0, half3(0,0,0), 0);
		}
		ENDCG
	} 
	}
	FallBack "Diffuse"
}
