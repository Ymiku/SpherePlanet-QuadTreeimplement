//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/Camera Overlay"
{
    Properties {
        _MainTex ("Texture", 2D) = "" {}
        _AlphaPower ("Alpha Power", Float) = 0.5
        _Level ("Level", Float) = 0.5
    }
 
    SubShader {
 
		Tags { "RenderType"="Transparent" "QUEUE"="Transparent+100" }
		Lighting Off
		Cull Off 
		ZTest Always
		ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

       
        Pass {  
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
 
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            sampler2D _MainTex;
 			uniform float _AlphaPower;	
 			uniform float _Level;
            uniform float4 _MainTex_ST;
           
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }
 
            fixed4 frag (v2f i) : COLOR
            {
                float4 texColor = tex2D(_MainTex, i.texcoord);
                texColor.a*=_AlphaPower;
                return texColor*_Level;
            }
            ENDCG
        }
    }  
 
    Fallback off
}