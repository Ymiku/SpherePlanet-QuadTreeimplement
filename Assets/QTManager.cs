using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QTManager : UnitySingleton<QTManager> {
	public Transform playerTrans;
	public QTPlanet activePlanet;
	public QTTerrain activeTerrain;
	public float backBuffer = 0.1f;
	public bool needChange = true;
	public int changedLOD;
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

}
