using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QTManager : UnitySingleton<QTManager> {
	public Transform playerTrans;
	public QTPlanet activePlanet;
	public QTTerrain activeTerrain;
	public float backBuffer = 0.1f;
	List<QTNode> nodeList;
	QTNode tNode;
	// Use this for initialization
	public void Awake()
	{
		Init ();
	}
	public void Enter(QTPlanet planet)
	{
		activePlanet = planet;
		planet.Enter ();
	}
	public void Leave()
	{
		activePlanet.Clear ();
	}
	public void Init()
	{
		playerTrans = GameObject.Find ("Player").transform;
	}

	// Update is called once per frame
	public void Update()
	{
		Execute ();
	}
	private void Execute()
	{
		for (int i = 0; i < activePlanet.quadList.Count; i++) {
			activeTerrain = activePlanet.quadList[i];
			activeTerrain.Execute ();
			activeTerrain.TryGenerateBorder ();
			activeTerrain.CalculateMesh ();
			activeTerrain.UpdateMesh ();
		}

	}
	public bool CanGenerate(QTNode node)
	{
		return MathExtra.GetV3L (QTManager.Instance.playerTrans.position - node.sphereCenter) / node.sphereLength < QTManager.Instance.activePlanet.cl;
	}
	public bool NeedBack(QTNode node)
	{
		return MathExtra.GetV3L (QTManager.Instance.playerTrans.position - node.sphereCenter) / node.sphereLength >= (QTManager.Instance.activePlanet.cl+backBuffer);
	}
	public QTNode FindBorder(QTNode node,int dir)
	{
		nodeList = null;
		tNode = null;
		for (int i = node.lodLevel+2; i < activePlanet.maxLodLevel; i++) {
			nodeList = activeTerrain.activeNodeListArray [i];
			for (int m = 0; m < nodeList.Count; m++) {
				tNode = nodeList[m];
				switch (dir) {
				case 0:
					if (MathExtra.ApproEquals (node.center.z + node.length * 0.5f, tNode.center.z - tNode.length * 0.5f) &&
					   (node.center.x > tNode.center.x - tNode.length * 0.5f) && (node.center.x < tNode.center.x + tNode.length * 0.5f))
						return tNode;
					break;
				case 1:
					if (MathExtra.ApproEquals (node.center.x + node.length * 0.5f, tNode.center.x - tNode.length * 0.5f) &&
						(node.center.z > tNode.center.z - tNode.length * 0.5f) && (node.center.z < tNode.center.z + tNode.length * 0.5f))
						return tNode;
					break;
				case 2:
					if (MathExtra.ApproEquals (node.center.z - node.length * 0.5f, tNode.center.z + tNode.length * 0.5f) &&
						(node.center.x > tNode.center.x - tNode.length * 0.5f) && (node.center.x < tNode.center.x + tNode.length * 0.5f))
						return tNode;
					break;
				case 3:
					if (MathExtra.ApproEquals (node.center.x - node.length * 0.5f, tNode.center.x + tNode.length * 0.5f) &&
						(node.center.z > tNode.center.z - tNode.length * 0.5f) && (node.center.z < tNode.center.z + tNode.length * 0.5f))
						return tNode;
					break;
				}
			}
		}
		return null;
	}
}
