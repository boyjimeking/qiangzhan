using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UIChat : UIWindow
{
    public UIInput mInput;
    public UIButton mSendButton;
    public UIButton mBackButton;
   // public UITextList mTextList;
    public UILabel mAreaLabel = null;

    public UIScrollView mScrollView = null;
    public UIScrollBar mScrollBar = null;

    private UIToggle mWorldToggle = null;
    private UIToggle mCityToggle = null;
    private UIToggle mSystemToggle = null;

    private UIButton mFaceButton = null;

    private UIGrid mGrid = null;
    private GameObject mFacePanel = null;
    private UIButton mFacePanelBtn = null;

    private GameObject mFaceItem = null;

    private GameObject mInputBack = null;

    private ChatChannelType mCurChannel = ChatChannelType.ChannelType_City;
    public UIChat()
    {

    }
    protected override void OnLoad()
    {
        mInput = this.FindComponent<UIInput>("Container/InputBack/ChatInput");
        mSendButton = this.FindComponent<UIButton>("Container/InputBack/ButtonSend");
        mBackButton = this.FindComponent<UIButton>("BackButton");

        mInputBack = this.FindChild("Container/InputBack");

        //mTextList = this.FindComponent<UITextList>("Container/TextList");

        mAreaLabel = this.FindComponent<UILabel>("Container/ScrollView/Label");

        mScrollView = this.FindComponent<UIScrollView>("Container/ScrollView");

        mWorldToggle = this.FindComponent<UIToggle>("Container/BntWorld");
        mCityToggle = this.FindComponent<UIToggle>("Container/BntCity");
        mSystemToggle = this.FindComponent<UIToggle>("Container/BntSystem");

        mFaceButton = this.FindComponent<UIButton>("Container/InputBack/ButtonFace");

        mScrollBar = this.FindComponent<UIScrollBar>("Container/ScrollBar");

        //mFontLabel = this.FindComponent<UILabel>("Container/TextList/Label");

        mGrid = this.FindComponent<UIGrid>("FacePanel/UIGrid");
        mFacePanel = this.FindChild("FacePanel");

        mFacePanelBtn = this.FindComponent<UIButton>("FacePanel");

        mFacePanel.SetActive(false);

        mFaceItem = this.FindChild("Items/GridItem");

        mInput.characterLimit = 32;
        mInput.label.maxLineCount = 1;
        EventDelegate.Add(mInput.onSubmit, OnSubmit2);
        EventDelegate.Add(mSendButton.onClick, OnSubmit);
        EventDelegate.Add(mBackButton.onClick, OnBackGame);

        EventDelegate.Add(mWorldToggle.onChange, OnChannelChanged);
        EventDelegate.Add(mCityToggle.onChange, OnChannelChanged);
        EventDelegate.Add(mSystemToggle.onChange, OnChannelChanged);

        EventDelegate.Add(mFaceButton.onClick, onFaceButtonClick);

        EventDelegate.Add(mFacePanelBtn.onClick, onFacePanelClick);

        InitGrid();
    }

    void InitGrid()
    {
        UIDynamicFace dface = new UIDynamicFace();

        dface.SetAtlas(UIResourceManager.Instance.GetAtlas("Face"));

        for( int i = 0 ; i < 60 ; ++i )
        {
            GameObject obj = WindowManager.Instance.CloneGameObject(mFaceItem);

            UIButton btn = obj.GetComponent<UIButton>();

            string key = (i+1).ToString();
            if( i+1 < 10 )
            {
                key = "0" + key;
            }

            if( btn != null )
            {
                btn.CustomData = "#" + key;
                UIAtlasHelper.SetButtonImage(btn, "Face:" + key);

                UIEventListener.Get(obj).onClick = onFaceClick;
                obj.transform.parent = mGrid.gameObject.transform;
                obj.transform.localScale = Vector3.one;

                BoxCollider boxCollider = obj.collider as BoxCollider;
                if (boxCollider != null)
                {
                    UnityEngine.Vector3 size = boxCollider.size;

                    size.y = 80;
                    size.x = 80;
                    boxCollider.size = size;
                }
            }
            dface.AddSymbol("#" + key, key);
        }
        mGrid.repositionNow = true;

       mAreaLabel.SetDynamicFace(dface);
    }
    void onFaceClick(GameObject obj)
    {
        UIButton btn = obj.GetComponent<UIButton>();

        string key = btn.CustomData as string;

        mInput.value += key;
        //mInput.Insert(key);

        //onFaceButtonClick();
    }

    void onFacePanelClick()
    {
        mFacePanel.SetActive(false);
    }
    void onFaceButtonClick()
    {
        if( mFacePanel.activeSelf )
        {
            mFacePanel.SetActive(false);
        }else
        {
            mFacePanel.SetActive(true);
        }
    }
    void OnBackGame()
    {
        WindowManager.Instance.CloseUI("chat");
    }
    void OnSubmit2()
    {
        OnSubmit();
    }
    public void OnSubmit()
    {
        mInput.RemoveFocus();

        string text = NGUIText.StripSymbols(mInput.value);

        if (!string.IsNullOrEmpty(text))
        {
            ChatModule module = ModuleManager.Instance.FindModule<ChatModule>();
            module.SendText(mCurChannel, text);
            mInput.value = "";
        }
    }

    private void OnChannelChanged()
    {
        if( mWorldToggle.value )
        {
            mCurChannel = ChatChannelType.ChannelType_World;
        }
        if (mCityToggle.value)
        {
            mCurChannel = ChatChannelType.ChannelType_City;
        }
        if (mSystemToggle.value)
        {
            mCurChannel = ChatChannelType.ChannelType_System;
            mInputBack.SetActive(false);
        }else
        {
            mInputBack.SetActive(true);
        }
        UpdateViews();
    }

    private void UpdateViews()
    {
        mAreaLabel.text = "";

        ChatModule module = ModuleManager.Instance.FindModule<ChatModule>();
        ChatCacheData[]  msgs =  module.GetMessageCache(mCurChannel);
        if( msgs == null )
        {
            return;
        }
        for( int i = 0 ; i < msgs.Length ; ++i )
        {
            ChatCacheData data = msgs[i];

            if (i != msgs.Length - 1)
            {
                data.msg += "\n";
            }           
            AddMessage(mCurChannel, data.name, data.msg);
        }

        mScrollBar.value = 1.0f;
    }

    private void AddMessage(ChatChannelType type , string name , string msg)
    {
        string channelname = "[fed514]【城市】";
        if (type == ChatChannelType.ChannelType_World)
            channelname = "[3eff00]【世界】";
        if (type == ChatChannelType.ChannelType_System)
            channelname = "[e92224]【系统】";

        mAreaLabel.text += (channelname + "[u]" + name + "[/u]:[-] " + msg);

        mScrollView.ResetPosition();
    }

    private void OnMessageUpdate(EventBase e)
    {
        UpdateViews();
    }

    protected override void OnOpen(object param = null)
    {
        UpdateViews();
        EventSystem.Instance.addEventListener(ChatEvent.MODULE_TO_UI_MESSAGE_UPDATE, OnMessageUpdate);
    }

    protected override void OnClose()
    {
        EventSystem.Instance.removeEventListener(ChatEvent.MODULE_TO_UI_MESSAGE_UPDATE, OnMessageUpdate);
    }
}
