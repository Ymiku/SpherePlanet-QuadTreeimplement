//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/VignetteFG" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_VignetteMax("Max value", Range(0,1)) = 0.5
		_Power("Intensity", Range(0,1)) = 0.5
		
		
	}
	SubShader {
		Tags { "RenderType"="Transparent" "QUEUE"="Transparent+100" }
		Lighting Off
		Cull Off 
		ZTest Always
		ZWrite Off
		Blend Zero SrcColor
		
		Pass {

		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		
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
		uniform float _VignetteMax;
			
		OUT vert(IN v) {
			OUT o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv =  v.texcoord;
			o.color = v.color;
			return o;
		}
		
		fixed4 frag(OUT vert) : COLOR {
			half2 coords = vert.uv;
			coords = (coords - 0.5) * 2.0;	
			half coordDot = dot (coords,coords); 
			float mask = 1.0 - coordDot  * _VignetteMax * _Power;

			return lerp(_Color,fixed4(1,1,1,1),mask);
		}
		ENDCG
	}
	} 
	FallBack "Diffuse"
}
