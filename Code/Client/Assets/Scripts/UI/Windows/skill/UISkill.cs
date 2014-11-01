using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UISkill : UIWindow
{
    //上翻页按钮;
    public UIButton pagePrev;
    public UIButton pagePrevAni;

    //下翻页按钮;
    public UIButton pageNext;
    public UIButton pageNextAni;

    //管界关闭按钮;
    //	public UIButton close;

    //所有技能展示列表根节点;
    public GameObject skillRoot;

    //所有技能展示列表的ScrollBar;
    public UIScrollBar skillSB;

    //技能槽技能根节点;
    public GameObject equipRoot;

    //文字说明处;
    public UILabel skillName;
    public UILabel skillLV;
    public UILabel detail1;
    public UILabel detail2;

    //消费金钱处;
    public UILabel costLb;
    public UILabel needLvLb;  //需要人物等级;
    public UISprite costTypeSp;
    public UISprite fullLvSp;
    public UIButton upgradeBtn;
    public UIButton unlockBtn;

    //技能未解锁提示信息处;
    public UILabel openLvLb;
    public UISprite openSp;

    public GameObject skillItemprefab = null;

    //装备槽动画;
    public UISpriteAnimation[] mEquiAnis = new UISpriteAnimation[4];

    public List<SkillItemUI> equipSkills = new List<SkillItemUI>();/// 技能槽列表;
    public GameObject[] jianTous = new GameObject[6];

    /*-----------------用于操作界面的数据------------
     * 界面操作，造成控制管理器数据改变，从而通知界面刷新;
     * 界面显示与管理器数据保持一致;
     * ------------------------------------------**/
    private const string SKILL_ITEM_NAME = "SkillSlot";
    ///一排最多显示技能个数;
    public const int mMaxShowSkillNum = 6;

    /// <summary>
    /// 是否需要初始化;
    /// </summary>
    private bool mNeedInit = true;

    /// <summary>
    /// 第一次是否摇晃;
    /// </summary>
    private bool mShakeAtStart = false;

    /// <summary>
    /// 是否可以装备技能;
    /// </summary>
    private bool mCanEquip = true;
    bool CanEquip { set { if (mCanEquip != value) { mCanEquip = value; } } }

    /// <summary>
    /// 拖动是否翻页;
    /// </summary>
    private bool mDragToPage = false;

    ///当前的页数;
    private int mCurPageNum = 0;

    /// <summary>
    /// 当前选择的技能的ID;
    /// </summary>
    private int mCurSkillId = -1;

    /// <summary>
    /// 当前选择的技能槽索引[0..3]
    /// </summary>
    private int mCurSlotIdx = -1;

    ///所有创建好的技能列表;
    private List<SkillItemUI> skillsList = new List<SkillItemUI>();/// 所有技能列表;

    SkillModule MySkillModule
    {
        get
        {
            return ModuleManager.Instance.FindModule<SkillModule>();
        }
    }

    public UISkill()
    {

    }
    protected override void OnLoad()
    {
        base.OnLoad();

        pagePrev = this.FindComponent<UIButton>("Center/PagePrev");
        pagePrevAni = this.FindComponent<UIButton>("Center/PagePrevAni");

        pageNext = this.FindComponent<UIButton>("Center/PageNext");
        pageNextAni = this.FindComponent<UIButton>("Center/PageNextAni");

        skillRoot = this.FindChild("Center/SkillList/SkillRoot");
        skillSB = this.FindComponent<UIScrollBar>("Center/SkillScrollBar");
        equipRoot = this.FindChild("Bottom/EquipSkillRoot");
        skillRoot = this.FindChild("Center/SkillList/SkillRoot");
        skillRoot = this.FindChild("Center/SkillList/SkillRoot");

        skillName = this.FindComponent<UILabel>("Top/Name/nameLb");
        skillLV = this.FindComponent<UILabel>("Top/Name/levelLb");

        detail1 = this.FindComponent<UILabel>("Top/leftLb");
        detail2 = this.FindComponent<UILabel>("Top/rightLb");
        needLvLb = this.FindComponent<UILabel>("Top/levelLabel");

        costLb = this.FindComponent<UILabel>("Bottom/Cost/costNumLb");
        costTypeSp = this.FindComponent<UISprite>("Bottom/Cost/costTypeSp");
        fullLvSp = this.FindComponent<UISprite>("Bottom/fullLv");
        upgradeBtn = this.FindComponent<UIButton>("Bottom/upgradeBtn");
        unlockBtn = this.FindComponent<UIButton>("Bottom/unlockBtn");
        openLvLb = this.FindComponent<UILabel>("Bottom/HintObj/hintLb");
        openSp = this.FindComponent<UISprite>("Bottom/HintObj/Sprite");

        mEquiAnis[0] = this.FindComponent<UISpriteAnimation>("Bottom/EquipSkillRoot/SkillSlot1/ani");
        mEquiAnis[1] = this.FindComponent<UISpriteAnimation>("Bottom/EquipSkillRoot/SkillSlot2/ani");
        mEquiAnis[2] = this.FindComponent<UISpriteAnimation>("Bottom/EquipSkillRoot/SkillSlot3/ani");
        mEquiAnis[3] = this.FindComponent<UISpriteAnimation>("Bottom/EquipSkillRoot/SkillSlot4/ani");

        equipSkills.Add(new SkillItemUI(this.FindChild("Bottom/EquipSkillRoot/SkillSlot1")));
        equipSkills.Add(new SkillItemUI(this.FindChild("Bottom/EquipSkillRoot/SkillSlot2")));
        equipSkills.Add(new SkillItemUI(this.FindChild("Bottom/EquipSkillRoot/SkillSlot3")));
        equipSkills.Add(new SkillItemUI(this.FindChild("Bottom/EquipSkillRoot/SkillSlot4")));

        jianTous[0] = this.FindChild("Top/Grid/sp1");
        jianTous[1] = this.FindChild("Top/Grid/sp2");
        jianTous[2] = this.FindChild("Top/Grid/sp3");
        jianTous[3] = this.FindChild("Top/Grid/sp4");
        jianTous[4] = this.FindChild("Top/Grid/sp5");
        jianTous[5] = this.FindChild("Top/Grid/sp6");

        skillItemprefab = this.FindChild("Items/SkillItem");


        if (mNeedInit)
            Init();

        UIEventListener.Get(skillRoot.transform.parent.gameObject).onDrag = onSkillItemDrag;

    }

    /// <summary>
    /// 技能索引，有效范围[1..]
    /// </summary>
    /// <value>The current skill identifier.</value>
    int CurSkillId
    {
        get
        {
            return mCurSkillId;
        }

        set
        {
            if (value == mCurSkillId)
                return;

            mCurSkillId = value;
            SetEquipSlotAniActive(mCurSkillId);
            SetSelect(mCurSkillId);
            UpdateDetails(true);
            UpdateUpgradeAndCost();
        }
    }

    /// <summary>
    /// 技能槽索引，有效范围[0..3]
    /// </summary>
    /// <value>The index of the current slot.</value>
    int CurSlotIdx
    {
        get
        {
            return mCurSlotIdx;
        }

        set
        {
            if (value == mCurSlotIdx)
                return;

            if (value > 3 || value < 0)
                return;

            mCurSlotIdx = value;
        }
    }

    /// <summary>
    /// 所有技能列表的当前页数信息;
    /// </summary>
    /// <value>The current page number.</value>
    public int CurPageNum
    {
        get
        {
            return mCurPageNum;
        }
        set
        {
            if (value == mCurPageNum)
                return;

            //保证当前页数的合法性;
            int effPageNum = value > 0 ? Mathf.Min(value, TotalPageNum) : Mathf.Max(value, 1);

            if (effPageNum != mCurPageNum)
            {
                mCurPageNum = effPageNum;
            }

            int pageNum = GetPageNumBySkillID(mCurSkillId);
            SetJianTousActive(pageNum == mCurPageNum);

            float x = 350f + (mCurPageNum - 1) * 840f;
            TweenPosition tp = TweenPosition.Begin(skillRoot, 0.3f, new Vector3(-x, 0f, 0f));
            EventDelegate.Add(tp.onFinished, Shake);
            tp.PlayForward();

            //			int startIdx = (mCurPageNum - 1) * mMaxShowSkillNum;
            //			int next = (mCurPageNum) * mMaxShowSkillNum;
            //			int endIdx = Mathf.Min(skillsList.Count , next);
            //			for(int m = 0 , n = skillsList.Count ; m < n ; m++)
            //			{
            //				skillsList[m].gameObject.SetActive(false);
            //			}
            //			for(int i = startIdx ; i < endIdx ; i++)
            //			{
            //				skillsList[i].gameObject.SetActive(true);
            //			}
            //
            //			UIGrid grid = skillRoot.GetComponent<UIGrid>();
            //			if(grid != null)
            //			{
            //				grid.hideInactive = true;
            //				grid.repositionNow = true;
            //			}
        }
    }

    void Shake()
    {
        if (!mShakeAtStart)
        {
            mShakeAtStart = true;
            return;
        }

        iTween.ShakePosition(skillRoot.gameObject, new Vector3(-0.05f, 0f, 0f), 0.3f);
    }

    /// <summary>
    /// 总共有几页技能
    /// </summary>
    /// <value>The total page number.</value>
    public int TotalPageNum
    {
        get
        {
            int totalSkillNum = MySkillModule.GetAllSkillsNum();
            int max = mMaxShowSkillNum;

            return (totalSkillNum / max) + (totalSkillNum % max == 0 ? 0 : 1);
        }
    }

    /// <summary>
    /// 判断是否是最后一页
    /// </summary>
    /// <value><c>true</c> if this instance is last page; otherwise, <c>false</c>.</value>
    public bool IsLastPage
    {
        get
        {
            return mCurPageNum >= TotalPageNum;
        }
    }

    /// <summary>
    /// 判断是否是第一页
    /// </summary>
    /// <value><c>true</c> if this instance is first page; otherwise, <c>false</c>.</value>
    public bool IsFirstPage
    {
        get
        {
            return mCurPageNum <= 1;
        }
    }

    Pos getCurPagePos()
    {
        if (IsFirstPage && IsLastPage)
            return Pos.FirstLast;

        if (IsFirstPage)
            return Pos.First;

        if (IsLastPage)
            return Pos.Last;

        return Pos.Med;
    }

    void upDownAniHandler()
    {

        switch (getCurPagePos())
        {
            case Pos.FirstLast:
                pageNextAni.gameObject.SetActive(false);
                pagePrevAni.gameObject.SetActive(false);
                pageNext.gameObject.SetActive(true);
                pagePrev.gameObject.SetActive(true);
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

        UISpriteAnimation ani1 = pageNextAni.GetComponent<UISpriteAnimation>();
        if (ani1 != null) ani1.Reset();

        UISpriteAnimation ani2 = pagePrevAni.GetComponent<UISpriteAnimation>();
        if (ani2 != null) ani2.Reset();

    }

    /// <summary>
    /// 下翻页;
    /// </summary>
    void PageNext()
    {
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
    void PagePrev()
    {
        if (IsFirstPage)
        {
            //已经是第一页了;
            return;
        }

        CurPageNum--;
        upDownAniHandler();
    }

    void AddEventListener()
    {
        //EventSystem.Instance.addEventListener(SkillUIEvent.SKILL_SLOT_CHANGE, onSkillSlotChange);
        //EventSystem.Instance.addEventListener(SkillUIEvent.SKILL_LIST_CHANGE, onSkillListChange);
        EventSystem.Instance.addEventListener(SkillUIEvent.SKILL_LEVEL_UP, onSkillLvChanged);
        EventSystem.Instance.addEventListener(SkillUIEvent.SKILL_EQUIP, onEquipSlotChanged);
    }

    void RemoveEventListener()
    {
        //EventSystem.Instance.removeEventListener(SkillUIEvent.SKILL_SLOT_CHANGE, onSkillSlotChange);
        //EventSystem.Instance.removeEventListener(SkillUIEvent.SKILL_LIST_CHANGE, onSkillListChange);
        EventSystem.Instance.removeEventListener(SkillUIEvent.SKILL_LEVEL_UP, onSkillLvChanged);
        EventSystem.Instance.removeEventListener(SkillUIEvent.SKILL_EQUIP, onEquipSlotChanged);
    }

    void AddEventDelegate()
    {
        //		EventDelegate.Add(close.onClick , OnClose);
        EventDelegate.Add(pageNext.onClick, PageNext);
        EventDelegate.Add(pageNextAni.onClick, PageNext);
        EventDelegate.Add(upgradeBtn.onClick, Upgrade);
        EventDelegate.Add(unlockBtn.onClick, UnLock);
        EventDelegate.Add(pagePrev.onClick, PagePrev);
        EventDelegate.Add(pagePrevAni.onClick, PagePrev);
    }

    void RemoveEventDelegate()
    {
        //		EventDelegate.Remove(close.onClick , OnClose);
        EventDelegate.Remove(pageNext.onClick, PageNext);
        EventDelegate.Remove(pageNextAni.onClick, PageNext);
        EventDelegate.Remove(upgradeBtn.onClick, Upgrade);
        EventDelegate.Remove(unlockBtn.onClick, UnLock);
        EventDelegate.Remove(pagePrev.onClick, PagePrev);
        EventDelegate.Remove(pagePrevAni.onClick, PagePrev);
    }

    void SetEquipSlotAniActive(int skillID)
    {
        int idx = 0;

        bool isShow = !MySkillModule.GetSlotIdxBySkillID(skillID, ref idx);

        for (int i = 0, j = mEquiAnis.Length; i < j; i++)
        {
            if (isShow)
            {
                mEquiAnis[i].Reset();
            }
            mEquiAnis[i].gameObject.SetActive(isShow);
        }
    }

    void Init()
    {
        mNeedInit = false;

        if (equipSkills.Count != 4)
            Debug.LogError("技能槽");

        int count = jianTous.Length;
        if (count != 6)
            Debug.LogError("箭头");

        for (int i = 0; i < count; i++)
        {
            jianTous[i].SetActive(false);
        }

        if (!detail1 || !detail2)
        {
#if UNITY_EDITOR
            Debug.LogError("Detail1 or Detail2 not found!");
#endif
        }

        InitSkills();
        InitEquipSkills();

        CurSkillId = 1;
        CurPageNum = 1;
        skillSB.value = Mathf.Epsilon;

        needLvLb.color = new Color(1f, 1f, 1f);
    }

    /// <summary>
    /// 读档创建所有技能列表;
    /// </summary>
    void InitSkills()
    {
        int i = 0, j = 0;
        for (j = MySkillModule.GetAllSkillsNum(); i < j; i++)
        {
            CreateSkill(i + 1);
        }

        int num = j % 6 == 0 ? j / 6 : j / 6 + 1;
        int total = num * 6;
        for (; j < num * 6; j++)
        {
            CreateNullSkill(j + 1);
        }

        UIGrid grid = skillRoot.GetComponent<UIGrid>();
        if (grid != null)
        {
            grid.repositionNow = true;
        }
    }

    void InitEquipSkills()
    {
        for (int i = 0, j = equipSkills.Count; i < j; i++)
        {
            SkillItemUI item = equipSkills[i] as SkillItemUI;
            item.IsShowLv = false;
            item.IsShowEquiSlotNum = true;
            UIEventListener.Get(item.Bt.gameObject).onClick = onEquipItemClick;
        }
    }

    /// <summary>
    /// invalid 是否无效;
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="invalid"></param>
    void CreateSkill(int skillId, bool invalid = false)
    {
        GameObject go = UIResourceManager.Instance.CloneGameObject(skillItemprefab);
        
        if (go == null)
        {
            Debug.LogError("SkillItemUI Prefab Not Found");
            return;
        }

        UIEventListener.Get(go).onDrag = onSkillItemDrag;
        go.SetActive(true);
        go.name = SKILL_ITEM_NAME + skillId.ToString();
        go.transform.parent = skillRoot.transform;
        go.transform.localScale = Vector3.one;//防止缩放;

        SkillItemUI item = new SkillItemUI(go);
        if (item == null)
        {
            Debug.LogError("SkillItemUI Component Not Found");
            return;
        }

        //item.onDragScrollView = onSkillItemDrag;

        //		UIEventListener.Get(skillRoot.transform.parent.gameObject).onPress = onSkillItemPress;
        //		UIEventListener.Get(skillRoot.transform.parent.gameObject).onDrag = onSkillItemDrag;

        //		UIEventListener.Get(go).onDrag = onSkillItemDrag;

        if (invalid)//无效的技能;
        {
            go.GetComponent<UIButton>().normalSprite = "touming";
            go.GetComponent<UISprite>().spriteName = "touming";
            for (int m = 0, n = go.transform.childCount; m < n; m++)
            {
                go.transform.GetChild(m).gameObject.SetActive(false);
            }
        }
        else
        {
            //图标是不需要变的放在这里进行处理;
            item.SetSkillIcon(MySkillModule.GetSkillIconByID(skillId));

            UIEventListener.Get(go).onClick = onSkillItemClick;

            skillsList.Add(item);
        }
    }

    void CreateNullSkill(int skillID)
    {
        CreateSkill(skillID, true);
    }


    void onSkillSlotChange(EventBase ev)
    {
        SkillUIEvent sue = ev as SkillUIEvent;
        if (sue == null)
            return;

        ArrayList temp = new ArrayList();
        temp = sue.msg;//记录的slotIdx有效范围[0...3]
        if (temp == null)
        {
            ///默认刷新所有技能槽信息;
            UpdateEquipSkills();

            return;
        }

        for (int i = 0, j = temp.Count; i < j; i++)
        {
            int slotIdx = System.Convert.ToInt32(sue.msg[i]);
            UpdateEquipSkill(slotIdx);
        }
    }

    void onSkillListChange(EventBase ev)
    {
        SkillUIEvent sue = ev as SkillUIEvent;
        if (sue == null)
            return;

        ArrayList temp = new ArrayList();
        temp = sue.msg;//记录的slotIdx有效范围[0...3]
        if (temp == null)
        {
            ///默认刷新所有技能信息;
            UpdateSkills();

            return;
        }

        for (int i = 0, j = temp.Count; i < j; i++)
        {
            int skillId = System.Convert.ToInt32(sue.msg[i]);
            UpdateSkillInfo(skillId);
        }
    }

    void UpdateSkills()
    {
        IDictionaryEnumerator itr = DataManager.SkillLearnTable.GetEnumerator();
        while (itr.MoveNext())
        {
            UpdateSkillInfo(Convert.ToInt32(itr.Key));
        }
//         foreach (int key in DataManager.SkillLearnTable.Keys)
//         {
//             UpdateSkillInfo(key);
//         }
    }

    /// <summary>
    /// 更新指定技能ID的信息，技能列表中，技能索引减1即列表索引;
    /// </summary>
    /// <param name="skillId">Skill identifier.</param>
    void UpdateSkillInfo(int skillId)
    {
        if (!MySkillModule.IsLegalSkillID(skillId))
            return;

        //		if(skillId > skillsList.Count || skillId < 1)
        //			return;

        SkillItemUI item = skillsList[skillId - 1] as SkillItemUI;
        if (item == null)
            return;

        int skillLv = MySkillModule.GetSkillLvBySkillID(skillId);
        bool canOpen = MySkillModule.canOpen(skillId);
        if (skillLv == 0 && !canOpen)//锁定状态;
        {
            item.SLockType = SkillLockType.Locked;
            item.IsShowEquiSlotNum = false;
            item.IsShowLv = false;
            int openLv = MySkillModule.GetSkillUnlockLvByID(skillId);
            item.SetShowHint(openLv.ToString() + "级解锁");
            return;
        }

        else if (skillLv == 0 && canOpen)//可解锁状态;
        {
            item.SLockType = SkillLockType.Opened;
            item.IsShowEquiSlotNum = false;
            item.IsShowLv = false;
            item.SetShowHint("可解锁");
            return;
        }

        else
        {
            item.SLockType = SkillLockType.UnLocked;
            item.SetShowLevel(skillLv);
            item.IsShowHint = false;

            int slotIdx = -1;
            if (MySkillModule.GetSlotIdxBySkillID(skillId, ref slotIdx))
            {
                item.IsShowEquiSlotNum = true;
                item.SetShowEquipNum(slotIdx + 1);
            }
            else
                item.IsShowEquiSlotNum = false;
        }
    }

    /// <summary>
    /// 刷新技能说明处显示信息;
    /// </summary>
    void UpdateDetails(bool needTypeWriterEffect)
    {
        SkillLearnTableItem learnItem = MySkillModule.GetDetailBySkillId(CurSkillId);
        if (learnItem == null) return;

        SkillLevelTableItem levelItem = null;
        int levelId = 0;

        int lv = MySkillModule.GetSkillLvBySkillID(CurSkillId);
        if (lv < 0)
            return;

        skillName.text = learnItem.skill_name;

        if (lv == 0)//锁定状态/可解锁状态;
        {
            if (!MySkillModule.canOpen(CurSkillId))//锁定;
            {
                skillLV.text = StringHelper.StringWithColor(FontColor.Red, "未解锁");
            }
            else
            {
                skillLV.text = "[079d6a]可解锁[-]";
            }

            levelId = SkillModule.SkillLearnToLevel(CurSkillId, 1);
            levelItem = MySkillModule.GetDetailByLevelID(levelId);
            if (levelItem == null)
            {
                Debug.LogError("SkillLevel表格缺少ID为" + levelId.ToString() + "的数据！");
                return;
            }
            detail1.text = learnItem.skill_desc + "\n" + levelItem.skill_desc.Replace("\\n", "\n");

            levelId = SkillModule.SkillLearnToLevel(CurSkillId, 2);
            levelItem = MySkillModule.GetDetailByLevelID(levelId);
            if (levelItem == null)
            {
                Debug.LogError("SkillLevel表格缺少ID为" + levelId.ToString() + "的数据！");
                return;
            }
            detail2.text = levelItem.skill_desc.Replace("\\n", "\n");

            needLvLb.gameObject.SetActive(true);
            int lvUpLv = SkillModule.GetPlayerLvBySkillLevelId(levelId);
            if (lvUpLv > 0)
            {
                PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
                if (pdm == null)
                {
                    GameDebug.LogError("fuck");
                    return;
                }

                if (pdm.GetLevel() < lvUpLv)
                {
                    needLvLb.text = string.Format(StringHelper.GetString("skill_open_lv", FontColor.Red), lvUpLv);
                }
                else
                {
                    needLvLb.text = string.Format(StringHelper.GetString("skill_open_lv", FontColor.Green), lvUpLv);
                }
            }
            else
            {
                needLvLb.text = "";
            }
        }

        else
        {
            skillLV.text = "Lv. " + lv.ToString();

            //levelItem = MySkillModule.GetDetailByCurLvSkillID(CurSkillId, MySkillModule.GetSkillLvBySkillID(CurSkillId) - 1);
            levelId = SkillModule.SkillLearnToLevel(CurSkillId, lv);
            levelItem = MySkillModule.GetDetailByLevelID(levelId);
            
            if (levelItem == null)
            {
                Debug.LogError("error le");
                return;
            }

            detail1.text = learnItem.skill_desc + "\n" + levelItem.skill_desc.Replace("\\n", "\n");

            if (MySkillModule.IsFullLv(CurSkillId))
            {
                detail2.text = StringHelper.GetString("skill_full_lv");
                needLvLb.gameObject.SetActive(false);
            }

            else
            {
                levelId = SkillModule.SkillLearnToLevel(CurSkillId, lv + 1);
                levelItem = MySkillModule.GetDetailByLevelID(levelId);

                if (levelItem == null)
                {
                    Debug.LogError("SkillLevel表格缺少ID为" + levelId.ToString() + "的数据！");
                    return;
                }
                
                needLvLb.gameObject.SetActive(true);
                int lvUpLv = SkillModule.GetPlayerLvBySkillLevelId(levelId);
                if(lvUpLv > 0)
                {
                    PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
                    if(pdm == null)
                    {
                        GameDebug.LogError("fuck");
                        return;
                    }
                    
                    if(pdm.GetLevel() < lvUpLv)
                    {
                        needLvLb.text = string.Format(StringHelper.GetString("skill_open_lv" , FontColor.Red) , lvUpLv);
                    }
                    else
                    {
                        needLvLb.text = string.Format(StringHelper.GetString("skill_open_lv", FontColor.Green), lvUpLv);
                    }
                }
                else
                {
                    needLvLb.text = "";
                }

                detail2.text = levelItem.skill_desc.Replace("\\n", "\n");
            }
        }


        TypewriterEffect te1 = null, te2 = null;

        if (UIEffectManager.IsUIEffectActive<TypewriterEffect>(detail1.gameObject, ref te1))
        {
            if (needTypeWriterEffect)
            {
                te1.ReStart();
            }
            else
            {
                te1.SetProcessText();
            }
        }
        else
        {
            if (needTypeWriterEffect)
                detail1.gameObject.AddMissingComponent<TypewriterEffect>();
        }
        if (UIEffectManager.IsUIEffectActive<TypewriterEffect>(detail2.gameObject, ref te2))
        {
            if (needTypeWriterEffect)
            {
                te2.ReStart();
            }
            else
            {
                te2.SetProcessText();
            }
        }
        else
        {
            if (needTypeWriterEffect)
                detail2.gameObject.AddMissingComponent<TypewriterEffect>();
        }
    }

    void UpdateEquipSkills()
    {
        uint j = SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM;
        for (int i = 0; i < j; i++)
        {
            UpdateEquipSkill(i);
        }
    }

    /// <summary>
    /// 刷新技能槽上技能的信息，参数技能槽索引，有效范围[0..3]
    /// </summary>
    void UpdateEquipSkill(int slotIdx)
    {
        SkillItemUI item = equipSkills[slotIdx] as SkillItemUI;
        if (item == null)
            return;

        //锁定状态;
        if (MySkillModule.IsSlotLocked(slotIdx + 1))
        {
            item.SLockType = SkillLockType.Locked;
            item.IsShowIcon = false;
            item.IsTrigger = false;//停止响应点击事件;
        }
        else
        {
            int skillID = MySkillModule.GetEquipSkillID(slotIdx);

            SkillLearnTableItem slti = MySkillModule.GetDetailBySkillId(skillID);
            if (slti == null)
            {
                item.SetSkillIcon("");
                item.SLockType = SkillLockType.UnLocked;
                return;
            }

            item.SetSkillIcon(slti.skill_icon);
            item.SLockType = SkillLockType.UnLocked;
            item.IsShowIcon = true;
            item.IsTrigger = true;
        }
    }

    /// <summary>
    /// 刷新所有技能展示界面;
    /// 默认选中技能处改变，从而技能说明处改变;
    /// </summary>
    /// <param name="ev">Ev.</param>
    public void PageShowSkill(EventBase ev)
    {
        if (skillSB == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Scrollbar not found");
#endif
            return;
        }

        //几个技能评分scrollView 1.0f;
        int der = MySkillModule.GetAllSkillsNum() - mMaxShowSkillNum;
        float ratio = 1.0f / der;
        skillSB.value = (CurPageNum - 1) * mMaxShowSkillNum * ratio;
        foreach (EventDelegate ed in skillSB.onChange)
        {
            if (ed != null && ed.isValid)
                ed.Execute();
        }

        //当前显示页数之前技能和之后技能不显示;
        //当前是第2页，第一页*每页6个（skillsList索引0-5是隐藏的，6-11是显示的，12到最后一个技能是隐藏的）


    }

    /// <summary>
    /// 刷新技能升级消耗显示、按钮内容显示
    /// </summary>
    void UpdateUpgradeAndCost()
    {
        int lv = MySkillModule.GetSkillLvBySkillID(CurSkillId);

        //非法数据;
        if (lv < 0)
            return;

        //锁定状态;
        if (lv == 0 && !MySkillModule.canOpen(CurSkillId))
        {
            int unlockLv = MySkillModule.GetSkillUnlockLvByID(CurSkillId);
            if (unlockLv < 1)
                return;

            //			costLb.gameObject.SetActive(false);
            fullLvSp.gameObject.SetActive(false);
            costLb.transform.parent.gameObject.SetActive(false);
            upgradeBtn.gameObject.SetActive(false);
            equipRoot.SetActive(false);

            unlockBtn.gameObject.SetActive(true);
            //			unlockBtn.isEnabled = false;

            openSp.gameObject.SetActive(true);
            openLvLb.gameObject.SetActive(true);
            openLvLb.text = StringHelper.StringWithColor(FontColor.Red , "LV" + unlockLv.ToString()) + "才可以解锁此技能！";

            return;
        }
        //可解锁状态;
        else if (0 == lv && MySkillModule.canOpen(CurSkillId))
        {
            //			costLb.gameObject.SetActive(false);
            fullLvSp.gameObject.SetActive(false);
            costLb.transform.parent.gameObject.SetActive(false);
            upgradeBtn.gameObject.SetActive(false);
            equipRoot.SetActive(false);

            unlockBtn.gameObject.SetActive(true);
            //			unlockBtn.isEnabled = true;

            openSp.gameObject.SetActive(true);
            openLvLb.gameObject.SetActive(true);
            openLvLb.text = "可以解锁此技能！";
        }

        //有等级状态;
        else
        {
            equipRoot.SetActive(true);
            openSp.gameObject.SetActive(false);
            openLvLb.gameObject.SetActive(false);
            unlockBtn.gameObject.SetActive(false);

            //满级了;
            if (MySkillModule.IsFullLv(CurSkillId))
            {
                //				costLb.gameObject.SetActive(false);
                costLb.transform.parent.gameObject.SetActive(false);
                upgradeBtn.gameObject.SetActive(false);

                fullLvSp.gameObject.SetActive(true);
            }
            else
            {
                fullLvSp.gameObject.SetActive(false);

                //				costLb.gameObject.SetActive(true);
                costLb.transform.parent.gameObject.SetActive(true);
                upgradeBtn.gameObject.SetActive(true);

                SkillLevelTableItem slti = MySkillModule.GetDetailByCurLvSkillID(CurSkillId);
                if (slti == null) return;

                UIAtlasHelper.SetSpriteByMoneyType(costTypeSp, (ProceedsType)slti.money_type, false);
                //if (slti.money_type == 0)
                //{
                //    //costTypeSp.spriteName = "jinbi";
                //}
                //else if (slti.money_type == 1)
                //    costTypeSp.spriteName = "zuanshi";
                //				costTypeSp.MakePixelPerfect();

                PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
                if (pdm == null) GameDebug.LogError(@"PlayerData没了");

                uint curMoney = pdm.GetProceeds((ProceedsType)slti.money_type);
                FontColor color = curMoney < slti.money_num ? FontColor.Red : FontColor.Green;
                costLb.text = StringHelper.StringWithColor(color , slti.money_num.ToString());
            }
        }
    }

    void onSkillItemPress(GameObject go, bool isPressed)
    {

    }

    void onSkillItemDrag(Vector2 delta)
    {
        onSkillItemDrag(null, delta);
    }

    void onSkillItemDrag(GameObject go, Vector2 delta)
    {
        if (delta.x > 0)
            PagePrev();
        else if (delta.x < 0)
            PageNext();
    }

    void onSkillItemClick(GameObject go)
    {
        int skillId = System.Convert.ToInt32(go.name.Replace(SKILL_ITEM_NAME, ""));

        CurSkillId = skillId;
        CanEquip = true;
    }

    void onEquipItemClick(GameObject go)
    {
        ///范围[1...4]
        int slotIdx = System.Convert.ToInt32(go.name.Replace(SKILL_ITEM_NAME, ""));

        if (mCanEquip)
        {
            MySkillModule.EquipSkill(slotIdx, CurSkillId);
            SetSelect(CurSkillId);
        }
        else
        {
            CurSkillId = MySkillModule.GetEquipSkillID(slotIdx - 1);
        }

        CanEquip = false;
    }

    /// <summary>
    /// 设置当前选中的技能id
    /// </summary>
    /// <param name="skillId">Skill identifier.</param>
    void SetSelect(int skillId)
    {
        if( skillId <= 0 || skillId > skillsList.Count )
        {
            return;
        }

        int pageNum = GetPageNumBySkillID(skillId);

        //打开到指定页，并将该技能ID对应的技能设置为选中状态;
        GoToPage(pageNum);
        for (int i = 0, j = skillsList.Count; i < j; i++)
        {
            skillsList[i].IsSelected = false;
        }
        skillsList[skillId - 1].IsSelected = true;

        for (int i = 0, j = jianTous.Length; i < j; i++)
        {
            jianTous[i].SetActive(false);
        }
        int jIdx = (skillId - 1) % mMaxShowSkillNum;
        jianTous[jIdx].SetActive(true);

        SetJianTousActive(pageNum == mCurPageNum);

        //如果该技能ID在技能槽中，也选中;
        int slotIdx = 0;
        for (int i = 0, j = equipSkills.Count; i < j; i++)
        {
            equipSkills[i].IsSelected = false;
        }
        if (MySkillModule.GetSlotIdxBySkillID(skillId, ref slotIdx))
            equipSkills[slotIdx].IsSelected = true;
    }

    /// <summary>
    /// 设置技能展示列表翻到第几页;
    /// </summary>
    void GoToPage(int pageNum)
    {
        CurPageNum = pageNum;
    }

    void SetJianTousActive(bool isShow)
    {
        GameObject go = jianTous[0].transform.parent.gameObject;
        TweenAlpha.Begin(go, 0.5f, isShow ? 1f : 0f).PlayForward();
        //		.SetActive(isShow);
    }

    /// <summary>
    /// 根据技能ID，取出当前技能在技能列表中的第几页;
    /// return 范围[1...]
    /// </summary>
    /// <returns>The page number skill I.</returns>
    /// <param name="skillID">Skill I.</param>
    int GetPageNumBySkillID(int skillID)
    {
        if (skillID < 1)
            return 1;

        int pageNum = (skillID - 1) / mMaxShowSkillNum + 1;

        if (pageNum < 1 || pageNum > TotalPageNum)
        {
#if UNITY_EDITOR
            Debug.LogError("技能ID没有在所有技能中找到");
#endif
        }

        return pageNum;
    }

    void onEquipSlotChanged(EventBase ev)
    {
        if (ev == null) return;

        SkillUIEvent se = ev as SkillUIEvent;
        if (se == null) return;

        UpdateEquipSkills();
        UpdateSkills();

        for (int i = 0, j = mEquiAnis.Length; i < j; i++)
        {
            mEquiAnis[i].gameObject.SetActive(false);
        }
    }

    void onSkillLvChanged(EventBase ev)
    {
        if (ev == null) return;

        SkillUIEvent se = ev as SkillUIEvent;
        if (se == null) return;

        int skillId = se.skillId;
        UpdateSkillInfo(skillId);
        UpdateUpgradeAndCost();
        UpdateDetails(false);
    }

    ///技能升级;
    void Upgrade()
    {
        MySkillModule.Upgrade(CurSkillId);
    }

    /// <summary>
    /// 技能解锁;
    /// </summary>
    void UnLock()
    {
        SkillItemUI item = skillsList[CurSkillId - 1] as SkillItemUI;
        if (item == null)
            return;
        if (item.SLockType == SkillLockType.Opened)
            MySkillModule.UnLock(CurSkillId);
        else 
        {
            //人物等级不足，解锁失败！;
            PopTipManager.Instance.AddNewTip(StringHelper.StringWithColor(FontColor.Red,StringHelper.GetString("skill_unlock_nolv")));
        }
    }

    /// <summary>
    /// 解锁技能槽，界面显示;
    /// </summary>
    public void UnLockSlot()
    {

    }

    /// <summary>
    /// 装备/切换技能,可能涉及到动画播放(切换动画，装备/切换成功动画);
    /// </summary>
    public void EquipSkill()
    {

    }


    /**--------------技能选中显示规则;---------------
     * 1.点击技能列表处的技能，高亮，同时技能槽中如果存在该技能，也高亮。
     * 2.点击技能槽处的技能，高亮，同时技能列表处，显示该技能所在页的所有技能，并在技能列表处高亮显示;
     */
    /**--------------装备的技能切换显示规则-----------
     * 点击技能列表处的技能，再点击要装备技能的技能槽
     * 1.如果该技能没被装备在技能槽中，择替换并装备该技能;
     * 2.如果该技能被装备在技能槽中，
     *   2.1点击的技能槽位置与原来一致，不作处理;
     *   2.2点击的技能槽位置与原来不一直，互换这两个技能的位置;
     */

    protected override void OnOpen(object param = null)
    {
        //添加事件监听器;
        AddEventListener();
        AddEventDelegate();

        if (mNeedInit)
        {
            Init();
        }
        else
        {
            CurSkillId = 1;
            CanEquip = true;
            CurPageNum = 1;
            skillSB.value = Mathf.Epsilon;
        }

        //		UpdateDetails(null);
        UpdateEquipSkills();
        UpdateSkills();
        UpdateDetails(true);
        UpdateUpgradeAndCost();
        upDownAniHandler();
    }
    protected override void OnClose()
    {
        RemoveEventListener();
        RemoveEventDelegate();
    }
    public override void Update(uint elapsed)
    {

    }
}
