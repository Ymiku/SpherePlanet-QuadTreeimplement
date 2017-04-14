//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Kirnu
{
	[ExecuteInEditMode]
	public class MeshTools : MonoBehaviour
	{

		public void OnGUI ()
		{
			EditorUtility.ClearProgressBar ();
		}

		void Update ()
		{
			EditorUtility.ClearProgressBar ();
		}

		[MenuItem ("Tools/Marvelous/Create Ocean")]
		static void createOcean ()
		{
			try {

				Mesh mesh = OceanCreator.createOcean ();

				GameObject ocean = new GameObject ("Ocean");
				ocean.AddComponent (typeof(MeshFilter));
				MeshRenderer renderer = ocean.AddComponent (typeof(MeshRenderer)) as MeshRenderer;
				Material mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/MarvelousTechniques/Materials/Ocean.mat", typeof(Material));
				renderer.material = mat;
				OceanWaver ow=(OceanWaver)ocean.AddComponent (typeof(OceanWaver));
				ow.mesh=mesh;

			} catch (Exception e) {
				EditorUtility.DisplayDialog ("An Exception occured", e.ToString (), "Ok");
			}
		}

		[MenuItem ("Tools/Marvelous/Combine Meshes",true,0)]
		static bool ValidateCombineMeshes ()
		{
			EditorUtility.ClearProgressBar ();
			return Selection.transforms.Length > 0;
		}

		[MenuItem ("Tools/Marvelous/Combine Meshes")]
		static void CombineMeshes ()
		{

			GameObject parent = Selection.activeGameObject;
		
			if (parent == null) {
				EditorUtility.DisplayDialog ("Nothing selected!", "Select one mesh. Please!", "Ok");
				return;
			}

			Vector3 parentPosition = parent.transform.position;
			Quaternion parentRotation = parent.transform.rotation;
			try {
				parent.transform.position = Vector3.zero;
				parent.transform.rotation = Quaternion.identity;

				bool canceled = MeshUtils.combineMeshes (parent, parentPosition, parentRotation);
				if (!canceled) {
					parent.SetActive (false);
				}
			} catch (Exception e) {
				EditorUtility.ClearProgressBar ();
				EditorUtility.DisplayDialog ("An Exception occured", e.ToString (), "Ok");
			} finally {
				EditorUtility.ClearProgressBar ();
				parent.transform.position = parentPosition;
				parent.transform.rotation = parentRotation;
			}
		}
	}
};