using UnityEngine;
/// <summary>
/// 防护盾界面.
/// </summary>
public class UIShield
{
	private GameObject gameObject = null;
	private UISlider mSlider = null;

	public UIShield()
	{
		gameObject = WindowManager.Instance.CloneCommonUI("ShieldUI");

		GameObject.DontDestroyOnLoad(gameObject);

		mSlider = gameObject.GetComponent<UISlider>();

		gameObject.transform.position = WindowManager.current2DCamera.ScreenToWorldPoint(new Vector3(-100.0f, -100.0f, 0.0f));

		NGUITools.SetActive(gameObject, true);
	}

	public bool Visible {
		get { return gameObject.activeSelf; }
		set { NGUITools.SetActive(gameObject, value); }
	}

	public float Progress {
		set { mSlider.value = value; }
	}

	public Vector3 Position {
		set
		{
			if (WindowManager.current2DCamera != null)
			{
				value = WindowManager.current2DCamera.ScreenToWorldPoint(value);
				gameObject.transform.position = value;
			}
		}
	}
}
