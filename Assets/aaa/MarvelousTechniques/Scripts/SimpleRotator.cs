using UnityEngine;
using System.Collections;

public class SimpleRotator : MonoBehaviour {

	public float speed = 10;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3(0,Time.deltaTime*speed,0),Space.World);
	}
}
