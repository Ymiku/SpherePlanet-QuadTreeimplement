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
	public class Primitive{
		public List<Vector3> positions = new List<Vector3>();
		public Bounds bounds;
		public Bounds extendedBounds;
		public List<Vector3> normals = new List<Vector3>();
		public Vector3 avgNormal = Vector3.zero;
		public bool removed = false;
		public float area = 0;
		protected Vector3 centerPos = Vector3.zero;
		Plane plane;

		protected void init(){
			for (int i=0; i<normals.Count; i++) {
				avgNormal+=normals[i];
			}
			avgNormal /= normals.Count;
			plane = new Plane ();
			plane.Set3Points (positions[0],positions[1],positions[2]);
			bounds = MeshUtils.createBounds (positions/*, avgNormal, centerPos, true*/);
			extendedBounds = new Bounds (bounds.center, bounds.size + MeshUtils.MAX_ROUND_ERROR);
		}

		public bool intersectWithPrimitive (Primitive q)
		{
			bool intersects = extendedBounds.Intersects (q.extendedBounds);
			return intersects;
		}
		
		public bool containsBounds(Bounds target){
			return extendedBounds.Contains(target.min) && extendedBounds.Contains(target.max);
		}

		public int getPointIndexWithMaxError (Vector3 point, float maxError)
		{
			for (int i=0; i<positions.Count; i++) {
				if (Vector3.Distance (positions [i], point) <= maxError) {
					return i;
				}
			}
			return -1;
		}

		bool distanceFromPrimitivePlane(Primitive p){
			float distance = 0;
			for (int i=0; i < p.positions.Count; i++) {
				distance += plane.GetDistanceToPoint(p.positions[i]);
			}
			distance /= p.positions.Count;
			return distance <= MeshUtils.MAX_ROUND_ERROR.x;
		}

		// Remove both if their dot product is -1 (=opposite) or 1 and they have all the same points.
		// If points are not same thay are different sized quads. In this case remove the smaller quad.
		public void calculatePrimitiveRemoval (Primitive p,ref bool deleteSelf,ref bool deleteOther)
		{
			float dot = Vector3.Dot (avgNormal, p.avgNormal);
			float diff = Mathf.Abs (dot) + MeshUtils.MAX_ROUND_ERROR.x;

			if (!distanceFromPrimitivePlane(p) || !intersectWithPrimitive (p) || diff<1.0f) {
				return;
			}
			int samePoints = 0;
			for (int i=0; i<p.positions.Count; i++) {
				if (getPointIndexWithMaxError (p.positions [i], MeshUtils.MAX_ROUND_ERROR.x) >= 0) {
					samePoints++;
				}
			}
			
			if (samePoints == p.positions.Count) {
				if(samePoints == positions.Count){
					deleteSelf = true;
				}
				deleteOther = true;
				return;
			}
			if (containsBounds (p.bounds) && area > p.area) {
				deleteOther = true;
				return;
			} else if (p.containsBounds (bounds) && p.area > area) {
				deleteSelf = true;
			}
		}
	};

	public class Triangle : Primitive
	{
		public Vector2[] uvs;
		public Color[] colors;

		public bool addedToQuad = false;
		public bool isLeft;

		public Triangle (Vector3[] positions, Color[] colors, Vector2[] uvs, Vector3[] normals)
		{
			for (int i=0; i<3; i++) {
				this.positions.Add(positions[i]);
			}

			this.isLeft = false;
			this.normals = new List<Vector3>(normals);
			this.colors = colors;
			this.uvs = uvs;
			
			Vector3 A = (positions [0]);
			Vector3 B = (positions [1]);
			Vector3 C = (positions [2]);
				
			centerPos = (A+B+C)/3f;	
			float a = Vector3.Distance(positions [1],positions [0]);
			float b = Vector3.Distance(positions [2],positions [1]);
			float c = Vector3.Distance(positions [0],positions [2]);
			
			area = Mathf.Sqrt(Mathf.Sqrt((a+b+c)*(-a+b+c)*(a-b+c)*(a+b-c)/16));
			base.init ();

		}

		public void setUVs (Vector2[] uvs)
		{
			this.uvs = uvs;
		}
	
		public Vector3 getPosition (int idx)
		{
			return positions [idx];
		}
	
		public bool hasSameSharedVertices (Triangle t)
		{
			if (t.avgNormal != avgNormal) {
				return false;
			}
			int found = 0;
			for (int i=0; i<positions.Count; i++) {
				for (int j=0; j<t.positions.Count; j++) {
					if (positions [i] == t.positions [j]) {
						found++;
					}
				}
			}
			return found == 2;
		}
	};

	public class Quad : Primitive
	{
		// Näitä on aina vain 2
		public Triangle[] triangles;
		public int row;
		public List<int> vertexIndices = new List<int> ();
		public List<Vector2> uvs = new List<Vector2> ();

		public Quad (Triangle triangle1, Triangle triangle2)
		{
			row = 0;
			this.triangles = new Triangle[2];
			triangles [0] = triangle1;
			triangles [0].isLeft = true;
		
			triangles [1] = triangle2;

			for (int t=0; t<2; t++) {
				area += triangles[t].area;
				List<Vector3> tpositions = triangles [t].positions;
				for (int i=0; i<tpositions.Count; i++) {
					int pointIndex = getPointIndex (tpositions [i]);
					if (pointIndex == -1) {
						vertexIndices.Add (positions.Count);
						positions.Add (tpositions [i]);
						uvs.Add (triangles [t].uvs [i]);
						normals.Add(triangles [t].normals [i]);
					} else {
						vertexIndices.Add (pointIndex);
					}
				}
			}

			for (int i=0; i<4; i++) {
				centerPos += positions [i];
			}
			
			centerPos /= 4f;
			base.init ();
		}
	
		// Get UV from corner points
		public Vector2 getUVFromPosition (Vector3 pos)
		{
			for (int t=0; t<2; t++) {
				List<Vector3> tpositions = triangles [t].positions;
				for (int i=0; i<tpositions.Count; i++) {
					if (tpositions [i] == pos) {
						return triangles [t].uvs [i];
					}
				}
			}
			//Debug.Log ("Did not found UV for point!");
			return new Vector2 ();
		}
	
		public static bool isPointInsideTriangle (Vector3 point, List<Vector3> cornerPoints, int triangle)
		{
			Vector3 A = triangle == 0 ? cornerPoints [0] : cornerPoints [1];
			Vector3 B = triangle == 0 ? cornerPoints [1] : cornerPoints [2];
			Vector3 C = triangle == 0 ? cornerPoints [3] : cornerPoints [3];
		
			Vector3 v0 = C - A;
			Vector3 v1 = B - A;
			Vector3 v2 = point - A;
		
			// Compute dot products
			float dot00 = Vector3.Dot (v0, v0);
			float dot01 = Vector3.Dot (v0, v1);
			float dot02 = Vector3.Dot (v0, v2);
			float dot11 = Vector3.Dot (v1, v1);
			float dot12 = Vector3.Dot (v1, v2);
		
			// Compute barycentric coordinates
			float invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
			float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
			float v = (dot00 * dot12 - dot01 * dot02) * invDenom;
		
			// Check if point is in triangle
			return (u >= 0) && (v >= 0) && (u + v <= 1);
		}
	
		public static Vector2 getPointUV (Vector3 point, List<Vector3> cornerPoints, List<Vector2> cornerUVs, int triangle)
		{
			Vector3 A = triangle == 0 ? cornerPoints [0] : cornerPoints [1];
			Vector3 B = triangle == 0 ? cornerPoints [1] : cornerPoints [2];
			Vector3 C = triangle == 0 ? cornerPoints [3] : cornerPoints [3];
			// calculate vectors from point f to vertices p1, p2 and p3:
			var f1 = A - point;
			var f2 = B - point;
			var f3 = C - point;
			// calculate the areas and factors (order of parameters doesn't matter):
			float a = Vector3.Cross (A - B, A - C).magnitude; // main triangle area a
			float a1 = Vector3.Cross (f2, f3).magnitude / a; // p1's triangle area / a
			float a2 = Vector3.Cross (f3, f1).magnitude / a; // p2's triangle area / a 
			float a3 = Vector3.Cross (f1, f2).magnitude / a; // p3's triangle area / a
		
			// find the uv corresponding to point f (uv1/uv2/uv3 are associated to p1/p2/p3):
			Vector2 uv0 = triangle == 0 ? cornerUVs [0] : cornerUVs [1];
			Vector2 uv1 = triangle == 0 ? cornerUVs [1] : cornerUVs [2];
			Vector2 uv2 = triangle == 0 ? cornerUVs [3] : cornerUVs [3];
		
			Vector2 uv = uv0 * a1 + uv1 * a2 + uv2 * a3;
		
			return uv;
		}
	
		// Get UV from inside of this quad
		public static Vector2 calculateUVForPoint (Vector3 point, List<Vector3> cornerPoints, List<Vector2> cornerUVs)
		{
			if (isPointInsideTriangle (point, cornerPoints, 0)) {
				return getPointUV (point, cornerPoints, cornerUVs, 0);
			}
			return getPointUV (point, cornerPoints, cornerUVs, 1);
		}

		// Order positions
		//	0	1
		//
		//	3	2
		public static void arrangePositions (ref List<Vector3> positions, ref List<Vector2> uvs, Bounds bounds)
		{
		
			if (positions != null && positions.Count == 4) {
				List<Vector3> newPositions = new List<Vector3> ();
				List<Vector2> newUVs = new List<Vector2> ();
			
				for (int point=0; point<4; point++) {
					// Calculate smallest distance to the corners of the bounding box
					// This algorithm might not work in every quad type.
					float smallestDistance = float.MaxValue;
					int closest = -1;
					Vector3 cornerPositions = bounds.center;
				
					switch (point) {
					case 0:
						cornerPositions.x -= bounds.extents.x;
						cornerPositions.y += bounds.extents.y;
						break;
					case 1:
						cornerPositions.x += bounds.extents.x;
						cornerPositions.y += bounds.extents.y;
						break;
					case 2:
						cornerPositions.x += bounds.extents.x;
						cornerPositions.y -= bounds.extents.y;
						break;
					case 3:
						cornerPositions.x -= bounds.extents.x;
						cornerPositions.y -= bounds.extents.y;
						break;
					}
				
					for (int i=0; i<positions.Count; i++) {
						float distance = Vector3.Distance (positions [i], cornerPositions);
						if (distance < smallestDistance) {
							smallestDistance = distance;
							closest = i;
						}
					}
					newPositions.Add (positions [closest]);
					newUVs.Add (uvs [closest]);
				}
				positions = newPositions;
				uvs = newUVs;
			}
		}
	
		public void getTrianglesAndVertices (ref int[] triangles, ref Vector3[] vertices, ref Vector2[] uv,ref Vector3[] normals, ref int triangleIndex, ref int vertexIndex)
		{
			for (int i=0; i<vertexIndices.Count; i++) {
				triangles [triangleIndex + i] = vertexIndices [i] + vertexIndex;
			}
			triangleIndex += vertexIndices.Count;
		
			for (int i=0; i<positions.Count; i++) {
				vertices [vertexIndex + i] = positions [i];
				uv [vertexIndex + i] = uvs [i];
				normals [vertexIndex + i] = this.normals[i];
			}
			vertexIndex += positions.Count;
		}
	
		public int getPointIndex (Vector3 point)
		{
			for (int i=0; i<positions.Count; i++) {
				if (positions [i] == point) {
					return i;
				}
			}
			return -1;
		}
	};
}
