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
	[AddComponentMenu("Image Effects/Marvelous/Marvelous Bloom")]
	public class MarvelousBloom : MonoBehaviour
	{	
		public enum Resolution
		{
			Lower = 0,
			Low = 1,
			High = 2,
			Higher = 3,
		}
		
		public enum BlurType
		{
			Standard = 0,
			Sgx = 1,
		}
		
		public Color bloomColor = Color.white;
		[Range(0.0f, 1.5f)]
		public float threshold = 0.05f;
		[Range(0.0f, 1f)]
		public float intensity = 0.05f;
		
		public Resolution resolution = Resolution.Low;
		[Range(1, 8)]
		public int blurIterations = 1;
		
		Shader shader;
		public Material material;
		
		void Start ()
		{
			if (!SystemInfo.supportsImageEffects)
			{
				enabled = false;
				return;
			}
		}
		
		public void OnRenderImage (RenderTexture src, RenderTexture dest)
		{
			if (enabled && material)
			{
				int divider =  (resolution == Resolution.Lower ? 8 : (resolution == Resolution.Low ? 4 : (resolution == Resolution.High ? 2 : 1)));
				
				var rtW= src.width/divider;
				var rtH= src.height/divider;
				
				material.SetColor("_BloomColor",bloomColor);
				material.SetVector ("_Parameter", new Vector4 (rtW, rtH, threshold, intensity));
				src.filterMode = FilterMode.Bilinear;
				
				// Downsample
				RenderTexture rt = RenderTexture.GetTemporary (rtW, rtH, 0, src.format);
				rt.filterMode = FilterMode.Bilinear;
				Graphics.Blit (src, rt, material, 1);
				
				for(int i = 0; i < blurIterations; i++)
				{
					
					material.SetVector ("_Parameter", new Vector4 (0, 1, threshold, intensity));
					// vertical blur
					RenderTexture rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, src.format);
					rt2.filterMode = FilterMode.Bilinear;
					Graphics.Blit (rt, rt2, material, 2);
					RenderTexture.ReleaseTemporary (rt);
					rt = rt2;
					
					material.SetVector ("_Parameter", new Vector4 (1,0, threshold, intensity));
					// horizontal blur
					rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, src.format);
					rt2.filterMode = FilterMode.Bilinear;
					Graphics.Blit (rt, rt2, material, 2);
					RenderTexture.ReleaseTemporary (rt);
					rt = rt2;
				}
				
				material.SetTexture ("_Bloom", rt);
				
				Graphics.Blit (src, dest, material, 0);
				//Graphics.Blit (rt, dest);
				RenderTexture.ReleaseTemporary (rt);
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




