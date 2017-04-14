//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/OceanDepth" {
	Properties {
 		_FoamMap ("Foam Map", 2D) = "white" {}
 		_ColorMap ("Color map", 2D) = "white" {}
 		_LightDir ("Light Direction", Vector) = (0,-1,0)
 		
 		_FoamSpeed ("Foam speed", Float) = 0.1
 		_FoamColor ("Foam color", Color) = (1,1,1,0)
 		_FoamPower ("Foam power", Float) = 0.5
 		_FoamLevel ("Foam y-level", Float) = 0.5
 		_FoamThreshold("Foam threshold", Float) = 1
	}
	SubShader {
		Tags { "Queue" = "Transparent" "RenderType"="Transparent"  }
		LOD 200
		
		Pass {
		Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Transparent" }
			
			Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				uniform sampler2D _CameraDepthTexture;
				uniform float4 _LightDir;


				uniform float _FoamPower;
				uniform float _FoamLevel;
				uniform float4 _FoamColor;
				sampler2D _ColorMap;
				sampler2D _FoamMap;
				float4 _FoamMap_ST;
				uniform float _FoamThreshold;
				uniform float _FoamSpeed;
				
				struct IN{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					fixed4 color    : COLOR;
					float4 texcoord : TEXCOORD0;
					
				};
			
				struct OUT {
					float4 pos : SV_POSITION;
					float2 textcoord : TEXCOORD0;
					float4 projPos : TEXCOORD1;
					float2 wYPos : TEXCOORD2;
				};
						
				float4 getDepthColor(OUT i,fixed4 mainColor,fixed4 foamColorMap){
                	float4 finalColor = mainColor;
 
                	float sceneZ = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
                	float partZ = i.projPos.z;

                	float diff = (sceneZ - partZ) / (_FoamThreshold/100);
 
                	if(diff <= 1 && diff > 0){
                		if(diff < 0.7 &&i.wYPos.y  > _FoamLevel + foamColorMap.x && foamColorMap.x > 0.01){
                    		finalColor = lerp(_FoamColor *_FoamPower, mainColor, float4(diff, diff, diff, diff));
                    		finalColor.w=(1.7-diff);
                    	}
                    	else{
                    		finalColor.w=(diff);
                    	}
                	}
                	
                	return finalColor;
                }				
				OUT vert(IN v) {
					
					half3 normal =  normalize(mul(_Object2World,half4(v.normal,0.0)).xyz);

  					// How much the light affects to this triangle
  					// range -1..1 to 0..1
  					fixed light = 1+(dot (normal, normalize(_LightDir.xyz).xyz));
  					light*=0.5;
  				
  					OUT o;
  	
  					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
  					o.textcoord = TRANSFORM_TEX (v.texcoord, _FoamMap);
  					o.textcoord.x+=_Time*_FoamSpeed;
  					o.wYPos.x = half(clamp(light+(1-_LightDir.w/100),0,1));
  					o.wYPos.y = mul(_Object2World, v.vertex).y;
					o.projPos = ComputeScreenPos(o.pos);
					
					return o;
				}
			
				float4 frag(OUT vert) : COLOR {
  					fixed4 mainColor;
  					mainColor. w = 1;
  					float2 colorUv;
  					colorUv.x = vert.wYPos.x;
  					colorUv.y = vert.textcoord.y;
  					mainColor.xyz = tex2D (_ColorMap, colorUv).xyz;
					
					fixed4 foamColorMap = tex2D (_FoamMap, vert.textcoord);
					mainColor = getDepthColor(vert,mainColor,foamColorMap);
					
					return mainColor;
				}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
