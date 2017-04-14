//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/OceanOpaque" {
	Properties {
 		_ColorMap2 ("Color map", 2D) = "white" {}
 		_LightDir ("Light Direction", Vector) = (0,-1,0)
	}
	SubShader {
		Tags {  "RenderType"="Opaque"  }
		LOD 200
		
		Pass {
		Tags { "LIGHTMODE"="ForwardBase" "Queue" = "Geometry" "RenderType"="Opaque" }
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				uniform float4 _LightDir;

				sampler2D _ColorMap2;
				float4 _ColorMap2_ST;

				struct IN{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					fixed4 color    : COLOR;
					float4 texcoord : TEXCOORD0;
					
				};
			
				struct OUT {
					float4 pos : SV_POSITION;
					float2 textcoord : TEXCOORD0;
				};
									
				OUT vert(IN v) {
					
					half3 normal =  normalize(mul(_Object2World,half4(v.normal,0.0)).xyz);

  					// How much the light affects to this triangle
  					// range -1..1 to 0..1
  					fixed light = 1+(dot (normal, normalize(_LightDir.xyz).xyz));
  					light*=0.5;
  				
  					OUT o;
  	
  					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
  					o.textcoord = TRANSFORM_TEX (v.texcoord, _ColorMap2);
  					o.textcoord.x = half(clamp(light+(1-_LightDir.w/100),0,1));

					return o;
				}
			
				float4 frag(OUT vert) : COLOR {
					half2 colorUv = half2(vert.textcoord.x,vert.textcoord.y);
  					return tex2D (_ColorMap2, colorUv);
				}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
