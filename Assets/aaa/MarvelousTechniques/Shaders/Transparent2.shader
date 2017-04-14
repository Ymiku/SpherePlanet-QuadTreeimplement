//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/Transparent2" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "QUEUE"="Transparent+2" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent"}
		
		
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		
		struct IN{
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
			float4  color  :  COLOR0;
		};
		
		struct OUT {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 color : TEXCOORD1;
		};
						
		sampler2D _MainTex;
		float4 _MainTex_ST;
		
		uniform half4 _Color;
		
		OUT vert(IN v) {
			OUT o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv =  TRANSFORM_TEX (v.texcoord, _MainTex);
			o.color = v.color;
			return o;
		}
		
		fixed4 frag(OUT vert) : COLOR {
			half4 ocolor = half4(1,1,1,1);
			half4 textureColor = tex2D (_MainTex, vert.uv);
			ocolor.xyz = textureColor*_Color.xyz;
			ocolor.a= textureColor.a*_Color.a*vert.color.r;
			return ocolor;
		}
		ENDCG
	}
	} 
	FallBack "Diffuse"
}
