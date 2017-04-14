//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

namespace Kirnu
{
	public class OceanCreator
	{
	
		static Mesh createPlaneMesh (int widthSegments, int lengthSegments, float width, float length)
		{
			Mesh m = new Mesh ();
			m.name = "OceanMesh";

			int hCount2 = widthSegments + 1;
			int vCount2 = lengthSegments + 1;
			int numTriangles = widthSegments * lengthSegments * 6;

			int numVertices = hCount2 * vCount2;
		
			Vector3[] vertices = new Vector3[numVertices];
			Color[] speeds = new Color[numVertices];
			Vector2[] uvs = new Vector2[numVertices];
			int[] triangles = new int[numTriangles];
		
			int index = 0;
			float uvFactorX = 1.0f / widthSegments;
			float uvFactorY = 1.0f / lengthSegments;
			float scaleX = width / widthSegments;
			float scaleY = length / lengthSegments;


			for (float y = 0.0f; y < vCount2; y++) {
				for (float x = 0.0f; x < hCount2; x++) {
					float xpos = x * scaleX - width / 2f;
					float zpos = y * scaleY - length / 2f ;
					float ypos=0f;
					speeds[index] = new Color(Random.Range(0.0F, 1.0F),0f,0f);
					vertices [index] = new Vector3 (xpos, ypos, zpos);
					uvs [index++] = new Vector2 (x * uvFactorX, y * uvFactorY);
				}
			}
		
			index = 0;
			for (int y = 0; y < lengthSegments; y++) {
				for (int x = 0; x < widthSegments; x++) {
					triangles [index + 2] = (y * hCount2) + x;
					triangles [index + 0] = ((y + 1) * hCount2) + x;
					triangles [index + 1] = (y * hCount2) + x + 1;
				
					triangles [index + 5] = ((y + 1) * hCount2) + x;
					triangles [index + 3] = ((y + 1) * hCount2) + x + 1;
					triangles [index + 4] = (y * hCount2) + x + 1;
					index += 6;
				}
			}
		
			m.vertices = vertices;
			m.uv = uvs;
			m.colors = speeds;
			m.triangles = triangles;
			m.RecalculateNormals ();
		
			return m;
		}

		static public Mesh createOcean ()
		{
			// The hard coded size
			Mesh mesh = createPlaneMesh (30, 30, 30, 30);
			Vector3[] newVertices = new Vector3[mesh.triangles.Length];
			Color[] newColors = new Color[mesh.triangles.Length];
			Vector2[] newUV = new Vector2[newVertices.Length];
			Vector3[] newNormals = new Vector3[newVertices.Length];
			int[] newTriangles = new int[mesh.triangles.Length];

			// Rebuild mesh so that every triangle has unique vertices
			for (int triangle = 0; triangle < mesh.triangles.Length; triangle++) {
				int i = triangle;
				newVertices [i] = mesh.vertices [mesh.triangles [i]];
				newUV [i] = mesh.uv [mesh.triangles [i]];

				newColors[i]=mesh.colors [mesh.triangles [i]];
				newNormals [i] = mesh.normals [mesh.triangles [i]];
				newTriangles [i] = i;
			}

			mesh.vertices = newVertices;
			mesh.colors = newColors;

			mesh.normals = newNormals;
			mesh.triangles = newTriangles;

			mesh.uv = newUV;
			mesh.uv2 = newUV;
		
			mesh.RecalculateBounds ();
			mesh.RecalculateNormals ();

			return mesh;
		}
	};
}
