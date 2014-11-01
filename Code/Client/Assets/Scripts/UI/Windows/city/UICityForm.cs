using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class SubMenuForm
{
    protected UISprite mBg;
    protected UIGrid mGrid;

    private GameObject mGo;
    private Dictionary<int, GameObject> items = new Dictionary<int, GameObject>();

    // 图标与背景框的边距;
    private readonly Vector2 PaddingVector2 = new Vector2(20f, 20f);

    // go与父图标的偏移值;
    private readonly Vector3 offsetVector3 = new Vector3(0f, -100f, 0f);

    public GameObject gameObject
    {
        get
        {
            return mGo;
        }
    }

    public GameObject grid
    {
        get
        {
            return mGrid.gameObject;
        }
    }

    public Dictionary<int, GameObject> SubMenuDict
    {
        get
        {
            return items;
        }
    }

    public SubMenuForm(GameObject go)
    {
        mGo = go;

        mBg = ObjectCommon.GetChildComponent<UISprite>(go, "bg");
        mGrid = ObjectCommon.GetChildComponent<UIGrid>(go, "grid");
    }

    public void SetSubFormPosition(Vector3 pos)
    {
        pos = UICamera.currentCamera.ScreenToWorldPoint(pos + offsetVector3);
        mGo.transform.position = pos;

        ResetSubFormGridPos();
    }

    void ResetSubFormGridPos()
    {
        Vector2 wh = getWidthAndHeight();

        float x = -(wh.x / 2f - mGrid.cellWidth / 2f - PaddingVector2.x / 2f);
        float y = wh.y / 2f - mGrid.cellHeight / 2f - PaddingVector2.y / 2f;

        mGrid.transform.localPosition = new Vector3(x, y, 0f);
    }

    public void Reposition()
    {
        mGrid.repositionNow = true;

        Vector2 wh = getWidthAndHeight() + PaddingVector2;
        
        mBg.width = (int)wh.x;
        mBg.height = (int)wh.y;
    }

    Vector2 getWidthAndHeight()
    {
        int x = items.Count < mGrid.maxPerLine ? items.Count : mGrid.maxPerLine;
        int y = items.Count/mGrid.maxPerLine + (items.Count%mGrid.maxPerLine == 0 ? 0 : 1);

        return new Vector2(x * mGrid.cellWidth, y * mGrid.cellHeight);
    }

    public void clear()
    {
        foreach (GameObject go in items.Values)
        {
            if (go != null)
            {
                GameObject.DestroyImmediate(go);
            }
        }

        items.Clear();
    }
}

public class UICityForm : UIWindow
{
    //protected UISprite mBg;
    /// 助手按钮;
    public UIButton mHelpBtn;
    /// 邮件按钮;
    public UIButton mEmailBtn;
    /// 邮件按钮特效;
    public UISprite mMailAnimation;
    /// 邮件按钮小标数字;
    public UILabel mUnReadNum;
    //聊天按钮
    public UIButton mChatBtn;
    /// 功能按钮 显示/隐藏 开关按钮;
    public UIButton mFuncToggleBtn;
    /// 活动列表根节点，所有活动按钮加在该节点下;
    public GameObject mActiMenuRoot;
    /// 功能列别根节点，所有功能按钮加在该节点下;
    public GameObject mFuncMenuRoot;
    /// 打开或者关闭功能按钮列表的时间;
    public float time = 1f;
    /// 所有活动按钮字典;
    private Dictionary<int, GameObject> mActiBtnDict = new Dictionary<int, GameObject>();
    /// 所有功能按钮字典;
    private Dictionary<int, GameObject> mFuncBtnDict = new Dictionary<int, GameObject>();
    /// 是否功能按键列表正在打开;
    private bool isOpening = false;

    //private Vector3 rot = new Vector3(0.0f, 0.0f, 90.0f);

    //headUI
    public UISprite faceSp;
    public UILabel levelLb;
    public UILabel nameLb;
    public UILabel gradeLb;


    private UISpriteAnimation mAnimation = null;


    //FuncIcon
    private GameObject mFuncIcon = null;


    private string mLastOpenName = null;

    protected GameObject mSubMenuObj = null;
    private SubMenuForm mSubMenuForm = null;
    private int mLastOpenSubId = -1;


    private UIButton mChatUI = null;
    private UILabel mChatLabel = null;

    SubMenuForm subMenuForm 
    {
        get
        {
            if (mSubMenuForm == null)
            {
                if (mSubMenuObj == null)
                    return null;

                mSubMenuForm = new SubMenuForm(mSubMenuObj);
            }
            return mSubMenuForm;
        }
    }

    public UICityForm()
    {
        
    }
    protected override void OnLoad()
    {
        //mBg = this.FindComponent<UISprite>("bg");

        mChatUI = this.FindComponent<UIButton>("ChatUI");
        mChatLabel = this.FindComponent<UILabel>("ChatUI/Label");

        mHelpBtn = this.FindComponent<UIButton>("ActivityMenuUI/HelpBtn");
        mChatBtn = this.FindComponent<UIButton>("ActivityMenuUI/ChatBtn");
        mEmailBtn = this.FindComponent<UIButton>("HeadUI/EmailBtn");
        mMailAnimation = this.FindComponent<UISprite>("HeadUI/EmailBtn/MailAnimation");
        mUnReadNum = this.FindComponent<UILabel>("HeadUI/EmailBtn/MailAnimation/UnRead/NUM");
        mFuncToggleBtn = this.FindComponent<UIButton>("FuncMenuUI/ToggleBtn");

        mAnimation = this.FindComponent<UISpriteAnimation>("FuncMenuUI/ToggleBtn/toggleAni");

        mActiMenuRoot = this.FindChild("ActivityMenuUI/ActiMenusRoot");
        mFuncMenuRoot = this.FindChild("FuncMenuUI/FuncMenusRoot");

        faceSp = this.FindComponent<UISprite>("HeadUI/HeadBG/faceSp");
        levelLb = this.FindComponent<UILabel>("HeadUI/HeadBG/LevelLb");
        nameLb = this.FindComponent<UILabel>("HeadUI/HeadBG/NameLb");
        gradeLb = this.FindComponent<UILabel>("HeadUI/GradeBG/GradeLb");

        mFuncIcon = this.FindChild("Items/FuncButton");

        //UISprite spr = this.FindComponent<UISprite>("Sprite");
        //NGUITools.SetActive(spr.gameObject, false);

        mSubMenuObj = this.FindChild("SubMenuForm");

//         spr.material.shader = Shader.Find("Unlit/Transparent Colored Mask");
//         Texture t = Resources.Load("Texture/Mask") as Texture;
//         spr.material.SetTexture("_MaskTex", t);

//         Material obj = Resources.Load("Texture/Mask") as Material;
//         mtl.mainTexture = rd.material.mainTexture;
//         if (rd.material.shader.name == "FantasyEngine/weapon/environment")
//             mtl.SetTexture("_HightLightTex", rd.material.GetTexture("_HightLightTex"));
//         rd.material = mtl;
        //添加事件
        //UIEventListener.Get(mBg.gameObject).onClick = OnBlankClick;

        EventDelegate.Add(mHelpBtn.onClick, OnHelpBtnClick);
        EventDelegate.Add(mFuncToggleBtn.onClick, OnFuncToggleBtnClick);
        EventDelegate.Add(mEmailBtn.onClick, OnEmailBtnClick);

        //EventDelegate.Add(mChatBtn.onClick, OnChatBtnClick);
        EventDelegate.Add(mChatUI.onClick, OnChatBtnClick);

        mChatLabel.text = "";

    }
    protected override void OnOpen(object param = null)
    {
        //Head
        //EventSystem.Instance.addEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, onPlayerMainPropChange);
        //EventSystem.Instance.addEventListener(PropertyEvent.FIGHT_PROPERTY_CHANGE, onPlayerFightPropChange);
        EventSystem.Instance.addEventListener(PropertyEvent.PLAYER_DATA_PROPERTY_CHANGED, onPlayerPropChange);

        updateUI();
        //RefreshHeadUI();
        //RefreshFightProp();

        //
        EventSystem.Instance.addEventListener(FunctionEvent.FUNCTION_LOCKED, OnFunctionLocked);
        EventSystem.Instance.addEventListener(FunctionEvent.FUNCTION_UNLOCKED, OnFuntionUnlock);
        EventSystem.Instance.addEventListener(FunctionEvent.FUNCTION_RED_POINT, onRedPointEvent);

        EventSystem.Instance.addEventListener(ChatEvent.MODULE_TO_UI_MESSAGE_UPDATE, OnChatUIUpdate);

        UpdateAllMenus();

        mFuncMenuRoot.gameObject.transform.localScale = Vector3.zero;
        mFuncToggleBtn.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        isOpening = false;

        startToggltAni();
        if (!isOpening)
        {
            stopToggleAni();
        }

        ZhushouManager.Instance.Begin();
        PlayerDataModule mPlayerDataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (mPlayerDataModule.GetMailNumber() <= 0)
        {
            if (NGUITools.GetActive(mEmailBtn.gameObject))
            {
                NGUITools.SetActive(mEmailBtn.gameObject, false);
            }
        }
        if (mPlayerDataModule.GetUnreadMailNumber() > 0)
        {
            NGUITools.SetActive(mMailAnimation.gameObject, true);
            mUnReadNum.text = (mPlayerDataModule.GetUnreadMailNumber()).ToString();
        }
        EventSystem.Instance.addEventListener(MailEvent.MAIL_NUMBER_UPDATE, OnMailNumUpdate);
        EventSystem.Instance.addEventListener(MailEvent.MAIL_UNREAD_NUMBER_UPDATE, OnUnReadMailNumUpdate);
    }

    void startToggltAni()
    {
        mAnimation.gameObject.SetActive(true);
        mAnimation.Reset();
    }

    void stopToggleAni() 
    {
        mAnimation.Stop();
        mAnimation.gameObject.SetActive(false);
    }
    private void OnMailNumUpdate(EventBase e)
    {
        MailEvent evt = (MailEvent)e;
        if (evt.number > 0)
        {
            if (!NGUITools.GetActive(mEmailBtn.gameObject))
            {
                NGUITools.SetActive(mEmailBtn.gameObject, true);
            }
        }
    }
    private void OnUnReadMailNumUpdate(EventBase e)
    {
        MailEvent evt = (MailEvent)e;
        if (evt.unreadnumber > 0)
        {
            NGUITools.SetActive(mMailAnimation.gameObject, true);
            mUnReadNum.text = (evt.unreadnumber).ToString();
        }
        else if (evt.unreadnumber == 0)
        {
            NGUITools.SetActive(mMailAnimation.gameObject, false);
            mUnReadNum.text = "";
        }
    }
    protected override void OnClose()
    {
        EventSystem.Instance.removeEventListener(PropertyEvent.PLAYER_DATA_PROPERTY_CHANGED, onPlayerPropChange);
        //EventSystem.Instance.removeEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, onPlayerMainPropChange);
        //EventSystem.Instance.removeEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, onPlayerFightPropChange);

        EventSystem.Instance.removeEventListener(FunctionEvent.FUNCTION_LOCKED, OnFunctionLocked);
        EventSystem.Instance.removeEventListener(FunctionEvent.FUNCTION_UNLOCKED, OnFuntionUnlock);
        EventSystem.Instance.removeEventListener(FunctionEvent.FUNCTION_RED_POINT, onRedPointEvent);
        EventSystem.Instance.removeEventListener(MailEvent.MAIL_NUMBER_UPDATE, OnMailNumUpdate);
        EventSystem.Instance.removeEventListener(MailEvent.MAIL_UNREAD_NUMBER_UPDATE, OnUnReadMailNumUpdate);

        EventSystem.Instance.removeEventListener(ChatEvent.MODULE_TO_UI_MESSAGE_UPDATE, OnChatUIUpdate);


        if (mFuncMenuRoot != null)
        {
            //防止播放动画途中关闭界面导致缩放显示不正确;
            mFuncMenuRoot.transform.localScale = isOpening ? Vector3.one : Vector3.zero;
        }

        ZhushouManager.Instance.EndPaoPao();
    }


    void OnChatUIUpdate(EventBase e)
    {
        ChatEvent evt = (ChatEvent)e;
        string channelname = "[fed514]【城市】";
        if (evt.channel_type == (int)ChatChannelType.ChannelType_World)
            channelname = "[3eff00]【世界】";
        if (evt.channel_type == (int)ChatChannelType.ChannelType_System)
            channelname = "[e92224]【系统】";
        string text = evt.msg;
        if (text.Length > 10)
        {
            text = text.Substring(0, 10);
            text += "....";
        }

        mChatLabel.text = (channelname + "[u]" + evt.name + "[/u]:[-] " + text);
    }

    void UpdateAllMenus()
    {
        FunctionModule module = ModuleManager.Instance.FindModule<FunctionModule>();

        Hashtable ht = module.GetUnlocks();
        IDictionaryEnumerator itr = ht.GetEnumerator();
        while (itr.MoveNext())
        {
            AddMenu(Convert.ToInt32(itr.Key));
        }
//         foreach (DictionaryEntry de in ht)
//         {
//             //MenuTableItem item = DataManager.MenuTable[(int)de.Key] as MenuTableItem;
// 
//             //if (FunctionModule.IsParentOrChildType(item))
//             //    return;
// 
//             
//         }
    }

    void OnFunctionLocked(EventBase evt)
    {
        FunctionEvent fe = (FunctionEvent) evt;

        RemoveMenu(fe.functionid);
    }

    void RemoveMenu(int resid)
    {
        if (mActiBtnDict.ContainsKey(resid))
        {
            RemoveBtn(mActiMenuRoot, mActiBtnDict, resid);
        }
        else if(mFuncBtnDict.ContainsKey(resid))
        {
            RemoveBtn(mFuncMenuRoot, mFuncBtnDict, resid);
        }
        else if(mSubMenuForm.SubMenuDict.ContainsKey(resid))
        {
            RemoveBtn(mSubMenuForm.grid, mSubMenuForm.SubMenuDict, resid);
        }
    }

    void OnFuntionUnlock(EventBase evt)
    {
        FunctionEvent ev = (FunctionEvent)evt;
        AddMenu(ev.functionid);
    }

    void AddMenu(int functionid)
    {
        //处理解锁
        if (functionid != -1 && DataManager.MenuTable.ContainsKey(functionid))
        {
            MenuTableItem item = DataManager.MenuTable[functionid] as MenuTableItem;

            if (item.type == (int)FunctionType.FunctionActivtiy)
            {
                AddBtn(item.uiName, mActiMenuRoot, mActiBtnDict, functionid, item.icon, new Quaternion(0, 180, 0, 0));
            }
            else if (item.type == (int)FunctionType.FunctionFunc)
            {
                AddBtn(item.uiName, mFuncMenuRoot, mFuncBtnDict, functionid, item.icon, new Quaternion(0, 0, 180, 0));
            }
        }
    }

    public override void Update(uint elapsed)
    {
        UnityEngine.Vector3 pos = WindowManager.current2DCamera.WorldToScreenPoint(mHelpBtn.gameObject.transform.position);

        BoxCollider collider = mHelpBtn.collider as BoxCollider;

        pos.x -= (collider.size.x / 2.0f -70);
        pos.y -= (collider.size.y/2.0f - 30);
        ZhushouManager.Instance.Update(pos, elapsed);
        PlayerDataModule mPlayerDataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (mPlayerDataModule.GetMailNumber() <= 0)
        {
            if (NGUITools.GetActive(mEmailBtn.gameObject))
            {
                NGUITools.SetActive(mEmailBtn.gameObject, false);
            }
        }
        else
        {
            if (!NGUITools.GetActive(mEmailBtn.gameObject))
            {
                NGUITools.SetActive(mEmailBtn.gameObject, true);
            }
        }
    }

    /// <summary>
    /// 功能界面按钮点击后的操作
    /// </summary>
    void OnFuncToggleBtnClick()
    {
        closeSubMenuForm();

        Vector3 rot = Vector3.zero;
        if( !isOpening )
        {
            rot = new Vector3(0.0f, 0.0f, 90.0f);
        }
        TweenRotation tween = TweenRotation.Begin(mFuncToggleBtn.gameObject, 0.5f, Quaternion.Euler(rot));
        tween.AddOnFinished(OnRotFinished);
        tween.method = UITweener.Method.EaseInOut;

        ShowOrHideFuncBtn();

    }

    void OnRotFinished()
    {
        startToggltAni();
        if( !isOpening )
        {
            stopToggleAni();
        }
    }

    /// <summary>
    /// 显示或者隐藏功能按钮;
    /// </summary>
    void ShowOrHideFuncBtn()
    {
        //		bool isShow = mFuncMenuRoot.activeSelf;
        //		mFuncMenuRoot.gameObject.SetActive(!isShow);

        //		TweenScale ts = NGUITools.AddMissingComponent<TweenScale>(mFuncMenuRoot);
        //		ts.method = UITweener.Method.EaseInOut;

        if (isOpening)
        {
            TweenScale ts = TweenScale.Begin(mFuncMenuRoot.gameObject, time, Vector3.zero);
            if (ts != null)
            {
                ts.method = UITweener.Method.EaseInOut;
                ts.PlayForward();
                isOpening = false;
            }
        }
        else
        {
            TweenScale ts = TweenScale.Begin(mFuncMenuRoot.gameObject, time, Vector3.one);
            if (ts != null)
            {
                ts.method = UITweener.Method.EaseInOut;
                ts.PlayForward();
                isOpening = true;
            }
        }
    }

    //void OnBlankClick(GameObject go)
    //{
    //    closeSubMenuForm();
    //}

    /// <summary>
    /// 助手按钮点击后的操作;
    /// </summary>
    void OnHelpBtnClick()
    {
        closeSubMenuForm();
        ZhushouManager.Instance.Execute();
    }

    /// <summary>
    /// 邮件按钮点击操作;
    /// </summary>
    void OnEmailBtnClick()
    {
        closeSubMenuForm();
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_MAIL_LOAD, null);
    }
    void OnChatBtnClick()
    {
        closeSubMenuForm();
        WindowManager.Instance.OpenUI("chat");
    }

    public Vector3 GetMenuPos(int menuid, FunctionType type)
    {
        Dictionary<int, GameObject> temp = null;

        if (type == FunctionType.FunctionActivtiy)
            temp = mActiBtnDict;
        else if (type == FunctionType.FunctionFunc)
            temp = mFuncBtnDict;

        if (!temp.ContainsKey(menuid))
            return Vector3.zero;

        return temp[menuid].transform.position;
    }

    public Vector3 GetNextMenuPos(int type)
    {
        GameObject menuRoot = null;
        if (type == (int)FunctionType.FunctionActivtiy)
        {
            menuRoot = mActiMenuRoot;
        }
        else if (type == (int)FunctionType.FunctionFunc)
        {
            menuRoot = mFuncMenuRoot;
        }

        GameObject tempObject = System.Activator.CreateInstance<GameObject>() as GameObject;
        tempObject.transform.parent = menuRoot.transform;
        tempObject.transform.localScale = Vector3.one;

        UIGrid grid = menuRoot.GetComponent<UIGrid>();
        if (grid != null)
            grid.Reposition();

        Vector3 pos = tempObject.transform.position;

        GameObject.Destroy(tempObject);

        return pos;
    }

    /// <summary>
    /// 在某个GameObject下添加按钮
    /// </summary>
    void AddBtn(string uiName, GameObject rootObj, Dictionary<int, GameObject> dict, int functionid, string icon, Quaternion rotate)
    {
        if (!rootObj /*|| string.IsNullOrEmpty(titleName) || (dict == null) || (rotate == null)*/)
            return;

        //已经存在了
        if (dict.ContainsKey(functionid))
            return;

        //GameObject go = ResourceManager.Instance.LoadUI("UI/city/FuncButton");
        GameObject go = UIResourceManager.Instance.CloneGameObject(mFuncIcon);
        if (!go)
        {
            GameDebug.LogError("Create Function Btn error");
            return;
        }

        go.name = uiName;

        UIButton btn = go.GetComponent<UIButton>();
        btn.CustomData = functionid;

        UIEventListener.Get(go).onClick = onFunctionClick;

        UISprite sprite = go.GetComponent<UISprite>();

        UIAtlasHelper.SetButtonImage(btn, icon, true);

        go.transform.parent = rootObj.transform;
        go.transform.localScale = Vector3.one;
        // 		go.name = titleName + dict.Count.ToString();
        go.transform.localRotation = rotate;

        //加入缓存
        dict.Add(functionid, go);


        TweenScale ts = NGUITools.AddMissingComponent<TweenScale>(go);
        ts.from = Vector3.zero;
        ts.to = Vector3.one;
        ts.PlayForward();

        RepositionGrid(rootObj);
    }

    /// <summary>
    /// Removes the button.
    /// </summary>
    void RemoveBtn(GameObject rootObj, Dictionary<int, GameObject> dict, int id)
    {
        if (!rootObj)
            return;

        if (!dict.ContainsKey(id))
            return;

        GameObject go = dict[id];
        GameObject.DestroyImmediate(go);

        dict.Remove(id);

        RepositionGrid(rootObj);
    }

    void RepositionGrid(GameObject rootObj)
    {
        if (!rootObj)
            return;

        UIGrid grid = rootObj.GetComponent<UIGrid>();
        if (grid != null)
            grid.repositionNow = true;
    }

    private void onFunctionClick(GameObject obj)
    {
      
        UIButton btn = obj.GetComponent<UIButton>();
        if (btn == null)
            return;
        int functionid = (int)btn.CustomData;
        MenuTableItem item = DataManager.MenuTable[functionid] as MenuTableItem;
        if (item == null)
            return;
        if (item.soundid != -1)
        {
            SoundManager.Instance.Play(item.soundid);
        }
        
        if (item.menuOpType == (uint)MenuOpType.MenuOpType_OpenUI)
        {
            closeSubMenuForm();

            if (string.IsNullOrEmpty(mLastOpenName) || 
                (!string.IsNullOrEmpty(mLastOpenName) && !WindowManager.Instance.IsOpen(mLastOpenName)))
            {
                WindowManager.Instance.OpenUI(item.param);
                mLastOpenName = item.param;
            }
        }
        else if (item.menuOpType == (uint)MenuOpType.MenuOpType_Scene)
        {
            closeSubMenuForm();

            string[] str = item.param.Split(new char[] { '|' });
            if (str.Length != 2)
            {
                return;
            }
            SceneManager.Instance.RequestEnterScene(int.Parse(str[1]));
        }
        else if (item.menuOpType == (uint) MenuOpType.MenuOpType_ParentUI)
        {
            FunctionModule fm = ModuleManager.Instance.FindModule<FunctionModule>();
            if(fm == null)
            {
                Debug.LogError("fuctionmodule not found error");
                return;
            }


            List<int> ids = fm.GetChildIdsByParentId(functionid);

            if (ids == null || ids.Count <= 0)
                return;

            if (functionid != mLastOpenSubId)
            {
                createSubMenuForm(ids, UICamera.currentCamera.WorldToScreenPoint(obj.transform.position));
                mLastOpenSubId = functionid;
            }

            openOrCloseSubMenuForm();
        }
    }

    /// <summary>
    /// 检测是否存在
    /// </summary>
    void checkParentFuncBtn()
    {

    }

    void createSubMenuForm(List<int> ids, Vector3 pos)
    {
        subMenuForm.clear();

        foreach (int id in ids)
        {
            MenuTableItem item = FunctionModule.GetItem(id);

            if(item == null)
                continue;

            AddBtn(item.uiName, subMenuForm.grid.gameObject, subMenuForm.SubMenuDict, id, item.icon, new Quaternion());
        }

        subMenuForm.Reposition();

        subMenuForm.SetSubFormPosition(pos);
    }

    void openOrCloseSubMenuForm()
    {
        subMenuForm.gameObject.SetActive(!mSubMenuForm.gameObject.activeSelf);
    }

    public void closeSubMenuForm()
    {
        if (subMenuForm == null)
            return;

        if (subMenuForm.gameObject.activeSelf)
            subMenuForm.gameObject.SetActive(false);
    }

    void onPlayerPropChange(EventBase ev)
    {
        updateUI();
    }

    void updateUI()
    {
        PlayerDataModule pd = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pd == null)
            return;

        nameLb.text = pd.GetName();
        levelLb.text = pd.GetLevel().ToString();
        gradeLb.text = pd.GetGrade().ToString();

        UIAtlasHelper.SetSpriteImage(faceSp, pd.GetFace());

    }

    void onRedPointEvent(EventBase ev)
    {
        if (ev == null) return;

        FunctionEvent fe = ev as FunctionEvent;

        if (fe == null) return;

        setRedPointActive(fe.functionid, fe.isShow);
    }

    void setRedPointActive(int funcionId , bool active)
    {
        GameObject go = null;

        if (mActiBtnDict.ContainsKey(funcionId))
        {
            go = mActiBtnDict[funcionId];
        }
        else if (mFuncBtnDict.ContainsKey(funcionId))
        {
            go = mFuncBtnDict[funcionId];
        }

        setRedPointActive(go, active);
    }

    void setRedPointActive(GameObject parent, bool active)
    {
        if (parent == null)
            return;

        GameObject go = ObjectCommon.GetChild(parent, "hongdian");
       
        if (go != null)
            go.SetActive(active);
    }

    //void onPlayerMainPropChange(EventBase ev)
    //{
    //    RefreshHeadUI();
    //}

    //void onPlayerFightPropChange(EventBase ev)
    //{
    //    RefreshFightProp();
    //}

    //void RefreshFightProp()
    //{
    //    PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

    //    gradeLb.text = module.GetFightingCapacity().ToString();
    //}

    //void RefreshHeadUI()
    //{
    //    PlayerDataModule pd = ModuleManager.Instance.FindModule<PlayerDataModule>();
    //    if (pd == null)
    //        return;

    //    // 		faceSp.spriteName = "";
    //    // 		faceSp.MakePixelPerfect();

    //    nameLb.text = pd.GetName();
    //    levelLb.text = pd.GetLevel().ToString();
    //}
}
