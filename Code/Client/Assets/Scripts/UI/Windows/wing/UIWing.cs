using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Message;

public class UIWing : UIWindow
{
    //翻页
    public UIButton pagePrev;
    public UIButton pagePrevAni;
    public UIButton pageNext;
    public UIButton pageNextAni;
    private LockStatePageBarUI pageBar;
    private GameObject pageSprite;

    // 是否需要初始化;
    private bool mNeedInit = true;

    private GameObject midBk;
    private GameObject wingItemProfab = null;
    private GameObject winggrid;
    private UILabel wingName;
    private UILabel tip;
    private GameObject WingDescript;
    private UILabel WingDescriptText;
    private UILabel buffDesc;
    private GameObject levelbar;
    private UILabel levelnum;
    private UILabel nextLevel;
    private UILabel levelPercent;
    private UISlider levelProcessbar;
    private UISprite levbarAni;

    //所需道具
    private UISprite stuff;
    private UILabel stuffname;
    private UILabel stuffnum;

    private UILabel jihuotiaojianTxt;
    private UILabel zhanValue;

    private GameObject wingPropObj;
    private List<WingPropertyItem> mProps;

    private UIButton jinglianBtn;
    private UILabel jinglianTxt;
    private UIButton equipBtn;
    private UILabel equipWord;
    private UIButton backBtn;
    private UIButton tryOnBtn;
    private UIButton jihuoBtn;
    private UISprite jihuoAni;

    public GameObject UnlockCondition;
    public List<wingConditionUI> mConditionUIList;
    private List<WingItemUI> mWingItemUIList;

    //预览
    private UISprite mPreviewSprite;
    private UICharacterPreview mCharacterPreview;

    private WingModule mWingMoudle;
    private bool mIsDirty = false;
    private bool mIsTryOn;
    public WingState mState;


    public UIWing()
    {

    }


    public bool IsTryOn
    {
        get { return mIsTryOn; }
        set
        {
            mIsTryOn = value;
            if(!value)
                mPreviewSprite.gameObject.SetActive(false);
        }
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        midBk = FindChild("Content/Background/MidBK");

        //翻页
        pagePrev = FindComponent<UIButton>("Content/Background/MidBK/WingList/pagePre");
        pagePrevAni = FindComponent<UIButton>("Content/Background/MidBK/WingList/pagePreAni");

        pageNext = FindComponent<UIButton>("Content/Background/MidBK/WingList/pageNext");
        pageNextAni = FindComponent<UIButton>("Content/Background/MidBK/WingList/pageNextAni");

        //翅膀
        wingItemProfab = FindChild("wingItem");
        mWingItemUIList = new List<WingItemUI>();
        winggrid = FindChild("Content/Background/MidBK/WingList/winggrid");
        wingName = FindComponent<UILabel>("Content/Background/MidBK/WingList/wingName");
        buffDesc = FindComponent<UILabel>("Content/Background/MidBK/WingList/bufftext");
        //翅膀试穿
        mPreviewSprite = FindComponent<UISprite>("Content/Background/MidBK/WingList/mPreView");
        mPreviewSprite.transform.localPosition = Vector3.zero;
        mCharacterPreview = new UICharacterPreview();
        wingItemProfab.SetActive(false);

        tip = FindComponent<UILabel>("Content/Background/MidBK/WingList/tip");
        tip.gameObject.SetActive(true);
        tip.pivot = UIWidget.Pivot.Left;
        tip.gameObject.transform.localPosition = new Vector3(-304f,-145f,0);
        pageSprite = FindChild("pageItem");
        pageBar = new LockStatePageBarUI(FindChild("Content/Background/MidBK/lockStateBar"), WingDefine.MaxWingNum,
            pageSprite);

        WingDescript = FindChild("Content/Background/WingDescript");
        WingDescriptText = FindComponent<UILabel>("Content/Background/WingDescript/WingDescriptText");

        //翅膀进度条
        levelbar = FindChild("Content/Background/levelbar");
        levelnum = FindComponent<UILabel>("Content/Background/levelbar/levelnum");
        nextLevel = FindComponent<UILabel>("Content/Background/levelbar/nextLevel");
        levelPercent = FindComponent<UILabel>("Content/Background/levelbar/levelPercent");
        levelProcessbar = FindComponent<UISlider>("Content/Background/levelbar/levelProcessbar");
        levbarAni = FindComponent<UISprite>("Content/Background/levelbar/levbarAni");
        levbarAni.gameObject.SetActive(false);
        UISpriteAnimation ani = levbarAni.gameObject.GetComponent<UISpriteAnimation>();
        ani.Reset();
        ani.onFinished += AnimationFinesh;

        //翅膀属性
        wingPropObj = FindChild("Content/Background/wingPrperty");
        mProps = new List<WingPropertyItem>();
        for (int i = 0; i < WingDefine.PropertyNum; ++i)
        {
            var tempP = new WingPropertyItem(ObjectCommon.GetChild(wingPropObj, "Grid/prop" + (i + 1)))
            {
                mType = (WingPropertyType) i
            };
            mProps.Add(tempP);
        }

        //消耗物品
        stuff = FindComponent<UISprite>("Content/Background/wingPrperty/stuff");
        stuffname = FindComponent<UILabel>("Content/Background/wingPrperty/stuff/stuffname");
        stuffnum = FindComponent<UILabel>("Content/Background/wingPrperty/stuff/stuffnum");

        jihuotiaojianTxt = FindComponent<UILabel>("Content/Background/jihuotiaojian/jihuotiaojianTxt");
        backBtn = FindComponent<UIButton>("Content/Background/MidBK/WingList/backBtn");
        tryOnBtn = FindComponent<UIButton>("Content/Background/MidBK/WingList/wearBtn");
        jihuoAni = FindComponent<UISprite>("Content/Background/UnlockCondition/jihuoAni");
        jihuoAni.gameObject.SetActive(false);
        jihuoBtn = FindComponent<UIButton>("Content/Background/UnlockCondition/jihuoBtn");
        zhanValue = FindComponent<UILabel>("Content/Background/jihuotiaojian/zhanBK/zhanValue");
        jinglianBtn = FindComponent<UIButton>("Content/Background/wingPrperty/jinglianBtn");
        jinglianTxt = FindComponent<UILabel>("Content/Background/wingPrperty/jinglianBtn/Label");
        equipBtn = FindComponent<UIButton>("Content/Background/wingPrperty/equipBtn");
        equipWord = FindComponent<UILabel>("Content/Background/wingPrperty/equipBtn/Label");
        UnlockCondition = FindChild("Content/Background/UnlockCondition");
        mConditionUIList = new List<wingConditionUI>();
        for (int i = 0; i < WingDefine.MaxConditonNum; ++i)
        {
            mConditionUIList.Add(new wingConditionUI(ObjectCommon.GetChild(UnlockCondition, "Grid/dachengdiaojian" + (i + 1))));
        }

        if (mNeedInit)
        {
            Init();
        }
    }

    private void Init()
    {
        mNeedInit = false;
        mWingMoudle = ModuleManager.Instance.FindModule<WingModule>();
        for (int i = 1; i <= mWingMoudle.GetWingPageCount(); ++i)
        {
            CreateWingItem(i);
        }
        winggrid.GetComponent<UIGrid>().repositionNow = true;
        CurPageNum = 1;
        upDownAniHandler();
        mIsDirty = true;

    }

    private void upDownAniHandler()
    {
        if (mWingItemUIList[CurPageNum - 1].IsTryOn)
        {
            pageNextAni.gameObject.SetActive(false);
            pagePrevAni.gameObject.SetActive(false);
            pageNext.gameObject.SetActive(false);
            pagePrev.gameObject.SetActive(false);
        }
        else
        {
            switch (getCurPagePos())
            {
                case Pos.FirstLast:
                    pageNextAni.gameObject.SetActive(false);
                    pagePrevAni.gameObject.SetActive(false);
                    pageNext.gameObject.SetActive(false);
                    pagePrev.gameObject.SetActive(false);
                    break;
                case Pos.First:
                    pagePrevAni.gameObject.SetActive(false);
                    pagePrev.gameObject.SetActive(true);
                    pageNextAni.gameObject.SetActive(true);
                    pageNext.gameObject.SetActive(false);
                    break;
                case Pos.Last:
                    pageNextAni.gameObject.SetActive(false);
                    pageNext.gameObject.SetActive(true);
                    pagePrevAni.gameObject.SetActive(true);
                    pagePrev.gameObject.SetActive(false);
                    break;
                case Pos.Med:
                    pageNextAni.gameObject.SetActive(true);
                    pagePrevAni.gameObject.SetActive(true);
                    pageNext.gameObject.SetActive(false);
                    pagePrev.gameObject.SetActive(false);
                    break;
            }
        }


        UISpriteAnimation ani1 = pageNextAni.GetComponent<UISpriteAnimation>();
        if (ani1 != null) ani1.Reset();

        UISpriteAnimation ani2 = pagePrevAni.GetComponent<UISpriteAnimation>();
        if (ani2 != null) ani2.Reset();
    }

    private Pos getCurPagePos()
    {
        if (IsFirstPage && IsLastPage)
            return Pos.FirstLast;

        if (IsFirstPage)
            return Pos.First;

        if (IsLastPage)
            return Pos.Last;

        return Pos.Med;
    }

    public bool IsLastPage
    {
        get
        {
            return mWingMoudle.mCurPageNum >= mWingMoudle.GetWingPageCount();
        }
    }

    public bool IsFirstPage
    {
        get
        {
            return mWingMoudle.mCurPageNum <= 1;
        }
    }

    /// <summary>
    /// 下翻页;
    /// </summary>
    private void PageNext()
    {
        SoundManager.Instance.Play(15);
        if (IsLastPage)
        {
            //已经是最后一页了，如何处理;
            return;
        }

        CurPageNum++;
        upDownAniHandler();
    }

    /// <summary>
    /// 上翻页
    /// </summary>
    private void PagePrev()
    {
        SoundManager.Instance.Play(15);
        if (IsFirstPage)
        {
            return;
        }

        CurPageNum--;
        upDownAniHandler();
    }

    public int CurPageNum
    {
        get
        {
            return mWingMoudle.mCurPageNum;
        }
        set
        {
            //保证当前页数的合法性;upuu
            int effPageNum = value > 0 ? Mathf.Min(value, mWingMoudle.GetWingPageCount()) : Mathf.Max(value, 1);

            if (effPageNum != mWingMoudle.mCurPageNum)
            {
                mWingMoudle.mCurPageNum = effPageNum;
            }

            pageBar.CurPage = mWingMoudle.mCurPageNum;
            float x = (mWingMoudle.mCurPageNum - 1)*650f;
            TweenPosition tp = TweenPosition.Begin(winggrid, 0.3f, new Vector3(-x, 0f, 0f));
            EventDelegate.Add(tp.onFinished, OnPageChange);
            tp.PlayForward();

        }
    }

    private void OnPageChange()
    {
        mIsDirty = true;
    }

    private void CreateWingItem(int index)
    {
        GameObject go = UIResourceManager.Instance.CloneGameObject(wingItemProfab);

        if (go == null)
        {
            GameDebug.Log("wingItemProfab not found");
        }

        UIEventListener.Get(go).onDrag = OnWingItemDrag;
        UIEventTrigger et = go.GetComponent<UIEventTrigger>();
        EventDelegate.Add(et.onPress, OnWingItemPress);
        EventDelegate.Add(et.onRelease, OnWingItemRelease);
        go.SetActive(true);
        go.name = index.ToString(CultureInfo.InvariantCulture);
        go.transform.parent = winggrid.transform;
        go.transform.localScale = Vector3.one;
        WingItemUI tempItem = new WingItemUI(go);
        var commonRes =  DataManager.WingCommonTable[mWingMoudle.GetWingIdByIndex(index)] as WingCommonTableItem;
        UIAtlasHelper.SetSpriteImage(tempItem.mWingSprite, commonRes.wingPicLow);
        mWingItemUIList.Add(tempItem);

        //tempItem.mWingPicAni = AnimationManager.Instance.CreateParticleAnimation(4, go);
       // GameDebug.Log(commonRes.wingPicAni);
       // tempItem.mWingPicAni.gameObject.SetActive(true);
//         tempItem.mWingPicAni.RebuildSpriteList();
//         tempItem.mWingPicAni.Reset();
    }


    public WingState State
    {
        get { return mState; }
        set
        {
            mState = value;
            switch (value)
            {
                case WingState.Locked:
                    WingDescript.SetActive(true);
                    levelbar.SetActive(false);
                    pageBar.mView.SetActive(true);
                    wingPropObj.SetActive(false);
                    UnlockCondition.SetActive(true);
                    buffDesc.gameObject.SetActive(true);
                    break;

                case WingState.UnLocked:
                case WingState.Wear:
                    if (IsTryOn)
                    {
                        WingDescript.SetActive(false);
                        levelbar.SetActive(true);
                        pageBar.mView.SetActive(true);
                        wingPropObj.SetActive(true);
                        UnlockCondition.SetActive(false);
                        buffDesc.gameObject.SetActive(false);

                    }
                    else
                    {
                        WingDescript.SetActive(false);
                        levelbar.SetActive(true);
                        pageBar.mView.SetActive(true);
                        wingPropObj.SetActive(true);
                        UnlockCondition.SetActive(false);
                        buffDesc.gameObject.SetActive(true);
                    }
                    break;
            }

            if (IsTryOn)
            {
                tryOnBtn.gameObject.SetActive(false);
                backBtn.gameObject.SetActive(true);
                wingName.gameObject.SetActive(false);
            }
            else
            {
                tryOnBtn.gameObject.SetActive(true);
                backBtn.gameObject.SetActive(false);
                wingName.gameObject.SetActive(true);
            }
        }
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen(param);
        AddEventListener();
        AddEventDelegate();

        mWingItemUIList[CurPageNum - 1].IsTryOn = false;
        mWingItemUIList[CurPageNum - 1].mView.SetActive(true);
        mPreviewSprite.gameObject.SetActive(false);
        mCharacterPreview.RotationY = 180;
        upDownAniHandler();

        if (mCharacterPreview != null)
            mCharacterPreview.Enable = false;
        
        if (mWingItemUIList.Count < mWingMoudle.GetWingPageCount())
        {
            for (int i = mWingItemUIList.Count + 1; i <= mWingMoudle.GetWingPageCount(); ++i)
            {
                CreateWingItem(i);
            }

            winggrid.GetComponent<UIGrid>().repositionNow = true;
        }
        else if (mWingItemUIList.Count > mWingMoudle.GetWingPageCount())
        {
            foreach (var wingItemUi in mWingItemUIList)
            {
                wingItemUi.Clear();
            }

            mWingItemUIList.Clear();
            Init();
        }
        mCharacterPreview.BackgroundSprite = mPreviewSprite;
        mCharacterPreview.SetTargetSprite(mPreviewSprite);
        mCharacterPreview.SetCameraOrthographicSize(1.5f);
        mCharacterPreview.RotationY = 180;
        upDownAniHandler();
        mIsDirty = true;
    }

    protected override void OnClose()
    {
        base.OnClose();
        RemoveEventListener();
        RemoveEventDelegate();

        if (mCharacterPreview != null)
            mCharacterPreview.Enable = false;
    }


    private void AddEventListener()
    {
        EventSystem.Instance.addEventListener(WingUIEvent.Wing_UI_UPDATE, OnUpdateUI);
        EventSystem.Instance.addEventListener(WingUIEvent.WING_UI_ACTIVE, OnRespondActive);
        EventSystem.Instance.addEventListener(WingUIEvent.WING_UI_EQUIP, OnRespondEquip);
        EventSystem.Instance.addEventListener(WingUIEvent.WING_UI_FORGE, OnRespondForge);
        EventSystem.Instance.addEventListener(ItemEvent.UPDATE_CHANGE,OnUpdateItem);

    }

    private void RemoveEventListener()
    {
        EventSystem.Instance.removeEventListener(WingUIEvent.Wing_UI_UPDATE, OnUpdateUI);
        EventSystem.Instance.removeEventListener(WingUIEvent.WING_UI_ACTIVE, OnRespondActive);
        EventSystem.Instance.removeEventListener(WingUIEvent.WING_UI_EQUIP, OnRespondEquip);
        EventSystem.Instance.removeEventListener(WingUIEvent.WING_UI_FORGE, OnRespondForge);
        EventSystem.Instance.removeEventListener(ItemEvent.UPDATE_CHANGE, OnUpdateItem);
    }

    private void OnUpdateItem(EventBase evt)
    {
        var mCommorRes = mWingMoudle.GetCommonResByIndex(CurPageNum);
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        stuffnum.text = pdm.GetItemNumByID(mCommorRes.costId).ToString();
    }
    private void OnUpdateUI(EventBase evt)
    {
        UpdateUI();
    }



    private void OnRespondActive(EventBase evt)
    {
        WingUIEvent wevt = evt as WingUIEvent;
        if (wevt.result == (int)ERROR_CODE.ERR_WING_ACTIVE_OK)
        {
            jihuoAni.gameObject.SetActive(false);
            GameDebug.Log("激活" + mWingMoudle.GetWingPageCount());
            CreateWingItem(mWingMoudle.GetWingPageCount());
            winggrid.GetComponent<UIGrid>().repositionNow = true;
            upDownAniHandler();
        }


    }

    private void OnRespondEquip(EventBase evt)
    {
        WingUIEvent wevt = evt as WingUIEvent;

        if (wevt.action == 0)
        {
            equipWord.text = StringHelper.GetString("wing_takeoff");
            UIAtlasHelper.SetButtonImage(equipBtn, "common:btn_blue_4word");

        }
        else if (wevt.action == 1)
        {
            equipWord.text = StringHelper.GetString("wing_equip");
            UIAtlasHelper.SetButtonImage(equipBtn, "common:btn_yellow_4word");
        }
    }

    private void OnRespondForge(EventBase evt)
    {
        levbarAni.gameObject.SetActive(true);
        UISpriteAnimation ani = levbarAni.gameObject.GetComponent<UISpriteAnimation>();
        ani.Reset();
        Player ply = PlayerController.Instance.GetControlObj() as Player;
        int wingid = mWingMoudle.GetWingIdByIndex(CurPageNum);      
        uint wingLevel = mWingMoudle.GetWingLevel(wingid);       
        if (!mWingMoudle.IsReEquip) return;     
        mCharacterPreview.ChangeWing(wingid, wingLevel);
        if (mWingMoudle.GetWingState(CurPageNum) == WingState.Wear)
        {
            ply.WingEquip(mWingMoudle.GetWingIdByIndex(CurPageNum), 0);
        }      
        mWingMoudle.IsReEquip = false;
       
        
    }


    public override void Update(uint elapsed)
    {
        base.Update(elapsed);
        if (mIsDirty)
        {
            UpdateUI();
        }

        if (mCharacterPreview.Enable != mPreviewSprite.gameObject.activeInHierarchy)
        {
            mCharacterPreview.Enable = mPreviewSprite.gameObject.activeInHierarchy;
        }

        mCharacterPreview.Update();
    }
 
    private void AnimationFinesh(GameObject go)
    {
        go.SetActive(false);
    }

    private void UpdateUI()
    {
        mIsDirty = false;
        jinglianBtn.isEnabled = !mWingMoudle.IsFullLevel();
        jinglianTxt.applyGradient = jinglianBtn.isEnabled;
       
        WingCommonTableItem mCommorRes;

        WingItemUI wing_item_ui = mWingItemUIList[CurPageNum - 1];

        if (wing_item_ui == null)
        {
            GameDebug.LogError("获取wing_item_ui失败");
            return;
        }

        IsTryOn = wing_item_ui.IsTryOn;
        State = mWingMoudle.GetWingState(CurPageNum);
        wing_item_ui.State = State;
        mCommorRes = mWingMoudle.GetCommonResByIndex(CurPageNum);
        //GameDebug.Log("state:"+State);
        wingName.text = mCommorRes.wingName;
        
        if (State == WingState.Locked)
        {
            int condition;
            bool isActive = mWingMoudle.CheckCondition(ref mCommorRes, out condition);
            wing_item_ui.SetDaChengPicVisable(Convert.ToInt32(condition));
            WingDescriptText.text = mCommorRes.desc;
            jihuotiaojianTxt.text = StringHelper.GetString("wing_condition");
            zhanValue.text = StringHelper.GetString("wing_no_active");
            for (int i = 0; i < WingDefine.MaxConditonNum; ++i)
            {
                if ((condition & (1 << i)) == 0) //条件没有达成
                {
                    UIAtlasHelper.SetSpriteImage(mConditionUIList[i].biaoji1, "wing0:wing0-nocom", true);
                    UIAtlasHelper.SetSpriteImage(mConditionUIList[i].num1, "wing0:wing0-019");
                }
                else
                {
                    UIAtlasHelper.SetSpriteImage(mConditionUIList[i].biaoji1, "wing0:wing0-003", true);
                    UIAtlasHelper.SetSpriteImage(mConditionUIList[i].num1, "wing0:wing0-011");
                }

                Type tp = mCommorRes.GetType();
                int conditionId = Convert.ToInt32(tp.GetField(("condition" + (i + 1))).GetValue(mCommorRes));
                ConditionTableItem cti = DataManager.ConditionTable[conditionId] as ConditionTableItem;
                if (cti != null)
                {
                    mConditionUIList[i].content1.text = cti.mDesc;
                }
            }

            jihuoAni.gameObject.SetActive(isActive);
            equipWord.text = StringHelper.GetString("wing_equip");
            UIAtlasHelper.SetButtonImage(equipBtn, "common:btn_yellow_4word");
            tip.gameObject.SetActive(true);
            tip.text = StringHelper.GetString("wingtip").Replace("?", mCommorRes.middleLevel.ToString());
        }
        else
        {
            WingItemData wing_item_data;
            WingLevelTableItem wing_level_res;
            
            if (!mWingMoudle.GetWingItemData(CurPageNum, out wing_item_data)) return;
            if (!mWingMoudle.GetWingLevelRes(wing_item_data.id, (int) wing_item_data.level, out wing_level_res)) return;
            levelnum.text = "LV" + wing_item_data.level;
            zhanValue.text = wing_level_res.battleScore.ToString();
            jihuotiaojianTxt.text = StringHelper.GetString("wing_property");
            if (wing_item_data.level < WingDefine.Max_Wing_Level)
            {
                string re = StringHelper.GetString("nextlevel") + ":";
                var next_level_res = DataManager.WingLevelTable[wing_item_data.id*1000 + wing_item_data.level + 1] as WingLevelTableItem;
                if (next_level_res != null)
                {                 
                    nextLevel.text = re + GetPropString(next_level_res.propertyType, next_level_res.propertyNum);
                    levelPercent.text = wing_item_data.process + "/" + next_level_res.costNum;
                    levelProcessbar.value = (float)wing_item_data.process / (float)next_level_res.costNum;
                }
               
            }
            else
            {
                nextLevel.text = "";
                levelPercent.text = "已满级";
                levelProcessbar.value = 1;
            }
            buffDesc.text = wing_level_res.buffDesc;
            string picName = WingModule.GetWingPic(wing_item_data.id, (int)wing_item_data.level);
            if (picName != mWingItemUIList[CurPageNum - 1].mWingSprite.spriteName)
            {
                UIAtlasHelper.SetSpriteImage(mWingItemUIList[CurPageNum - 1].mWingSprite, picName);
            }

            for (int i = 0; i < WingDefine.PropertyNum; i++)
            {
                uint num = 0;
                uint totalnum = 0;
                switch (i)
                {
                    case 0:
                        num = wing_item_data.life;
                        totalnum = wing_item_data.liftTotal;
                        break;
                    case 1:
                        num = wing_item_data.attack;
                        totalnum = wing_item_data.attackTotal;
                        break;
                    case 2:
                        num = wing_item_data.defence;
                        totalnum = wing_item_data.defenceTotal;
                        break;
                    case 3:
                        num = wing_item_data.critical;
                        totalnum = wing_item_data.criticalTotal;
                        break;
                    case 4:
                        num = wing_item_data.power;
                        totalnum = wing_item_data.powerTotal;
                        break;
                }

                mProps[i].propname.text = GetPropString(i, num);
                mProps[i].propProcessBar.value = (float) num/(float) totalnum;

                if (mProps[i].propProcessBar.value.Equals(1))
                {
                    UIAtlasHelper.SetSpriteImage(mProps[i].foreGround, "common:process_short_fg1");

                }
                else
                {
                    UIAtlasHelper.SetSpriteImage(mProps[i].foreGround, "common:processbar_short_fg2");
                }
            }

            UIAtlasHelper.SetSpriteImage(stuff, ItemManager.Instance.getItemBmp(mCommorRes.costId));
            stuffname.text = ItemManager.Instance.getItemName(mCommorRes.costId);
            PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
            stuffnum.text = pdm.GetItemNumByID(mCommorRes.costId).ToString();
           
            if (State == WingState.Wear)
            {
                equipWord.text = StringHelper.GetString("wing_takeoff");
                UIAtlasHelper.SetButtonImage(equipBtn, "common:btn_blue_4word");

            }
            else
            {
                equipWord.text = StringHelper.GetString("wing_equip");
                UIAtlasHelper.SetButtonImage(equipBtn, "common:btn_yellow_4word");
            }
            int level = wing_item_data.level < mCommorRes.middleLevel ? mCommorRes.middleLevel : mCommorRes.hightLevel;
            tip.text = StringHelper.GetString("wingtip").Replace("?", level.ToString());
            tip.gameObject.SetActive(wing_item_data.level < mCommorRes.middleLevel);
           
        }
        pageBar.CurPageNum = mWingMoudle.GetWingPageCount();
        pageBar.LockPage = PlayerDataPool.Instance.MainData.mWingData.GetLockIndex();
       
    }

    private string GetPropString(int type, uint num)
    {
        string re = "";
        switch (type)
        {
            case 0:
                re += StringHelper.GetString("life");
                break;
            case 1:
                re += StringHelper.GetString("attack");
                break;
            case 2:
                re += StringHelper.GetString("defence");
                break;
            case 3:
                re += StringHelper.GetString("critical");
                break;
            case 4:
                re += StringHelper.GetString("power");
                break;
        }

        return re += ("+" + num);
    }

    private void AddEventDelegate()
    {
        EventDelegate.Add(pageNext.onClick, PageNext);
        EventDelegate.Add(pagePrev.onClick, PagePrev);
        EventDelegate.Add(pageNextAni.onClick, PageNext);
        EventDelegate.Add(pagePrevAni.onClick, PagePrev);
        EventDelegate.Add(jihuoBtn.onClick, OnActive);
        EventDelegate.Add(jinglianBtn.onClick, OnForge);
        EventDelegate.Add(equipBtn.onClick, OnEquip);
        EventDelegate.Add(tryOnBtn.onClick, OnTryOn);
        EventDelegate.Add(backBtn.onClick, OnBack);
        UIEventListener.Get(midBk).onDrag = OnWingItemDrag;
    }

    private void RemoveEventDelegate()
    {
        EventDelegate.Remove(pageNext.onClick, PageNext);
        EventDelegate.Remove(pagePrev.onClick, PagePrev);
        EventDelegate.Remove(pageNextAni.onClick, PageNext);
        EventDelegate.Remove(pagePrevAni.onClick, PagePrev);
        EventDelegate.Remove(jihuoBtn.onClick, OnActive);
        EventDelegate.Remove(jinglianBtn.onClick, OnForge);
        EventDelegate.Remove(equipBtn.onClick, OnEquip);
        EventDelegate.Remove(tryOnBtn.onClick, OnTryOn);
        EventDelegate.Remove(backBtn.onClick, OnBack);
    }

    private void OnActive()
    {
        mWingMoudle.RequestActive();
        SoundManager.Instance.Play(15);
    }

    //精炼
    private void OnForge()
    {
        if (mWingMoudle.IsFullLevel())
        {
            PopTipManager.Instance.AddNewTip("已到达满级");
        }
        else
        {
            mWingMoudle.RequestForge();
        }

        SoundManager.Instance.Play(15);
       
    }

    private void OnEquip()
    {
        mWingMoudle.RequestEquip();
        SoundManager.Instance.Play(15);

    }

    private void OnTryOn()
    {
        SoundManager.Instance.Play(15);
        mWingItemUIList[CurPageNum - 1].IsTryOn = true;
        Player player = PlayerController.Instance.GetControlObj() as Player;
        if (player != null)
        {
            int wingid = mWingMoudle.GetWingIdByIndex(CurPageNum);
            mCharacterPreview.SetupCharacter(player.ModelID, null, wingid, mWingMoudle.GetWingLevel(wingid));


            PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

            mCharacterPreview.ChangeWeapon(pdm.GetMainWeaponId());
        }
        mWingItemUIList[CurPageNum - 1].mView.SetActive(false);
        mPreviewSprite.gameObject.SetActive(true);
        upDownAniHandler();
        mIsDirty = true;
    }

    private void OnBack()
    {
        SoundManager.Instance.Play(15);
        mWingItemUIList[CurPageNum - 1].IsTryOn = false;
        mWingItemUIList[CurPageNum - 1].mView.SetActive(true);
        mPreviewSprite.gameObject.SetActive(false);
        mCharacterPreview.RotationY = 180;
        upDownAniHandler();
        mIsDirty = true;
    }

    private float dragDelta;

    private void OnWingItemDrag(GameObject go, Vector2 delta)
    {
        dragDelta += delta.x;
    }

    private void OnWingItemPress()
    {
        dragDelta = 0;
    }

    private void OnWingItemRelease()
    {
        if (!dragDelta.Equals(0))
        {
            if (dragDelta > 0)
            {
                PagePrev();
            }
            else
            {
                PageNext();
            }
        }
        dragDelta = 0;
    }

}
