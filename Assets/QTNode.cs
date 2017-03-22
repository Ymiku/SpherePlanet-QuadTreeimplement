using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class QTNode:IPoolable {
	public int lodLevel = 0;
	public QTNode parent;
	public int quadrantID = 0;
	public Vector3 center = Vector3.zero;
	public Vector3 sphereCenter = Vector3.zero;
	public float length;
	public float sphereLength;
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
		this.center = new Vector3 (parent.center.x + ((quadrantID == 0 || quadrantID == 3) ? offSet : -offSet),parent.center.y, 
			parent.center.z + ((quadrantID == 0 || quadrantID == 1) ? offSet : -offSet));
		this.sphereCenter = QTManager.Instance.activeTerrain.transform.TransformPoint(MathExtra.FastNormalize(center) * QTManager.Instance.activePlanet.sphereRadius);
		this.sphereLength = this.length;
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
	private void Hide()
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
			if((borderStatus&BorderStatus.UpBorder)!=BorderStatus.UpBorder)
			TryGenerate (0);
			if((borderStatus&BorderStatus.RightBorder)!=BorderStatus.RightBorder)
			TryGenerate (1);
			break;
		case 1:
			if((borderStatus&BorderStatus.UpBorder)!=BorderStatus.UpBorder)
			TryGenerate (0);
			if((borderStatus&BorderStatus.LeftBorder)!=BorderStatus.LeftBorder)
			TryGenerate (3);
			break;
		case 2:
			if((borderStatus&BorderStatus.DownBorder)!=BorderStatus.DownBorder)
			TryGenerate (2);
			if((borderStatus&BorderStatus.LeftBorder)!=BorderStatus.LeftBorder)
			TryGenerate (3);
			break;
		case 3:
			if((borderStatus&BorderStatus.RightBorder)!=BorderStatus.RightBorder)
			TryGenerate (1);
			if((borderStatus&BorderStatus.DownBorder)!=BorderStatus.DownBorder)
			TryGenerate (2);
			break;
		}
	}
	private void TryGenerate(int dir)
	{
		if (lodLevel == QTManager.Instance.activePlanet.maxLodLevel)
			return;
		if (dir == 0 && ((borderStatus & BorderStatus.UpBorder) == BorderStatus.UpBorder))
			return;
		if (dir == 1 && ((borderStatus & BorderStatus.RightBorder) == BorderStatus.RightBorder))
			return;
		if (dir == 2 && ((borderStatus & BorderStatus.DownBorder) == BorderStatus.DownBorder))
			return;
		if (dir == 3 && ((borderStatus & BorderStatus.LeftBorder) == BorderStatus.LeftBorder))
			return;
		Vector3 pos = center;
		float halfLength = length * 0.5f;
		switch (dir) {
		case 0:
			pos = pos + new Vector3 (0f,0f,halfLength+0.1f);
			break;
		case 1:
			pos = pos + new Vector3 (halfLength+0.1f,0f,0f);
			break;
		case 2:
			pos = pos - new Vector3 (0f,0f,halfLength+0.1f);
			break;
		case 3:
			pos = pos - new Vector3 (halfLength+0.1f,0f,0f);
			break;
		}
		QTNode border = FindNearest (QTManager.Instance.activeTerrain.GetRoot(),pos);
		if (border.lodLevel > this.lodLevel+1) {
			border.Generate ();
			QTManager.Instance.needChange = true;
		}
	}

	//used for mesh calculate
	public void UpdateMesh()
	{
		//qtMesh.CreatMesh (center, length,GetNeighbourStatusArray(), QTManager.Instance.activeTerrain.splitCount);
		//	return;
		GetNeighbourStatusArray();
		if ( (_neighbourStatus[0] != _LastNeighbourStatus[0])||(_neighbourStatus[1] != _LastNeighbourStatus[1])||(_neighbourStatus[2] != _LastNeighbourStatus[2])||(_neighbourStatus[3] != _LastNeighbourStatus[3])) {
			qtMesh.CreatMesh (center, length, _neighbourStatus, QTManager.Instance.activePlanet.splitCount);
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
		if (lodLevel == QTManager.Instance.activePlanet.maxLodLevel)
			return false;
		if (dir == 0 && ((borderStatus & BorderStatus.UpBorder) == BorderStatus.UpBorder))
			return false;
		if (dir == 1 && ((borderStatus & BorderStatus.RightBorder) == BorderStatus.RightBorder))
			return false;
		if (dir == 2 && ((borderStatus & BorderStatus.DownBorder) == BorderStatus.DownBorder))
			return false;
		if (dir == 3 && ((borderStatus & BorderStatus.LeftBorder) == BorderStatus.LeftBorder))
			return false;
		Vector3 pos = center;
		float halfLength = length * 0.5f;
		switch (dir) {
		case 0:
			pos = pos + new Vector3 (0f,0f,halfLength+0.1f);
			break;
		case 1:
			pos = pos + new Vector3 (halfLength+0.1f,0f,0f);
			break;
		case 2:
			pos = pos - new Vector3 (0f,0f,halfLength+0.1f);
			break;
		case 3:
			pos = pos - new Vector3 (halfLength+0.1f,0f,0f);
			break;
		}
		return FindNearest (QTManager.Instance.activeTerrain.GetRoot(),pos).lodLevel>this.lodLevel;

	}
	private QTNode FindNearest(QTNode root,Vector3 pos)
	{
		QTNode node = root.GetChild (0);
		if (node == null||root.lodLevel==this.lodLevel)
			return root;
		float dis;
		float disTemp;
		dis = MathExtra.FastDis (node.center, pos);

		disTemp = MathExtra.FastDis (root.GetChild (1).center, pos);
		if (disTemp < dis) {
			dis = disTemp;
			node = root.GetChild (1);
		}
		disTemp = MathExtra.FastDis (root.GetChild (2).center, pos);
		if (disTemp < dis) {
			dis = disTemp;
			node = root.GetChild (2);
		}
		disTemp = MathExtra.FastDis (root.GetChild (3).center, pos);
		if (disTemp < dis) {
			dis = disTemp;
			node = root.GetChild (3);
		}
		return FindNearest(node,pos);
	}
}
