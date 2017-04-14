//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/FlagWave" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		
		_Magnitude ("Wave Magnitude", Float) = 1
 		_Frequency ("Wave Frequency", Float) = 1
 		_WaveLength ("Wave Length", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Geometry" "RenderType"="Opaque" }
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				sampler2D _MainTex;
				float4 _MainTex_ST;
				
				uniform half4 _Color;
				uniform float _Magnitude;
				uniform float _UseLightMap;
				uniform float _Frequency;
				uniform float _WaveLength;

				struct IN{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 texcoord : TEXCOORD0;
				};
			
				struct OUT {
					float4 pos : SV_POSITION;
					float2 uv_MainTex : TEXCOORD0;
				};
			
							
				OUT vert(IN v) {
					
					OUT o;

  					o.uv_MainTex = TRANSFORM_TEX (v.texcoord, _MainTex);
  					
  					v.vertex.z += (sin((v.vertex.x / _WaveLength) + (_Frequency * _Time)) * _Magnitude) * o.uv_MainTex.y;
  					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);

					return o;
				}
			
				float4 frag(OUT vert) : COLOR {
  					half4 textureColor = tex2D (_MainTex, vert.uv_MainTex)*_Color;

  					return textureColor;
				}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
