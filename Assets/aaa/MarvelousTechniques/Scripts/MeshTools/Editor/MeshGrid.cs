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
	public class MeshGridItem
	{
		public Bounds bounds;
		public Vector3 gridPos;
		public List<CustomMesh> meshes = new List<CustomMesh> ();
	
		public MeshGridItem (float size, Vector3 center, Vector3 gridPos)
		{
			bounds = new Bounds (center, new Vector3 (size, size, size));
			this.gridPos = gridPos;
		}

		public void checkMesh (CustomMesh m)
		{
			for (int i=0; i<meshes.Count; i++) {
				if (m.keep && meshes [i].keep) {
					continue;
				}
				CustomMesh m2 = meshes [i];

				if (m2 != m) {
					m.checkMesh (m2);
				}
			}
		}
	
		bool meshInsideGrid (CustomMesh m)
		{
			if (m.bounds.Intersects (bounds)) {
				return true;
			}
			return false;
		}
	
		public bool addMesh (CustomMesh m)
		{
			meshes.Add (m);
			m.gridItems.Add (this);
			return true;
		}
	};

	public class MeshGrid
	{
		public Bounds bounds;
		// sizexsizexsize
		MeshGridItem[,,] gridItems;
	
		public MeshGrid (int size, Vector3 center)
		{
			gridItems = new MeshGridItem[size,size,size];
			bounds = new Bounds (center, new Vector3 (size, size, size));	
			float startXPos = bounds.center.x + bounds.extents.x - 0.5f;
			float startYPos = bounds.center.y + bounds.extents.y - 0.5f;
			float startZPos = bounds.center.z + bounds.extents.z - 0.5f;
		
			for (int x=0; x<size; x++) {
				for (int y=0; y<size; y++) {
					for (int z=0; z<size; z++) {
						Vector3 newCenter = new Vector3 (startXPos - x, startYPos - y, startZPos - z);
						MeshGridItem item = new MeshGridItem (1.0f, newCenter, new Vector3 (x, y, z));
						gridItems[x,y,z]=item;
					}
				}
			}
		}
	
		bool getGridPositionFromPosition (Vector3 position, ref Vector3 gridPosition)
		{
			float xPos = Mathf.Floor (bounds.center.x + bounds.extents.x - position.x);
			float yPos = Mathf.Floor (bounds.center.y + bounds.extents.y - position.y);
			float zPos = Mathf.Floor (bounds.center.z + bounds.extents.z - position.z);
		
			gridPosition = new Vector3 (xPos, yPos, zPos);
			return true;
		}
	
		// Add Mesh reference to gridItem it belongs
		// Mesh also gets reference to all gridItems it belongs
		public bool addMesh (CustomMesh m)
		{
			// Expand bounding box by one
			Bounds meshBounds = m.bounds;
			meshBounds.size = new Vector3 (meshBounds.size.x + 2, meshBounds.size.y + 2, meshBounds.size.z + 2);
		
			bool added = false;
			// Get starting grid point
			Vector3 gridStartPos = meshBounds.max;
			getGridPositionFromPosition (gridStartPos, ref gridStartPos);
			for (int x=0; x<meshBounds.size.x; x++) {
				for (int y=0; y<meshBounds.size.y; y++) {
					for (int z=0; z<meshBounds.size.z; z++) {
						// Skip the corners
						if((x==0&&z==0)||(x==meshBounds.size.x-1&&z==0)||(x==0&&z==meshBounds.size.z-1)||
						   (x==meshBounds.size.x-1&&z==meshBounds.size.z-1)){
							continue;
						}
						if((z==0&&y==0)||(z==meshBounds.size.z-1&&y==0)||(z==0&&y==meshBounds.size.y-1)||(z==meshBounds.size.z-1&&y==meshBounds.size.y-1)){
							continue;
						}
						if((x==0&&y==0)||(x==meshBounds.size.x-1&&y==0)||(x==0&&y==meshBounds.size.y-1)||(x==meshBounds.size.x-1&&y==meshBounds.size.y-1)){
							continue;
						}

						int xPos = (int)gridStartPos.x + x;
						int yPos = (int)gridStartPos.y + y;
						int zPos = (int)gridStartPos.z + z;
					
						if ((xPos < 0 || xPos > bounds.size.x-1) || (yPos < 0 || yPos > bounds.size.y-1) || (zPos < 0 || zPos > bounds.size.z-1)) {
							// Ignore the point
						} else {
							gridItems [xPos,yPos,zPos].addMesh(m);
							added = true;
						}
					}
				}
			}
		
			return added;
		}
	
		bool meshInsideGrid (CustomMesh m)
		{
			if (m.bounds.Intersects (bounds)) {
				return true;
			}
			return false;
		}
	};

	public class CustomMesh
	{
		protected List<Quad> quads = new List<Quad> ();
		protected List<Triangle> triangles = new List<Triangle> ();
		public List<Primitive> primitives = new List<Primitive> ();
		public Mesh mesh;
		public bool keep = false;
		public bool noCompare = false;
		public List<MeshGridItem> gridItems = new List<MeshGridItem> ();
		public string name;
		public List<CustomMesh> checkedMeshes = new List<CustomMesh>();
		public Bounds bounds;
		public int materialIndex = -1;

		public CustomMesh ()
		{
		}

		public void clear(){
			quads.Clear ();
			triangles.Clear ();
			primitives.Clear ();
			checkedMeshes.Clear ();
		}

		public List<Triangle> getTriangles(){
			return triangles;
		}

		int getQuadCount ()
		{
			int qcount = 0;
			for (int i=0; i<quads.Count; i++) {
				if (!quads [i].removed) {
					qcount++;
				}
			}
			return qcount;
		}

		int getTriangleCount ()
		{
			int tcount = 0;
			for (int i=0; i<triangles.Count; i++) {
				if (!triangles [i].removed) {
					tcount++;
				}
			}
			return tcount;
		}
	
		public void getVertexAndTriangleCount (ref int vertexCount, ref int triangleCount)
		{
			int quadCount = getQuadCount ();
			triangleCount = quadCount * 2 * 3;
			vertexCount = quadCount * 4;
		
			int tCount = getTriangleCount ()*3;
			triangleCount += tCount;
			vertexCount += tCount;
		}

		public void addQuad (Quad q)
		{
			if (primitives.Count == 0) {
				bounds = q.bounds;
			} else {
				bounds.Encapsulate(q.bounds);
			}
			primitives.Add (q);
			quads.Add (q);
		}

		public void addTriangle (Triangle t)
		{
			if (primitives.Count == 0) {
				bounds = t.bounds;
			} else {
				bounds.Encapsulate(t.bounds);
			}
			primitives.Add (t);
			triangles.Add (t);
		}
	
		public void getTrianglesAndVertices (ref int[] trianglesx, ref Vector3[] vertices, ref Vector2[] uv, ref Vector3[] normals, ref int triangleIndex, ref int vertexIndex)
		{
		
			for (int i=0; i<quads.Count; i++) {
				if (!quads [i].removed) {
					Quad q=quads [i];
					q.getTrianglesAndVertices (ref trianglesx, ref vertices, ref uv, ref normals,ref triangleIndex, ref vertexIndex);
				}
			}

			List<Vector3> ttriangles = new List<Vector3> ();
			List<Vector3> tnormals = new List<Vector3> ();
			List<int> vvertexIndices = new List<int> ();
			List<Vector2> uvs = new List<Vector2> ();
			for (int i=0; i<triangles.Count; i++) {
				Triangle t = triangles [i];
				if (!t.removed) {
					for (int j=0; j<t.positions.Count; j++) {
						vvertexIndices.Add (vvertexIndices.Count);
						ttriangles.Add (t.positions [j]);
						uvs.Add (t.uvs [j]);
						tnormals.Add(t.normals[j]);
					}
				}
			}
			for (int i=0; i<vvertexIndices.Count; i++) {
				trianglesx [triangleIndex + i] = vvertexIndices [i] + vertexIndex;
			}
			triangleIndex += vvertexIndices.Count;
		
			for (int i=0; i<ttriangles.Count; i++) {
				vertices [vertexIndex + i] = ttriangles [i];
				uv [vertexIndex + i] = uvs [i];
				normals [vertexIndex + i] = tnormals[i];
			}
			vertexIndex += ttriangles.Count;
		}

		public void checkMesh (CustomMesh m2)
		{	
			if (checkedMeshes.Contains (m2)) {
				return;
			}
			checkedMeshes.Add (m2);
			m2.checkedMeshes.Add (this);

			for (int j=0; j<primitives.Count; j++) {
				Primitive p1 = primitives [j];
				if (p1.removed) {
					continue;
				}

				for (int k=0; k<m2.primitives.Count; k++) {
					Primitive p2 = m2.primitives [k];
					
					if (p1 != p2 && !p2.removed) {
						bool removeSelf = false;
						bool removeOther = false;
						p1.calculatePrimitiveRemoval (p2, ref removeSelf, ref removeOther);
						p1.removed = (keep ? false : removeSelf || p1.removed);
						p2.removed = (m2.keep ? false : removeOther || p2.removed);
						if (removeSelf) {
							//Debug.Log ("*** Removed Primitive from self " + name + " other " + m2.name + "!");
						}
						if (removeOther) {
							//Debug.Log ("*** Removed Primitive from other " + m2.name + " self " + name + "!");
						}
					}
				}
			}
		}
	}
};

