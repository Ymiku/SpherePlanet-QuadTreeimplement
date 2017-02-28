using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class LCGameObject : MonoBehaviour,IPoolable {
	public virtual void Destroy()
	{
		if (PoolManager.Instance == null)
			return;
		if (!PoolManager.Instance.IsPoolFull(GetType()))
		{
			PoolManager.Instance.PutPoolObject(GetType(), this);
		}
		else
		{
			GameObject.Destroy(this.gameObject);
		}
	}
}
