using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QTPlanetUtility{
	public enum Quad
	{
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3,
		Front = 4,
		Back = 5
	}
	public class QTPlanet : MonoBehaviour {
		[HideInInspector]
		public int seed;
		public float sphereDiameter = 20f;
		public float sphereRadius = 10f;
		public int maxLodLevel = 4;
		public float cl = 1f;
		public int splitCount = 4;
		public int peakCount;
		public float borderDepth = -1;
		public Material mat;
		[HideInInspector]
		public List<QTTerrain> quadList;
		[HideInInspector]
		public float[] lengthArray;
		public PlaneMap map;
		public int fullLODWidth;
		public int heightMapWidth;
		[HideInInspector]
		public float mapScale;
		public Texture2D heightTex;
		public float heightScale = 0.2f;
		[HideInInspector]
		public int[,] vectorToPosTable;
		public int[,] vectorToHeightMapTable;
		// Use this for initialization
		void Awake()
		{
			QTManager.Instance.Enter (this);
		}
		public void Enter()
		{
			fullLODWidth = splitCount * (1<<maxLodLevel)+1;
			peakCount = Mathf.Max (peakCount,1);
			heightMapWidth = fullLODWidth;//-1>>2;//peakCount * (int)Mathf.Pow (map.lacunarity, map.octaves-1);

			mapScale = (float)heightMapWidth/fullLODWidth;
			map.scale = (float)heightMapWidth/peakCount;
			vectorToPosTable = new int[splitCount+1,splitCount+1];
			for (int x = 0; x <= splitCount; x++) {
				for (int z = 0; z <= splitCount; z++) {
					vectorToPosTable[x,z] = x + z * (splitCount + 1);
				}
			}
			vectorToHeightMapTable = new int[heightMapWidth,heightMapWidth];
			for (int x = 0; x < heightMapWidth; x++) {
				for (int z = 0; z < heightMapWidth; z++) {
					vectorToHeightMapTable[x,z]=x + z * heightMapWidth;
				}
			}
			map.mapHeight = heightMapWidth;
			map.mapWidth = heightMapWidth;

			sphereDiameter = sphereRadius * 2f;
			lengthArray = new float[maxLodLevel+1];
			lengthArray [maxLodLevel] = Mathf.PI * sphereDiameter*0.25f;
			for (int i = maxLodLevel-1; i >=0; i--) {
				lengthArray [i] = lengthArray [i+1]*0.5f;
			}
			if (splitCount < 1)
				splitCount = 1;
			if (splitCount % 2 != 0&&splitCount!=1)
				splitCount--;
			GameObject go;
			QTTerrain t;
			quadList = new List<QTTerrain> ();
			for (int i = 0; i < 6; i++) {
				go = new GameObject ();
				go.transform.parent = this.transform;
				go.transform.localPosition = Vector3.zero;
				t = go.AddComponent<QTTerrain> ();
				quadList.Add (t);
			}
			quadList [(int)Quad.Up].transform.rotation = Quaternion.Euler (new Vector3 (0f,0f,0f));
			quadList [(int)Quad.Up].gameObject.name = "Up";
			quadList [(int)Quad.Down].transform.rotation = Quaternion.Euler (new Vector3 (-180f,0f,0f));
			quadList [(int)Quad.Down].gameObject.name = "Dwon";
			quadList [(int)Quad.Left].transform.rotation = Quaternion.Euler (new Vector3 (0f,0f,90f));
			quadList [(int)Quad.Left].gameObject.name = "Left";
			quadList [(int)Quad.Right].transform.rotation = Quaternion.Euler (new Vector3 (0f,0f,-90f));
			quadList [(int)Quad.Right].gameObject.name = "Right";
			quadList [(int)Quad.Front].transform.rotation = Quaternion.Euler (new Vector3 (-90f,0f,0f));
			quadList [(int)Quad.Front].gameObject.name = "Front";
			quadList [(int)Quad.Back].transform.rotation = Quaternion.Euler (new Vector3 (90f,0f,0f));
			quadList [(int)Quad.Back].gameObject.name = "Back";
			for (int i = 0; i < 6; i++) {
				t = quadList [i];
				QTManager.Instance.activeTerrain = t;
				t.Init ((Quad)i);
			}
		}
		public void Clear()
		{
			quadList = null;
		}
		// Update is called once per frame
	}
}