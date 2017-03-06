using UnityEngine;
using System.Collections;

public class MeshTest : MonoBehaviour {
	public MeshFilter a;
	public MeshFilter b;
	public Mesh m;
	// Use this for initialization
	void Start () {
		
		m = new Mesh();
		m.vertices = new Vector3[]{ Vector3.zero,Vector3.zero,Vector3.zero};
		a.mesh = m;
		//b.mesh= m;
		//a.mesh.triangles = new int[]{ 0,1,2};
		//Debug.Log(a.mesh==b.mesh);
		Debug.Log(a.mesh==m);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
