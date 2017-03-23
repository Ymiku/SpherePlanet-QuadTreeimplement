using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QTManager : UnitySingleton<QTManager> {
	public Transform playerTrans;
	[HideInInspector]
	public QTPlanet activePlanet;
	[HideInInspector]
	public QTTerrain activeTerrain;
	public float disPower = 0.1f;
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
	public float GetDisPower()
	{
		return Mathf.Max (0.2f,disPower);
	}
	public bool CanGenerate(QTNode node)
	{
		return MathExtra.GetV3L (QTManager.Instance.playerTrans.position - node.sphereCenter)*GetDisPower() / node.sphereLength < QTManager.Instance.activePlanet.cl;
	}
	public bool NeedBack(QTNode node)
	{
		return MathExtra.GetV3L (QTManager.Instance.playerTrans.position - node.sphereCenter)*GetDisPower() / node.sphereLength >= (QTManager.Instance.activePlanet.cl+backBuffer);
	}
}
