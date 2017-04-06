using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum Quad
{
	Up = 0,
	Down = 1,
	Left = 2,
	Right = 3,
	Front = 4,
	Back = 5
}
namespace QTPlanetUtility{
	
	public class QTPlanet : MonoBehaviour {
		static int Up = 0;
		static int Down = 1;
		static int Left = 2;
		static int Right = 3;
		static int Front = 4;
		static int Back = 5;
		[HideInInspector]
		public float sphereRange = 20f;
		public float sphereRadius = 10f;
		public int maxLodLevel = 4;
		public float cl = 1f;
		public int splitCount = 4;
		public int peakCount;
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
			heightMapWidth = peakCount * (int)Mathf.Pow (map.lacunarity, map.octaves-1);

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
			map.GenerateMap ();
			sphereRange = sphereRadius * 2f;
			lengthArray = new float[maxLodLevel+1];
			lengthArray [maxLodLevel] = Mathf.PI * sphereRange*0.25f;
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
			quadList [Up].transform.rotation = Quaternion.Euler (new Vector3 (0f,0f,0f));
			quadList [Up].gameObject.name = "Up";
			quadList [Down].transform.rotation = Quaternion.Euler (new Vector3 (-180f,0f,0f));
			quadList [Down].gameObject.name = "Dwon";
			quadList [Left].transform.rotation = Quaternion.Euler (new Vector3 (0f,0f,90f));
			quadList [Left].gameObject.name = "Left";
			quadList [Right].transform.rotation = Quaternion.Euler (new Vector3 (0f,0f,-90f));
			quadList [Right].gameObject.name = "Right";
			quadList [Front].transform.rotation = Quaternion.Euler (new Vector3 (-90f,0f,0f));
			quadList [Front].gameObject.name = "Front";
			quadList [Back].transform.rotation = Quaternion.Euler (new Vector3 (90f,0f,0f));
			quadList [Back].gameObject.name = "Back";
			for (int i = 0; i < 6; i++) {
				t = quadList [i];
				QTManager.Instance.activeTerrain = t;
				t.Init ();
			}
		}
		public void Clear()
		{
			quadList = null;
		}
		// Update is called once per frame
	}
}