using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIZhaoCaiMaoRankInfo : UIWindow
{
	public static readonly int Display_Count = 10;

	// 主角
	private UILabel mRankingText;

	// 主角姓名
	private UILabel mNameText;

	// 主角分数
	private UILabel mScoreText;

	// 记录面板
	public UIGrid mGrid;

	// 待复制的预设
	public GameObject mCloneSrcPrefab = null;

	private List<UIRankInfoBar> mBarList = new List<UIRankInfoBar>();

    protected override void OnLoad()
    {
		mRankingText = this.FindComponent<UILabel>("mWidget/PlayerRankInfo/mRankingText");
		mNameText = this.FindComponent<UILabel>("mWidget/PlayerRankInfo/mNameText");
		mScoreText = this.FindComponent<UILabel>("mWidget/PlayerRankInfo/mScoreText");
		mGrid = this.FindComponent<UIGrid>("mWidget/mItemPanel/mGrid");

		mCloneSrcPrefab = this.FindChild("mCloneSrc/RankInfoBarUI");

		InitBarList();
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
		EventSystem.Instance.addEventListener(ZhaoCaiMaoUpdateDamageEvent.ZHAOCAIMAO_UPDATE_DAMAGE_EVENT, OnReceivePlayerDamage);
		EventSystem.Instance.addEventListener(ZhaoCaiMaoUpdateRankListEvent.ZHAOCAIMAO_UPDATE_RANKLIST_EVENT, OnReceiveRankList);

		foreach (UIRankInfoBar ui in mBarList)
		{
			ui.ClearUI();
		}

		mGrid.repositionNow = true;

		ClearUI();

		mNameText.text = PlayerDataPool.Instance.MainData.name;
    }

    //界面关闭
    protected override void OnClose()
    {
		EventSystem.Instance.removeEventListener(ZhaoCaiMaoUpdateDamageEvent.ZHAOCAIMAO_UPDATE_DAMAGE_EVENT, OnReceivePlayerDamage);
		EventSystem.Instance.removeEventListener(ZhaoCaiMaoUpdateRankListEvent.ZHAOCAIMAO_UPDATE_RANKLIST_EVENT, OnReceiveRankList);
    }

	private void OnReceivePlayerDamage(EventBase ev)
	{
		ZhaoCaiMaoUpdateDamageEvent e = ev as ZhaoCaiMaoUpdateDamageEvent;
		if (e == null)
			return;

		mScoreText.text = e.mDamage.ToString();
	}

	private void OnReceiveRankList(EventBase ev)
	{
		ZhaoCaiMaoUpdateRankListEvent e = ev as ZhaoCaiMaoUpdateRankListEvent;
		if (e == null || e.sortInfo == null)
			return;

		for (int i = 0; i < mBarList.Count; ++i)
		{
			UIRankInfoBar ui = mBarList[i];
			if(i >= e.sortInfo.Count)
			{
				ui.ClearUI();
			}
			else
			{
				ui.UpdateUI((i+1).ToString(), e.sortInfo[i].name, e.sortInfo[i].damage.ToString());
			}
		}

		mGrid.repositionNow = true;
	}

	private void ClearUI()
	{
		mRankingText.text = null;
		mScoreText.text = "0";
	}

	private void InitBarList()
	{
		if (mBarList == null)
			return;

		mBarList.Clear();

		for (int i = 0; i < Display_Count; ++i)
		{
			GameObject obj = WindowManager.Instance.CloneGameObject(mCloneSrcPrefab);
			if (obj == null)
			{
				continue;
			}

			obj.SetActive(true);
			obj.name = "BarUI" + i.ToString();
			obj.transform.parent = mGrid.transform;
			obj.transform.localScale = Vector3.one;

			UIRankInfoBar itemui = new UIRankInfoBar(obj);
			itemui.Idx = i;

			mBarList.Add(itemui);
		}

		mGrid.repositionNow = true;
	}
}
