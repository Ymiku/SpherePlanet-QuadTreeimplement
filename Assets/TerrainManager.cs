using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TerrainManager : MonoBehaviour {
	public float sphereRange = 10f;
	public int maxLodLevel = 4;
	public float cl = 1f;
	public int splitCount = 4;
	public Material mat;
	public List<QTNode>[] activeNodeListArray;
	public List<QTNode>[] allNodeListArray;
	public List<QTNode> updateList = new List<QTNode> ();
	public bool showGizmos = true;
	public int[,] vectorToPosTable;
	private RootNode _rootNode;
	private QTNode tempNode;
	// Use this for initialization
	void Start () {

	}
	public void Init()
	{
		if (splitCount < 1)
			splitCount = 1;
		if (splitCount % 2 != 0&&splitCount!=1)
			splitCount--;
		vectorToPosTable = new int[splitCount+1,splitCount+1];
		for (int x = 0; x <= splitCount; x++) {
			for (int z = 0; z <= splitCount; z++) {
				vectorToPosTable[x,z] = x + z * (splitCount + 1);
			}
		}
		activeNodeListArray = new List<QTNode>[maxLodLevel+1];
		allNodeListArray = new List<QTNode>[maxLodLevel+1];
		for (int i = 0; i <= maxLodLevel; i++) {
			activeNodeListArray [i] = new List<QTNode> ();
			allNodeListArray [i] = new List<QTNode> ();
		}
		_rootNode = new RootNode (sphereRange);
		TryGenerateBorder ();
		CalculateMesh ();
	}
	// Update is called once per frame
	public void Execute()
	{
		tempNode = null;
		for (int i = 0; i < activeNodeListArray.Length; i++) {
			for (int m = 0; m < activeNodeListArray[i].Count; m++) {
				tempNode = activeNodeListArray [i] [m];
				if (tempNode.lodLevel > 0&&QTManager.Instance.CanGenerate (tempNode)) {
					tempNode.Generate ();
				} else {
					if (tempNode.lodLevel < maxLodLevel && QTManager.Instance.NeedBack (tempNode.parent)) {
						tempNode.parent.Back ();
					}
				}
			}
		}
	}
	public void UpdateMesh()
	{
		for (int i = 0; i < activeNodeListArray.Length; i++) {
			for (int m = 0; m < activeNodeListArray[i].Count; m++) {
				activeNodeListArray [i] [m].UpdateMesh();
			}
		}
	}
	public void TryGenerateBorder()
	{
		for (int i = 0; i < activeNodeListArray.Length; i++) {
			for (int m = 0; m < activeNodeListArray[i].Count; m++) {
				activeNodeListArray [i] [m].TryGenerateBorder();
			}
		}
	}
	public void CalculateMesh()
	{
		tempNode = null;
		for (int i = 0; i < updateList.Count; i++) {
			tempNode = updateList [i];
			tempNode.qtMesh = LCQTMesh.CreatObject ();
			tempNode.qtMesh.transform.parent = transform;
			tempNode.qtMesh.transform.position = transform.position;
			tempNode.qtMesh.meshRenderer.material = mat;
			tempNode.qtMesh.CreatMesh (tempNode.center,tempNode.length,tempNode.GetNeighbourStatusArray (),splitCount);
			//node.qtMesh.CreatMesh (node.center,node.length,new bool[]{false,false,false,false},splitCount);
			tempNode.qtMesh.gameObject.SetActive (true);
		}
		updateList.Clear ();
	}
	public void AddToUpdateList(QTNode node)
	{
		updateList.Add (node);
	}
	public void RemoveFromUpdateList(QTNode node)
	{
		updateList.Remove (node);
	}
	void OnDrawGizmos()
	{
		if (activeNodeListArray == null||!showGizmos)
			return;
		tempNode = null;
		for (int i = 0; i < activeNodeListArray.Length; i++) {
			//Debug.Log ("LOD为"+i.ToString()+"的节点个数为"+activeNodeListArray[i].Count.ToString());
			for (int m = 0; m < activeNodeListArray[i].Count; m++) {
				tempNode = activeNodeListArray [i] [m];
				if(tempNode.isDisplay)
					Gizmos.DrawWireCube(transform.TransformPoint(new Vector3 (tempNode.center.x, 0f, tempNode.center.z)), new Vector3 (tempNode.length, 1f,tempNode.length));
			}
		}
	}
}
