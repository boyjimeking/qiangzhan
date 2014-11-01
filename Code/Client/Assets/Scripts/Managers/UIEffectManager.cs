using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEffectManager{

	private static UIEffectManager instance;

	public UIEffectManager()
	{
		instance = this;
	}

	public static UIEffectManager Instance
	{
		get
		{
			return instance;
		}
	}

	public static bool IsUIEffectActive<T>(GameObject go , ref T t) where T:Component
	{
		if(go == null)
			return false;

		T com = go.GetComponent<T>();

		if(com == null || !com.gameObject.activeSelf)
			return false;

		t = com;
		return true;
	}

	/// <summary>
	/// multi = true;准许一个gameobject添加多个特效脚本;
	/// multi = false;
	/// </summary>
	public static T AddUIEffect<T>(GameObject go , bool multi = false) where T : Component
	{
		if(go == null)
			return null;

		if(multi)
			return go.AddComponent<T>();
		else
			return go.AddMissingComponent<T>();
	}

	/// <summary>
	/// multi= true删除全部特效组件	
	/// multi = false删除一个特效组件;
	/// </summary>
	public static void RemoveUIEffect<T>(GameObject go , bool multi = false) where T : Component
	{
		if(go == null)
			return;


		if(multi)
		{
			foreach(T t in go.GetComponents<T>())
			{
				if(t == null)
					continue;

				GameObject.Destroy(t);
			}
		}
		else
		{
			GameObject.Destroy(go.GetComponent<T>());
		}
	}
}
