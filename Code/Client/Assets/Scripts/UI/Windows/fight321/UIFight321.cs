using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFight321 : UIWindow
{
#region 界面控件
	// 3
	private UITweener mText3;
	// 2
	private UITweener mText2;
	// 1
	private UITweener mText1;
	// fight
	private UITweener mFight;
	// fire
	private UISpriteAnimation mAni;
#endregion

	private uint mTimer = uint.MaxValue;

	protected override void OnLoad()
    {
		mText3 = this.FindComponent<UITweener>("mText3");
		mText2 = this.FindComponent<UITweener>("mText2");
		mText1 = this.FindComponent<UITweener>("mText1");
		mFight = this.FindComponent<UITweener>("mFight");
		mAni = this.FindComponent<UISpriteAnimation>("mAni");
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
		mTimer = uint.MaxValue;

		mText3.AddOnFinished(OnText3Finished);
		mText2.AddOnFinished(OnText2Finished);
		mText1.AddOnFinished(OnText1Finished);
		mFight.AddOnFinished(OnFightFinished);

		mText3.gameObject.SetActive(true);
		mText3.Play();
		mFight.ResetToBeginning();
		mFight.gameObject.SetActive(false);
		mAni.Reset();
		mAni.gameObject.SetActive(false);

		CameraController.Instance.ShakeCamera(GameConfig.LevelUpShakeCameraAmount, 0.5f);
    }

    //界面关闭
    protected override void OnClose()
    {
		mText3.RemoveOnFinished(OnText3Finished);
		mText2.RemoveOnFinished(OnText2Finished);
		mText1.RemoveOnFinished(OnText1Finished);
		mFight.RemoveOnFinished(OnFightFinished);
    }

	public override void Update(uint elapsed)
	{
		base.Update(elapsed);

		if (mTimer > 1500)
			return;

		mTimer += elapsed;

		if (mTimer > 1500)
		{
			WindowManager.Instance.CloseUI("fight321");
		}
	}

	private void OnText3Finished()
    {
		mText3.ResetToBeginning();
		mText3.gameObject.SetActive(false);
		mText2.gameObject.SetActive(true);
		mText2.Play();
		CameraController.Instance.ShakeCamera(GameConfig.LevelUpShakeCameraAmount, 0.5f);
    }

	private void OnText2Finished()
	{
		mText2.ResetToBeginning();
		mText2.gameObject.SetActive(false);
		mText1.gameObject.SetActive(true);
		mText1.Play();
		CameraController.Instance.ShakeCamera(GameConfig.LevelUpShakeCameraAmount, 0.5f);
	}

	private void OnText1Finished()
	{
		mText1.ResetToBeginning();
		mText1.gameObject.SetActive(false);
		mFight.gameObject.SetActive(true);
		mFight.Play();
		mAni.gameObject.SetActive(true);
		SoundManager.Instance.Play(104);
		CameraController.Instance.ShakeCamera(GameConfig.LevelUpShakeCameraAmount, 1.0f);
	}

	private void OnFightFinished()
	{
		mTimer = 0;
	}
}
