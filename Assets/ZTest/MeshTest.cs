using UnityEngine;
using System.Collections;

public class MeshTest : MonoBehaviour {
	public MeshFilter a;
	int[] ta;
	Vector3[] va;
	Vector3[] na;
	// Use this for initialization
	void Start () {
		ta = a.mesh.triangles;
		va = a.mesh.vertices;
		na = a.mesh.normals;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
