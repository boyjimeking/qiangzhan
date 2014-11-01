using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIRecord : UIWindow
{
	public static readonly uint Style_Arena = 0;
	public static readonly uint Style_Qualifying = 1;
	public static readonly int Record_Display_Count = 10;

	// 返回按钮
	public UIButton mReturnBtn;

	// 记录面板
	public UIGrid mGrid;

	// 待复制的预设
	public GameObject mCloneSrcPrefab = null;

	private List<UIRecordBar> mRecordList = new List<UIRecordBar>();

	// 竞技场/排位赛记录
	private uint mStyle = uint.MaxValue;

    protected override void OnLoad()
    {
		mReturnBtn = this.FindComponent<UIButton>("mReturnBtn");

		mGrid = this.FindComponent<UIGrid>("mRecordPanel/mGrid");

		mCloneSrcPrefab = this.FindChild("mCloneSrc/RecordBarUI");

		InitRecordList();
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
		EventDelegate.Add(mReturnBtn.onClick, OnReturnBtnClicked);

		if (param == null)
			return;

		if((uint)param == Style_Arena)
		{
			mStyle = Style_Arena;

			EventSystem.Instance.addEventListener(ArenaEvent.RECEIVE_RECORD_DATA, OnReceiveArenaRecordData);
		}
		else if ((uint)param == Style_Qualifying)
		{
			mStyle = Style_Qualifying;

			EventSystem.Instance.addEventListener(QualifyingEvent.RECEIVE_RECORD_DATA, OnReceiveQualifyingRecordData);
		}
		else
		{
			return;
		}

		foreach (UIRecordBar ui in mRecordList)
		{
			ui.SetStyle(mStyle);
			ui.AddListener();
			ui.UpdateUI();
		}

		mGrid.repositionNow = true;
    }

    //界面关闭
    protected override void OnClose()
    {
		EventDelegate.Remove(mReturnBtn.onClick, OnReturnBtnClicked);

		if (mStyle == Style_Arena)
		{
			EventSystem.Instance.removeEventListener(ArenaEvent.RECEIVE_RECORD_DATA, OnReceiveArenaRecordData);
		}
		else if(mStyle == Style_Qualifying)
		{
			EventSystem.Instance.removeEventListener(QualifyingEvent.RECEIVE_RECORD_DATA, OnReceiveQualifyingRecordData);
		}
		else
		{
			return;
		}

		foreach (UIRecordBar ui in mRecordList)
		{
			ui.RemoveListener();
		}
    }

	private void OnReturnBtnClicked()
    {
		WindowManager.Instance.CloseUI("record");
    }

	private void OnReceiveArenaRecordData(EventBase ev)
	{
		foreach (UIRecordBar ui in mRecordList)
		{
			ui.UpdateUI();
		}

		mGrid.repositionNow = true;
	}

	private void OnReceiveQualifyingRecordData(EventBase ev)
	{
		foreach (UIRecordBar ui in mRecordList)
		{
			ui.UpdateUI();
		}

		mGrid.repositionNow = true;
	}

	private void InitRecordList()
	{
		if (mRecordList == null)
			return;

		mRecordList.Clear();

		for (int i = 0; i < Record_Display_Count; ++i)
		{
			GameObject obj = WindowManager.Instance.CloneGameObject(mCloneSrcPrefab);
			if (obj == null)
			{
				continue;
			}

			obj.SetActive(true);
			obj.name = "RecordBarUI" + i.ToString();
			obj.transform.parent = mGrid.transform;
			obj.transform.localScale = Vector3.one;

			UIRecordBar itemui = new UIRecordBar(obj);
			itemui.Idx = i;

			mRecordList.Add(itemui);
		}

		mGrid.repositionNow = true;
	}
}
