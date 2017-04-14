Shader "Kirnu/Marvelous/Flower" {
	Properties {
		_Color1 ("Color 1", Color) = (1,1,1,1)
		_Color2 ("Color 2", Color) = (1,1,1,1)
		_PowerX ("X Power", Float) = 1
		_PowerY ("Y Power", Float) = 1
		_PowerZ ("Z Power", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		LOD 200
		
		Pass {
		Tags { "LIGHTMODE"="ForwardBase"  "RenderType"="Opaque" }
		
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				uniform float _PowerX;
				uniform float _PowerY;
				uniform float _PowerZ;
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
					
  					o.color = lerp(_Color1,_Color2,abs(v.vertex.x)*_PowerX+abs(v.vertex.y)*_PowerY+abs(v.vertex.z)*_PowerZ);

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
