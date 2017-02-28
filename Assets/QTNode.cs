using UnityEngine;
using System.Collections;
public class QTNode:IPoolable {
	public int lodLevel = 0;
	public QTNode parent;
	public int quadrantID = 0;
	public Vector2 center = Vector2.zero;
	public float length;
	private QTNode[] _childArray;
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
		this.center = new Vector2 (parent.center.x + ((quadrantID == 0 || quadrantID == 3) ? offSet : -offSet), 
			parent.center.y + ((quadrantID == 0 || quadrantID == 1) ? offSet : -offSet));
		CheckForLOD ();
	}
	public static QTNode GetQTNode()
	{
		if (PoolManager.Instance != null&&PoolManager.Instance.HasPoolObject (typeof(QTNode)))
			return PoolManager.Instance.TakePoolObject (typeof(QTNode)) as QTNode;
		return new QTNode ();
	}
	public void CheckForLOD()
	{
		if (lodLevel > 0) {
			GenerateChild ();
		} else {
			QTManager.Instance.activeNodeListArray [lodLevel].Add (this);
		}
	}
	public void Update()
	{
		
	}
	public void GenerateChild()
	{
		_childArray = new QTNode[4];
		_childArray [0] = QTNode.GetQTNode ();
		_childArray [1] = QTNode.GetQTNode ();
		_childArray [2] = QTNode.GetQTNode ();
		_childArray [3] = QTNode.GetQTNode ();
		_childArray [0].Init(this,0);
		_childArray [1].Init(this,1);
		_childArray [2].Init(this,2);
		_childArray [3].Init(this,3);
	}
	public virtual void Destroy()
	{
		parent = null;
		_childArray = null;
		if (PoolManager.Instance == null)
			return;
		if (!PoolManager.Instance.IsPoolFull(GetType()))
		{
			PoolManager.Instance.PutPoolObject(GetType(), this);
		}
	}
}
