//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent (typeof (Camera))]
public class CameraOverlay : MonoBehaviour {

	List<Transform> quads =null;

	void Start () {
		Transform[] ts = gameObject.GetComponentsInChildren<Transform> ();
		if (ts.Length > 0) {
			foreach(Transform t in ts){
				if(t != transform){
					if(quads == null){
						quads = new List<Transform>();
					}
					quads.Add(t);
				}
			}
		}
		if (quads == null || !GetComponent<Camera>()) {
			Debug.Log("This script must be attached to Camera. Camera must have at least one Quad Transform as a child");
			return;
		}

	}

	void Update () {
		if (GetComponent<Camera> () && quads != null) {
			if(GetComponent<Camera> ().orthographic){
				float screenAspect = (float)Screen.width / (float)Screen.height;
				float cameraHeight = GetComponent<Camera> ().orthographicSize * 2;
				Vector3 bounds = new Vector3 (cameraHeight * screenAspect, cameraHeight, 0);
				foreach (Transform quad in quads) {
					quad.localScale = new Vector3 (bounds.x, bounds.y, 1);
				}
			}
			else
			{
				foreach (Transform quad in quads) {
					float distance = Vector3.Distance(quad.position,GetComponent<Camera> ().transform.position);
					quad.LookAt (GetComponent<Camera> ().transform);
					if(distance < GetComponent<Camera> ().nearClipPlane){
						Vector3 dir = 1.1f*GetComponent<Camera> ().nearClipPlane*quad.forward;
						quad.Translate(dir);
						distance = Vector3.Distance(quad.position,GetComponent<Camera> ().transform.position);
					}

					
					float h = (Mathf.Tan(GetComponent<Camera> ().fieldOfView*Mathf.Deg2Rad*0.5f)*distance*2f);
					var frustumWidth = h * GetComponent<Camera> ().aspect;


					quad.localScale = new Vector3(frustumWidth, h, 1.0f);
				}
			}

		}
	}
}
