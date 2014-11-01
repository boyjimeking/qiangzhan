using UnityEngine;
using System.Collections;

public class BtnInfo
{
	public string name;
	public EventDelegate ev;
}

public class MainFormModule : ModuleBase {

	public enum BtnType
	{
		HuoDong,
		GongNeng,
	}


	public MainFormModule()
	{

	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();
	}

	/// <summary>
	/// 添加按钮
	/// </summary>
	public void Add(BtnType bt , EventDelegate callBack  )
	{

	}
}
