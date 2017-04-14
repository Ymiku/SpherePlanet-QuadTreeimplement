//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/WaterPool" {
	Properties {
		_MainTex ("Texture (RGB)", 2D) = "white" {}
		_WaveMagnitude ("Wave Magnitude", Float) = 1
 		_WaveFrequency ("Wave Frequency", Float) = 1
 		_WaveLength ("Wave Length", Float) = 10
 		_Speed ("Speed", Float) = 1	
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass {
		Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

		
		uniform float _WaveLength;
		uniform float _WaveFrequency;
		uniform float _WaveMagnitude;
		uniform float _Speed;
		
		sampler2D _MainTex;
		float4 _MainTex_ST;
		
		struct IN{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			fixed4 color    : COLOR;
			float4 texcoord : TEXCOORD0;
		};
	
		struct OUT {
			float4 pos : SV_POSITION;
			float2 texcoord : TEXCOORD0;
		};

		OUT vert(IN v) {
			OUT o;
			v.vertex.y+=((sin((((_WaveFrequency * _Time) * 25.0) + ((v.vertex.x * _WaveLength) + (v.vertex.y * _WaveLength) + (v.vertex.z * _WaveLength)))) + 1.0) * _WaveMagnitude/100.0).x;
  					
			o.pos = mul (UNITY_MATRIX_MVP, (v.vertex));
			float mult=_Time*_Speed ;
			float multy=fmod(mult/8.0,0.125);
			float2 offset=_MainTex_ST.zw;
			offset.y+=multy;
			offset.x+=mult;

			o.texcoord = (v.texcoord.xy * _MainTex_ST.xy) + offset;
			return o;
		}
		
		float4 frag(OUT vert) : COLOR {
			fixed4 mainColor = tex2D (_MainTex, vert.texcoord);
			return mainColor;
		}
		
		ENDCG
		} 
	}
	FallBack "Diffuse"
}
