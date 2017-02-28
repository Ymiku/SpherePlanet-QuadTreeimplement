using UnityEngine;
using System.Collections;

public class RootNode : QTNode {
	public RootNode(float length)
	{
		this.parent = null;
		this.lodLevel = QTManager.Instance.activeTerrain.maxLodLevel;
		this.quadrantID = 0;
		this.length = length;
		this.center = Vector2.zero;
		CheckForLOD ();
	}
	public override void Destroy()
	{
		parent = null;
	}
}
