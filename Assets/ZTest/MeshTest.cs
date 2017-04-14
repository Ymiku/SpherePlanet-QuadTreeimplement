using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MeshTest : MonoBehaviour {
	public MeshFilter mf;
	public Mesh m;
	void Start()    {        
		m = mf.mesh;           
	}
	void Update()
	{
		m.RecalculateHardNormals ();
	}
}
