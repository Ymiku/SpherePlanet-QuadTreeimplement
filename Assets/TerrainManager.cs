using UnityEngine;
using System.Collections;

public class TerrainManager : MonoBehaviour {
	public float sphereRange = 10f;
	public int maxLodLevel = 4;
	private RootNode _rootNode;
	// Use this for initialization
	void Start () {
		Debug.Log (MathExtra.fastSqrt(4f));
		Debug.Log (MathExtra.fastSqrt(10000f));
		Init ();

	}
	void Init()
	{
		QTManager.Instance.activeTerrain = this;
		_rootNode = new RootNode (sphereRange);
	}
	// Update is called once per frame
	void Update () {
	
	}
	void CalculateMesh()
	{
		
	}
}
