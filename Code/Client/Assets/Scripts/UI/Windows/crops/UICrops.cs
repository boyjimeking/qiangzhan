using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

public class UICrops : UIWindow
{
    #region 定义
    //Toggle
    private List<GameObject> mTabIndex = new List<GameObject>();

    //佣兵营地
    private UIButton mBtnCampBuy = null;
    private UIButton mBtnMainCrops = null;
    private UIButton mBtnSubCrops = null;
    private UILabel mCropsName = null;
    private List<GameObject> mSkillObjList = new List<GameObject>();
    private List<UISprite> mStarsList = new List<UISprite>();
    private UIGrid mCampGrid = null;
    private GameObject mPropertyPanel = null;
    private UILabel mLife = null;
    private UILabel mDefence = null;
    private UILabel mCrits = null;
    private UILabel mEnergy = null;
    private UILabel mDamage = null;
    private UIButton mBtnSkill1 = null;
    private UIButton mBtnSkill2 = null;
    private UIButton mBtnSkill3 = null;
    private GameObject mSkillNotes = null;
    private GameObject mLogo1 = null;
    private GameObject mLogo2 = null;

    //升星
    private UIButton mBtnRiseStars = null;
    private UILabel mItemNameAndNum = null;
    private UILabel mHasItemNum = null;
    private GameObject mItemObj = null;
    private GameObject mStage0 = null;
    private GameObject mStage1 = null;
    private GameObject mStage2 = null;
    private GameObject mSkillPanel = null;
    private GameObject mArrowPanel = null;
    private UIGrid mRiseGrid = null;
    private GameObject mPerItem = null;

    //Item
    private GameObject mCropsItemPrefab1 = null;
    private GameObject mCropsItemPrefab2 = null;
    private GameObject mCropsItemPrefab3 = null;

    //佣兵模型
    private UICharacterPreview mPreview = new UICharacterPreview();
    private UISprite mPreveiwSprite = null;
    
    //define
    private const int SKILLMAX = 3;
    private const int STARSMAX = 5;
    private Dictionary<int, CropsShopGridUI> mShopCrid = new Dictionary<int, CropsShopGridUI>();
    private Dictionary<int, CropsItemGridUI> mItemGrid = new Dictionary<int, CropsItemGridUI>();
    private CropsShopGridUI mOldSelected = null;
    private CropsItemGridUI mOldSelectedofStarsUI = null;
    #endregion
    public UICrops()
    {

    }
    protected override void OnLoad()
    {
        base.OnLoad();

        #region 定义
        //Toggle
        mTabIndex.Add(this.FindChild("TagGroup/Toggle0"));
        mTabIndex.Add(this.FindChild("TagGroup/Toggle1"));

        //佣兵营地
        mBtnCampBuy = this.FindComponent<UIButton>("BuyBtn");
        mBtnMainCrops = this.FindComponent<UIButton>("MainBtn");
        mBtnSubCrops = this.FindComponent<UIButton>("SubBtn");
        mLogo1 = this.FindChild("CampPanel/logo1");
        mLogo2 = this.FindChild("CampPanel/logo2");
        mCropsName = this.FindComponent<UILabel>("CampPanel/Crops/plat/name");
        for (int i = 0; i < SKILLMAX; ++i)
        {
            mSkillObjList.Add(this.FindChild("CampPanel/Crops/plat/skill" + (i + 1)));
        }
        for (int i = 0; i < STARSMAX; ++i)
        {
            mStarsList.Add(this.FindComponent<UISprite>("CampPanel/Crops/stars" + (i + 1)));
        }
        mCampGrid = this.FindComponent<UIGrid>("CampPanel/ScrollPanel/Scroll View/UIGrid");
        mLife = this.FindComponent<UILabel>("CampPanel/Cropsinfopanel/life/Label");
        mDefence = this.FindComponent<UILabel>("CampPanel/Cropsinfopanel/defence/Label");
        mDamage = this.FindComponent<UILabel>("CampPanel/Cropsinfopanel/damage/Label");
        mCrits = this.FindComponent<UILabel>("CampPanel/Cropsinfopanel/crits/Label");
        mEnergy = this.FindComponent<UILabel>("CampPanel/Cropsinfopanel/energy/Label");
        mPropertyPanel = this.FindChild("Cropsinfopanel");
        mSkillNotes = this.FindChild("SkillNotes");
        mBtnSkill1 = this.FindComponent<UIButton>("CampPanel/Crops/plat/skill1");
        mBtnSkill2 = this.FindComponent<UIButton>("CampPanel/Crops/plat/skill2");
        mBtnSkill3 = this.FindComponent<UIButton>("CampPanel/Crops/plat/skill3");

        //升星
        mBtnRiseStars = this.FindComponent<UIButton>("Shengjie/RiseBtn");
        mItemNameAndNum = this.FindComponent<UILabel>("Shengjie/PerItem/nameLb");
        mHasItemNum = this.FindComponent<UILabel>("Shengjie/PerItem/countLb");
        mItemObj = this.FindChild("Shengjie/PerItem/iconSp");
        mStage0 = this.FindChild("Shengjie/Stage0");
        mStage1 = this.FindChild("Shengjie/Stage1");
        mStage2 = this.FindChild("Shengjie/Stage2");
        mSkillPanel = this.FindChild("Shengjie/SkillPanel");
        mArrowPanel = this.FindChild("Shengjie/ArrowPanel");
        mRiseGrid = this.FindComponent<UIGrid>("Shengjie/ScrollPanel/Scroll View/UIGrid");
        mPerItem = this.FindChild("Shengjie/PerItem");

        //Item
        mCropsItemPrefab1 = this.FindChild("Items/CropsItem1");
        mCropsItemPrefab2 = this.FindChild("Items/CropsItem2");
        mCropsItemPrefab3 = this.FindChild("Items/Stage");

        //佣兵模型
        mPreveiwSprite = this.FindComponent<UISprite>("CampPanel/Crops/pic");

        #endregion
    }
    protected override void OnOpen(object param = null)
    {
        CropsModule cropsm = ModuleManager.Instance.FindModule<CropsModule>();
        if (null == cropsm)
            return;

        cropsm.SetTabIndex(0);
        InitUI();
    }
    public override void Update(uint elapsed)
    {
        if (!mPreveiwSprite.gameObject.activeInHierarchy /*&& !mPreveiwSpriteStren.gameObject.activeInHierarchy*/)
        {
            mPreview.Enable = false;
        }
        else if (!mPreview.Enable)
        {
            mPreview.Enable = true;
        }

        mPreview.Update();
    }
    protected override void OnClose()
    {
        EventSystem.Instance.removeEventListener(CropsEvent.TAB_INDEX, SetTabIndex);
        EventSystem.Instance.removeEventListener(CropsEvent.BUY_CROPS, OnCropsBuyHandler);
        EventSystem.Instance.removeEventListener(CropsEvent.CHANGE_CROPS, OnChangeHandler);
        EventSystem.Instance.removeEventListener(CropsEvent.RISE_STARS, OnRiseStarsHandler);
        EventDelegate.Remove(mBtnCampBuy.onClick, OnBtnBuyCropsHandler);
        EventDelegate.Remove(mBtnMainCrops.onClick, OnBtnSetMainCropsHandler);
        EventDelegate.Remove(mBtnSubCrops.onClick, OnBtnSetSubCropsHandler);
        EventDelegate.Remove(mBtnRiseStars.onClick, OnBtnRiseStarsHandler);
    }
   
    private void InitUI()
    {
        EventSystem.Instance.addEventListener(CropsEvent.TAB_INDEX, SetTabIndex);
        EventSystem.Instance.addEventListener(CropsEvent.BUY_CROPS,OnCropsBuyHandler);
        EventSystem.Instance.addEventListener(CropsEvent.CHANGE_CROPS, OnChangeHandler);
        EventSystem.Instance.addEventListener(CropsEvent.RISE_STARS, OnRiseStarsHandler);
        EventDelegate.Add(mBtnCampBuy.onClick, OnBtnBuyCropsHandler);
        EventDelegate.Add(mBtnMainCrops.onClick, OnBtnSetMainCropsHandler);
        EventDelegate.Add(mBtnSubCrops.onClick, OnBtnSetSubCropsHandler);
        EventDelegate.Add(mBtnRiseStars.onClick, OnBtnRiseStarsHandler);
        UIEventListener.Get(mBtnSkill1.gameObject).onPress = OnBtnSkill1Press;
        UIEventListener.Get(mBtnSkill2.gameObject).onPress = OnBtnSkill2Press;
        UIEventListener.Get(mBtnSkill3.gameObject).onPress = OnBtnSkill3Press;

        InitCampUI();
        InitRiseStarsUI();

        mPreview.SetTargetSprite(mPreveiwSprite);
        mPreview.SetCameraOrthographicSize(1.5f);
        mPreview.RotationY = 180;
    }

    public void SetTabIndex(EventBase evt)
    {
        CropsModule module = ModuleManager.Instance.FindModule<CropsModule>();
        if (module == null)
            return;

        int index = module.GetTabIndex();

        if (index < 0 || index >= mTabIndex.Count)
            return;

        UIToggle tg = mTabIndex[index].GetComponent<UIToggle>();
        if (tg == null)
            return;

        tg.value = true;

        module.SetTabIndex(0, false);
    }

    #region 营地
    private void InitCampUI()
    {
        mPropertyPanel.SetActive(false);
        if(null != mOldSelected)
            mOldSelected.SetSelect(false);
        mSkillNotes.SetActive(false);
        IDictionaryEnumerator itr = DataManager.CropsTable.GetEnumerator();
        while (itr.MoveNext())
        {
            CropsTableItem item = itr.Value as CropsTableItem;
            InitCropsShopGridUI(item);
        }
//         foreach (CropsTableItem item in DataManager.CropsTable.Values)
//         {
//             InitCropsShopGridUI(item);
//         }

        SetCurSelectCrops(mOldSelected);
    }
    private void InitCropsShopGridUI(CropsTableItem item)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
            return;
        CropsShopGridUI grid = null;
        GameObject sp = null;

        if (!mShopCrid.ContainsKey(item.id))
        {
            sp = WindowManager.Instance.CloneGameObject(mCropsItemPrefab1);
            if (sp == null)
            {
                GameDebug.LogError("mCropsItemPrefab1 prefab not found");
                return;
            }

            sp.SetActive(true);
            sp.name = item.id.ToString();
            grid = new CropsShopGridUI(sp);
            if (null == grid)
                return;
            grid.SetCropsId(item.id);
            grid.SetIcon(item.picname, item.picname2);
            grid.SetName(item.name);
            grid.SetSelect(false);
            grid.SetStarslv(module.GetCropsStarsLv(item.id));
            grid.SetState(module.GetCropsState(item.id));
            if (module.HasObtainCrops(item.id))
            {
                grid.SetUnlockCrops();
            }
            else
            {
                grid.SetProgress((int)module.GetItemNumByID(item.itemid), item.itemnum);
            }
            grid.onClick = SetCurSelectCrops;
            sp.transform.parent = mCampGrid.transform;
            sp.transform.localScale = Vector3.one;
            mCampGrid.Reposition();
            mShopCrid.Add(item.id, grid);
            if (null == mOldSelected || item.id < mOldSelected.GetCropsId())
                mOldSelected = grid;
        }
        else
        {
            grid = mShopCrid[item.id];
            grid.SetStarslv(module.GetCropsStarsLv(item.id));
            grid.SetState(module.GetCropsState(item.id));
            if (!module.HasObtainCrops(item.id))
            {
                grid.SetProgress((int)module.GetItemNumByID(item.itemid), item.itemnum);
            }
            else
            {
                grid.SetUnlockCrops();
            }
        }
    }

    private void SetCurSelectCrops(CropsShopGridUI target)
    {
        if (null == target)
            return;
        if (null != mOldSelected)
        {
            mOldSelected.SetSelect(false);
        }

        mOldSelected = target;
        mOldSelected.SetSelect(true);

        mPropertyPanel.SetActive(true);
        //设置属性信息
        SetCurSelecteProperty(target.GetCropsId());
        SetCurSelecteStarsLv(target.GetCropsId());
        SetCurSelecteSkill(target.GetCropsId());
        SetCropsBtn(target.GetCropsId());

        NPCTableItem npc = DataManager.NPCTable[target.GetCropsId()] as NPCTableItem;
        if (null == npc)
            return;
        mPreview.SetupCharacter(npc.model, null, -1, uint.MaxValue);
        mPreview.ChangeWeapon(npc.weaponid);
    }

    private void SetCurSelecteProperty(int resid)
    {
        PlayerDataModule pmodule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == pmodule)
            return;
        CropsModule module = ModuleManager.Instance.FindModule<CropsModule>();
        if (null == module)
            return;

        int starslv = pmodule.GetCropsStarsLv(resid) == -1 ? 1 : pmodule.GetCropsStarsLv(resid);
        float hp = 0.0f;
        float damage = 0.0f;
        float crits = 0.0f;
        float defence = 0.0f;
        float energy = 0.0f;
        module.GetProperty(resid, starslv, ref hp, ref damage, ref crits, ref defence, ref energy);

        mLife.text = hp.ToString();
        mDamage.text = damage.ToString();
        mCrits.text = crits.ToString();
        mDefence.text = defence.ToString();
        mEnergy.text = energy.ToString();
        mPropertyPanel.SetActive(true);
    }
    
    /*设置试衣间*/
    //1.佣兵星级和名称
    private void SetCurSelecteStarsLv(int resid)
    {
        PlayerDataModule pmodule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == pmodule)
            return;
        CropsTableItem item = DataManager.CropsTable[resid] as CropsTableItem;
        if (null == item)
            return;

        mCropsName.text = item.name;

        int starslv = pmodule.GetCropsStarsLv(resid) == -1 ? 1 : pmodule.GetCropsStarsLv(resid);

        for (int i = 0; i < starslv; ++i)
        {
            UIAtlasHelper.SetSpriteImage(mStarsList[i], "common:strenth (11)");
        }
        for (int i = starslv; i < STARSMAX; ++i)
        {
            UIAtlasHelper.SetSpriteImage(mStarsList[i], "common:starslvback");
        }
    }
    //2.佣兵技能
    private void SetCurSelecteSkill(int resid)
    {
        CropsTableItem item = DataManager.CropsTable[resid] as CropsTableItem;
        if (null == item)
            return;
        IDictionaryEnumerator itr = DataManager.CropsSkillLearnTable.GetEnumerator();
        while (itr.MoveNext())
        {
            CropsSkillLearnTableItem skill = itr.Value as CropsSkillLearnTableItem;
            if (skill.id == item.skillid1)
            {
                SetSkillIcon(skill.unlock_lv, skill.skill_icon, resid, item, mSkillObjList[0]);
                continue;
            }

            if (skill.id == item.skillid2)
            {
                SetSkillIcon(skill.unlock_lv, skill.skill_icon, resid, item, mSkillObjList[1]);
                continue;
            }

            if (skill.id == item.skillid3)
            {
                SetSkillIcon(skill.unlock_lv, skill.skill_icon, resid, item, mSkillObjList[2]);
                continue;
            }
        }
//         foreach(CropsSkillLearnTableItem skill in DataManager.CropsSkillLearnTable.Values)
//         {
//             if (skill.id == item.skillid1)
//             {
//                 SetSkillIcon(skill.unlock_lv,skill.skill_icon, resid, item, mSkillObjList[0]);
//                 continue;
//             }
// 
//             if (skill.id == item.skillid2)
//             {
//                 SetSkillIcon(skill.unlock_lv, skill.skill_icon, resid, item, mSkillObjList[1]);
//                 continue;
//             }
// 
//             if (skill.id == item.skillid3)
//             {
//                 SetSkillIcon(skill.unlock_lv, skill.skill_icon, resid, item, mSkillObjList[2]);
//                 continue;
//             }
//         }
    }
    private void SetSkillIcon(int unlock_lv, string icon, int resid, CropsTableItem item, GameObject sp)
    {
        PlayerDataModule pmodule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == pmodule)
            return;

        int starslv = pmodule.GetCropsStarsLv(resid);
        UISprite mIcon = ObjectCommon.GetChildComponent<UISprite>(sp, "icon");
        UISprite mLock = ObjectCommon.GetChildComponent<UISprite>(sp, "lock");
        //佣兵技能没解锁或者佣兵没解锁，技能锁定,解锁过的技能正常显示
        UIAtlasHelper.SetSpriteImage(mIcon, icon);
        if (starslv >= unlock_lv)
        {
            mLock.gameObject.SetActive(false);
        }
        else
        {
            mLock.gameObject.SetActive(true);
        }
    }

    public void OnBtnSkill1Press(GameObject target, bool isPressed)
    {
        if (isPressed)
        {
            CropsTableItem cropsitem = DataManager.CropsTable[mOldSelected.GetCropsId()] as CropsTableItem;
            if (null == cropsitem)
                return;
            CropsSkillLearnTableItem learn = DataManager.CropsSkillLearnTable[cropsitem.skillid1] as CropsSkillLearnTableItem;
            if (null == learn)
                return;

            UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mSkillNotes, "Label");
            UILabel head = ObjectCommon.GetChildComponent<UILabel>(mSkillNotes, "head");
            lb.text = learn.skill_desc;
            head.text = "[fed514]" + learn.skill_name + ":";

            mSkillNotes.SetActive(true);
            mPropertyPanel.SetActive(false);
        }
        else
        {
            mSkillNotes.SetActive(false);
            mPropertyPanel.SetActive(true);
        }
    }
    public void OnBtnSkill2Press(GameObject target, bool isPressed)
    {
        if (isPressed)
        {
            CropsTableItem cropsitem = DataManager.CropsTable[mOldSelected.GetCropsId()] as CropsTableItem;
            if (null == cropsitem)
                return;
            CropsSkillLearnTableItem learn = DataManager.CropsSkillLearnTable[cropsitem.skillid2] as CropsSkillLearnTableItem;
            if (null == learn)
                return;

            UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mSkillNotes, "Label");
            UILabel head = ObjectCommon.GetChildComponent<UILabel>(mSkillNotes, "head");
            lb.text = learn.skill_desc;
            head.text = "[fed514]" + learn.skill_name + ":";

            mSkillNotes.SetActive(true);
            mPropertyPanel.SetActive(false);
        }
        else
        {
            mSkillNotes.SetActive(false);
            mPropertyPanel.SetActive(true);
        }
    }
    public void OnBtnSkill3Press(GameObject target, bool isPressed)
    {
        if (isPressed)
        {
            CropsTableItem cropsitem = DataManager.CropsTable[mOldSelected.GetCropsId()] as CropsTableItem;
            if (null == cropsitem)
                return;

            CropsSkillLearnTableItem learn = DataManager.CropsSkillLearnTable[cropsitem.skillid3] as CropsSkillLearnTableItem;
            if (null == learn)
                return;

            UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mSkillNotes, "Label");
            UILabel head = ObjectCommon.GetChildComponent<UILabel>(mSkillNotes, "head");
            lb.text = learn.skill_desc;
            head.text = "[fed514]" + learn.skill_name + ":";

            mSkillNotes.SetActive(true);
            mPropertyPanel.SetActive(false);
        }
        else
        {
            mSkillNotes.SetActive(false);
            mPropertyPanel.SetActive(true);
        }
    }
    //3.佣兵模型
    private void SetCropsModel()
    { 
        
    }

    //设定Btn显示
    private void SetCropsBtn(int resid)
    {
        PlayerDataModule pmodule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == pmodule)
            return;

        mLogo1.SetActive(false);
        mLogo2.SetActive(false);
        //不会出现只装备副将的情况
        //已装备主将 & 没装备副将 则 显示主将已出战
        if (resid == pmodule.GetMainCropsId() && -1== pmodule.GetSubCropsId())
        {
            mBtnCampBuy.gameObject.SetActive(false);
            mBtnMainCrops.gameObject.SetActive(false);
            mBtnSubCrops.gameObject.SetActive(false);

            //TODO 主将已出战
            mLogo1.SetActive(true);
            
        }
        else if (resid == pmodule.GetMainCropsId() && -1 != pmodule.GetSubCropsId())
        {
            mBtnCampBuy.gameObject.SetActive(false);
            mBtnMainCrops.gameObject.SetActive(false);
            mBtnSubCrops.gameObject.SetActive(true);

            //TODO 主将已出战
            mLogo1.SetActive(true);
            //设置为副将？
        }
        else if (resid == pmodule.GetSubCropsId() && -1 != pmodule.GetMainCropsId())
        {
            mBtnCampBuy.gameObject.SetActive(false);
            mBtnMainCrops.gameObject.SetActive(true);
            mBtnSubCrops.gameObject.SetActive(false);

            // 副将已出战
            mLogo2.SetActive(true);
            //设置为主将？
        }
        else
        {
            if (pmodule.HasObtainCrops(resid) && -1 == pmodule.GetMainCropsId())
            {
                mBtnCampBuy.gameObject.SetActive(false);
                mBtnMainCrops.gameObject.SetActive(true);
                mBtnSubCrops.gameObject.SetActive(false);
            }
            else if (pmodule.HasObtainCrops(resid) && -1 != pmodule.GetMainCropsId())
            {
                mBtnCampBuy.gameObject.SetActive(false);
                mBtnMainCrops.gameObject.SetActive(true);
                mBtnSubCrops.gameObject.SetActive(true);
            }
            else
            {
                mBtnCampBuy.gameObject.SetActive(true);
                mBtnMainCrops.gameObject.SetActive(false);
                mBtnSubCrops.gameObject.SetActive(false);
            }
        }
    }

    //购买佣兵
    private void OnBtnBuyCropsHandler()
    {
        PlayerDataModule pmodule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == pmodule)
            return;
        CropsModule module = ModuleManager.Instance.FindModule<CropsModule>();
        if (null == module)
            return;

        CropsTableItem item = DataManager.CropsTable[mOldSelected.GetCropsId()] as CropsTableItem;
        if (null == item)
            return;
        uint mHasNum = pmodule.GetItemNumByID(item.itemid);
        if (mHasNum < item.itemnum)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("item_need").Replace("?", ItemManager.Instance.getItemName(item.itemid)));
            return;
        }
        module.BuyCrops(mOldSelected.GetCropsId());
    }

    //设置为主佣兵
    private void OnBtnSetMainCropsHandler()
    {
        PlayerDataModule pmodule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == pmodule)
            return;
        CropsModule module = ModuleManager.Instance.FindModule<CropsModule>();
        if (null == module)
            return;

        module.SetStateCrops(mOldSelected.GetCropsId(), mOldSelected.GetCropsId() == pmodule.GetSubCropsId() ? pmodule.GetMainCropsId() : pmodule.GetSubCropsId());
    }
    //设置为副佣兵
    private void OnBtnSetSubCropsHandler()
    {
        PlayerDataModule pmodule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == pmodule)
            return;
        CropsModule module = ModuleManager.Instance.FindModule<CropsModule>();
        if (null == module)
            return;

        module.SetStateCrops(mOldSelected.GetCropsId() == pmodule.GetMainCropsId() ? pmodule.GetSubCropsId() : pmodule.GetMainCropsId(),mOldSelected.GetCropsId());
    }

    private void OnCropsBuyHandler(EventBase evt)
    {
        InitCampUI();
        InitRiseStarsUI();
    }

    private void OnChangeHandler(EventBase evt)
    {
        InitCampUI();
        InitRiseStarsUI();
    }
    #endregion

    #region 升星
    private void InitRiseStarsUI()
    {
        InitObjActive();
        IDictionaryEnumerator itr = DataManager.CropsTable.GetEnumerator();
        while (itr.MoveNext())
        {
            CropsTableItem item = itr.Value as CropsTableItem;
            InitCropsItemGridUI(item);

        }
//         foreach (CropsTableItem item in DataManager.CropsTable.Values)
//         {
//             InitCropsItemGridUI(item);
//         }
        SetCurSelectCropsStarsUI(mOldSelectedofStarsUI);
        mOldSelectedofStarsUI.SetSelect(true);
    }

    private void InitObjActive()
    {
        if (null != mOldSelectedofStarsUI)
            mOldSelectedofStarsUI.SetSelect(false);
        mStage0.SetActive(false);
        mStage1.SetActive(false);
        mSkillPanel.SetActive(false);
        mArrowPanel.SetActive(false);
        mPerItem.SetActive(false);
        mBtnRiseStars.gameObject.SetActive(false);
    }

    private void InitCropsItemGridUI(CropsTableItem item)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
            return;
        CropsItemGridUI grid = null;
        GameObject sp = null;

        if (!mItemGrid.ContainsKey(item.id))
        {
            sp = WindowManager.Instance.CloneGameObject(mCropsItemPrefab2);
            if (sp == null)
            {
                GameDebug.LogError("mCropsItemPrefab2 prefab not found");
                return;
            }

            sp.name = item.id.ToString();
            sp.SetActive(true);
            grid = new CropsItemGridUI(sp);
            if (null == grid)
                return;
            grid.SetCropsId(item.id);
            grid.SetIcon(item.cropsheadpic);
            grid.SetName(item.name);
            grid.SetSelect(false);
            grid.SetStarslv(module.GetCropsStarsLv(item.id));
            grid.SetState(module.GetCropsState(item.id));
            if (module.HasObtainCrops(item.id))
            {
                grid.SetUnlockCrops();
            }
            grid.onClick = SetCurSelectCropsStarsUI;
            sp.transform.parent = mRiseGrid.transform;
            sp.transform.localScale = Vector3.one;
            mRiseGrid.Reposition();
            mItemGrid.Add(item.id, grid);
            if (null == mOldSelectedofStarsUI || item.id < mOldSelectedofStarsUI.GetCropsId())
                mOldSelectedofStarsUI = grid;
        }
        else
        {
            grid = mItemGrid[item.id];
            if (module.HasObtainCrops(item.id))
            {
                grid.SetUnlockCrops();
            }
            grid.SetStarslv(module.GetCropsStarsLv(item.id));
            grid.SetState(module.GetCropsState(item.id));
        }
    }

    private void SetCurSelectCropsStarsUI(CropsItemGridUI target)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
            return;
        if (null == target)
            return;
        if (null != mOldSelectedofStarsUI)
        {
            mOldSelectedofStarsUI.SetSelect(false);
        }

        mOldSelectedofStarsUI = target;
        mOldSelectedofStarsUI.SetSelect(true);


        CropsTableItem item = DataManager.CropsTable[target.GetCropsId()] as CropsTableItem;
        if (null == item)
            return;
        foreach (Transform trans in mItemObj.transform)
        {
            trans.gameObject.SetActive(false);
            GameObject.Destroy(trans.gameObject);
        }
        int starslv = module.GetCropsStarsLv(target.GetCropsId());
        starslv = starslv == -1 ? 1 : starslv;
        if (starslv != STARSMAX)
        {
            if (mStage2.activeSelf)
                mStage2.SetActive(false);
            CommonItemUI ui = new CommonItemUI(item.itemid);
            ui.gameObject.transform.parent = mItemObj.transform;
            ui.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            ui.gameObject.transform.localScale = Vector3.one;

            mItemNameAndNum.text = ItemManager.Instance.getItemName(item.itemid) + "X" + item[starslv + 1];
            mHasItemNum.text = "已有：" + module.GetItemNumByID(item.itemid);
            mPerItem.SetActive(true);

            SetNowStarsLvProperty(target.GetCropsId(), starslv);
            SetNextStarsLvProperty(target.GetCropsId(), starslv + 1);

            InitSkillPanel(target.GetCropsId(), starslv);
        }
        else
        {
            //升星到最高等级，中间显示卡片属性
            SetMaxStarsLvProperty(target.GetCropsId(), starslv);
            mBtnRiseStars.gameObject.SetActive(false);
            mStage0.SetActive(false);
            mStage1.SetActive(false);
            mSkillPanel.SetActive(false);
            mArrowPanel.SetActive(false);
            mPerItem.SetActive(false);
        }
        if (!module.HasObtainCrops(target.GetCropsId()))
        {
            mBtnRiseStars.gameObject.SetActive(false);
        }

    }

    private void SetNowStarsLvProperty(int resid, int starslv)
    {
        InitStage(mStage0,resid,starslv, new Vector3(-481,140,0));
    }

    private void SetNextStarsLvProperty(int resid,int starslv)
    {
        InitStage(mStage1, resid, starslv, new Vector3(-105, 140, 0));
    }

    private void SetMaxStarsLvProperty(int resid, int starslv)
    {
        InitStage(mStage2, resid, starslv, new Vector3(-298, 140, 0));
    }

    private void InitStage(GameObject obj, int resid,int starslv, Vector3 localPosition)
    {
        CropsModule module = ModuleManager.Instance.FindModule<CropsModule>();
        if (null == module)
            return;

        CropsTableItem item = DataManager.CropsTable[resid] as CropsTableItem;
        if (null == item)
            return;
        CropsStageGridUI grid = null;
        GameObject sp = null;

        sp = WindowManager.Instance.CloneGameObject(mCropsItemPrefab3);
        if (sp == null)
        {
            GameDebug.LogError("mCropsItemPrefab3 prefab not found");
            return;
        }
        foreach (Transform trans in obj.transform)
        {
            trans.gameObject.SetActive(false);
            GameObject.Destroy(trans.gameObject);
        }

        sp.SetActive(true);
        grid = new CropsStageGridUI(sp);
        if (null == grid)
            return;
        grid.SetCropsId(item.id);
        grid.SetIcon(item.picname, item.picname2);
        grid.SetName(item.name);
        grid.SetStarslv(starslv);

        float hp = 0.0f;
        float damage = 0.0f;
        float crits = 0.0f;
        float defence = 0.0f;
        float energy = 0.0f;
        module.GetProperty(resid, starslv, ref hp, ref damage, ref crits, ref defence, ref energy);
        grid.SetProperty(hp, damage, crits, defence, energy);
        sp.transform.parent = obj.transform;
        sp.transform.localScale = Vector3.one;
        sp.transform.localPosition = localPosition;
        mRiseGrid.Reposition();

        obj.SetActive(true);
    }

    private void InitSkillPanel(int resid,int starslv)
    {
        mBtnRiseStars.gameObject.SetActive(true);
        mArrowPanel.SetActive(true);
        //skill相关信息
        CropsTableItem item = DataManager.CropsTable[resid] as CropsTableItem;
        if (null == item)
            return;
        CropsSkillLearnTableItem learn = DataManager.CropsSkillLearnTable[item.skillid1] as CropsSkillLearnTableItem;
        if (null == learn)
            return;

        UILabel mSkillName = ObjectCommon.GetChildComponent<UILabel>(mSkillPanel, "name");
        UISprite mSkillIcon = ObjectCommon.GetChildComponent<UISprite>(mSkillPanel, "icon");
        UILabel mUnLockNotes = ObjectCommon.GetChildComponent<UILabel>(mSkillPanel, "Label");

        if (starslv < learn.unlock_lv)
        {
            mSkillName.text = learn.skill_name;
            UIAtlasHelper.SetSpriteImage(mSkillIcon, learn.skill_icon);
            mUnLockNotes.text = string.Format(StringHelper.GetString("crops_skill_unlock_hint"), learn.unlock_lv.ToString());
            mSkillPanel.SetActive(true);
            return;
        }

        learn = DataManager.CropsSkillLearnTable[item.skillid2] as CropsSkillLearnTableItem;
        if (null == learn)
            return;

        if (starslv < learn.unlock_lv)
        {
            mSkillName.text = learn.skill_name;
            UIAtlasHelper.SetSpriteImage(mSkillIcon, learn.skill_icon);
            mUnLockNotes.text = string.Format(StringHelper.GetString("crops_skill_unlock_hint"), learn.unlock_lv.ToString());
            mSkillPanel.SetActive(true);
            return;
        }

        learn = DataManager.CropsSkillLearnTable[item.skillid3] as CropsSkillLearnTableItem;
        if (null == learn)
            return;

        if (starslv < learn.unlock_lv)
        {
            mSkillName.text = learn.skill_name;
            UIAtlasHelper.SetSpriteImage(mSkillIcon, learn.skill_icon);
            mUnLockNotes.text = string.Format(StringHelper.GetString("crops_skill_unlock_hint"), learn.unlock_lv.ToString());
            mSkillPanel.SetActive(true);
            return;
        }

        mSkillPanel.SetActive(false);
    }

    private void OnBtnRiseStarsHandler()
    {
        CropsModule module = ModuleManager.Instance.FindModule<CropsModule>();
        if (null == module)
            return;

        module.RiseCropsStars(mOldSelectedofStarsUI.GetCropsId());
    }

    private void OnRiseStarsHandler(EventBase evt)
    {
        InitRiseStarsUI();
        InitCampUI();
    }
    #endregion
}
