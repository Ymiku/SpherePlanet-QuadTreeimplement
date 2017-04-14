//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/ScreenTextureBlend"
{
	Properties
	{	
	 	_MainTex ("", 2D) = "white" {}
	 	_Gradient ("", 2D) = "white" {}
	 	_BlendMode ("Mode", Float) = 0
	 	[HideInInspector][Toggle(B1)] _B1 ("b1", Float) = 0
	 	[HideInInspector][Toggle(B2)] _B2 ("b2", Float) = 0
	 	[HideInInspector][Toggle(B3)] _B3 ("b3", Float) = 0
	 	[HideInInspector][Toggle(B4)] _B4 ("b4", Float) = 0
	}
	 
	SubShader
	{
		Lighting Off
		ZTest Always
		Cull Off
		ZWrite Off
		Fog { Mode Off }
	 	
	 	Pass
	 	{
	  		CGPROGRAM

	  		#define USE_MAIN_TEX;
	  		#pragma vertex vert_img
	  		#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
	    	#include "../Marvelous.cginc"
	    	
	    	#pragma shader_feature B1
	    	#pragma shader_feature B2 
	    	#pragma shader_feature B3
	    	#pragma shader_feature B4
	    	
	    	#define BLEND_MODE(s,s2,s3,s4) str(s,s2,s3,s4)
     		#define str(s,s2,s3,s4) applyBlend_##s##s2##s3##s4
     		
 			#ifdef B1
				#define B1_2 1
			#else
				#define B1_2 0
			#endif
			
			#ifdef B2
				#define B2_2 1
			#else
				#define B2_2 0
			#endif
			
			#ifdef B3
				#define B3_2 1
			#else
				#define B3_2 0
			#endif
			
			#ifdef B4
				#define B4_2 1
			#else
				#define B4_2 0
			#endif
				
	    	uniform int _BlendMode;
	    	uniform int _B1;
	    	uniform int _B2;
	    	uniform int _B3;
	    	uniform int _B4;
	    	
			uniform float _BlendIntensity;
			uniform float _VignetteIntensity;
			uniform float _VignetteMax;
	    	uniform sampler2D _Gradient;
	    	
	    	fixed3 applyBlend_0000(fixed3 original,fixed3 gradient){
	    		return Darken(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_0001(fixed3 original,fixed3 gradient){
	    		return Multiply(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_0010(fixed3 original,fixed3 gradient){
	    		return ColorBurn(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_0011(fixed3 original,fixed3 gradient){
	    		return LinearBurn(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_0100(fixed3 original,fixed3 gradient){
	    		return Lighten(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_0101(fixed3 original,fixed3 gradient){
	    		return Screen(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_0110(fixed3 original,fixed3 gradient){
	    		return ColorDodge(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_0111(fixed3 original,fixed3 gradient){
	    		return LinearDodge(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_1000(fixed3 original,fixed3 gradient){
	    		return Overlay(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_1001(fixed3 original,fixed3 gradient){
	    		return SoftLight(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_1010(fixed3 original,fixed3 gradient){
	    		return HardLight(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_1011(fixed3 original,fixed3 gradient){
	    		return VividLight(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_1100(fixed3 original,fixed3 gradient){
	    		return LinearLight(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_1101(fixed3 original,fixed3 gradient){
	    		return PinLight(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_1110(fixed3 original,fixed3 gradient){
	    		return Difference(original, gradient).rgb;
	    	}
	    	fixed3 applyBlend_1111(fixed3 original,fixed3 gradient){
	    		return Exclusion(original, gradient).rgb;
	    	}
	    		
	  		fixed4 frag (v2f_img i) : COLOR
	  		{
	   			fixed3 original = tex2D (_MainTex, i.uv).rgb;
				fixed4 gradient = tex2D(_Gradient,i.uv);

				fixed4 effect=fixed4(BLEND_MODE(B4_2,B3_2,B2_2,B1_2)(original,gradient.rgb),0);	
	   			effect = fixed4(lerp(original,effect,_BlendIntensity),0);
	   			half2 coords = i.uv;
				half2 uv = i.uv;
		
				coords = (coords - 0.5) * 2.0;		
				half coordDot = dot (coords,coords); 
				float mask = 1.0 - coordDot  * _VignetteMax * _VignetteIntensity;
	   			effect.rgb *= mask;
				
	   			return effect;
	  		}
	  		
	  		ENDCG
	 	}
	}
	
	FallBack "Diffuse"
}
