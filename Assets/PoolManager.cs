using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PoolManager : UnitySingleton<PoolManager> {
	public Transform root;
	public static Dictionary<Type, Stack<IPoolable>> ObjectPoolDic = new Dictionary<Type, Stack<IPoolable>>();
	public static Dictionary<Type, int> ObjectPoolSizeDic = new Dictionary<Type,int>();
	public override void Awake(){
		base.Awake ();

	}
	void Start () {
		
	}
	
	public void RegistPoolableType(Type type, int poolSize)
	{
		if (!ObjectPoolDic.ContainsKey(type))
		{
			ObjectPoolDic[type] = new Stack<IPoolable>();
			ObjectPoolSizeDic[type] = poolSize;
		}
	}
	
	public bool HasPoolObject(Type type)
	{
		return ObjectPoolDic.ContainsKey(type) && ObjectPoolDic[type].Count > 0;
	}
	
	public bool IsPoolFull(Type type)
	{
		if (!ObjectPoolDic.ContainsKey(type))
			return true;
		else if (ObjectPoolDic[type].Count >= ObjectPoolSizeDic[type])
			return true;
		return false;
	}
	
	public IPoolable TakePoolObject(Type type)
	{
		if (ObjectPoolDic.ContainsKey(type) && ObjectPoolDic[type].Count > 0)
		{
			return ObjectPoolDic[type].Pop();
		}
		else
		{
			return null;
		}
	}
	
	public bool PutPoolObject(Type type, IPoolable obj)
	{
		if (!ObjectPoolDic.ContainsKey(type) || ObjectPoolDic[type].Count >= ObjectPoolSizeDic[type])
		{
			GameObject.Destroy((obj as MonoBehaviour).gameObject);
			return false;
		}
		else
		{
			(obj as MonoBehaviour).gameObject.SetActive(false);
			(obj as MonoBehaviour).transform.parent = transform;
			ObjectPoolDic[type].Push(obj);
			return true;
		}
	}
}
