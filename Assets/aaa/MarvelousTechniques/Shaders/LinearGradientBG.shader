//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/LinearGradientBG" {
	Properties
	{
		_TopColor ("Top Color", Color) = (1,1,1,1)
		_BottomColor ("Bottom Color", Color) = (0,0,0,0)
		_Ratio("Ratio", Range(0,1)) = 0.5
	}
	SubShader
	{
		Cull Off
        ZWrite Off
                 
		Tags { "QUEUE"="Background" "RenderType"="Opaque" }
		LOD 200
		
		Pass {

		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Background" "RenderType"="Opaque" }
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			uniform fixed4 _TopColor;
			uniform fixed4 _BottomColor;
			uniform fixed _Ratio;
			
			struct IN
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct OUT
			{
				float4 pos : SV_POSITION;
				float4 color : TEXCOORD0;
			};

			OUT vert (IN v)
			{
				OUT o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				_Ratio *= 2;
				_Ratio -=1;
				o.color = lerp(_BottomColor,_TopColor,clamp(v.uv.y+(_Ratio),0,1));
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (OUT i) : SV_Target{
				return i.color;
			}
			ENDCG
		}
	}
}
