using UnityEngine;
using System.Collections;
namespace QTPlanetUtility{
	public class RootNode : QTNode {
		public Vector3 rootOriPos;
		public float interval;
		public RootNode(float length)
		{
			this.parent = null;
			this.lodLevel = QTManager.Instance.activePlanet.maxLodLevel;
			this.quadrantID = 0;
			this.length = length;
			this.center = Vector3.up*length*0.5f;
			this.rootOriPos = center - new Vector3 (length*0.5f,0f,length*0.5f);
			this.interval = length / (QTManager.Instance.activePlanet._width - 1);
			this.sphereCenter = QTManager.Instance.activeTerrain.transform.TransformPoint(MathExtra.FastNormalize(center) * QTManager.Instance.activePlanet.sphereRadius);
			this.sphereLength = QTManager.Instance.activePlanet.lengthArray[this.lodLevel];
			borderStatus = BorderStatus.UpBorder | BorderStatus.RightBorder | BorderStatus.DownBorder | BorderStatus.LeftBorder;
			CheckForLOD ();
		}
		public override void Destroy()
		{
			parent = null;
			base.Destroy ();
		}
	}
}