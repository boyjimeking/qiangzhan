using UnityEngine;
using System.Collections;

public class TouchManager 
{
	private GameObject mTouchObj = null;
	private static TouchManager instance = null;
	public static TouchManager Instance
	{
		get
		{
			return instance;
		}
	}
	public TouchManager()
	{
		instance = this;
	}
	public void OpenTouch()
	{
        WindowManager.Instance.OpenUI("joystick");
	}

	public void DisableTouch()
	{

	}

	public void HideTouch()
	{
        WindowManager.Instance.CloseUI("joystick");
	}

}
