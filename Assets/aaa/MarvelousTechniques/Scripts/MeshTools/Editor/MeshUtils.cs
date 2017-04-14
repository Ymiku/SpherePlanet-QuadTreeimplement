//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Kirnu
{
	public class MeshesByMaterial
	{
		public List<CustomMesh> meshes = new List<CustomMesh> ();

		public void addMesh (CustomMesh m)
		{
			meshes.Add (m);
		}
	};

	public class MeshUtils
	{
		public static Vector3 MAX_ROUND_ERROR = new Vector3 (0.001f, 0.001f, 0.001f);

		public static Bounds createBounds (List<Vector3> positions)
		{
			List<Vector3> newPositions = new List<Vector3> (positions);

			Vector2 xbounds = new Vector3 (newPositions [0].x, newPositions [0].x);
			Vector2 ybounds = new Vector3 (newPositions [0].y, newPositions [0].y);
			Vector2 zbounds = new Vector3 (newPositions [0].z, newPositions [0].z);
			
			for (int i=1; i<newPositions.Count; i++) {
				Vector3 p = newPositions [i];
				if (p.x < xbounds.x) {
					xbounds.x = p.x;
				}
				if (p.x > xbounds.y) {
					xbounds.y = p.x;
				}
				
				if (p.y < ybounds.x) {
					ybounds.x = p.y;
				}
				if (p.y > ybounds.y) {
					ybounds.y = p.y;
				}
				
				if (p.z < zbounds.x) {
					zbounds.x = p.z;
				}
				if (p.z > zbounds.y) {
					zbounds.y = p.z;
				}
			}
			Vector3 min = new Vector3 (xbounds.x, ybounds.x, zbounds.x);
			Vector3 max = new Vector3 (xbounds.y, ybounds.y, zbounds.y);

			Vector3 center = (max + min) * 0.5f;
			Vector3 size = max - min;
			Bounds bounds = new Bounds (center, size);
			
			
			return bounds;
		}

		public static bool hasUniqueVertices (Mesh mesh)
		{
			return mesh.triangles.Length == mesh.vertices.Length;
		}

		private static CustomMesh buildTriangles (Mesh mesh, string name, Transform transform, bool keep, bool noCompare, int materialIndex)
		{
			CustomMesh customMesh = new CustomMesh ();
			customMesh.mesh = mesh;
			customMesh.keep = keep;
			customMesh.noCompare = noCompare;
			customMesh.name = name;
			customMesh.materialIndex = materialIndex;

			for (int triangle = 0; triangle < mesh.triangles.Length/3; triangle++) {
				int[] vertices = new int[3];
				Vector3[] positions = new Vector3[3];
				Color[] colors = new Color[3];
				Vector2[] uvs = new Vector2[3];
				Vector3[] normals = new Vector3[3];
				
				for (int i=0; i<3; i++) {
					int vertexIndex = mesh.triangles [triangle * 3 + i];
					
					if (transform != null) {
						positions [i] = transform.TransformPoint (mesh.vertices [vertexIndex]);
					} else {
						positions [i] = mesh.vertices [vertexIndex];
					}
					if (mesh.colors.Length > vertexIndex) {
						colors [i] = mesh.colors [vertexIndex];
					}
					if (transform != null) {
						normals[i] = transform.TransformDirection (mesh.normals [vertexIndex]);
					} else {
						normals[i] = mesh.normals [vertexIndex];
					}
					if(mesh.uv.Length > 0){
						uvs [i] = mesh.uv [vertexIndex];
					}
					vertices [i] = vertexIndex;
					
				}
				
				Triangle tr = new Triangle (positions, colors, uvs, normals);
				customMesh.addTriangle (tr);
			}

			return customMesh;
		}

		private static void buildQuadsFromTriangles (List<CustomMesh> meshes)
		{
			for (int mi=0; mi<meshes.Count; mi++) {
				CustomMesh customMesh = meshes [mi];

				List<Triangle> triangles = new List<Triangle>(customMesh.getTriangles ());
				customMesh.clear();

				for (int t=0; t<triangles.Count; t++) {
					Triangle t1 = triangles [t];
					if(t1.removed){
						continue;
					}

					if ((customMesh.noCompare || (Mathf.Abs (t1.avgNormal.y)) < 1.0f - MeshUtils.MAX_ROUND_ERROR.x && Mathf.Abs (t1.avgNormal.y) > MeshUtils.MAX_ROUND_ERROR.x)) {
						t1.addedToQuad = true;
						customMesh.addTriangle (t1);
						continue;
					}
					if (!t1.addedToQuad) {
						Triangle t2 = findQuadTriangle (t1, triangles);
						if (t2 != null) {
							t1.addedToQuad = true;
							Quad quad = new Quad (t1, t2);
						
							List<Vector3> positions = new List<Vector3> (quad.positions);
							List<Vector2> uvs = new List<Vector2> (quad.uvs);
						
							// Rotate object so the points are always in XY-space
							Quaternion rot = Quaternion.FromToRotation (quad.avgNormal, new Vector3 (0, 0, -1));

							// Move center to origo
							for (int i=0; i<positions.Count; i++) {
								positions [i] = rot * (positions [i] - quad.bounds.center);
							}

							Bounds quadBounds = createBounds (positions);

							// After this method quad points are in this order:
							// 
							//	0	1
							//
							//	3	2
							Quad.arrangePositions (ref positions, ref uvs, quadBounds);
						
							float x1length = Vector3.Distance (positions [1], positions [0]) - MeshUtils.MAX_ROUND_ERROR.x;
							float x2length = Vector3.Distance (positions [2], positions [3]) - MeshUtils.MAX_ROUND_ERROR.x;
						
							float y1length = Vector3.Distance (positions [0], positions [3]) - MeshUtils.MAX_ROUND_ERROR.x;
							float y2length = Vector3.Distance (positions [1], positions [2]) - MeshUtils.MAX_ROUND_ERROR.x;
						
							bool divide = false;//!customMesh.keep;
							float xdiff = Mathf.Abs (x1length - x2length);
							float ydiff = Mathf.Abs (y1length - y2length);
						
							if (divide && (x1length > 0 && y1length > 0 && x2length > 0 && y2length > 0 && xdiff < 0.001f && ydiff < 0.001f && (x1length > 1 || y1length > 1))) {
								Color[] colors = new Color[3];
							
								int ydivisions = (int)Mathf.Ceil (y1length);
								int xdivisions = (int)Mathf.Ceil (x1length);
							
								for (int y=0; y<ydivisions; y++) {
									Vector3 currentStartPoint = positions [0] + Vector3.Normalize (positions [3] - positions [0]) * y;
								
									for (int x=0; x<xdivisions; x++) {
										Vector3 point1 = currentStartPoint + Vector3.Normalize (positions [1] - positions [0]);
										Vector3 point2 = point1 + Vector3.Normalize (positions [2] - positions [1]);
										Vector3 point3 = currentStartPoint + Vector3.Normalize (positions [3] - positions [0]);
									
										// Check boundaries
										if (point1.x > positions [1].x) {
											point1.x = positions [1].x;
										}
										if (point1.y < positions [2].y) {
											point1.y = positions [1].y;
										}
										if (point2.x > positions [2].x) {
											point2.x = positions [2].x;
										}
										if (point2.y < positions [2].y) {
											point2.y = positions [2].y;
										}
										if (point3.y < positions [3].y) {
											point3.y = positions [3].y;
										}
										// Create triangles
										List<Vector3> trPositions = new List<Vector3> ();
									
										trPositions.Add (currentStartPoint);
										trPositions.Add (point1);
										trPositions.Add (point3);
									
										List<Vector3> tr2Positions = new List<Vector3> ();
									
										tr2Positions.Add (point1);
										tr2Positions.Add (point2);
										tr2Positions.Add (point3);
									
										// Calculate UVs
										List<Vector2> trUVs = new List<Vector2> ();
										Vector2 uv0 = Quad.calculateUVForPoint (trPositions [0], positions, uvs);
										Vector2 uv1 = Quad.calculateUVForPoint (trPositions [1], positions, uvs);
										Vector2 uv2 = Quad.calculateUVForPoint (tr2Positions [1], positions, uvs);
										Vector2 uv3 = Quad.calculateUVForPoint (trPositions [2], positions, uvs);
									
										trUVs.Add (uv0);
										trUVs.Add (uv1);
										trUVs.Add (uv3);
									
										List<Vector2> tr2UVs = new List<Vector2> ();
										tr2UVs.Add (uv1);
										tr2UVs.Add (uv2);
										tr2UVs.Add (uv3);
									
										// Revert the previous movement
										for (int i=0; i<trPositions.Count; i++) {
											// Invert XY-space changes 
											trPositions [i] = Quaternion.Inverse (rot) * trPositions [i];
											trPositions [i] += quad.bounds.center;
										
											tr2Positions [i] = Quaternion.Inverse (rot) * tr2Positions [i];
											tr2Positions [i] += quad.bounds.center;
										}
									
										// Create new Quad
										List<Vector3> nnormals = new List<Vector3> ();
										for (int i=0; i<3; i++) {
											nnormals.Add (quad.avgNormal);
										} 
									
										Triangle tr1 = new Triangle (trPositions.ToArray (), colors, trUVs.ToArray (), nnormals.ToArray ());
										Triangle tr2 = new Triangle (tr2Positions.ToArray (), colors, tr2UVs.ToArray (), nnormals.ToArray ());
									
										Quad newQuad = new Quad (tr1, tr2);
										customMesh.addQuad (newQuad);
									
										currentStartPoint = point1;
									}
								}
							} else {
								customMesh.addQuad (quad);
							}
						} else {
							t1.addedToQuad = true;
							customMesh.addTriangle (t1);
						}
					}
				}
			}
		}

		private static Triangle findQuadTriangle (Triangle t, List<Triangle> triangles)
		{
			for (int i=0; i<triangles.Count; i++) {
				Triangle t2 = triangles [i];
				if (t2 != t && t2.hasSameSharedVertices (t) && !t2.addedToQuad && !t2.removed) {
					t2.addedToQuad = true;
					return t2;
				}
			}
			return null;
		}

		public static bool combineMeshes (GameObject parent, Vector3 parentPosition, Quaternion parentRotation)
		{
			//bool showCompileReady = true;
			//double timeStart = EditorApplication.timeSinceStartup;
			List<CustomMesh> meshes = new List<CustomMesh> ();
			List<MeshesByMaterial> meshesByMaterial = new List<MeshesByMaterial> ();

			int numChildren = parent.transform.childCount;
			int childIdx = 0;
			List<Material> materials = new List<Material> ();

			Bounds gridBounds = new Bounds ();
			bool foundBounds = false;

			//Build custom meshes
			foreach (Transform child in parent.transform) {
				// Get bounds
				Renderer[] renderers = child.GetComponentsInChildren<Renderer> ();
				List<Renderer> rds = new List<Renderer> (renderers);

				if (rds.Count == 0) {
					continue;
				}
				for (int i=0; i<rds.Count; i++) {
					Renderer r = rds [i];
					Material material = r.sharedMaterial;
					if (material) {
						if (!materials.Contains (material)) {
							materials.Add (material);
						}
					}

					if (!foundBounds) {
						foundBounds = true;
						gridBounds = new Bounds (r.bounds.center, r.bounds.size);
					} else {
						gridBounds.Encapsulate (r.bounds);
					}
				}

				MeshFilter[] ccmfs = child.GetComponentsInChildren<MeshFilter> ();
				List<MeshFilter> mfs = new List<MeshFilter> (ccmfs);

				for (int i=0; i<mfs.Count; i++) {
					MeshFilter mf = mfs [i];
					Renderer r = mf.gameObject.GetComponent<Renderer> ();
					if(!r){
						continue;
					}
					Material material = r.sharedMaterial;
					if (!material || !materials.Contains (material)) {
						continue;
					}

					int materialIndex = materials.IndexOf (material);

					bool cancel = false;

					Mesh mesh = mf.sharedMesh;
					string name = child.name;
					if (mesh == null) {
						continue;
					}

					float percent = ((float)childIdx) / ((float)numChildren);
					cancel = EditorUtility.DisplayCancelableProgressBar ("Creating custom meshes...", "(" + childIdx + "/" + numChildren + ")  " + name, percent);
					if (cancel) {
						return true;
					}

					// Keeps this mesh but compares to others. No subdivision is performed.
					bool keep = (child.tag == "keep");
					// Keeps this mesh without comparing to others. No subdivision is performed.
					bool noCompare = (child.tag == "nocompare");

					CustomMesh cm = buildTriangles (mesh, name, mf.transform, keep, noCompare, materialIndex);
					meshes.Add (cm);

					if (meshesByMaterial.Count <= materialIndex) {
						MeshesByMaterial mbm = new MeshesByMaterial ();
						meshesByMaterial.Add (mbm);
					}
					
					meshesByMaterial [materialIndex].addMesh (cm);
				}
				childIdx ++;
			}

			// Create mesh grid
			int gridSize = (int)Mathf.Ceil (gridBounds.size.x);
			if (Mathf.Ceil (gridBounds.size.y) > gridSize) {
				gridSize = (int)Mathf.Ceil (gridBounds.size.y);
			}
			if (Mathf.Ceil (gridBounds.size.z) > gridSize) {
				gridSize = (int)Mathf.Ceil (gridBounds.size.z);
			}
			MeshGrid meshGrid = new MeshGrid (gridSize, gridBounds.center);

			foreach (CustomMesh m in meshes) {
				if (!m.noCompare) {
					meshGrid.addMesh (m);
				}
			}

			int triangleCount = 0;
			int vertexCount = 0;

			// Remove obsolete triangles
			for (int i=0; i<meshes.Count; i++) {
				CustomMesh m = meshes [i];
				if (!m.noCompare) {
					float percent = ((float)i) / ((float)meshes.Count);
					string name = m.name + "";
					bool cancel = false;

					cancel = EditorUtility.DisplayCancelableProgressBar ("Checking triangles...", "  (" + i + "/" + meshes.Count + ")  " + name, percent);

					if (cancel) {
						return true;
					}
					for (int j=0; j<m.gridItems.Count; j++) {
						m.gridItems [j].checkMesh (m);
					}
				}

				int tCount = 0;
				int vCount = 0;
				m.getVertexAndTriangleCount (ref vCount, ref tCount);
				//Debug.Log ("Mesh " + m.name + " has " + tCount / 3 + " triangles.");
				triangleCount += tCount;
				vertexCount += vCount;

				EditorApplication.Step ();
			}

			buildQuadsFromTriangles (meshes);

			triangleCount = 0;
			vertexCount = 0;

			for (int i=0; i<meshes.Count; i++) {
				CustomMesh m = meshes [i];
				if (!m.noCompare) {
					float percent = ((float)i) / ((float)meshes.Count);
					string name = m.name + "";
					bool cancel = false;
					
					cancel = EditorUtility.DisplayCancelableProgressBar ("Checking quads...", "  (" + i + "/" + meshes.Count + ")  " + name, percent);
					
					if (cancel) {
						return true;
					}
					
					for (int j=0; j<m.gridItems.Count; j++) {
						m.gridItems [j].checkMesh (m);
					}
				}
				int tCount = 0;
				int vCount = 0;
				m.getVertexAndTriangleCount (ref vCount, ref tCount);
				//Debug.Log ("Mesh " + m.name + " has " + tCount / 3 + " triangles.");
				triangleCount += tCount;
				vertexCount += vCount;
				
				EditorApplication.Step ();
			}

			EditorUtility.ClearProgressBar ();

			int currentMaterial = 0;
			foreach (MeshesByMaterial mbm in meshesByMaterial) {
				triangleCount = 0;
				vertexCount = 0;
				for (int i=0; i < mbm.meshes.Count; i++) {
					int vCount = 0;
					int tCount = 0;
					mbm.meshes [i].getVertexAndTriangleCount (ref vCount, ref tCount);
					triangleCount += tCount;
					vertexCount += vCount;
				}
				if(vertexCount == 0 || triangleCount == 0){
					continue;
				}
				// Combine all quads/triangles to one big mesh
				GameObject go = new GameObject ("_Combined_" + parent.name + "_material_" + materials [currentMaterial].name);
				go.transform.position = parentPosition;
				go.transform.rotation = parentRotation;
				go.transform.localScale = Vector3.one;
				if (parent.transform && parent.transform.parent && parent.transform.parent.gameObject) {
					go.transform.parent = parent.transform.parent;
				}
				GameObjectUtility.SetStaticEditorFlags (go, StaticEditorFlags.LightmapStatic);

				MeshRenderer renderer = go.AddComponent<MeshRenderer> ();
				if (materials [currentMaterial] != null) {
					renderer.material = materials [currentMaterial];
				}
				currentMaterial ++;
				MeshFilter newFilter = go.AddComponent<MeshFilter> ();
				Mesh newMesh = new Mesh ();

			
				Vector3[] newVertices = new Vector3[vertexCount];
				Vector2[] newUV = new Vector2[newVertices.Length];
				Vector3[] newNormals = new Vector3[newVertices.Length];
				int[] newTriangles = new int[triangleCount];
				
				int triangleIndex = 0;
				int vertexIndex = 0;
				for (int i=0; i<mbm.meshes.Count; i++) {
					mbm.meshes [i].getTrianglesAndVertices (ref newTriangles, ref newVertices, ref newUV, ref newNormals, ref triangleIndex, ref vertexIndex);
				}
				

				newMesh.vertices = newVertices;
				newMesh.normals = newNormals;
				newMesh.uv = newUV;
				newMesh.uv2 = newUV;
				newMesh.triangles = newTriangles;
				
				newMesh.Optimize ();
				newMesh.RecalculateBounds ();
				
				newFilter.mesh = newMesh;
				Unwrapping.GenerateSecondaryUVSet (newFilter.sharedMesh);
			}

			//Debug.Log ("Combine meshes finished! Elapsed time: " + (EditorApplication.timeSinceStartup - timeStart));

			return false;
		}
	}
};
