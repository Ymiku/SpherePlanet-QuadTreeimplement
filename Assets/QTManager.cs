using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QTPlanetUtility{
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
		Vector3 oldPos = Vector3.zero;
		public Vector3 playerPos;
		public Vector3 localPlayerPos;
		// Use this for initialization
		public override void Awake()
		{
			base.Awake ();
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
			if(!playerTrans)
			playerTrans = GameObject.Find ("Player").transform;
		}

		// Update is called once per frame
		public void Update()
		{
			if (MathExtra.FastDis (playerTrans.position, oldPos) >= 1f) {
				Execute ();
				oldPos = playerPos;
			}
		}
		private void Execute()
		{
			//playerPos = playerTrans.position;
			float dis = MathExtra.FastSqrt(playerTrans.position.sqrMagnitude);
			playerPos =  playerTrans.position/dis*Mathf.Max((dis-activePlanet.sphereRadius*activePlanet.heightScale),activePlanet.sphereRadius);
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
			return (MathExtra.GetV3L (playerPos - node.sphereCenter) * GetDisPower () / node.sphereLength < QTManager.Instance.activePlanet.cl);
				//||(MathExtra.GetV3L (QTManager.Instance.playerTrans.position - node.terrainCenter)*GetDisPower() / node.sphereLength < QTManager.Instance.activePlanet.cl);
		}
		public bool NeedBack(QTNode node)
		{
			return (MathExtra.GetV3L (playerPos - node.sphereCenter)*GetDisPower() / node.sphereLength >= (QTManager.Instance.activePlanet.cl+backBuffer));
				//&&(MathExtra.GetV3L (QTManager.Instance.playerTrans.position - node.terrainCenter)*GetDisPower() / node.sphereLength >= (QTManager.Instance.activePlanet.cl+backBuffer));
		}
	}
}