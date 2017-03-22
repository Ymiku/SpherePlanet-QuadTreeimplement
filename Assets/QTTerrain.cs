using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class QTTerrain : MonoBehaviour {

	public List<QTNode>[] activeNodeListArray;
	public List<QTNode>[] allNodeListArray;
	public List<QTNode> updateList = new List<QTNode> ();
	public bool showGizmos = false;
	public int[,] vectorToPosTable;
	private RootNode _rootNode;
	private QTNode _tempNode;
	private QTPlanet _planet;
	// Use this for initialization
	public void Init()
	{
		_planet = QTManager.Instance.activePlanet;
		vectorToPosTable = new int[_planet.splitCount+1,_planet.splitCount+1];
		for (int x = 0; x <= _planet.splitCount; x++) {
			for (int z = 0; z <= _planet.splitCount; z++) {
				vectorToPosTable[x,z] = x + z * (_planet.splitCount + 1);
			}
		}
		activeNodeListArray = new List<QTNode>[_planet.maxLodLevel+1];
		allNodeListArray = new List<QTNode>[_planet.maxLodLevel+1];
		for (int i = 0; i <= _planet.maxLodLevel; i++) {
			activeNodeListArray [i] = new List<QTNode> ();
			allNodeListArray [i] = new List<QTNode> ();
		}
		_rootNode = new RootNode (_planet.sphereRange);
		TryGenerateBorder ();
		CalculateMesh ();
	}
	public QTNode GetRoot()
	{
		return _rootNode;
	}
	public void Clear()
	{
		_rootNode.Destroy ();
	}

	// Update is called once per frame
	public void Execute()
	{
		_tempNode = null;
		for (int i = 0; i < activeNodeListArray.Length; i++) {
			for (int m = 0; m < activeNodeListArray[i].Count; m++) {
				_tempNode = activeNodeListArray [i] [m];
				if (_tempNode.lodLevel > 0&&QTManager.Instance.CanGenerate (_tempNode)) {
					_tempNode.Generate ();
				} else {
					if (_tempNode.lodLevel < _planet.maxLodLevel && QTManager.Instance.NeedBack (_tempNode.parent)) {
						_tempNode.parent.Back ();
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
		_tempNode = null;
		for (int i = 0; i < updateList.Count; i++) {
			_tempNode = updateList [i];
			_tempNode.qtMesh = LCQTMesh.CreatObject ();
			_tempNode.qtMesh.transform.parent = transform;
			_tempNode.qtMesh.transform.localPosition = Vector3.zero;
			_tempNode.qtMesh.transform.localRotation = Quaternion.Euler (new Vector3(0f,0f,0f));
			_tempNode.qtMesh.meshRenderer.material = _planet.mat;

			_tempNode.qtMesh.CreatMesh (_tempNode.center,_tempNode.length,_tempNode.GetNeighbourStatusArray (),_planet.splitCount);
			//node.qtMesh.CreatMesh (node.center,node.length,new bool[]{false,false,false,false},splitCount);
			_tempNode.qtMesh.gameObject.SetActive (true);
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
		_tempNode = null;
		for (int i = 0; i < activeNodeListArray.Length; i++) {
			//Debug.Log ("LOD为"+i.ToString()+"的节点个数为"+activeNodeListArray[i].Count.ToString());
			for (int m = 0; m < activeNodeListArray[i].Count; m++) {
				_tempNode = activeNodeListArray [i] [m];
				if(_tempNode.isDisplay)
					Gizmos.DrawWireCube(transform.TransformPoint(new Vector3 (_tempNode.center.x, 0f, _tempNode.center.z)), new Vector3 (_tempNode.length, 1f,_tempNode.length));
			}
		}
	}
}
