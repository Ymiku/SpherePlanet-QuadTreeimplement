//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/DefaultTexture" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		LOD 200
		
		Pass {
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				sampler2D _MainTex;
				float4 _MainTex_ST;
				
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
  					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
  	
					return o;
				}
			
				float4 frag(OUT vert) : COLOR {

  					fixed4 textureColor = tex2D (_MainTex, vert.uv_MainTex);
					if(textureColor.a == 0){
						discard;
					}
  					return textureColor;
				}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
