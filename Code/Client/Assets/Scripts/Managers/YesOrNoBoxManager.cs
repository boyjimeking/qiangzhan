using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class YesOrNoBoxManager {
    public readonly string YES_BTN_LABEL = "确认";
    public readonly string NO_BTN_LABEL = "取消";

	public delegate void OnYesClick(object param);
	private OnYesClick onYesClick;

	public delegate void OnNoClick(object param);
	private OnNoClick onNoClick;

	public class YesNoInfo
	{
		public string title;
		public string content;
        public object param;
        public string noLabel;
        public string yesLabel;
		public OnYesClick onYesClick;
		public OnNoClick onNoClick;
		
		public YesNoInfo()
		{
			title = "";
			content = "";
            noLabel = "";
            yesLabel = "";
			onYesClick = null;
			onNoClick = null;
		}
	}

    //private List<YesNoInfo> mDatas = new List<YesNoInfo>();
    //private int mIdx = 0;//当前展示第几个界面;
    private Queue<YesNoInfo> mDatas = new Queue<YesNoInfo>();

	private static YesOrNoBoxManager instance;
	public YesOrNoBoxManager()
	{
		instance = this;
	}
	public static YesOrNoBoxManager Instance
	{
		get
		{
			return instance;
		}
	}

	public string TitleString
	{
		get
		{
            //return mDatas[mIdx].title;
            return mDatas.Peek().title;
		}
	}
	
	public string ContentString
	{
		get
		{
            //return mDatas[mIdx].content;
            return mDatas.Peek().content;
		}
	}

    public string YesLabel
    {
        get
        {
            //if (string.IsNullOrEmpty(mDatas[mIdx].yesLabel))
            //    return YES_BTN_LABEL;

            //return mDatas[mIdx].yesLabel;

            if (string.IsNullOrEmpty(mDatas.Peek().yesLabel))
                return YES_BTN_LABEL;

            return mDatas.Peek().yesLabel;
        }
    }

    public string NoLabel
    {
        get
        {
            //if (string.IsNullOrEmpty(mDatas[mIdx].noLabel))
            //    return NO_BTN_LABEL;

            //return mDatas[mIdx].noLabel;

            if (string.IsNullOrEmpty(mDatas.Peek().noLabel))
                return NO_BTN_LABEL;

            return mDatas.Peek().noLabel;
        }
    }

	OnYesClick CurYesCall{
		get
		{
            //return mDatas[mIdx].onYesClick;
            return mDatas.Peek().onYesClick;
		}
	}

	OnNoClick CurNoCall{
		get
		{
            //return mDatas[mIdx].onNoClick;
            return mDatas.Peek().onNoClick;
		}
	}

    public void ShowYesOrNoUI(YesNoInfo info)
    {
        mDatas.Enqueue(info);
        Show();
    }

	public void ShowYesOrNoUI(string title , string content , OnYesClick onYes , object param = null , OnNoClick onNo = null, string yesLabel = "", string noLabel = "")
	{
		YesNoInfo yni = new YesNoInfo();

		yni.title = title;
		yni.content =  content;
		yni.onYesClick = onYes;
        yni.param = param;
		yni.onNoClick = onNo;
        yni.yesLabel = yesLabel;
        yni.noLabel = noLabel;

        //mDatas.Add(yni);
        mDatas.Enqueue(yni);

		Show();
	}

	void Show()
	{
        //if(mIdx > (mDatas.Count - 1))
        //    return;
        if (mDatas.Count <= 0)
            return;

		WindowManager.Instance.OpenUI("queren");
		
		QueRenEvent ev = new QueRenEvent(QueRenEvent.CONTENT_CHANGE);
		EventSystem.Instance.PushEvent(ev);
	}

	public void HideYesOrNoUI()
	{
		WindowManager.Instance.CloseUI("queren");
	}
	
	void MoveNext()
	{
        //mIdx++;
        mDatas.Dequeue();

		Show();
	}

	public void OnYesBtnClick()
	{
        HideYesOrNoUI();
		if(CurYesCall == null)
			return;

		CurYesCall(mDatas.Peek().param);
		MoveNext();
	}

	public void OnNoBtnClick()
	{
        HideYesOrNoUI();

		if(CurNoCall == null)
			return;

        CurNoCall(mDatas.Peek().param);
		MoveNext();
	}

    public void OnReturnClick()
    {
        HideYesOrNoUI();
        MoveNext();
    }
}
