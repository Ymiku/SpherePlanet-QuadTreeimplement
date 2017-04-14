//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CubeMover : MonoBehaviour {

	public float xSpeed = 0f;
	public float ySpeed = 0f;
	public float zSpeed = 0f;
	public float maxMovement = 10f;
	Vector3 startPos = Vector3.zero;
	bool forward = true;
	// Use this for initialization
	void Start () {
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 p = transform.position;

		p.x += xSpeed * Time.deltaTime*(forward?1:-1);
		p.y += ySpeed * Time.deltaTime*(forward?1:-1);
		p.z += zSpeed * Time.deltaTime*(forward?1:-1);

		transform.position = p;
		if (Vector3.Distance (startPos, p) >= maxMovement) {
			forward = !forward;
			startPos = transform.position;
		}
	}
}
