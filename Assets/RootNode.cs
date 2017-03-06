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
		borderStatus = BorderStatus.UpBorder | BorderStatus.RightBorder | BorderStatus.DownBorder | BorderStatus.LeftBorder;
		CheckForLOD ();
	}
	public override void Destroy()
	{
		parent = null;
	}
}
