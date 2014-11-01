using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MailItem
{
    private UILabel mTitle;
    private UILabel mDate;
    private UILabel mTime;
    private GameObject mObj;
    private UIButton mItem;
    private UISprite mStateClose;
    private UISprite mStateOpen;
    public bool isOpened;
    public bool isPicked;
    public MailsData mMaildata;

    public MailItem(GameObject go, MailsData maildata)
    {
        mObj = go;
        if (go != null)
        {
            isOpened = false;
            isPicked = false;
            mTitle = ObjectCommon.GetChildComponent<UILabel>(go, "Title");
            mDate = ObjectCommon.GetChildComponent<UILabel>(go, "Date");
            mTime = ObjectCommon.GetChildComponent<UILabel>(go, "Time");
            mItem = go.GetComponent<UIButton>();
            mStateClose = ObjectCommon.GetChildComponent<UISprite>(go, "IconClose");
            mStateOpen = ObjectCommon.GetChildComponent<UISprite>(go, "IconOpen");
            EventDelegate.Add(mItem.onClick, OnClickItem);
            mMaildata = maildata;
        }
    }
    private void OnClickItem()
    {
        if(isOpened == false)
        {
            isOpened = true;
            if (!NGUITools.GetActive(mStateOpen.gameObject))
            {
                NGUITools.SetActive(mStateOpen.gameObject, true);
            }
            if (NGUITools.GetActive(mStateClose.gameObject))
            {
                NGUITools.SetActive(mStateClose.gameObject, false);
            }
            MailActionParam param = new MailActionParam();
            param.mailid = mMaildata.mailid;
            Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_MAIL_STATE_OPENED, param);
        }
        MailModule module = ModuleManager.Instance.FindModule<MailModule>();
        module.SelectedMail = this;
        ShowMailContent(mMaildata);
        UIWindow MailUI = WindowManager.Instance.GetUI("mail");
        UIButton mDeleteBtn = MailUI.FindComponent<UIButton>("background/mGiveUpBtn");
        UIButton mPickBtn = MailUI.FindComponent<UIButton>("background/mGetBtn");
        UIAtlasHelper.SetButtonImage(mDeleteBtn, "common:btn_blue_4word", true);
        if (mMaildata.itemcnt > 0 && mMaildata.mItemsList != null && !isPicked)
        {
            UIAtlasHelper.SetButtonImage(mPickBtn, "common:btn_yellow_4word", true);
        }
    }
    public void OnDeleteItem()
    {
        if (NGUITools.GetActive(this.gameObject))
        {
            NGUITools.SetActive(this.gameObject, false);
        }
        UIWindow MailUI = WindowManager.Instance.GetUI("mail");
        UIGrid mGrid = MailUI.FindComponent<UIGrid>("ScrollView/UIGrid");
        mGrid.Reposition();
        mGrid.repositionNow = true;
        if (mMaildata.state != 0)
        {
            MailActionParam param = new MailActionParam();
            param.mailid = mMaildata.mailid;
            Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_MAIL_STATE_DELETE, param);
        }
        MailModule module = ModuleManager.Instance.FindModule<MailModule>();
        module.SelectedMail = null;
        HideMailContent();
    }
    public void OnPickItem()
    {
        if (!isPicked)
        {
            MailActionParam param = new MailActionParam();
            param.mailid = mMaildata.mailid;
            Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_MAIL_STATE_PICKED, param);
        }
    }
    public void SetTitle(string title)
    {
        if (mTitle)
        {
            mTitle.text = title;
        }
    }
    public void SetTime(string time)
    {
        if (mTime)
        {
            mTime.text = time;
        }
    }

    public void HideMailContent()
    {
        UIWindow MailUI = WindowManager.Instance.GetUI("mail");
        UIPanel MailContent = MailUI.FindComponent<UIPanel>("Open");
        UISprite Empty = MailUI.FindComponent<UISprite>("Empty");
      
        if (!NGUITools.GetActive(Empty.gameObject))
        {
            NGUITools.SetActive(Empty.gameObject, true);
        }
        if (NGUITools.GetActive(MailContent.gameObject))
        {
            NGUITools.SetActive(MailContent.gameObject, false);
        }
    }
    public void ShowMailContent(MailsData maildata)
    {
        
        UIWindow MailUI = WindowManager.Instance.GetUI("mail");
        UIPanel MailContent = MailUI.FindComponent<UIPanel>("Open");
        UISprite Empty = MailUI.FindComponent<UISprite>("Empty");
        UILabel Title = MailUI.FindComponent<UILabel>("Open/mTitle");
        UIGrid Grid = MailUI.FindComponent<UIGrid>("Open/ItemGridBK/Scroll View/UIGrid");
        List<AwardItemUI> mItemList = new List<AwardItemUI>();
        mItemList.Clear();
        if (Grid)
        {
            ObjectCommon.DestoryChildren(Grid.gameObject);
        }
        Title.text = maildata.title;
        UILabel Text = MailUI.FindComponent<UILabel>("Text");
        Text.text = maildata.content;
        if (maildata.mItemsList.Count > 0 && !isPicked)
        {
            for (int i = 0; i < maildata.mItemsList.Count; i++)
            {
                AwardItemUI item = new AwardItemUI(maildata.mItemsList[i].resid, maildata.mItemsList[i].num);
                item.gameObject.transform.parent = Grid.gameObject.transform;
                item.gameObject.transform.localScale = Vector3.one;

                mItemList.Add(item);
            }
            Grid.Reposition();
            Grid.repositionNow = true;
        }
        if (NGUITools.GetActive(Empty.gameObject))
        {
            NGUITools.SetActive(Empty.gameObject, false);
        }
        if (!NGUITools.GetActive(MailContent.gameObject))
        {
            NGUITools.SetActive(MailContent.gameObject, true);
        }
        
    } 
    public void SetDate(string data)
    {
        if (mDate)
        {
            mDate.text = data;
        }
    }
    public void ShowState(uint state)
    {
        if (state != 0)
        {
            if (state == 1)
            {
                NGUITools.SetActive(mStateClose.gameObject, true);

                NGUITools.SetActive(mStateOpen.gameObject, false);

            }
            else
            {

                NGUITools.SetActive(mStateOpen.gameObject, true);

                NGUITools.SetActive(mStateClose.gameObject, false);

            }
        }
    } 
    public GameObject gameObject
    {
        get { return mObj; }
    }
}
public class MailItemNode
{
    public MailItem mUI = null;
    public bool isPicked = false;
    public MailItemNode()
    {

    }

    public bool Init(MailsData maildata)
    {

        //等待新机制修改
        UIWindow MailUI = WindowManager.Instance.GetUI("mail");
        UIGrid mGrid = MailUI.FindComponent<UIGrid>("ScrollView/UIGrid");
        GameObject mClone = MailUI.FindChild("MailItem");
        GameObject clone = WindowManager.Instance.CloneGameObject(mClone);
       // GameObject.DontDestroyOnLoad(clone);
        mUI = new MailItem(clone,maildata);
        mUI.gameObject.transform.parent = mGrid.transform;
        mUI.gameObject.transform.localScale = Vector3.one;
        mGrid.Reposition();
        mGrid.repositionNow = true;

        return true;
    }
    public bool InitItem(MailsData maildata)
    {
        uint day = 0;
        uint sendtime = (uint)(maildata.start_time / 1000000);
        //s = s.Add(new TimeSpan(sendtime)
        DateTime s = new DateTime(1970, 1, 1);
        s = s.AddSeconds(sendtime);
        ulong now = TimeUtilities.GetNow();
        string date = DateTime.Now.ToLongDateString().ToString();
        ulong time = now/1000 - (ulong)sendtime; 
        if(time > 24*60*60)
        {
           day = (uint)(time/(24*60*60));
        }
        else
        {
            day = 1;
        }
        SetTitle(maildata.title);
        SetData(s.ToString("MM月dd日"));
        SetTime(day.ToString()+"天");
        mUI.ShowState(maildata.state);
        Show();

        return true;
    }
    public bool IsHide()
    {
        return !mUI.gameObject.activeSelf;
    }

    public void Show()
    {
        if (mUI != null && IsHide() && (!NGUITools.GetActive(mUI.gameObject)))
        {
            NGUITools.SetActive(mUI.gameObject, true);
        }
    }

    public void SetTitle(string text)
    {
        if (mUI == null)
            return;
        if (WindowManager.current2DCamera != null)
        {
            mUI.SetTitle(text);
        }
    }
    public void SetTime(string text)
    {
        if (mUI == null)
            return;
        if (WindowManager.current2DCamera != null)
        {
            mUI.SetTime(text);
        }
    }
    public void SetData(string text)
    {
        if (mUI == null)
            return;
        if (WindowManager.current2DCamera != null)
        {
            mUI.SetDate(text);
        }
    }
}

//邮件界面管理器
public class MailItemManager
{
    private static MailItemManager instance = null;
    public static MailItemManager Instance
    {
        get
        {
            return instance;
        }
    }
    public MailItemManager()
    {
        instance = this;
    }


    public MailItemNode CreateMailItem(MailsData maildata)
    {
        MailItemNode node = null;
        node = new MailItemNode();
        node.Init(maildata);
        node.InitItem(maildata);
        return node;
    }
    public void Update()
    {
        
    }
}
