//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/Waterfall" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_USpeed("U Speed", Float) = 1
		_VSpeed("V Speed", Float) = 1
		
		_WaveMagnitude ("Wave Magnitude", Float) = 1
 		_WaveFrequency ("Wave Frequency", Float) = 1
 		_WaveLength ("Wave Length", Float) = 10

 		_Color ("Color", Color) = (1,0.73,0.117,0)
 		_HighlightLength ("Highlight Length", Float) = 0
 		_HighlightFade ("Highlight Fade", Float) = 0.5
 		_HighlightColor ("Highlight color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "QUEUE"="Geometry" "RenderType"="Opaque" }
		LOD 200
		Pass {
		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Geometry" "RenderType"="Opaque" }
		CGPROGRAM
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		float4 _MainTex_ST;
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vert
		#pragma fragment frag

		
		struct IN{
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
			float3 normal : NORMAL;
		};
		
		struct OUT {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			half4 vpos: TEXCOORD1;
		};
						
		uniform float _USpeed;
		uniform float _VSpeed;
		
		uniform float _WaveLength;
		uniform float _WaveFrequency;
		uniform float _WaveMagnitude;
		
		uniform half4 _Color;
		uniform half _HighlightLength;
		uniform half _HighlightFade;
		uniform half4 _HighlightColor;
		
		OUT vert(IN v) {
			OUT o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			o.vpos = 0.5*(v.vertex + 1);

			o.pos.x+=(sin((((_WaveFrequency * _Time) * 20.0) + (o.pos.y  * _WaveLength)) ) * _WaveMagnitude/100.0);

			float2 offset=_MainTex_ST.zw;
			offset.x*=_USpeed*_Time;
			offset.y*=_VSpeed* _Time;
  			o.uv += offset;

			return o;
		}
		
		fixed4 frag(OUT v) : COLOR {
			fixed4 c = _Color * tex2D (_MainTex, v.uv);
			if(v.vpos.y <= _HighlightLength){
				c = lerp(c+_HighlightColor*_HighlightFade,c,(v.vpos.y/_HighlightLength));
			}
			else if(v.vpos.y >= 1-_HighlightLength){
				c = lerp(c+_HighlightColor*_HighlightFade,c,((1-v.vpos.y)/_HighlightLength));
			}
			return c;
		}
		ENDCG
	} 
	}
	FallBack "Diffuse"
}
