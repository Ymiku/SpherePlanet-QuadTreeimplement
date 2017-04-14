//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UVLayoutGenerator : MonoBehaviour {
	
	public float xScale = 5f;
	public float yScale = 5f;

	public void GenerateUVs () {
		calculateUVs(transform, gameObject);
		foreach (Transform child in transform) {
			GameObject go = child.gameObject;
			calculateUVs(child, go);		
		}
	}

	private void calculateUVs(Transform t,GameObject go){
		MeshFilter mf = go.GetComponent<MeshFilter> ();
		if (mf == null) {
			return;
		}

		Mesh mesh = mf.sharedMesh;    

		if (mesh == null) {
			return;
		}

		Vector2[] uvs = new Vector2[mesh.vertices.Length];
		
		for (int triangle = 0; triangle < mesh.triangles.Length/3; triangle++) {
			
			Vector3[] positions = new Vector3[3];
			int[] indices = new int[3];
			Vector3 normal = new Vector3();
			
			for (int i=0; i<3; i++) {
				
				int vertexIndex = mesh.triangles [triangle * 3 + i];
				indices[i] = vertexIndex;
				if (t != null) {
					positions [i] = t.TransformPoint (mesh.vertices [vertexIndex]);
				} else {
					positions [i] = mesh.vertices [vertexIndex];
				}

				if (t != null) {
					normal += t.TransformDirection (mesh.normals [vertexIndex]);
				} else {
					normal += mesh.normals [vertexIndex];
				}
			}
			
			normal/=3;
			
			Quaternion rot = Quaternion.FromToRotation (normal, new Vector3 (0, 0, 1));

			Vector3 up = Vabs(rot*Vector3.up);
			Vector3 right = rot*Vector3.right;
			
			for (int j=0; j<positions.Length; j++) {
				uvs [indices[j]].x = Vector3.Dot(right,positions[j])/xScale;
				uvs [indices[j]].y = Vector3.Dot(up,positions[j])/yScale;
			}
		}
		mesh.uv4 = uvs;
	}

	public static Vector3 Vabs(Vector3 a) {
		return new Vector3(Mathf.Abs(a.x), Mathf.Abs(a.y), Mathf.Abs(a.z));
	}
}
