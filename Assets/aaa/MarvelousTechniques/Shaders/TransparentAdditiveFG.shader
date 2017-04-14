//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/TransparentAdditiveFG" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Power ("Power", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "QUEUE"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
		Cull Off 
		ZTest Always
		ZWrite Off
		Blend One One
		
		Pass {
		Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Transparent"}

		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		
		struct IN{
			half4 vertex : POSITION;
			half4 texcoord : TEXCOORD0;
			fixed4  color  :  COLOR0;
		};
		
		struct OUT {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			fixed4 color : TEXCOORD1;
		};
						
		sampler2D _MainTex;
		float4 _MainTex_ST;
		
		uniform fixed4 _Color;
		uniform fixed _Power;
		
		OUT vert(IN v) {
			OUT o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv =  TRANSFORM_TEX (v.texcoord, _MainTex);
			o.color = v.color;
			return o;
		}
		
		fixed4 frag(OUT vert) : COLOR {
			fixed4 ocolor = half4(1,1,1,1);
			fixed4 textureColor = tex2D (_MainTex, vert.uv);
			ocolor = textureColor*_Color;

			return ocolor*_Power;
		}
		ENDCG
	}
	} 
	FallBack "Diffuse"
}
