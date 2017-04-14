//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/FloatingLeaf" {
	Properties {
		_Color1 ("Color 1", Color) = (1,1,1,1)
		_Color2 ("Color 2", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" "QUEUE"="Geometry+300"}
		
		LOD 200
		
		Pass {
		Tags { "LIGHTMODE"="ForwardBase"  "RenderType"="Opaque" }
		
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
			
				uniform half4 _Color1;
				uniform half4 _Color2;

				struct IN{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4  color  :  COLOR0;
				};
			
				struct OUT {
					float4 pos : SV_POSITION;
					float3 color : TEXCOORD0;
				};
						
				OUT vert(IN v) {
					OUT o;
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
  					o.color = lerp(_Color1,_Color2,v.color.x > 0.7);

					return o;
				}
			
				float4 frag(OUT vert) : COLOR {
  					return float4(vert.color,0);
				}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
