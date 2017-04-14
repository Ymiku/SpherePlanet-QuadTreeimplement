//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

namespace Kirnu
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu("Image Effects/Marvelous/Screen Texture Blend")]
	public class ScreenTextureBlend : MonoBehaviour
	{	
		public enum BlendMode {Darken, Multiply, ColorBurn, LinearBurn, Lighten, Screen, ColorDodge, LinearDodge, Overlay, SoftLight,HardLight, VividLight, LinearLight, PinLight,Difference,Exclusion};
		public BlendMode blendMode;
		int currentBlendMode;

		[Range(0.0f, 1.0f)]
		public float blendIntensity;
		[Range(0.0f, 1.0f)]
		public float vignetteIntensity;
		[Range(0.0f, 1.0f)]
		public float vignetteMaxValue = 0.2f;

		public Texture2D gradientTexture = null;
		Shader shader;
		public Material material;

		
		void Start ()
		{
			currentBlendMode = -1;
			if (gradientTexture == null) {
				gradientTexture = Texture2D.whiteTexture;
			}
			if (!SystemInfo.supportsImageEffects)
			{
				enabled = false;
				return;
			}
		}

		public void OnRenderImage (RenderTexture src, RenderTexture dest)
		{
			if (enabled && material && gradientTexture)
			{

				if(currentBlendMode != (int)blendMode){
					currentBlendMode = (int)blendMode;
					if(((int)blendMode & (1)) != 0){
						material.EnableKeyword("B1");
					}
					else{
						material.DisableKeyword("B1");
					}
					if(((int)blendMode & (1<<1)) != 0){
						material.EnableKeyword("B2");
					}
					else{
						material.DisableKeyword("B2");
					}
					if(((int)blendMode & (1<<2)) != 0){
						material.EnableKeyword("B3");
					}
					else{
						material.DisableKeyword("B3");
					}
					if(((int)blendMode & (1<<3)) != 0){
						material.EnableKeyword("B4");
					}
					else{
						material.DisableKeyword("B4");
					}
				}

				material.SetFloat("_VignetteMax",vignetteMaxValue);
				material.SetFloat("_BlendIntensity",blendIntensity);
				material.SetFloat("_VignetteIntensity",vignetteIntensity);
				material.SetTexture ("_Gradient", gradientTexture);

				Graphics.Blit (src, dest, material,0);
			}
			else
			{
				Graphics.Blit (src, dest);
			}
		}
		
		void OnDisable (){
		}
	}
}




