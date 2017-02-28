using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QTManager : UnitySingleton<QTManager> {
	public Transform playerTrans;
	public int splitTime = 4;
	public float[,] lodData = new float[,]{
		{1f,1f},
		{100f,100f}
	};
	public TerrainManager activeTerrain;
	public List<QTNode>[] activeNodeListArray;
	// Use this for initialization
	public void Awake()
	{
		Init ();
	}
	public void Init()
	{
		playerTrans = GameObject.Find ("Player").transform;
		activeNodeListArray = new List<QTNode>[activeTerrain.maxLodLevel+1];
		for (int i = 0; i <= activeTerrain.maxLodLevel; i++) {
			activeNodeListArray [i] = new List<QTNode> ();
		}
	}
	// Update is called once per frame
	public void Update()
	{
		
	}
	void OnDrawGizmos()
	{
		if (activeNodeListArray == null)
			return;
		QTNode temp;
		for (int i = 0; i < activeNodeListArray.Length; i++) {
			//Debug.Log ("LOD为"+i.ToString()+"的节点个数为"+activeNodeListArray[i].Count.ToString());
			for (int m = 0; m < activeNodeListArray[i].Count; m++) {
				temp = activeNodeListArray [i] [m];
				Gizmos.DrawWireCube(new Vector3 (temp.center.x, 0f, temp.center.y), new Vector3 (temp.length, 1f,temp.length));
			}
		}
	}
}
