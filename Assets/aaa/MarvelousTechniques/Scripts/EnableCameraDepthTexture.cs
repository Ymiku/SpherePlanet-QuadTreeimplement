//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EnableCameraDepthTexture : MonoBehaviour {
	void Start (){
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}
}
