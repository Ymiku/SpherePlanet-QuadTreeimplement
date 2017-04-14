using UnityEngine;
using System.Collections;
using QTPlanetUtility;
public class LCQTMesh : LCGameObject {
	public Mesh mesh;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	public MeshCollider meshCollider;
	Vector3[] verts;
	Vector3[] normals;
	int[] tris;
	Vector3[] vertsLowPoly;
	float[] height;
	Vector2[] uv;
	private float[] heightMap;
	private int[,] _vectorToPosTable;
	private int[,] _vectorToHeightMapTable;
	public static LCQTMesh CreatObject()
	{
		if(PoolManager.Instance.HasPoolObject(typeof(LCQTMesh)))
		{
			LCQTMesh mesh = PoolManager.Instance.TakePoolObject (typeof(LCQTMesh)) as LCQTMesh;
			return mesh;
		}
		else
		{
			GameObject temp = new GameObject();
			temp.hideFlags = HideFlags.HideInHierarchy;
			LCQTMesh qtmesh = temp.AddComponent<LCQTMesh> ();
			qtmesh.mesh = new Mesh ();
			qtmesh.meshFilter = temp.AddComponent<MeshFilter>();
			qtmesh.meshFilter.mesh = qtmesh.mesh;
			qtmesh.meshRenderer = temp.AddComponent<MeshRenderer>();
			qtmesh.meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			qtmesh.meshRenderer.receiveShadows = false;
			qtmesh.meshCollider = temp.AddComponent<MeshCollider>();
			qtmesh.meshCollider.sharedMesh = qtmesh.mesh;
			return qtmesh;
		}
	}
	public float GetY(float x)
	{
		return 0f;
	}
	public int GetVertsPos(int x,int z)
	{
		return _vectorToPosTable [x,z];
		//return x + z * (rowCount + 1);
	}
	public int GetMapPos(int x,int z)
	{
		return _vectorToHeightMapTable [x,z];
		//return x + z * (QTManager.Instance.activePlanet._width);
	}
	public override void Destroy ()
	{
		base.Destroy ();
		mesh.Clear ();
	}
	public float GetSmoothHeight(float x,float y)
	{
		int width = QTManager.Instance.activePlanet.heightMapWidth;
		int int_x = (int)x;
		int int_y = (int)y;
		float c1;
		float c2;
		//return heightMap[_vectorToHeightMapTable[int_x,int_y]];
		if (int_x >= width-1) {
			if (int_y >= width-1) {
				return heightMap[_vectorToHeightMapTable[int_x,int_y]];
			}
			c1 = heightMap[_vectorToHeightMapTable[int_x,int_y]];
			c2 = heightMap[_vectorToHeightMapTable[int_x,int_y+1]];
			return Mathf.Lerp (c1,c2,y-int_y);
		}
		else if(int_y >= width-1){
			return Mathf.Lerp (heightMap[_vectorToHeightMapTable[int_x,int_y]],heightMap[_vectorToHeightMapTable[int_x+1,int_y]],x-int_x);
		}
		c1 = Mathf.Lerp (heightMap[_vectorToHeightMapTable[int_x,int_y]],heightMap[_vectorToHeightMapTable[int_x+1,int_y]],x-int_x);
		c2 = Mathf.Lerp (heightMap[_vectorToHeightMapTable[int_x,int_y+1]],heightMap[_vectorToHeightMapTable[int_x+1,int_y+1]],x-int_x);
		return Mathf.Lerp (c1,c2,y-int_y);
	}
	public void CreatMesh(QTNode node,bool[] transformArray,int splitCount)
	{
		gameObject.SetActive (false);
		heightMap =  QTManager.Instance.activeTerrain.heightMap;
		_vectorToPosTable = QTManager.Instance.activePlanet.vectorToPosTable;
		_vectorToHeightMapTable = QTManager.Instance.activePlanet.vectorToHeightMapTable;
		mesh.Clear ();

		float offSet = node.length / splitCount;
		int lineCount = 0;

		if (verts == null || verts.Length != (splitCount + 1) * (splitCount + 1)) {
			verts = new Vector3[(splitCount + 1) * (splitCount + 1)];
			height = new float[verts.Length];
		}

		verts [0] = new Vector3 (node.center.x - node.length * 0.5f,node.center.y, node.center.z - node.length * 0.5f);
		int max = (splitCount + 1) * (splitCount + 1);

		for (int i = 1; i < max; i++) {
			if (lineCount < splitCount) {
				verts [i] = new Vector3 (verts [i - 1].x + offSet,node.center.y, verts [i - 1].z);

				lineCount++;
			} else {
				verts [i] = new Vector3 (verts[i-splitCount-1].x,node.center.y, verts [i-splitCount-1].z+offSet);
				lineCount = 0;
			}
		}
		float radius = QTManager.Instance.activePlanet.sphereRadius;
		Vector2 origin = node.fullGenerateOrigin*QTManager.Instance.activePlanet.mapScale;
		float hx = 0f;
		float hy = origin.y;
		lineCount = 0;
		offSet = node.fullGenerateOffset*QTManager.Instance.activePlanet.mapScale;
		float heightScale = QTManager.Instance.activePlanet.heightScale;
		for (int i = 0; i < max; i++) {
			hx = origin.x + offSet * lineCount;
			height [i] = GetSmoothHeight (hx, hy);
			verts [i] = MathExtra.FastNormalize (verts [i]) * radius*(1f+height[i]*heightScale);
			lineCount++;
			if (lineCount>=splitCount+1)
			{
				hy += offSet;
				lineCount = 0;
			}
		}
			
		if (splitCount == 1) {
			mesh.vertices = verts;
			mesh.triangles = new int[6]{0,2,1,2,3,1};
			mesh.RecalculateNormals ();
			return;
		}
		int reduceCount = 0;
		int pos = 0;
		for (int i = 0; i < transformArray.Length; i++) {
			if (transformArray [i] == true)
				reduceCount += (splitCount >> 1);
		}
		if (tris == null || tris.Length != splitCount * splitCount * 6) {
			tris = new int[splitCount * splitCount * 6];
			vertsLowPoly = new Vector3[tris.Length];
			normals = new Vector3[tris.Length];
			uv = new Vector2[tris.Length];


		} else {
			max = reduceCount * 3;
			pos = splitCount * splitCount * 6 - 1;
			for (int i = 0; i < max; i++) {
				tris [pos] = 0;
				pos--;
			}
		}
		//tris = new int[(splitCount*splitCount*2-reduceCount)*3];//Cause GC

		pos = 0;
		int x = 0;
		int z = 0;
		//Up
		x = 0;
		z = splitCount - 1;
		if (transformArray [0]) {
			max = splitCount >> 1;
			if (splitCount == 2) {
				tris [pos] = GetVertsPos (x + 1, z);
				tris [pos + 1] = GetVertsPos (x, z + 1);
				tris [pos + 2] = GetVertsPos (x + 2, z + 1);
				pos += 3;
			} else {
				for (int i = 0; i < max; i++) {
					if (i == 0) {
						tris [pos] = GetVertsPos (x + 1, z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 2, z + 1);

						tris [pos + 3] = tris [pos];
						tris [pos + 4] = tris [pos + 2];
						tris [pos + 5] = GetVertsPos (x + 2, z);
						pos += 6;
					} else if (i == max - 1) {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z);

						tris [pos + 3] = tris [pos + 2];
						tris [pos + 4] = tris [pos + 1];
						tris [pos + 5] = GetVertsPos (x + 2, z + 1);
						pos += 6;
					} else {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z);

						tris [pos + 3] = tris [pos + 2];
						tris [pos + 4] = tris [pos + 1];
						tris [pos + 5] = GetVertsPos (x + 2, z + 1);

						tris [pos + 6] = tris [pos + 2];
						tris [pos + 7] = tris [pos + 5];
						tris [pos + 8] = GetVertsPos (x + 2, z);
						pos += 9;
					}
					x += 2;
				}
			}
		} else {
			max = splitCount;
			if (splitCount == 2) {
				tris [pos] = GetVertsPos (x + 1, z);
				tris [pos + 1] = GetVertsPos (x, z + 1);
				tris [pos + 2] = GetVertsPos (x + 1, z + 1);
				tris [pos + 3] = tris [pos];
				tris [pos + 4] = tris [pos + 2];
				tris [pos + 5] = GetVertsPos (x + 2, z + 1);
				pos += 6;
			} else {
				for (int i = 0; i < max; i++) {
					if (i == 0) {
						tris [pos] = GetVertsPos (x + 1, z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z + 1);
						pos += 3;
					} else if (i == max - 1) {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z + 1);
						pos += 3;
					} else {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z);
						tris [pos+3] = tris [pos + 2];
						tris [pos + 4] = tris [pos + 1];
						tris [pos + 5] = GetVertsPos (x + 1, z+1);
						pos += 6;
					}
					x += 1;
				}
			}
		}
		//Right

		x = splitCount - 1;
		z = 0;
		if (transformArray [1]) {
			max = splitCount >> 1;
			if (splitCount == 2) {
				tris [pos] = GetVertsPos (x + 1, z);
				tris [pos + 1] = GetVertsPos (x, z + 1);
				tris [pos + 2] = GetVertsPos (x + 1, z + 2);
				pos += 3;
			} else {
				for (int i = 0; i < max; i++) {
					if (i == 0) {
						tris [pos] = GetVertsPos (x + 1, z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z + 2);

						tris [pos + 3] = tris [pos+2];
						tris [pos + 4] = tris [pos + 1];
						tris [pos + 5] = GetVertsPos (x, z+2);
						pos += 6;
					} else if (i == max - 1) {
						tris [pos] = GetVertsPos (x+1, z);
						tris [pos + 1] = GetVertsPos (x, z);
						tris [pos + 2] = GetVertsPos (x, z+1);

						tris [pos + 3] = tris [pos];
						tris [pos + 4] = tris [pos + 2];
						tris [pos + 5] = GetVertsPos (x + 1, z + 2);
						pos += 6;
					} else {
						tris [pos] = GetVertsPos (x+1, z);
						tris [pos + 1] = GetVertsPos (x, z);
						tris [pos + 2] = GetVertsPos (x, z+1);

						tris [pos + 3] = tris [pos];
						tris [pos + 4] = tris [pos + 2];
						tris [pos + 5] = GetVertsPos (x + 1, z + 2);

						tris [pos + 6] = tris [pos + 5];
						tris [pos + 7] = tris [pos + 2];
						tris [pos + 8] = GetVertsPos (x, z+2);
						pos += 9;
					}
					z += 2;
				}
			}
		} else {
			max = splitCount;
			if (splitCount == 2) {
				tris [pos] = GetVertsPos (x + 1, z);
				tris [pos + 1] = GetVertsPos (x, z + 1);
				tris [pos + 2] = GetVertsPos (x + 1, z + 1);
				tris [pos + 3] = tris [pos+2];
				tris [pos + 4] = tris [pos + 1];
				tris [pos + 5] = GetVertsPos (x + 1, z + 2);
				pos += 6;
			} else {
				for (int i = 0; i < max; i++) {
					if (i == 0) {
						tris [pos] = GetVertsPos (x + 1, z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z + 1);
						pos += 3;
					} else if (i == max - 1) {
						tris [pos] = GetVertsPos (x+1, z);
						tris [pos + 1] = GetVertsPos (x, z);
						tris [pos + 2] = GetVertsPos (x + 1, z + 1);
						pos += 3;
					} else {
						tris [pos] = GetVertsPos (x+1, z);
						tris [pos + 1] = GetVertsPos (x, z);
						tris [pos + 2] = GetVertsPos (x , z+1);
						tris [pos+3] = tris [pos];
						tris [pos + 4] = tris [pos + 2];
						tris [pos + 5] = GetVertsPos (x+1, z+1);
						pos += 6;
					}
					z += 1;
				}
			}
		}


		//Down
		x = 0;
		z = 0;
		if (transformArray [2]) {
			max = splitCount >> 1;
			if (splitCount == 2) {
				tris [pos] = GetVertsPos (x, z);
				tris [pos + 1] = GetVertsPos (x+1, z + 1);
				tris [pos + 2] = GetVertsPos (x + 2, z );
				pos += 3;
			} else {
				for (int i = 0; i < max; i++) {
					if (i == 0) {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x+1, z + 1);
						tris [pos + 2] = GetVertsPos (x + 2, z);

						tris [pos + 3] = tris [pos+2];
						tris [pos + 4] = tris [pos + 1];
						tris [pos + 5] = GetVertsPos (x + 2, z+1);
						pos += 6;
					} else if (i == max - 1) {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z+1);

						tris [pos + 3] = tris [pos ];
						tris [pos + 4] = tris [pos + 2];
						tris [pos + 5] = GetVertsPos (x + 2, z);
						pos += 6;
					} else {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z+1);

						tris [pos + 3] = tris [pos];
						tris [pos + 4] = tris [pos + 2];
						tris [pos + 5] = GetVertsPos (x + 2, z);

						tris [pos + 6] = tris [pos + 5];
						tris [pos + 7] = tris [pos+2];
						tris [pos + 8] = GetVertsPos (x + 2, z+1);
						pos += 9;
					}
					x += 2;
				}
			}
		} else {
			max = splitCount;
			if (splitCount == 2) {
				tris [pos] = GetVertsPos (x , z);
				tris [pos + 1] = GetVertsPos (x+1, z + 1);
				tris [pos + 2] = GetVertsPos (x + 1, z );
				tris [pos + 3] = tris [pos+2];
				tris [pos + 4] = tris [pos + 1];
				tris [pos + 5] = GetVertsPos (x + 2, z );
				pos += 6;
			} else {
				for (int i = 0; i < max; i++) {
					if (i == 0) {
						tris [pos] = GetVertsPos (x , z);
						tris [pos + 1] = GetVertsPos (x+1, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z);
						pos += 3;
					} else if (i == max - 1) {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z);
						pos += 3;
					} else {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z);
						tris [pos+3] = tris [pos + 2];
						tris [pos + 4] = tris [pos + 1];
						tris [pos + 5] = GetVertsPos (x + 1, z+1);
						pos += 6;
					}
					x += 1;
				}
			}
		}
		//Left

		x = 0;
		z = 0;
		if (transformArray [3]) {
			max = splitCount >> 1;
			if (splitCount == 2) {
				tris [pos] = GetVertsPos (x, z);
				tris [pos + 1] = GetVertsPos (x, z + 2);
				tris [pos + 2] = GetVertsPos (x + 1, z + 1);
				pos += 3;
			} else {
				for (int i = 0; i < max; i++) {
					if (i == 0) {
						tris [pos] = GetVertsPos (x , z);
						tris [pos + 1] = GetVertsPos (x, z + 2);
						tris [pos + 2] = GetVertsPos (x + 1, z + 1);

						tris [pos + 3] = tris [pos + 1];
						tris [pos + 4] = GetVertsPos (x+1, z+2);
						tris [pos + 5] = tris [pos + 2];
						pos += 6;
					} else if (i == max - 1) {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x+1, z+1);
						tris [pos + 2] = GetVertsPos (x+1, z);

						tris [pos + 3] = tris [pos];
						tris [pos + 4] = GetVertsPos (x, z+2);
						tris [pos + 5] = tris [pos + 1];
						pos += 6;
					} else {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x+1, z+1);
						tris [pos + 2] = GetVertsPos (x+1, z);

						tris [pos + 3] = tris [pos];
						tris [pos + 4] = GetVertsPos (x, z+2);
						tris [pos + 5] = tris [pos + 1];

						tris [pos + 6] = tris [pos + 4];
						tris [pos + 7] = GetVertsPos (x+1, z+2);
						tris [pos + 8] = tris [pos + 1];
						pos += 9;
					}
					z += 2;
				}
			}
		} else {
			max = splitCount;
			if (splitCount == 2) {
				tris [pos] = GetVertsPos (x , z);
				tris [pos + 1] = GetVertsPos (x, z + 1);
				tris [pos + 2] = GetVertsPos (x + 1, z + 1);
				tris [pos + 3] = tris [pos + 1];
				tris [pos + 4] = GetVertsPos (x , z + 2);
				tris [pos + 5] = tris [pos + 2];
				pos += 6;
			} else {
				for (int i = 0; i < max; i++) {
					if (i == 0) {
						tris [pos] = GetVertsPos (x , z);
						tris [pos + 1] = GetVertsPos (x, z + 1);
						tris [pos + 2] = GetVertsPos (x + 1, z + 1);
						pos += 3;
					} else if (i == max - 1) {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x, z+1);
						tris [pos + 2] = GetVertsPos (x + 1, z);
						pos += 3;
					} else {
						tris [pos] = GetVertsPos (x, z);
						tris [pos + 1] = GetVertsPos (x, z+1);
						tris [pos + 2] = GetVertsPos (x + 1, z);
						tris [pos+3] = tris [pos + 1];
						tris [pos + 4] = GetVertsPos (x+1, z+1);
						tris [pos + 5] = tris [pos + 2];
						pos += 6;
					}
					z += 1;
				}
			}
		}



		//Mid
		x = 1;
		z = 1;
		lineCount = splitCount - 1;
		int rowCount = splitCount - 2;
		max = rowCount*rowCount;
		for (int i = 0; i < max; i++) {
			tris[pos]=GetVertsPos(x,z);
			tris[pos+1]=GetVertsPos(x,z+1);
			tris[pos+2]=GetVertsPos(x+1,z);

			tris[pos+3]=tris[pos+2];
			tris[pos+4]=tris[pos+1];
			tris[pos+5]=GetVertsPos(x+1,z+1);
			pos+=6;
			x++;
			if(x>=lineCount)
			{
				x=1;
				z++;
			}
		}
		float heightTemp;
		lineCount = 0;
		for (int i = 0; i < tris.Length; i++) {
			vertsLowPoly[i] = verts[tris[i]];
			if (lineCount == 0) {
				heightTemp = MathExtra.Min(height[tris[i]],height[tris[i+1]],height[tris[i+2]]);
				if (heightTemp >= 0.5f) {
					uv [i] = uv [i + 1] = uv [i + 2] = new Vector2 (0f, 0f);
				} else if (heightTemp >= 0.7f) {
					uv [i] = uv [i + 1] = uv [i + 2] = new Vector2 (0.5f, 0f);
				} else {
					uv [i] = uv [i + 1] = uv [i + 2] = new Vector2 (0.8f, 0f);
				}
				lineCount = 3;
			}
			lineCount--;
			tris [i] = i;
		}


		Vector3 p1;
		Vector3 p2;
		for (int i = 0; i < vertsLowPoly.Length; i += 3) {
			// Calculate the normal of the triangle
			p1 = vertsLowPoly[i+1] - vertsLowPoly[i];
			p2 = vertsLowPoly[i+2] - vertsLowPoly[i];
			normals[i] = normals[i+1] = normals[i+2] = MathExtra.FastNormalize(Vector3.Cross(p1, p2));
		}
		mesh.vertices = vertsLowPoly;
		mesh.triangles = tris;
		mesh.normals = normals;
		mesh.uv = uv;
		gameObject.SetActive (true);
	}
}
