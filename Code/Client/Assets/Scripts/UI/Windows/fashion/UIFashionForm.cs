

  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;

  using UnityEngine;

public class UIFashionForm : UIWindow
{
    private UILabel mFashionName;
    private UISprite mPreview;
    private UICharacterPreview mCharacterPreview;
    private UILabel mGetWay;
    private UILabel mDesc;
    private UILabel mPropTitle;
    private UILabel mDefence;
    private UILabel mAttack;
    private UILabel mPower;
    private UILabel mLife;
    private UILabel mCritical;
    private UIButton mAddStarBtn;
    private UIButton mWearBtn;
    private UIButton mBuyBtn;
    private UILabel mBuyText;
    private UIGrid mBtnGrid;
    private UIGrid mFashionItemGrid;
    private GameObject mAwardContainer;
  
    public int mSelectedId;
    public Dictionary<int, FashionItemUI> mItemDic;
    public GameObject mExamFashionItem;
    private FashionModule mModule;
    private UILabel ownItemNum;
    private bool isDirty;
    private UILabel mWearTxt;

    protected override void OnLoad()
    {
        base.OnLoad();
        mFashionName = FindComponent<UILabel>("listContiner/namebg/fashionname");
        mGetWay = FindComponent<UILabel>("listContiner/getway");
        mDesc = FindComponent<UILabel>("listContiner/desc");
        mPropTitle = FindComponent<UILabel>("listContiner/proptitle");
        mDefence = FindComponent<UILabel>("listContiner/defence");
        mAttack = FindComponent<UILabel>("listContiner/attack");
        mPower = FindComponent<UILabel>("listContiner/power");
        mLife = FindComponent<UILabel>("listContiner/life");
        mCritical = FindComponent<UILabel>("listContiner/critical");
        mAddStarBtn = FindComponent<UIButton>("listContiner/btngrid/AddstarBtn");
        mWearBtn = FindComponent<UIButton>("listContiner/btngrid/EquipBtn");
        mWearTxt = FindComponent<UILabel>("listContiner/btngrid/EquipBtn/Label");
        mBuyBtn = FindComponent<UIButton>("listContiner/btngrid/BuyBtn");
        mBuyText = FindComponent<UILabel>("listContiner/btngrid/BuyBtn/Label");
        mBtnGrid = FindComponent<UIGrid>("listContiner/btngrid");
        mFashionItemGrid = FindComponent<UIGrid>("listContiner/ScrollView/itemgrid");
        ownItemNum = FindComponent<UILabel>("listContiner/ownItemNum");     
        mExamFashionItem = FindChild("FashionItem");
        mExamFashionItem.SetActive(false);
        mAwardContainer = FindChild("awardContainer");
        
        mPreview = FindComponent<UISprite>("listContiner/Preview");
        mCharacterPreview = new UICharacterPreview();      
        //初始化时装列表
        mItemDic = new Dictionary<int, FashionItemUI>();
        IDictionaryEnumerator enumerator = DataManager.FashionTable.GetEnumerator();
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null) return;
        while (enumerator.MoveNext())
        {
            FashionTableItem res = enumerator.Value as FashionTableItem;
            if (res.sex != pdm.GetResId()) continue;
            FashionItemUI item = new FashionItemUI(GameObject.Instantiate(mExamFashionItem) as GameObject);
            UIAtlasHelper.SetSpriteImage(item.cloth, res.pic);
            item.gameObject.SetActive(true);
            item.gameObject.transform.parent = mFashionItemGrid.gameObject.transform;          
            item.gameObject.transform.localScale = Vector3.one;
            item.name.text = res.name;
            item.mBtn.CustomData = res.id;
            mItemDic.Add(res.id, item);
        }

        mFashionItemGrid.maxPerLine = mItemDic.Count > 8 ? Mathf.CeilToInt(mItemDic.Count / 2) : 4;
       

        mPropTitle.text = StringHelper.GetString("fashion_pro");
    }
  
    protected override void OnOpen(object param = null)
    {        
        base.OnOpen(param);
        mModule = ModuleManager.Instance.FindModule<FashionModule>();
         IDictionaryEnumerator enumerator = mItemDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
           FashionItemUI item = enumerator.Value as FashionItemUI;
            item.clickCallBack = ItemClickCallBack;
        }
        EventDelegate.Add(mAddStarBtn.onClick, OnAddStartClick);
        EventDelegate.Add(mWearBtn.onClick,OnEquipClick);
        EventDelegate.Add(mBuyBtn.onClick, OnBuyBtnClick);
        EventSystem.Instance.addEventListener(FashionEvent.FASHION_ACTIVE, OnRespondActive);
        EventSystem.Instance.addEventListener(FashionEvent.FASHION_EQUIP,OnRespondEquip);
        mSelectedId = SelectedId;
        RefreshListPanel();
        if (mCharacterPreview != null)
            mCharacterPreview.Enable = true;

        Player player = PlayerController.Instance.GetControlObj() as Player;
        if (player != null)
        {
            mCharacterPreview.BackgroundSprite = mPreview;
            mCharacterPreview.SetTargetSprite(mPreview);
            mCharacterPreview.SetCameraOrthographicSize(1.2f);
            mCharacterPreview.RotationY = 180;
            mCharacterPreview.Pos = new Vector3(-0.25f, 0, 0);
            mCharacterPreview.SetupCharacter(player.ModelID, null, -1, 0);
            PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
            mCharacterPreview.ChangeWeapon(pdm.GetMainWeaponId());
           
        }
    }

    protected override void OnClose()
    {
        base.OnClose();
        IDictionaryEnumerator enumerator = mItemDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            FashionItemUI item = enumerator.Value as FashionItemUI;
            item.clickCallBack = null;
        }

        EventDelegate.Remove(mAddStarBtn.onClick, OnAddStartClick);
        EventDelegate.Remove(mWearBtn.onClick, OnEquipClick);
        EventDelegate.Remove(mBuyBtn.onClick, OnBuyBtnClick);
        EventSystem.Instance.removeEventListener(FashionEvent.FASHION_ACTIVE, OnRespondActive);
        EventSystem.Instance.removeEventListener(FashionEvent.FASHION_EQUIP, OnRespondEquip);
        if (mCharacterPreview != null)
            mCharacterPreview.Enable = false;
    }

    private void OnRespondEquip(EventBase evt)
    {
        FashionEvent fevt = evt as FashionEvent;
        isDirty = true;
    }
    private void OnRespondActive(EventBase evt)
    {
        FashionEvent fevt = evt as FashionEvent;
        if(!mItemDic.ContainsKey(fevt.mfashionid)) return;
        isDirty = true;
        mCharacterPreview.ChangeFashion(fevt.mfashionid,fevt.actionid);
    }

    private void OnAddStartClick()
    {
        WindowManager.Instance.OpenUI("fashionaddstar", SelectedId);
        SoundManager.Instance.Play(15);
    }

    private void OnEquipClick()
    {
        FashionTableItem res = DataManager.FashionTable[mSelectedId] as FashionTableItem;
        mModule.RequestEquip(SelectedId, mModule.GetEquipId(res.bodypart) == SelectedId ? 2 : 1);
        SoundManager.Instance.Play(15);
    }

    private void OnBuyBtnClick()
    {
        FashionTableItem res = DataManager.FashionTable[mSelectedId] as FashionTableItem;
        if (res.activity.Equals("no"))
        {
            mModule.RequestActiveFashion(SelectedId);
        }
        else
        {
            WindowManager.Instance.OpenUI(res.activity);
        }
        SoundManager.Instance.Play(15);
    }

    private void ItemClickCallBack(int id)
    {
        if (id == mSelectedId) return;
       
        mSelectedId = id;
        RefresItemInfo();
    }

    public override void Update(uint elapsed)
    {
        base.Update(elapsed);
        if (isDirty)
        {
            RefreshListPanel();
        }

        mCharacterPreview.Update();
    }

    private void RefreshListPanel()
    {
        isDirty = false;
        FashionItemData itemdata;
        foreach (var kvp in mItemDic)
        {
            FashionTableItem res = DataManager.FashionTable[kvp.Key] as FashionTableItem;
            if (mModule.GetEquipId(res.bodypart) == kvp.Key) //已穿戴
            {
                GameDebug.Log("kpv.key"+kvp.Key);
                kvp.Value.flag.gameObject.SetActive(true);
                kvp.Value.flagTxt.text = StringHelper.GetString("equiped");
            }
            else
            {
                kvp.Value.flag.gameObject.SetActive(false);
            }

            if (mModule.GetFashionByID(kvp.Key, out itemdata))
            {
                //已经获得
                kvp.Value.mLock.gameObject.SetActive(false);
                UIAtlasHelper.SetButtonImage(kvp.Value.mBtn, "Shizhuang:shizhuang_02");
            }
            else
            {
                kvp.Value.mLock.gameObject.SetActive(true);
                UIAtlasHelper.SetButtonImage(kvp.Value.mBtn, "Shizhuang:shizhuang_04");
            }

        }
        mFashionItemGrid.repositionNow = true;
        RefresItemInfo();

    }


    public int SelectedId
    {
        get
        {    
            //得到合法id
            if (!mItemDic.ContainsKey(mSelectedId))
            {return mItemDic.Keys.ElementAt(0);}
            return mSelectedId;
        }
        set { mSelectedId = value; }
    }

    private void RefresItemInfo()
    {
        FashionItemData itemdata;  
        FashionTableItem res = DataManager.FashionTable[SelectedId] as FashionTableItem;
        ObjectCommon.DestoryChildren(mAwardContainer);
        FashionPropTableItem propRes;
        if (mModule.GetFashionByID(SelectedId, out itemdata))
        {
            //时装已经解锁
            mWearTxt.text = StringHelper.GetString((mModule.GetEquipId(res.bodypart) == SelectedId) ? "takeoff" : "wear");
            mWearBtn.gameObject.SetActive(true);
            mBuyBtn.gameObject.SetActive(false);
            mAddStarBtn.gameObject.SetActive(true);
            ownItemNum.gameObject.SetActive(false);
            propRes = DataManager.FashionPropTable[itemdata.starnum + res.propid] as FashionPropTableItem;
        }
        else
        {
            //未解锁          
            mWearBtn.gameObject.SetActive(false);
            mBuyBtn.gameObject.SetActive(true);
            mAddStarBtn.gameObject.SetActive(false);
            mBuyText.text = StringHelper.GetString(res.activity.Equals("no") ? "buy" : "goingto");
            var proRes = DataManager.FashionPropTable[res.propid] as FashionPropTableItem;
            AwardItemUI awarItem = new AwardItemUI(res.costid, proRes.costnum);
            awarItem.gameObject.transform.parent = mAwardContainer.transform;
            awarItem.gameObject.transform.localPosition = Vector3.zero;
            awarItem.gameObject.transform.localScale = Vector3.one;
            ownItemNum.gameObject.SetActive(true);
            PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

            uint ownNum;
            if (res.costtype == 1)
            {
                ownNum = pdm.GetProceeds(ProceedsType.Money_Game);
            }
            else if (res.costtype == 2)
            {
                ownNum = pdm.GetProceeds(ProceedsType.Money_RMB);
            }
            else
            {
                ownNum = pdm.GetItemNumByID(res.costid);
            }
            ownItemNum.text = StringHelper.GetString("ownnum") + ownNum;
            propRes = DataManager.FashionPropTable[res.propid] as FashionPropTableItem;

        }
        mBtnGrid.repositionNow = true;

        if (propRes != null)
        {
            mLife.text = StringHelper.GetString("life") + ": " + propRes.life;
            mPower.text = StringHelper.GetString("power") + ": " + propRes.power;
            mCritical.text = StringHelper.GetString("critical") + ": " + propRes.critical;
            mAttack.text = StringHelper.GetString("attack") + ": " + propRes.fight;
            mDefence.text = StringHelper.GetString("defence") + ": " + propRes.defence;
        }

        mGetWay.text = res.getWay;
        mDesc.text = res.desc;
        mFashionName.text = res.name;

        //调用任务换装接口

    }
 

}

