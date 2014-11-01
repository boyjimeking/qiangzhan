using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIRankInfoBar
{
	// 排名
	private UILabel mRankingText;

	// 姓名
	private UILabel mNameText;

	// 分数
	private UILabel mScoreText;

	private GameObject mObj;

	private int mIdx = 0;

	public GameObject gameObject
	{
		get
		{
			return mObj;
		}
	}

	public int Idx
	{
		get
		{
			return mIdx;
		}

		set
		{
			mIdx = value;
		}
	}

	public UIRankInfoBar(GameObject obj)
    {
		mObj = obj;
		mRankingText = ObjectCommon.GetChildComponent<UILabel>(obj, "mRankingText");
		mNameText = ObjectCommon.GetChildComponent<UILabel>(obj, "mNameText");
		mScoreText = ObjectCommon.GetChildComponent<UILabel>(obj, "mScoreText");
	}

	public void UpdateUI(string ranking, string name, string score)
	{
		mRankingText.text = ranking;
		mNameText.text = name;
		mScoreText.text = score;

		this.gameObject.SetActive(true);
	}

	public void ClearUI()
	{
		mRankingText.text = null;
		mNameText.text = null;
		mScoreText.text = null;

		this.gameObject.SetActive(false);
	}
}
