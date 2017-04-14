Shader "Kirnu/Marvelous/OneColor" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
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
			
				uniform half4 _Color;

				struct IN{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4  color  :  COLOR0;
				};
			
				struct OUT {
					float4 pos : SV_POSITION;
				};
			
							
				OUT vert(IN v) {
					
					OUT o;
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);

					return o;
				}
			
				float4 frag(OUT vert) : COLOR {
  					return _Color;
				}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}