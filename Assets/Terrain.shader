Shader "Unlit/Terrain"
{
	Properties
	{
		_Radius("Radius", Float) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				fixed4 col:Color;
			};
			float _Radius;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				float dis = v.vertex.x*v.vertex.x+v.vertex.y*v.vertex.y+v.vertex.z*v.vertex.z;
				if(dis>_Radius)
				{
					o.col = (0,.1,0,.5);
				}
				else
				{
					o.col = (.1,0,0,1);
				}
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed col = i.col;
				return col;
			}
			ENDCG
		}
	}
}
