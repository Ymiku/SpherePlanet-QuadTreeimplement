Shader "Kirnu/Marvelous/Bloom" {
Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Bloom ("Bloom (RGB)", 2D) = "black" {}
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _Bloom;
				
		uniform half4 _MainTex_TexelSize;
		
		uniform half4 _Parameter;
		uniform half4 _BloomColor;
		
		#define INTENSITY _Parameter.w
		#define THRESHHOLD _Parameter.z

		struct v2f_simple 
		{
			float4 pos : SV_POSITION; 
			half2 uv : TEXCOORD0;

        #if UNITY_UV_STARTS_AT_TOP
				half2 uv2 : TEXCOORD1;
		#endif
		};	
		
		v2f_simple vertBloom ( appdata_img v ){
			v2f_simple o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
        	o.uv = v.texcoord;		
        	
        #if UNITY_UV_STARTS_AT_TOP
        	o.uv2 = v.texcoord;				
        	if (_MainTex_TexelSize.y < 0.0)
        		o.uv.y = 1.0 - o.uv.y;
        #endif
        	        	
			return o; 
		}

		fixed4 fragDownsample ( v2f_simple i ) : SV_Target{				
			#if UNITY_UV_STARTS_AT_TOP
				fixed4 color = tex2D(_MainTex, i.uv2);
			#else
				fixed4 color = tex2D(_MainTex, i.uv);			
			#endif	
			color =lerp(0,color,(Luminance(color)-(1-THRESHHOLD))*INTENSITY);

			return _BloomColor*max(color, 0);
		}
						
		fixed4 fragBloom ( v2f_simple i ) : SV_Target{	
        	#if UNITY_UV_STARTS_AT_TOP
			
			fixed4 color = tex2D(_MainTex, i.uv2);
			return color + tex2D(_Bloom, i.uv);
			
			#else

			fixed4 color = tex2D(_MainTex, i.uv);
			return color + tex2D(_Bloom, i.uv);
						
			#endif
		} 

		static const half offset[3] = {half( 0.0), half(1.3846153846), half(3.2307692308 )};
		static const half weight[3] = {half( 0.2270270270), half(0.3162162162), half(0.0702702703 )};

		struct v2f_Blur{
			float4 pos : SV_POSITION;
			half4 uv : TEXCOORD0;
		};	

		v2f_Blur vertBlurVertical (appdata_img v){
			v2f_Blur o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			
			o.uv = half4(v.texcoord.xy,1,1);
			return o; 
		}	
		

		half4 fragBlur ( v2f_Blur i ) : SV_Target{
			half2 uv = i.uv.xy; 

    		half4 color = tex2D(_MainTex, uv) * weight[0];
    		for (int l=1; l<3; l++) {
      			color += tex2D(_MainTex, uv + half2(offset[l]*_Parameter.x*_MainTex_TexelSize.x, offset[l]*_Parameter.y*_MainTex_TexelSize.y)) * weight[l];
      			color += tex2D(_MainTex, uv - half2(offset[l]*_Parameter.x*_MainTex_TexelSize.x, offset[l]*_Parameter.y*_MainTex_TexelSize.y))  * weight[l];
    		}

			return color;
		}
			
	ENDCG
	
	SubShader {
	  	Lighting Off
		ZTest Always
		Cull Off
		ZWrite Off
		Fog { Mode Off }
	  
	// 0
	Pass {
	
		CGPROGRAM
		#pragma vertex vertBloom
		#pragma fragment fragBloom
		
		ENDCG
		 
		}

	// 1
	Pass { 
	
		CGPROGRAM
		#pragma vertex vertBloom
		#pragma fragment fragDownsample
		
		ENDCG
		 
		}

	// 2
	Pass {
		CGPROGRAM 
		#pragma vertex vertBlurVertical
		#pragma fragment fragBlur
		
		ENDCG 
		}		
	}	

	FallBack Off
}