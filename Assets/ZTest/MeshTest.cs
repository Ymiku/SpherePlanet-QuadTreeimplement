using UnityEngine;
using System.Collections;

public class MeshTest : MonoBehaviour {
	public class A{
		public virtual void aaa()
		{
			Debug.Log ("a");
		}
	}
	class B:A{
		public override void aaa()
		{
			Debug.Log ("b");
		}
	}

	public MeshFilter a;
	int[] ta;
	Vector3[] va;
	Vector3[] na;
	// Use this for initialization
	void Start () {
		A b = new B ();
		b.aaa ();
		ta = a.mesh.triangles;
		va = a.mesh.vertices;
		na = a.mesh.normals;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
