//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
#ifndef MARVELOUS_INCLUDED
#define MARVELOUS_INCLUDED

#include "UnityCG.cginc"
#include "AutoLight.cginc"

#ifdef USE_MAIN_TEX
sampler2D _MainTex;
float4 _MainTex_ST;
#endif

#ifdef USE_LAYOUT_TEXTURE
sampler2D _LayoutTexture;
float4 _LayoutTexture_ST;
#endif

#ifdef USE_DIST_LIGHT
sampler2D _LightRampTexture;
float4 _LightRampTexture_ST;
#endif


struct CL_IN{
	half4 vertex : POSITION;
	half3 normal : NORMAL;
	half3 color : COLOR;
	half4 texcoord : TEXCOORD0;
	half4 texcoord1 : TEXCOORD1;
#ifdef USE_LAYOUT_TEXTURE
	half4 texcoord3 : TEXCOORD3;
#endif

};

struct CL_OUT{
	half4 pos : SV_POSITION;
	half2 main_uv : TEXCOORD0;
	half2 lightmap_uv : TEXCOORD1;
	half3 lighting : TEXCOORD2;
#ifdef USE_LAYOUT_TEXTURE
	half4 texcoord3 : TEXCOORD3;
#endif
};

struct CL_OUT_WPOS {
	half4 pos : SV_POSITION;
#ifdef USE_MAIN_TEX
	half2 main_uv : TEXCOORD0;
#endif	
	half2 lightmap_uv : TEXCOORD1;
	half3 lighting : TEXCOORD2;
	half4 wpos: TEXCOORD3;
#ifdef USE_LAYOUT_TEXTURE
	half2 layouttexture_uv : TEXCOORD4;
#endif	

#ifdef USE_DIST_LIGHT
	half3 normal : TEXCOORD5;
#endif

#ifdef USE_REALTIME_SHADOWS
	SHADOW_COORDS(6)
#endif
	half3 color : TEXCOORD7;
};

struct CL_OUT_WPOS_SOFT_FOG {
	half4 pos : SV_POSITION;
	half2 main_uv : TEXCOORD0;
	half2 lightmap_uv : TEXCOORD1;
	half3 lighting : TEXCOORD2;
	half4 wpos: TEXCOORD3;
	half fogStartY: TEXCOORD4;
#ifdef USE_LAYOUT_TEXTURE
	half2 layouttexture_uv : TEXCOORD5;
#endif	
	
#ifdef USE_DIST_FOG
	half fogPower : TEXCOORD8;
#endif
	half3 color : TEXCOORD7;
};

CL_OUT_WPOS calculateLighting(CL_IN v,half3 rimColor,half rimPower,half3 f_color,half3 r_color,half3 t_color){
	
	CL_OUT_WPOS o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.wpos = mul( _Object2World, half4(v.vertex.xyz,1) );
	half3 normal =  normalize(mul(_Object2World,half4(v.normal,0))).xyz;
#ifdef USE_DIST_LIGHT
	o.normal = normal;
#endif
	o.color=v.color;
	half f_d = acos(clamp(dot(half3(0,0,-1),half3(0,0,normal.z)),-1,1))/1.5708;
	half r_d = acos(clamp(dot(half3(1,0,0),half3(normal.x,0,0)),-1,1))/1.5708;
	half t_d = acos(clamp(dot(half3(0,1,0),half3(0,normal.y,0)),-1,1))/1.5708;

	f_d = lerp(0,1-f_d,half(normal.z<1));
	r_d = lerp(0,1-r_d,half(normal.x>0));
	t_d = lerp(0,1-t_d,half(normal.y>0));
	
#ifdef USE_LAYOUT_TEXTURE	
	o.layouttexture_uv = TRANSFORM_TEX (v.texcoord3, _LayoutTexture);
#endif
	
#ifdef USE_MAIN_TEX
	o.main_uv = TRANSFORM_TEX (v.texcoord, _MainTex);
#endif

	half rim = 1-(f_d+r_d+t_d);

	o.lightmap_uv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	o.lighting = (f_color*f_d) + (r_color*r_d) + (t_color*t_d)+(rimColor*rim*rimPower);
	
#ifdef USE_REALTIME_SHADOWS
	TRANSFER_SHADOW(o);
#endif

	return o;
}

CL_OUT_WPOS customLightingWPosVert(CL_IN v, half3 rimColor, half rimPower, half3 r_color, half3 f_color, half3 t_color, half ambientColor, float ambientPower){
			
	CL_OUT_WPOS o = calculateLighting(v,rimColor, rimPower,f_color,r_color,t_color);
	o.lighting += (ambientColor*ambientPower);
	
	return o;
}

CL_OUT_WPOS customLightingWPosVertSimple(CL_IN v, half3 mainColor,half3 rimColor, half rimPower,half rightLight, half frontLight, half topLight, half3 ambientColor, float ambientPower){
	
	CL_OUT_WPOS o=calculateLighting(v,rimColor, rimPower,frontLight,rightLight,topLight);
	o.lighting = mainColor*o.lighting+(ambientColor*ambientPower);

	return o;
}

CL_OUT_WPOS_SOFT_FOG customLightingSimpleSoftFogVert(CL_IN v,half3 mainColor, half3 rimColor, half rimPower, half3 rightLight, half3 frontLight, half3 topLight, half ambientColor,
	half ambientPower,half fogStartY, half animationHeight, half fogAnimationFreq){
			
		CL_OUT_WPOS o=customLightingWPosVertSimple(v,mainColor, rimColor, rimPower, rightLight, frontLight, topLight, ambientColor, ambientPower);
		
		CL_OUT_WPOS_SOFT_FOG o2;
		o2.pos=o.pos;
#ifdef USE_MAIN_TEX
		o2.main_uv=o.main_uv;
#endif
		o2.lightmap_uv=o.lightmap_uv;
#ifdef USE_LAYOUT_TEXTURE	
		o2.layouttexture_uv = o.layouttexture_uv;
#endif
		o2.color=o.color;
		o2.lighting=o.lighting;
		o2.wpos=o.wpos;
		fogStartY+=((sin( _Time * 10 * fogAnimationFreq))+1)*0.5* animationHeight;
		o2.fogStartY.x = fogStartY;
		return o2;
}

CL_OUT_WPOS_SOFT_FOG customLightingSoftFogVert(CL_IN v, half3 rimColor, half rimPower, half3 rightColor, half3 frontColor, half3 topColor, half ambientColor,
	half ambientPower,half fogStartY, half animationHeight, half fogAnimationFreq){
			
		CL_OUT_WPOS o=customLightingWPosVert(v, rimColor, rimPower,rightColor, frontColor, topColor, ambientColor, ambientPower);
		
		CL_OUT_WPOS_SOFT_FOG o2;
		o2.pos=o.pos;
#ifdef USE_MAIN_TEX
		o2.main_uv=o.main_uv;
#endif
		o2.lightmap_uv=o.lightmap_uv;
		o2.color=o.color;
		o2.lighting=o.lighting;
		o2.wpos=o.wpos;
		fogStartY+=((sin( _Time * 10 * fogAnimationFreq))+1)*0.5* animationHeight;
		o2.fogStartY.x = fogStartY;
#ifdef USE_DIST_FOG
		o2.fogPower = 0;
#endif
		return o2;
}

fixed shadowAttenuation(CL_OUT_WPOS v){
	#ifdef USE_REALTIME_SHADOWS
	return SHADOW_ATTENUATION(v);
	#else
	return 1;
	#endif
}

fixed4 customLightingFrag(CL_OUT_WPOS v, half3 lightTint, half useLightmap, half lightmapPower, half3 lightmapColor,half _ShadowPower){
	fixed4 outColor = fixed4(0.0, 0.0, 0.0, 0.0);

#ifdef USE_MAIN_TEX
	half4 textureColor = tex2D (_MainTex, v.main_uv);
	
#ifdef USE_LAYOUT_TEXTURE
		half4 cMap = tex2D (_LayoutTexture, v.layouttexture_uv);
		textureColor.xyz = ((lightTint * v.lighting) * textureColor.xyz)*lerp(half3(1,1,1),cMap,_LayoutTexturePower);
#else
		textureColor.xyz = ((lightTint * v.lighting) * textureColor.xyz);
#endif

#else
	half4 textureColor = half4((lightTint * v.lighting),1);
#endif
	
#if LIGHTMAP
		half3 realLightmapPower;
		fixed4 bakedColorTex  = UNITY_SAMPLE_TEX2D(unity_Lightmap, v.lightmap_uv)*shadowAttenuation(v);
		realLightmapPower = lerp(half3(1,1,1),clamp(DecodeLightmap(bakedColorTex)+(_ShadowPower),0,1),lightmapPower);
		outColor.xyz = lerp (lightmapColor.xyz,half3(1,1,1), realLightmapPower)*textureColor.xyz;
#else
		
		outColor = textureColor*shadowAttenuation(v);
#endif
	outColor.xyz*=v.color;
	return outColor;
}

#ifdef USE_DIST_LIGHT
fixed4 customLightingFragDistLight(CL_OUT_WPOS v, half3 lightTint, half useLightmap, half lightmapPower,
 half3 lightmapColor,half _ShadowPower,half3 lightPos){
 	fixed4 outColor = customLightingFrag(v, lightTint, useLightmap, lightmapPower, lightmapColor, _ShadowPower);
 	half l =length(lightPos - v.wpos);
 	l = clamp(min(l,_LightMaxDistance)/_LightMaxDistance,0,1);
 	/*half3 ldir= v.wpos-lightPos;
 	half ldist = length(ldir);
 	ldir = normalize(ldir);
 	half f = dot(v.normal, -ldir);
 	f = lerp(0,1-l,f>0);*/
 	half f = 1-l;
 	half4 c1 =tex2D (_LightRampTexture, half2(0,f));
 	//c1 = lerp(0,c1*f,f>0);
 #ifdef DIST_LIGHT_ADDITIVE
 	return outColor*0.5 +c1*0.5;
 #else
 	return outColor*c1;
 #endif
}
#endif

fixed4 customLightingHardFogFrag(CL_OUT_WPOS v,float fogYStartPos, half3 fogColor, half3 lightTint, half useLightmap, half lightmapPower, half3 lightmapColor,half _ShadowPower){

	fixed4 outColor = customLightingFrag(v, lightTint, useLightmap, lightmapPower, lightmapColor, _ShadowPower);
	return  fixed4(lerp( fogColor, outColor, v.wpos.y > fogYStartPos),0);
}

fixed4 customLightingSoftFogFrag(CL_OUT_WPOS_SOFT_FOG v, half3 fogColor, half fogHeight, half3 lightTint, half useLightmap, half lightmapPower, half3 lightmapColor,half _ShadowPower){
		CL_OUT_WPOS o2;
		o2.pos=v.pos;
#ifdef USE_MAIN_TEX
		o2.main_uv=v.main_uv;
#endif
		o2.lightmap_uv=v.lightmap_uv;
		o2.lighting=v.lighting;
		o2.wpos=v.wpos;
		o2.color = v.color;
#ifdef USE_LAYOUT_TEXTURE
		o2.layouttexture_uv = v.layouttexture_uv;
#endif
	fixed4 outColor = customLightingFrag(o2, lightTint, useLightmap, lightmapPower, lightmapColor, _ShadowPower);
	half fogDensity = clamp((v.wpos.y - v.fogStartY.x)/fogHeight,0,1);
  	outColor = fixed4(lerp ( fogColor, outColor.xyz, fogDensity),0);
  	return outColor;
}

/*
*
* Blend mode script by Aubergine
* http://forum.unity3d.com/threads/free-photoshop-blends.121661/
*
*/
fixed3 Darken (fixed3 a, fixed3 b) { return fixed3(min(a.rgb, b.rgb)); }
fixed3 Multiply (fixed3 a, fixed3 b) { return (a * b); }
fixed3 ColorBurn (fixed3 a, fixed3 b) { return (1-(1-a)/b); }
fixed3 LinearBurn (fixed3 a, fixed3 b) { return (a+b-1); }
fixed3 Lighten (fixed3 a, fixed3 b) { return fixed3(max(a.rgb, b.rgb)); }
fixed3 Screen (fixed3 a, fixed3 b) { return (1-(1-a)*(1-b)); }
fixed3 ColorDodge (fixed3 a, fixed3 b) { return (a/(1-b)); }
fixed3 LinearDodge (fixed3 a, fixed3 b) { return (a+b); }
fixed3 Overlay (fixed3 a, fixed3 b) {
    fixed3 r = fixed3(0,0,0);
    if (a.r > 0.5) { r.r = 1-(1-2*(a.r-0.5))*(1-b.r); }
    else { r.r = (2*a.r)*b.r; }
    if (a.g > 0.5) { r.g = 1-(1-2*(a.g-0.5))*(1-b.g); }
    else { r.g = (2*a.g)*b.g; }
    if (a.b > 0.5) { r.b = 1-(1-2*(a.b-0.5))*(1-b.b); }
    else { r.b = (2*a.b)*b.b; }
    return r;
}
fixed3 SoftLight (fixed3 a, fixed3 b) {
    fixed3 r = fixed3(0,0,0);
    if (b.r > 0.5) { r.r = a.r*(1-(1-a.r)*(1-2*(b.r))); }
    else { r.r = 1-(1-a.r)*(1-(a.r*(2*b.r))); }
    if (b.g > 0.5) { r.g = a.g*(1-(1-a.g)*(1-2*(b.g))); }
    else { r.g = 1-(1-a.g)*(1-(a.g*(2*b.g))); }
    if (b.b > 0.5) { r.b = a.b*(1-(1-a.b)*(1-2*(b.b))); }
    else { r.b = 1-(1-a.b)*(1-(a.b*(2*b.b))); }
    return r;
}
fixed3 HardLight (fixed3 a, fixed3 b) {
    fixed3 r = fixed3(0,0,0);
    if (b.r > 0.5) { r.r = 1-(1-a.r)*(1-2*(b.r)); }
    else { r.r = a.r*(2*b.r); }
    if (b.g > 0.5) { r.g = 1-(1-a.g)*(1-2*(b.g)); }
    else { r.g = a.g*(2*b.g); }
    if (b.b > 0.5) { r.b = 1-(1-a.b)*(1-2*(b.b)); }
    else { r.b = a.b*(2*b.b); }
    return r;
}
fixed3 VividLight (fixed3 a, fixed3 b) {
    fixed3 r = fixed3(0,0,0);
    if (b.r > 0.5) { r.r = 1-(1-a.r)/(2*(b.r-0.5)); }
    else { r.r = a.r/(1-2*b.r); }
    if (b.g > 0.5) { r.g = 1-(1-a.g)/(2*(b.g-0.5)); }
    else { r.g = a.g/(1-2*b.g); }
    if (b.b > 0.5) { r.b = 1-(1-a.b)/(2*(b.b-0.5)); }
    else { r.b = a.b/(1-2*b.b); }
    return r;
}
fixed3 LinearLight (fixed3 a, fixed3 b) {
    fixed3 r = fixed3(0,0,0);
    if (b.r > 0.5) { r.r = a.r+2*(b.r-0.5); }
    else { r.r = a.r+2*b.r-1; }
    if (b.g > 0.5) { r.g = a.g+2*(b.g-0.5); }
    else { r.g = a.g+2*b.g-1; }
    if (b.b > 0.5) { r.b = a.b+2*(b.b-0.5); }
    else { r.b = a.b+2*b.b-1; }
    return r;
}
fixed3 PinLight (fixed3 a, fixed3 b) {
    fixed3 r = fixed3(0,0,0);
    if (b.r > 0.5) { r.r = max(a.r, 2*(b.r-0.5)); }
    else { r.r = min(a.r, 2*b.r); }
    if (b.g > 0.5) { r.g = max(a.g, 2*(b.g-0.5)); }
    else { r.g = min(a.g, 2*b.g); }
    if (b.b > 0.5) { r.b = max(a.b, 2*(b.b-0.5)); }
    else { r.b = min(a.b, 2*b.b); }
    return r;
}
fixed3 Difference (fixed3 a, fixed3 b) { return (abs(a-b)); }
fixed3 Exclusion (fixed3 a, fixed3 b) { return (0.5-2*(a-0.5)*(b-0.5)); }
 
#endif