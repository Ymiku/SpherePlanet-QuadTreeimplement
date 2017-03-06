using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class QTNode:IPoolable {
	public int lodLevel = 0;
	public QTNode parent;
	public int quadrantID = 0;
	public Vector3 center = Vector3.zero;
	public float length;
	public bool isDisplay = false;
	public LCQTMesh qtMesh;
	public enum BorderStatus
	{
		NotBorder = 0,
		UpBorder = 1,
		RightBorder = 2,
		DownBorder = 4,
		LeftBorder = 8
	}
	public BorderStatus borderStatus;
	private QTNode[] _childArray = new QTNode[4];
	private bool[] _neighbourStatus = new bool[4];
	bool[] _LastNeighbourStatus = new bool[4];
	public QTNode()
	{
	}
	public void Init(QTNode parent,int quadrantID)
	{
		this.parent = parent;
		this.lodLevel = parent.lodLevel - 1;
		this.quadrantID = quadrantID;
		this.length = parent.length * 0.5f;
		float offSet = this.length*0.5f;
		this.center = new Vector3 (parent.center.x + ((quadrantID == 0 || quadrantID == 3) ? offSet : -offSet),0f, 
			parent.center.z + ((quadrantID == 0 || quadrantID == 1) ? offSet : -offSet));
		if(parent.borderStatus!=BorderStatus.NotBorder)
		{
			if(((parent.borderStatus&BorderStatus.UpBorder)==BorderStatus.UpBorder)&&(quadrantID==0||quadrantID==1))
				borderStatus &= BorderStatus.UpBorder;
			if(((parent.borderStatus&BorderStatus.RightBorder)==BorderStatus.RightBorder)&&(quadrantID==0||quadrantID==3))
				borderStatus &= BorderStatus.RightBorder;
			if(((parent.borderStatus&BorderStatus.DownBorder)==BorderStatus.DownBorder)&&(quadrantID==2||quadrantID==3))
				borderStatus &= BorderStatus.DownBorder;
			if(((parent.borderStatus&BorderStatus.LeftBorder)==BorderStatus.LeftBorder)&&(quadrantID==1||quadrantID==2))
				borderStatus &= BorderStatus.LeftBorder;
		}
		QTManager.Instance.activeTerrain.allNodeListArray [lodLevel].Add (this);
		CheckForLOD ();
	}
	public static Stack<QTNode> nodePool = new Stack<QTNode>();
	public static QTNode GetQTNode()
	{
		if (nodePool.Count>0)
			return nodePool.Pop();
		return new QTNode ();
	}
	public QTNode GetChild(int i)
	{
		return _childArray[i];
	}
	public void CheckForLOD()
	{
		if (lodLevel > 0&&QTManager.Instance.CanGenerate(this)) {
			Generate ();
			isDisplay = false;
		} else {
			Show ();
		}
	}

	private void GenerateChild()
	{
		_childArray [0] = QTNode.GetQTNode ();
		_childArray [1] = QTNode.GetQTNode ();
		_childArray [2] = QTNode.GetQTNode ();
		_childArray [3] = QTNode.GetQTNode ();
		_childArray [0].Init(this,0);
		_childArray [1].Init(this,1);
		_childArray [2].Init(this,2);
		_childArray [3].Init(this,3);
	}
	private void DestroyChild()
	{
		_childArray [0].Destroy ();
		_childArray [1].Destroy ();
		_childArray [2].Destroy ();
		_childArray [3].Destroy ();
		_childArray[0]=null;
		_childArray[1]=null;
		_childArray[2]=null;
		_childArray[3]=null;
	}
	public void Generate()
	{
		if (isDisplay) {
			Hide ();
		}
		GenerateChild ();
	}
	public void Back()
	{
		Show ();
		DestroyChild ();
	}
	public void Show()
	{
		isDisplay = true;
		QTManager.Instance.activeTerrain.AddToUpdateList (this);
		QTManager.Instance.activeTerrain.activeNodeListArray [lodLevel].Add (this);
	}
	public void Hide()
	{
		if (qtMesh == null) {
			QTManager.Instance.activeTerrain.RemoveFromUpdateList (this);
		} else {
			qtMesh.Destroy ();
			qtMesh = null;
		}
		QTManager.Instance.activeTerrain.activeNodeListArray [lodLevel].Remove (this);
		isDisplay = false;
	}
	public virtual void Destroy()
	{
		QTManager.Instance.activeTerrain.allNodeListArray [lodLevel].Remove(this);
		if (isDisplay) {
			Hide ();
		} else {
			DestroyChild ();
		}
		parent = null;
		if (nodePool.Count<100)
		{
			nodePool.Push (this);
		}
	}



	//Try GenerateBorder
	public void TryGenerateBorder()
	{
		QTNode border;
		switch (quadrantID) {
		case 0:
			TryGenerate (0);
			TryGenerate (1);
			break;
		case 1:
			TryGenerate (0);
			TryGenerate (3);
			break;
		case 2:
			TryGenerate (2);
			TryGenerate (3);
			break;
		case 3:
			TryGenerate (1);
			TryGenerate (2);
			break;
		}
	}
	private void TryGenerate(int dir)
	{
		switch (dir) {
		case 0:
			if ((borderStatus & BorderStatus.UpBorder) == BorderStatus.UpBorder)
				return;
			break;
		case 1:
			if ((borderStatus & BorderStatus.RightBorder) == BorderStatus.RightBorder)
				return;
			break;
		case 2:
			if ((borderStatus & BorderStatus.DownBorder) == BorderStatus.DownBorder)
				return;
			break;
		case 3:
			if ((borderStatus & BorderStatus.LeftBorder) == BorderStatus.LeftBorder)
				return;
			break;
		}

		QTNode border = GetSameLevelBorder (dir);
		if (border == null) {
			border = QTManager.Instance.FindBorder (this, dir);
			if(border!=null)
			border.Generate ();
		}

	}
	private QTNode GetSameLevelBorder (int dir)
	{
		List<QTNode> nodeList = QTManager.Instance.activeTerrain.allNodeListArray[lodLevel];
		Vector3 target = Vector3.zero;
		switch (dir) {
		case 0:
			target = center + new Vector3 (0f,0f,length);
			break;
		case 1:
			target = center + new Vector3 (length,0f,0f);
			break;
		case 2:
			target = center + new Vector3 (0f,0f,-length);
			break;
		case 3:
			target = center + new Vector3 (-length,0f,0f);
			break;
		}
		for (int i = 0; i < nodeList.Count; i++) {
			if (MathExtra.ApproEquals (nodeList [i].center.x, target.x) && MathExtra.ApproEquals (nodeList [i].center.z, target.z))
				return nodeList [i];
		}
		return null;
	}



	//used for mesh calculate
	public void UpdateMesh()
	{
		//qtMesh.CreatMesh (center, length,GetNeighbourStatusArray(), QTManager.Instance.activeTerrain.splitCount);
		//	return;
		GetNeighbourStatusArray();
		if ( (_neighbourStatus[0] != _LastNeighbourStatus[0])||(_neighbourStatus[1] != _LastNeighbourStatus[1])||(_neighbourStatus[2] != _LastNeighbourStatus[2])||(_neighbourStatus[3] != _LastNeighbourStatus[3])) {
			qtMesh.CreatMesh (center, length, _neighbourStatus, QTManager.Instance.activeTerrain.splitCount);
			for (int i = 0; i < 4; i++) {
				_LastNeighbourStatus [i] = _neighbourStatus [i];
			}
		}

	}
	public bool[] GetNeighbourStatusArray()
	{
		switch (quadrantID) {
		case 0:
			_neighbourStatus [2] = false;
			_neighbourStatus [3] = false;
			_neighbourStatus [0] = GetBorderStatus (0);
			_neighbourStatus [1] = GetBorderStatus (1);
			break;
		case 1:
			_neighbourStatus [1] = false;
			_neighbourStatus [2] = false;
			_neighbourStatus [0] = GetBorderStatus (0);
			_neighbourStatus [3] = GetBorderStatus (3);
			break;
		case 2:
			_neighbourStatus [0] = false;
			_neighbourStatus [1] = false;
			_neighbourStatus [2] = GetBorderStatus (2);
			_neighbourStatus [3] = GetBorderStatus (3);
			break;
		case 3:
			_neighbourStatus [0] = false;
			_neighbourStatus [3] = false;
			_neighbourStatus [1] = GetBorderStatus (1);
			_neighbourStatus [2] = GetBorderStatus (2);
			break;
		default:
			break;
		}
		return _neighbourStatus;
	}
	private bool GetBorderStatus(int dir)
	{
		if (lodLevel == QTManager.Instance.activeTerrain.maxLodLevel)
			return false;
		if (dir == 0 && ((borderStatus & BorderStatus.UpBorder) == BorderStatus.UpBorder))
			return false;
		if (dir == 1 && ((borderStatus & BorderStatus.RightBorder) == BorderStatus.RightBorder))
			return false;
		if (dir == 2 && ((borderStatus & BorderStatus.DownBorder) == BorderStatus.DownBorder))
			return false;
		if (dir == 3 && ((borderStatus & BorderStatus.LeftBorder) == BorderStatus.LeftBorder))
			return false;
		float halfLength = length * 0.5f;
		float targetA = (dir==0||dir==2)?(center.x-halfLength):(center.z-halfLength);
		float targetB = (dir==0||dir==2)?(center.x+halfLength):(center.z+halfLength);
		List<QTNode> checkList = QTManager.Instance.activeTerrain.activeNodeListArray [lodLevel+1];
		switch (dir) {
		case 0:
			for (int i = 0; i < checkList.Count; i++) {
				if (MathExtra.ApproEquals (checkList [i].center.x, targetA) || MathExtra.ApproEquals (checkList [i].center.x, targetB)) {
					if (MathExtra.ApproEquals (checkList [i].center.z - length, center.z + halfLength)) {
						return true;
					}
				}
			}
			break;
		case 1:
			for (int i = 0; i < checkList.Count; i++) {
				if (MathExtra.ApproEquals (checkList [i].center.z, targetA) || MathExtra.ApproEquals (checkList [i].center.z, targetB)) {
					if (MathExtra.ApproEquals (checkList [i].center.x - length, center.x + halfLength)) {
						return true;
					}
				}
			}
			break;
		case 2:
			for (int i = 0; i < checkList.Count; i++) {
				if (MathExtra.ApproEquals (checkList [i].center.x, targetA) || MathExtra.ApproEquals (checkList [i].center.x, targetB)) {
					if (MathExtra.ApproEquals (checkList [i].center.z + length, center.z - halfLength)) {
						return true;
					}
				}
			}
			break;
		case 3:
			for (int i = 0; i < checkList.Count; i++) {
				if (MathExtra.ApproEquals (checkList [i].center.z, targetA) || MathExtra.ApproEquals (checkList [i].center.z, targetB)) {
					if (MathExtra.ApproEquals (checkList [i].center.x + length, center.x - halfLength)) {
						return true;
					}
				}
			}
			break;
		}

		return false;
	}
}
