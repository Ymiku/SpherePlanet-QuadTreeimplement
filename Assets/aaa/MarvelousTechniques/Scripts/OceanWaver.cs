//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kirnu
{	
	class FloatingObject{
		public List<int> vertices = new List<int>();	
		public Transform t;
	}

	[ExecuteInEditMode]
	public class OceanWaver : MonoBehaviour
	{
		public List<Transform> floatingObjects = new List<Transform> ();
		public float waveHeight = 0.5f;
		public float waveSpeed = 1.0f;
		public float yPower = 0.1f;
		public float xPower = 0.1f;
		public float zPower = 0.1f;
		private Vector3[] vertices;
		private Vector3[] normals;
		private Color[] speeds;
		private Vector3[] newVertices;
		[HideInInspector]
		public Mesh mesh;
		Mesh newMesh;
		private List<FloatingObject> internalFloatingObjects = new List<FloatingObject>();

		void preCalculateFloatingObjects(){
			foreach (Transform t in floatingObjects) {
				FloatingObject fo=new FloatingObject();
				fo.t = t;
				for (int i=0; i<vertices.Length; i++) {
					if(Vector3.Distance(vertices[i],t.position)<=1.1f){
						fo.vertices.Add(i);
					}
				}
				internalFloatingObjects.Add(fo);
			}
		}

		void calculateFloatingObjects(){
			foreach (FloatingObject fo in internalFloatingObjects) {
				if(fo.vertices.Count == 0){
					continue;
				}
				float ypos = 0;
				Vector3 n = Vector3.zero;
				for(int i=0; i < fo.vertices.Count;i++){
					ypos += newVertices[fo.vertices[i]].y;
					n += normals[fo.vertices[i]];
				}
				Vector3 p = fo.t.position;
				p.y = ypos/fo.vertices.Count;
				fo.t.position = p;
				fo.t.transform.up = n/fo.vertices.Count;
			}
		}

		void Start ()
		{
			if (mesh) {
				newMesh = (Mesh)Instantiate (mesh, Vector3.zero, Quaternion.identity);
				GetComponent<MeshFilter> ().mesh = newMesh;
				vertices = mesh.vertices;
				normals = mesh.normals;
				newVertices = new Vector3[vertices.Length];
				speeds = mesh.colors;
			}
			preCalculateFloatingObjects ();
		}
	
		void Update ()
		{
			if (newMesh) {
				for (int i=0; i<newVertices.Length; i++) {
					Vector3 vertex = vertices [i];
					float xz = ((vertex.x + 0.5f*vertex.z)* xPower + vertex.z * zPower);
					float y = speeds [i].r * yPower;
					vertex.y += Mathf.Sin ((Time.time * waveSpeed) + y + xz) * (waveHeight);
					newVertices [i] = vertex;
				}
				newMesh.vertices = newVertices;
				newMesh.RecalculateNormals ();
				normals = newMesh.normals;
				calculateFloatingObjects();
			}
		}
	}
}
