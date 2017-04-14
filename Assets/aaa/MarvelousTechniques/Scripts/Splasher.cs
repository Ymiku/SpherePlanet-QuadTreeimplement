//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Splasher : MonoBehaviour {
	public List<Transform> splashers;
	public float minTime = 5.0f;
	public float maxTime = 10.0f;
	

	List<Transform> activeSplashes = new List<Transform>();

	float time=0;
	float nextTime;
	int nextSplash;

	void Start () {
		nextValues();
	}

	void nextValues(){
		nextTime = Random.Range (minTime,maxTime);
		nextSplash = (int)Mathf.Floor (Random.Range (0f,splashers.Count));
	}
	
	void Update () {
		time += Time.deltaTime;
		if (time > nextTime) {
			createSplash();
			nextValues();
		}

		if (Input.GetMouseButton (0)) {
			createSplash ();
		}
		for (int i=0; i<activeSplashes.Count; i++) {
			ParticleSystem ps=activeSplashes[i].GetComponent<ParticleSystem>();
			if(!ps.IsAlive()){
				Destroy(activeSplashes[i].gameObject);
				activeSplashes.Remove(activeSplashes[i]);
				break;
			}
		}
	}

	void createSplash(){
		time = 0;
		if (activeSplashes.Count > 20 || splashers.Count == 0) {
			return;
		}

		Transform go=(Transform)Instantiate(splashers[nextSplash], splashers[nextSplash].position, Quaternion.identity);
		ParticleSystem ps=go.GetComponent<ParticleSystem>();
		ps.Play ();
		activeSplashes.Add (go);
	}
}
