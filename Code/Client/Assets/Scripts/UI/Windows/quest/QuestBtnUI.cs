using UnityEngine;
using System.Collections.Generic;


public class QuestBtnUI 
{
	public static Queue<QuestBtnUI> QuestBtnCache = new Queue<QuestBtnUI>();
    public UIButton QBtn;
    public UISprite QIcon;
    public UILabel QName;
    public UISprite selectedSprite;
    public int mQuestId;

    private bool mIsSelected = false;
    private QuestModule mModule;

    GameObject mObject = null;
    public delegate void ItemClickCallback();
    public ItemClickCallback mClickCallback;
    public bool IsSelected
    {
        get { return mIsSelected; }
        set
        {
            mIsSelected = value;
            selectedSprite.gameObject.SetActive(value);
        }
    }
    public QuestBtnUI(GameObject clone)
    {       
        mObject = GameObject.Instantiate(clone) as GameObject;
		Init();
    }

	public void Init()
	{
		mModule = ModuleManager.Instance.FindModule<QuestModule>();
		mObject.SetActive(true);
		QBtn = mObject.GetComponent<UIButton>();		
		QIcon = ObjectCommon.GetChildComponent<UISprite>(mObject, "QIcon");
	    QName = ObjectCommon.GetChildComponent<UILabel>(mObject, "Label");
	    selectedSprite = ObjectCommon.GetChildComponent<UISprite>(mObject, "selected");
        selectedSprite.gameObject.SetActive(false);
		EventDelegate.Add(QBtn.onClick, OnBtnClick);
	}

	public void ResetBtn()
	{
		//QuestBtnCache.Enqueue(this);
		//mObject.SetActive(false);
        mModule = null;
        mClickCallback = null;
        QIcon = null;
        QName = null;
        EventDelegate.Remove(QBtn.onClick, OnBtnClick);
        QBtn = null;
        GameObject.DestroyImmediate(mObject);
	    mObject = null;
	}

    public GameObject gameObject
    {
        get
        {
            return mObject;
        }
    }

    private void OnBtnClick()
    {
        if (mModule == null)
        {
            mModule = ModuleManager.Instance.FindModule<QuestModule>();
        }

        mModule.CurShowIndex = mQuestId;
        IsSelected = true;
        if (mClickCallback != null)
        {
            mClickCallback();
        }
    }

	public static QuestBtnUI GetQuestBtn()
	{
		if(QuestBtnCache.Count>0)
		{
			return QuestBtnCache.Dequeue();
		}
		return null;
	}

}

