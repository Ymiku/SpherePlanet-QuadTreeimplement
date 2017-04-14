//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/WaterBox" {
	Properties {
 		_FrontColor ("Front color", Color) = (1,0.73,0.117,0)
 		_TopColor ("Top Color", Color) = (0.05,0.275,0.275,0)
 		_RightColor ("Right Color", Color) = (0,0,0,0)
 		_RimColor ("Rim colour", Color) = (0,0,0,0)
 		_RimPower ("Rim Power", Float) = 0.0
 		
 		_LightTint ("Light Multiplier", Color) = (1,1,1,0)
 		_AmbientColor ("Ambient Color", Color) = (0.5,0.1,0.2,0.0)
 		_AmbientPower ("Ambient Power", Float) = 0.0
 		
 		_Opacity ("Opacity", Float) = 0.5
 		
 		_Magnitude ("Wave Magnitude", Float) = 1.39
 		_Frequency ("Wave Frequency", Float) = 2.52
 		_Wavelength ("Wave Length", Float) = 3.42
	}
	SubShader {
		Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		Pass {
		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
				#pragma fragmentoption ARB_precision_hint_fastest
				
				#pragma vertex vert
				#pragma fragment frag
				
				
				uniform half3 _RimColor;
				uniform half _RimPower;
				uniform half3 _RightColor;
				uniform half3 _TopColor;
				uniform half3 _FrontColor;
				uniform half3 _AmbientColor;
				uniform half _AmbientPower;
				uniform half _Density;
				
				uniform half _Wavelength;
				uniform half _Frequency;
				uniform half _Magnitude;

				uniform half3 _LightTint;
				uniform half3 _FogColour;
				uniform half _Opacity;
				
				#include "Marvelous.cginc"
										
				CL_OUT_WPOS vert(CL_IN v) {
					
					CL_OUT_WPOS o = customLightingWPosVert(v, _RimColor, _RimPower,_RightColor, _FrontColor, _TopColor, _AmbientColor, _AmbientPower);
  					if(v.vertex.y>=0.9){
  						v.vertex.y+=(sin((((_Frequency * _Time) ) + ((v.vertex.x * _Wavelength) + (v.vertex.y * _Wavelength) + (v.vertex.z * _Wavelength)))) + 1.0) * _Magnitude/50;
  					}
  					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
  					
					return o;
				}

				fixed4 frag(CL_OUT_WPOS v) : COLOR {
					fixed4 ocolor = customLightingFrag(v, _LightTint, half3(0,0,0), 0, half3(0,0,0), 0);
					ocolor.w = _Opacity;
					return ocolor;
				}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
