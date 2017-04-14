//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;

[ExecuteInEditMode]
public class EditModeGridSnap : MonoBehaviour {
	public float snapValue = 0.5f;
	private bool snapX=true;
	private bool snapY=true;
	private bool snapZ=true;    
	
	void Update() {
		if(!Application.isPlaying){
		float snapInverse = 1/snapValue;
		
		float x, y, z;
		
		if(snapX){
				x = Mathf.Round(transform.localPosition.x * snapInverse)/snapInverse;
		}
		else{
				x=transform.localPosition.x;
		}
		if(snapY){
				y = Mathf.Round(transform.localPosition.y * snapInverse)/snapInverse;  
		}
		else{
				y=transform.localPosition.y;
		}
		if(snapZ){
				z = Mathf.Round(transform.localPosition.z * snapInverse)/snapInverse;  
		}
		else{
				z=transform.localPosition.z;
		}
		
		transform.localPosition = new Vector3(x, y, z);
		}
	}
}