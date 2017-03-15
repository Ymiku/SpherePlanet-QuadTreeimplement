using UnityEngine;
using System.Collections;

public class LCQTMesh : LCGameObject {
	public Mesh mesh;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	public MeshCollider meshCollider;
	Vector3[] verts;
	int[] tris;
	private int[,] _vectorToPosTable;
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
	public void CreatMesh(Vector3 center,float length,bool[] transformArray,int splitCount)
	{
		_vectorToPosTable = QTManager.Instance.activeTerrain.vectorToPosTable;
		mesh.Clear ();

		float offSet = length / splitCount;
		Vector3 oriPos = new Vector3 (center.x - length * 0.5f,center.y, center.z - length * 0.5f);
		int lineCount = 0;

		if(verts==null||verts.Length!=(splitCount+1)*(splitCount+1))
		verts = new Vector3[(splitCount+1)*(splitCount+1)];

		verts [0] = oriPos;
		int max = (splitCount + 1) * (splitCount + 1);

		for (int i = 1; i < max; i++) {
			if (lineCount < splitCount) {
				verts [i] = new Vector3 (verts [i - 1].x + offSet,center.y, verts [i - 1].z);

				lineCount++;
			} else {
				verts [i] = new Vector3 (verts[i-splitCount-1].x,center.y, verts [i-splitCount-1].z+offSet);
				lineCount = 0;
			}
		}
		float radius = QTManager.Instance.activePlanet.sphereRadius;
		for (int i = 0; i < max; i++) {
			verts [i] = MathExtra.FastNormalize(verts [i]) * radius;

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
						tris [pos + 1] = GetVertsPos (x+1, z+1);
						tris [pos + 2] = GetVertsPos (x + 1, z);
						tris [pos+3] = tris [pos];
						tris [pos + 4] = GetVertsPos (x, z+1);
						tris [pos + 5] = tris [pos + 1];
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
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.RecalculateNormals ();
	}
	public int GetVertsPos(int x,int z)
	{
		return _vectorToPosTable [x,z];
		//return x + z * (rowCount + 1);
	}
	public override void Destroy ()
	{
		base.Destroy ();
		mesh.Clear ();
	}
}
