//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DistanceLight : MonoBehaviour {

	public float maxDistance =5.0f;
	public bool additiveBlending =false; 
	public Texture rampTexture = null;
	List<Material> materials = new List<Material>();

	Texture lastRampTexture = null;
	Vector3 lastPosition = Vector3.zero;
	float lastDistance = float.MaxValue;
	bool lastBlending =false;

	// Use this for initialization
	void Start () {
		findMaterials();
	}

	public void distanceLightChanged(){
		findMaterials();
	}

	void findMaterials(){
		materials.Clear ();
		Renderer[] arrend = (Renderer[])Resources.FindObjectsOfTypeAll(typeof(Renderer));
		foreach (Renderer rend in arrend) {
			if (rend != null) {
				foreach (Material mat in rend.sharedMaterials) {
					if (mat && !materials.Contains (mat)) {
						if (mat.shader != null) {
							if(mat.shader.name.Contains("Kirnu/Marvelous/") && 
							   mat.shader.name.Contains("DistanceLight")){
								materials.Add(mat);
							}
						}
					}
				}
			}
		}
		
		updateMaterials ();
	}

	void updateMaterials(){
		foreach(Material m in materials){
			if(m && m.HasProperty("_LightPos") && lastPosition != transform.position){
				m.SetVector("_LightPos",transform.position);
			}
			if(m && m.HasProperty("_LightMaxDistance") && lastDistance != maxDistance){
				m.SetFloat("_LightMaxDistance",maxDistance);
			}
			if(m && m.HasProperty("_LightRampTexture") && rampTexture && lastRampTexture != rampTexture){
				m.SetTexture("_LightRampTexture",rampTexture);
			}
			if(m && lastBlending != additiveBlending){
				if(additiveBlending){
					m.EnableKeyword("DIST_LIGHT_ADDITIVE");
				}
				else{
					m.DisableKeyword("DIST_LIGHT_ADDITIVE");
				}
			}
		}
		lastPosition = transform.position;
		lastDistance = maxDistance;
		lastRampTexture = rampTexture;
		lastBlending = additiveBlending;
	}
	
	void Update () {
		updateMaterials ();
	}
}
