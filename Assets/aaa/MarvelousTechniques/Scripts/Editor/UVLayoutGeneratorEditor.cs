//----------------------------------------------
//            Monumental Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UVLayoutGenerator))]
public class UVGeneratorEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		UVLayoutGenerator myScript = (UVLayoutGenerator)target;
		if(GUILayout.Button("Generate UVs"))
		{
			myScript.GenerateUVs();
		}
	}
}
