using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBattleFormInitParam
{
    public bool DisplayLianJi = false;
    public bool DisplayerGuideArrow = true;
	public bool DisplayController = true;
}

class AttackUIComponents
{
	public GameObject regularAttackBtn = null;
	public GameObject itemBtn = null;
	public UISprite itemIcon = null;
	public List<GameObject> skillBtns = new List<GameObject>();
	public List<UISprite> skillCoolDown = new List<UISprite>();
	public List<UILabel> skillCoolDownNumber = new List<UILabel>();

	public void Initialize(UIWindow rootUI, string attackUIName)
	{
		if (!attackUIName.EndsWith("/"))
			attackUIName += "/";
		regularAttackBtn = rootUI.FindChild(attackUIName + "NoramlSkill");
		itemBtn = rootUI.FindChild(attackUIName + "Item");
		itemIcon = rootUI.FindComponent<UISprite>(attackUIName + "Item/ItemIcon");
		skillBtns.Add(rootUI.FindChild(attackUIName + "Skill1"));
		skillBtns.Add(rootUI.FindChild(attackUIName + "Skill2"));
		skillBtns.Add(rootUI.FindChild(attackUIName + "Skill3"));
		skillBtns.Add(rootUI.FindChild(attackUIName + "Skill4"));
		skillBtns.Add(rootUI.FindChild(attackUIName + "NoramlSkill/WeaponSkill"));

		foreach (GameObject btn in skillBtns)
			btn.GetComponent<UIButton>().CustomData = -1;

		skillCoolDown.Add(rootUI.FindComponent<UISprite>(attackUIName + "Skill1/CoolDown"));
		skillCoolDown.Add(rootUI.FindComponent<UISprite>(attackUIName + "Skill2/CoolDown"));
		skillCoolDown.Add(rootUI.FindComponent<UISprite>(attackUIName + "Skill3/CoolDown"));
		skillCoolDown.Add(rootUI.FindComponent<UISprite>(attackUIName + "Skill4/CoolDown"));
		skillCoolDown.Add(rootUI.FindComponent<UISprite>(attackUIName + "NoramlSkill/WeaponSkill/CoolDown"));

		skillCoolDownNumber.Add(rootUI.FindComponent<UILabel>(attackUIName + "Skill1/Time"));
		skillCoolDownNumber.Add(rootUI.FindComponent<UILabel>(attackUIName + "Skill2/Time"));
		skillCoolDownNumber.Add(rootUI.FindComponent<UILabel>(attackUIName + "Skill3/Time"));
		skillCoolDownNumber.Add(rootUI.FindComponent<UILabel>(attackUIName + "Skill4/Time"));
		skillCoolDownNumber.Add(rootUI.FindComponent<UILabel>(attackUIName + "NoramlSkill/WeaponSkill/Time"));
	}

	public UIButton ExtendedWeaponSkillButton { get { return skillBtns[skillBtns.Count - 2].GetComponent<UIButton>(); } }
	public UISprite ExtendedWeaponSkillCdSprite { get { return skillCoolDown[skillCoolDown.Count - 2]; } }
	public UILabel ExtendedWeaponSkillCdLable { get { return skillCoolDownNumber[skillCoolDownNumber.Count - 2]; } }

	public UIButton HiddenExtendedWeaponSkillButton { get { return skillBtns[skillBtns.Count - 1].GetComponent<UIButton>(); } }
	public UISprite HiddenExtendedWeaponSkillCdSprite { get { return skillCoolDown[skillCoolDown.Count - 1]; } }
	public UILabel HiddenExtendedWeaponSkillCdLable { get { return skillCoolDownNumber[skillCoolDownNumber.Count - 1]; } }
}

public class UIBattleForm : UIWindow
{

	AttackUIComponents mNormalAttackComponents = new AttackUIComponents();
	AttackUIComponents mTransformedAttackComponents = new AttackUIComponents();
	AttackUIComponents mActiveAttackComponents = null;

     public UIButton mWeaponSkillBtn = null;
     public UISprite mWeaponSkillCoolDown = null;
     public UILabel  mWeaponSkillCoolDownNumber = null;

//      public UIButton mWeaponSkillBtn1 = null;
//      public UISprite mWeaponSkillCoolDown1 = null;
//      public UILabel mWeaponSkillCoolDownNumber1 = null;
// 
//      public UIButton mWeaponSkillBtn2 = null;
//      public UISprite mWeaponSkillCoolDown2 = null;
//      public UILabel mWeaponSkillCoolDownNumber2 = null;

	private GameObject mWeaponUI;
	private GameObject mSkillUI;
    private UISprite mMainWeaponIcon = null;
    //private UISprite mSubWeaponIcon = null;

    private UIButton mChangeWeaponBtn = null;

    private GameObject mMainCropsObj = null;
    private GameObject mSubCropsObj = null;


    public GameObject bossUI;
    public UISprite bossicon;
    public UIMultiProgressBar bossHpBar;
    public GameObject bossKuangBao;
//     public GameObject hpBar0;
//     public GameObject hpBar1;
//     public GameObject hpBar2;
//     public GameObject hpBar3;
//     public GameObject hpBar4;
//     public GameObject hpBarBack0;
//     public GameObject hpBarBack1;
//     public GameObject hpBarBack2;
//     public GameObject hpBarBack3;
//     public GameObject hpBarBack4;
    public UILabel hpBarCount;
    public UILabel bossLvName;
    public UISprite comboUISprite;
    public UILabel comboNumber;
 
    public GameObject mGuideArrow;
	public GameObject mGuidePanel;

    public GameObject mZomebieUIObj;
    public GameObject mGearObj;
    public UISprite[] mGearSps = new UISprite[3];
    public GameObject mItemArrow;
    public UISprite mPickItemIcon;
    public GameObject mTimeCounter;
    public UILabel mTimeCountLabel;
    public GameObject mZombiesCrazySp;//僵尸狂暴;
    public UILabel mGoldCounterLb;//计算累计获得金币数;
    public GameObject mGoldTarget;//金币飞向的目标;
    private UISlider mZombieBar;
    public UISlider mEnermyBar;
    public UILabel mZombieEnermyLb;
    public UILabel mZombieBarHint;

	private GameObject mStageProgressBar;
	private GameObject mStageProgressObj;
	private UISlider mStageProgress;
	private UILabel mStageProgressText;
	private GameObject mGoArrowObj;
	//private GameObject mArrowObj;
	private UISpriteAnimation mProgressAni;
	private UITweener mGoAni;

    //Head
    public UISprite mFace = null;
    public UISlider mHPPro = null;
    public UISlider mSPPro = null;
    public UILabel mLevel = null;
    public UILabel mName = null;

    public UILabel mHPNumber = null;
    public UILabel mSPNumber = null;


    public UILabel mBulletNumber;
    public UISprite mReloadProgress;
    public UISprite mReloadSprite;
    public UISprite mReloadTip;


    private GameObject mSuperWeaponProBK = null;
    private UISprite mSuperWeaponProgress = null;

    public GameObject mWeaponBullet = null;

    public GameObject mWeaponUp = null;
    public UISprite mWeaponUpProgress = null;

    private UIButton mPauseBtn = null;

    private UIButton mTempChatButton = null;

    //crops
    private UISprite mMainCropsCoolDownBg = null;
    private UISprite mSubCropsCoolDownBg = null;


    private bool mReloading = false;
    private float mReloadTime = 0.0f;
    private float mReloadMaxTime = 0.0f;

    private bool mWeaponUping = false;
    private float mWeaponUpTime = 0.0f;
    private float mWeaponUpMaxTime = 0.0f;

    private bool mRegularAttackBtnDown = false;
	private bool mTransformedRegularAttackBtnDown = false;

    private SkillModule mSkillModule = null;

    private bool mComboShow = false;
    private float mKillInterval = 0.0f;

    private int mKillNumber = 0;

    private int mSoundTimeResId = -1;//剩余十秒音效id;
    private bool mNeedPlayingSound = false;//是否播放剩余十秒音效id;
    private int mGearNumber = 0;//齿轮碎片个数;
    private int mBuffSliceNum = 0; //僵尸本buff碎片个数;
    private bool mIsInBuffDur = false;//是否在buff持续时间（期间不可以在拾取buff碎片）;
    private bool mIsZombieGame = false;//是否打僵尸游戏;
    private bool mIsDouBiMaoGame = false;//逗比猫场景是否进入;
    //private MyTimer mTimer = null;//计时;
    private ScrollNumEffect mScrollNumEff = null;
    private float mFlyDuration = 0.3f;
    private List<uint> mGoldNumList = new List<uint>();//每次捡到金币后记录下当前的金币数;
    private bool ignorFirstGoldNumChanged = true;

    private float mScale = 0.03f;

    private bool mNeedInit = true;

    private List<GameObject> mHpBarList = new List<GameObject>();
    private List<GameObject> mHpBarBackList = new List<GameObject>();

    private bool mSuperWeapon = false;
    private int mSuperWeaponTime = 0;
    private int mMaxSuperWeaponTime = 0;
    private bool mMainCropsDie = false;
    private uint mMainCropsDieCoolDownTime = 0;
    private bool mSubCropsDie = false;
    private uint mSubCropsDieCoolDownTime = 0;
    private int mMainCropsOldHp = int.MaxValue;
    private int mSubCropsOldHp = int.MaxValue;
    private uint mMaxReliveTime = uint.MaxValue;


    private UISprite mBossFury = null;

    private bool mBossBeginFury = false;
    private int mBossFuryTime = 0;


    private GameObject mSkillFlyItem = null;
    private UISprite mSkillFlyIcon = null;

    private int mFireSkillID = -1;

    private Vector3 mSkillFlyPos = Vector3.zero;

    #region boss血条动画变量;
    private UISlider mHpAniBar;     //血条slider;
    private UISprite mHpAniFg;      //血条前景色;
    //boss血条;
    const float mBossBloodAniDur = 0.5f; //播放一条血需要多长时间;
    private float mToBossBloodTotalVal = -1f;    // mBossBloodIdx + mBossBloodFilledVal;
    private float mCurBossBloodTotalVal = -1f;   // 
    private bool mInitValDone = false;
    #endregion

	private BattleUIModule mBattleUIModule = ModuleManager.Instance.FindModule<BattleUIModule>();

    float BossBloodTotalVal
    {
        get
        {
            return mToBossBloodTotalVal;
        }

        set
        {
            if (Mathf.Approximately(mToBossBloodTotalVal, value))
            {
                return;
            }

            if (mToBossBloodTotalVal < 0f) // 第一次不记变化效果;
            {
                mToBossBloodTotalVal = value;
                return;
            }

            if (value > mToBossBloodTotalVal)// 加血效果;
            {
                return;
            }

            if (value < mToBossBloodTotalVal)// 减血效果;
            {
                
            }

            mToBossBloodTotalVal = value;
        }
    }

    // 记录冷却中的技能个数;
    private BetterList<int> mUseSkillIdxs = new BetterList<int>();
   // private GameObject mCurCoolDownObj = null;

    private TweenAlpha comAlpha = null;

    private int mCurTime;
    private int mTmpTime = -1;

    //挑战本
    private bool mIsMonsterFlood = false;//是否是挑战本；
    private UILabel mMonsterNum;
    private UILabel mTempMoneyNum;
    private GameObject mMonsterFloodObj;
	private UISprite mMonsterNumBg;
    private bool mComboScale = false;

    private bool mMayLianJi = true;
    private bool mDisplayerGuildArrow = true;

    public UIBattleForm()
    {
		mActiveAttackComponents = mNormalAttackComponents;
    }
    void InitMoudle()
    {
        mNeedInit = false;
        mSkillModule = ModuleManager.Instance.FindModule<SkillModule>();
    }

    protected override void OnLoad()
    {
        mSkillFlyItem = this.FindChild("SkillFlyUI/SkillFlyItem");

        mSkillFlyIcon = this.FindComponent<UISprite>("SkillFlyUI/SkillFlyItem/Icon");
        mSkillFlyPos = mSkillFlyItem.transform.position;

        mBossFury = this.FindComponent<UISprite>("BossFury");
        mBossFury.gameObject.SetActive(false);
		mNormalAttackComponents.Initialize(this, "Attack");
		mTransformedAttackComponents.Initialize(this, "TransformedAttack");
		NGUITools.SetActive(this.FindChild("TransformedAttack"), false);

        mPauseBtn = this.FindComponent<UIButton>("Head/PauseBtn");

		mSkillUI = this.FindChild("Attack");
		mWeaponUI = this.FindChild("ChangeUI");
        mMainWeaponIcon = this.FindComponent<UISprite>("ChangeUI/Sprite1/Sprite");
        //mSubWeaponIcon = this.FindComponent<UISprite>("ChangeUI/Sprite2/Sprite");
        mChangeWeaponBtn = this.FindComponent<UIButton>("ChangeUI/Btn");
        mMainCropsObj = this.FindChild("CropsUI/Sprite3");
        mSubCropsObj = this.FindChild("CropsUI/Sprite4");

        bossUI = this.FindChild("Boss");
        bossicon = this.FindComponent<UISprite>("Boss/BossIcon");
        bossHpBar = this.FindComponent<UIMultiProgressBar>("Boss/Progress");
        bossKuangBao = this.FindChild("Boss/Kuangbao");
        mHpBarList.Add(this.FindChild("Boss/Progress/Sprite0"));
        mHpBarList.Add(this.FindChild("Boss/Progress/Sprite1"));
        mHpBarList.Add(this.FindChild("Boss/Progress/Sprite2"));
        mHpBarList.Add(this.FindChild("Boss/Progress/Sprite3"));
        mHpBarList.Add(this.FindChild("Boss/Progress/Sprite4"));

        mHpBarBackList.Add(this.FindChild("Boss/Progress/Back0"));
        mHpBarBackList.Add(this.FindChild("Boss/Progress/Back1"));
        mHpBarBackList.Add(this.FindChild("Boss/Progress/Back2"));
        mHpBarBackList.Add(this.FindChild("Boss/Progress/Back3"));
        mHpBarBackList.Add(this.FindChild("Boss/Progress/Back4"));

        mHpAniBar = this.FindComponent<UISlider>("Boss/bloodAniBar");
        mHpAniFg = this.FindComponent<UISprite>("Boss/bloodAniBar/fg");

        hpBarCount = this.FindComponent<UILabel>("Boss/Progress/HP");
        bossLvName = this.FindComponent<UILabel>("Boss/Name");
        comboUISprite = this.FindComponent<UISprite>("Combo");
        comboNumber = this.FindComponent<UILabel>("Combo/Number");

		mGuideArrow = this.FindChild("GuideTargetUI/mArrow");
		mGuidePanel = this.FindChild("GuideTargetUI");
        mZomebieUIObj = this.FindChild("ZombiesUI");
        mGearObj = this.FindChild("ZombiesUI/Gear");
        mGearSps[0] = this.FindComponent<UISprite>("ZombiesUI/Gear/gearSp1");
        mGearSps[1] = this.FindComponent<UISprite>("ZombiesUI/Gear/gearSp2");
        mGearSps[2] = this.FindComponent<UISprite>("ZombiesUI/Gear/gearSp3");
        mItemArrow = this.FindChild("ItemTargetUI/mItemArrow");
        mPickItemIcon = this.FindComponent<UISprite>("ItemTargetUI/pickIcon");
        mTimeCounter = this.FindChild("CountDownUI");
        mTimeCountLabel = this.FindComponent<UILabel>("CountDownUI/Time");
        mZombiesCrazySp = this.FindChild("ZombiesUI/zombieCrazySp");
        mGoldCounterLb = this.FindComponent<UILabel>("ZombiesUI/goldIconSp/GoldNumLb");
        mGoldTarget = this.FindChild("ZombiesUI/goldIconSp");
        mZombieBar = this.FindComponent<UISlider>("ZombiesUI/BuffBar");
        mEnermyBar = this.FindComponent<UISlider>("ZombiesUI/CountBar");
        mZombieEnermyLb = this.FindComponent<UILabel>("ZombiesUI/CountBar/enermyNum");
        mZombieBarHint = this.FindComponent<UILabel>("ZombiesUI/BuffBar/Label");

		mStageProgressBar = this.FindChild("StageProgress/mProgressBar");
		mStageProgressObj = this.FindChild("StageProgress");
		mStageProgress = this.FindComponent<UISlider>("StageProgress/mProgressBar");
		mStageProgressText = this.FindComponent<UILabel>("StageProgress/mProgressBar/enermyNum");
		mGoArrowObj = this.FindChild("StageProgress/GoArrow");
		//mArrowObj = this.FindChild("StageProgress/GoArrow/Arrow");
		mProgressAni = this.FindComponent<UISpriteAnimation>("StageProgress/mAni");
		mGoAni = mGoArrowObj.GetComponent<UITweener>();

        mFace = this.FindComponent<UISprite>("Head/Face");
        mHPPro = this.FindComponent<UISlider>("Head/HPProgress");
        mSPPro = this.FindComponent<UISlider>("Head/SPProgress");
        mLevel = this.FindComponent<UILabel>("Head/LevelBG/Level");
        mName = this.FindComponent<UILabel>("Head/Name");
        mHPNumber = this.FindComponent<UILabel>("Head/HPProgress/HP");
        mSPNumber = this.FindComponent<UILabel>("Head/SPProgress/SP");

        mBulletNumber = this.FindComponent<UILabel>("Head/ReloadBG/Number");

        mReloadProgress = this.FindComponent<UISprite>("Head/ReloadBG/Progress");
        mReloadSprite = this.FindComponent<UISprite>("Head/ReloadBG/Progress/Sprite");
        mReloadTip = this.FindComponent<UISprite>("Head/ReloadBG/Tip");
        mWeaponBullet = this.FindChild("Head/ReloadBG");
        mWeaponUp = this.FindChild("Head/QianghuaBG");
        mWeaponUpProgress = this.FindComponent<UISprite>("Head/QianghuaBG/Progress/Sprite");

        //挑战本
        mMonsterNum = this.FindComponent<UILabel>("MonsterFloodUI/Count");
        mTempMoneyNum = this.FindComponent<UILabel>("MonsterFloodUI/daibiIconSp/GoldNumLb");
        mMonsterFloodObj = this.FindChild("MonsterFloodUI");
		mMonsterNumBg = this.FindComponent<UISprite>("MonsterFloodUI/Count/background");

        mSuperWeaponProgress = this.FindComponent<UISprite>("ChangeUI/Progress/Sprite");
        mSuperWeaponProBK = this.FindChild("ChangeUI/Progress");

        mTempChatButton = this.FindComponent<UIButton>("Chat");
        InitMoudle();

		UIEventListener.Get(mNormalAttackComponents.regularAttackBtn.gameObject).onPress = onRegularAttackBtnPress;
		foreach (GameObject go in mNormalAttackComponents.skillBtns)
			UIEventListener.Get(go).onClick = onSkillClick;
		UIEventListener.Get(mNormalAttackComponents.itemBtn).onClick = onItemClick;

		UIEventListener.Get(mTransformedAttackComponents.regularAttackBtn.gameObject).onPress = onTransformedRegularAttackBtnPress;
		foreach (GameObject go in mTransformedAttackComponents.skillBtns)
			UIEventListener.Get(go).onClick = onSkillClick;
		UIEventListener.Get(mTransformedAttackComponents.itemBtn).onClick = onTransformedItemClick;

        EventDelegate.Add(mChangeWeaponBtn.onClick, onChangeWeapon);

        EventDelegate.Add(mTempChatButton.onClick, onChatButtonClick);

        EventDelegate.Add(mPauseBtn.onClick, onPauseClick);

        NGUITools.SetActive(comboUISprite.gameObject, false);

        Init();
    }
	
    void Init()
    {
        initZombieHint();
    }

    private void onPauseClick()
    {
        WindowManager.Instance.OpenUI("pause");
    }

    //武器技能使用
    private void onWeaponSkillClick()
    {
        mSkillModule.UseSkillById(GetWeaponSkillID());
    }
    //换枪
	private void onChangeWeapon()
	{
		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

		if (module.SceneChangeWeapon())
		{
			UpdateWeapon();
		}
	}

    private void onChatButtonClick()
    {
        WindowManager.Instance.OpenUI("chat");
    }
    void UpdateWeaponUp()
    {
        if (mWeaponUping)
        {
            NGUITools.SetActive(mWeaponBullet, false);
            NGUITools.SetActive(mWeaponUp, true);
        }
        else
        {
            NGUITools.SetActive(mWeaponBullet, true);
            NGUITools.SetActive(mWeaponUp, false);
        }
    }
    protected override void OnOpen(object param = null)
    {
        mSuperWeaponProBK.SetActive(false);
        UpdateWeaponUp();
        ReloadEnd();
        EventSystem.Instance.addEventListener(PropertyEvent.FIGHT_PROPERTY_CHANGE, onPlayerPropertyChanged);
		EventSystem.Instance.addEventListener(PropertyEvent.GHOST_FIGHT_PROPERTY_CHANGE, onPlayerPropertyChanged);
        EventSystem.Instance.addEventListener(PropertyEvent.CROPS_PROPERTY_CHANGE, onCropsPropertyChange);

        EventSystem.Instance.addEventListener(ReloadEvent.RELOAD_EVENT, onReloadEvent);
        EventSystem.Instance.addEventListener(ReloadEvent.SUPER_WEAPON_EVENT, onSuperWeaponEvent);
        EventSystem.Instance.addEventListener(ReloadEvent.BULLET_CHANGE_EVENT, onBulletChange);
        EventSystem.Instance.addEventListener(ReloadEvent.WEAPON_UP_EVENT, onWeaponUpEvent);
        EventSystem.Instance.addEventListener(ReloadEvent.WEAPON_UP_REMOVE_EVENT, onWeaponUpRmoveEvent);
        UpdateInformation();
        UpdateProperty();
        UpdateBullets();
        UpdateCropsProperty();

        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_ZOMBIE_ENERMY_NUM, updateEnermyNum);
        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_TEN_SECOND, SetTimeStyleAndSound);
        //EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_SAY_SOMETHING, SaySomething);
        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_ZOMBIE_CRAZY, OnZombieCrazy);
        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_SHOW_TIMER, onTimerShow);
        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_ZOMBIE_PICK1, onPickZombieBuff);
        //EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_PICK_GEAR, OnPickGear);
        //EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_PICK_GOLD, OnPickGold);
        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_PICK_TEMPMONEY, OnPickTempMoney);
        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_PICK_BUFF,OnPickBuff);
        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_DAMAGE, onKillEvent);
        EventSystem.Instance.addEventListener(BossBloodUpdateEvent.BOSS_BLOOD_UPDATE, onBossBloodUpdateEvent);
        EventSystem.Instance.addEventListener(GuideTargetEvent.GUIDE_TARGET_CHANGED, onGuideTargetEvent);
        EventSystem.Instance.addEventListener(GuideTargetEvent.PICK_TARGET_CHANGED, onPickTargetEvent);

        EventSystem.Instance.addEventListener(BossBloodUpdateEvent.BOSS_ENTER_FURY, onBossEnterFury);
        EventSystem.Instance.addEventListener(BossBloodUpdateEvent.BOSS_LEAVE_FURY, onBossLeaveFury);
        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_UPDATE_MONSTER_FlOOD,OnUpdateMonsterFlood);
        EventSystem.Instance.addEventListener(SkillUIEvent.SKILL_LEVEL_UP, OnSkillUpdate);
		EventSystem.Instance.addEventListener(PlayerSkillTransformEvent.PLAYER_TRANSFORM_EVENT, onPlayerSkillTransform);

        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_SHOOT_TYPE_CHANGE, onShootTypeChange);

        EventSystem.Instance.addEventListener(SkillUIEvent.SKILL_WEAPON_SKILL, onWeaponSkillUpdate);
        EventSystem.Instance.addEventListener(CropsEvent.MAIN_CROPS_RELIVE_TIME_DOWN, onMainCropsDieHandler);
        EventSystem.Instance.addEventListener(CropsEvent.SUB_CROPS_RELIVE_TIME_DOWN, onSubCropsDieHandler);
        EventSystem.Instance.addEventListener(CropsEvent.CROPS_RELIVE_NO_TIME_DOWN, onCropsReliveNoTimeDown);

		EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_PROGRESS_OVER, onProgressOver);
		EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_PROGRESS_SHOW, onProgressShow);
		EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_PROGRESS_UPDATE, onProgressUpdate);
        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_BOSSBLOOD_RESET,onRestBossBlood);


        UIBattleFormInitParam initParam = param as UIBattleFormInitParam;
        if (initParam != null)
        {
            mMayLianJi = initParam.DisplayLianJi;
            mDisplayerGuildArrow = initParam.DisplayerGuideArrow;

            mHpAniBar.gameObject.SetActive(false);

            mProgressAni.onFinished += OnProgressAniFinished;

            mWeaponUI.SetActive(initParam.DisplayController);
            mSkillUI.SetActive(initParam.DisplayController);
        }


        NGUITools.SetActive(bossUI, false);
        UpdateFury(false);

        if (mNeedInit) InitMoudle();
        UpdateWeapon();
        UpdateItemIcon();
        UpdateNormalSkillIcons();
        UpdateCropsIcon();

        mIsZombieGame = isZombiesGame();
        mIsDouBiMaoGame = isDouBiMaoGame();
       
        mIsMonsterFlood = IsMonsterFlood();
        GameDebug.Log("mIsMonsterFlood:" + mIsMonsterFlood);
        SetGearVisible();
        if (mIsZombieGame)
        {
            ResetZombieGame();
            //UpdateGearInfo();
            //UpdateGoldNum();
        }

        if (mIsMonsterFlood)
        {
            mMonsterFloodObj.SetActive(true);
            mTempMoneyNum.text = ModuleManager.Instance.FindModule<MonsterFloodModule>().TempMoney.ToString();
            mMonsterNum.text = "";
			mMonsterNumBg.gameObject.SetActive(false);
        }
        else
        {
            mMonsterFloodObj.SetActive(false);
        }
        
        if (mIsZombieGame)
        {
            mTimeCounter.SetActive(true);
        }
        else
        {
            mTimeCounter.SetActive(false);
        }
		mGuidePanel.SetActive(false);

		ShowTimer(mBattleUIModule.IsShowTimer());
    }

    protected override void OnClose()
    {

        mWeaponUping = false;
        EventSystem.Instance.removeEventListener(PropertyEvent.FIGHT_PROPERTY_CHANGE, onPlayerPropertyChanged);
		EventSystem.Instance.removeEventListener(PropertyEvent.GHOST_FIGHT_PROPERTY_CHANGE, onPlayerPropertyChanged);
        EventSystem.Instance.removeEventListener(PropertyEvent.CROPS_PROPERTY_CHANGE, onCropsPropertyChange);

        EventSystem.Instance.removeEventListener(ReloadEvent.SUPER_WEAPON_EVENT, onSuperWeaponEvent);

        EventSystem.Instance.removeEventListener(ReloadEvent.RELOAD_EVENT, onReloadEvent);
        EventSystem.Instance.removeEventListener(ReloadEvent.BULLET_CHANGE_EVENT, onBulletChange);

        EventSystem.Instance.removeEventListener(ReloadEvent.WEAPON_UP_EVENT, onWeaponUpEvent);
        EventSystem.Instance.removeEventListener(ReloadEvent.WEAPON_UP_REMOVE_EVENT, onWeaponUpRmoveEvent);

        NGUITools.SetActive(comboUISprite.gameObject, false);

        mKillNumber = 0;

        mRegularAttackBtnDown = false;
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_ZOMBIE_ENERMY_NUM, updateEnermyNum);
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_TEN_SECOND, SetTimeStyleAndSound);
        //EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_SAY_SOMETHING, SaySomething);
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_ZOMBIE_CRAZY, OnZombieCrazy);
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_SHOW_TIMER, onTimerShow);
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_ZOMBIE_PICK1, onPickZombieBuff);
        //EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_PICK_GEAR, OnPickGear);
        //EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_PICK_GOLD, OnPickGold);
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_PICK_TEMPMONEY, OnPickTempMoney);
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_DAMAGE, onKillEvent);
        EventSystem.Instance.removeEventListener(BossBloodUpdateEvent.BOSS_BLOOD_UPDATE, onBossBloodUpdateEvent);
        EventSystem.Instance.removeEventListener(GuideTargetEvent.GUIDE_TARGET_CHANGED, onGuideTargetEvent);
        EventSystem.Instance.removeEventListener(GuideTargetEvent.PICK_TARGET_CHANGED, onPickTargetEvent);

        EventSystem.Instance.removeEventListener(BossBloodUpdateEvent.BOSS_ENTER_FURY, onBossEnterFury);
        EventSystem.Instance.removeEventListener(BossBloodUpdateEvent.BOSS_LEAVE_FURY, onBossLeaveFury);
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_UPDATE_MONSTER_FlOOD, OnUpdateMonsterFlood);

        EventSystem.Instance.removeEventListener(SkillUIEvent.SKILL_LEVEL_UP, OnSkillUpdate);

		EventSystem.Instance.removeEventListener(PlayerSkillTransformEvent.PLAYER_TRANSFORM_EVENT, onPlayerSkillTransform);
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_SHOOT_TYPE_CHANGE, onShootTypeChange);
        EventSystem.Instance.removeEventListener(CropsEvent.MAIN_CROPS_RELIVE_TIME_DOWN, onMainCropsDieHandler);
        EventSystem.Instance.removeEventListener(CropsEvent.SUB_CROPS_RELIVE_TIME_DOWN, onSubCropsDieHandler);
        EventSystem.Instance.removeEventListener(CropsEvent.CROPS_RELIVE_NO_TIME_DOWN, onCropsReliveNoTimeDown);

		EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_PROGRESS_OVER, onProgressOver);
		EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_PROGRESS_SHOW, onProgressShow);
		EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_PROGRESS_UPDATE, onProgressUpdate);
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_BOSSBLOOD_RESET, onRestBossBlood);
        ResetZombieGame();
        resetBossBloodBar();
    }

    private void onRestBossBlood(EventBase evt)
    {
       
        NGUITools.SetActive(bossUI, false);
    }
    private void OnSkillFly(int skillid)
    {
        mSkillFlyItem.SetActive(true);
        mSkillFlyItem.transform.position = mSkillFlyPos;

        Vector3 targetPos = Vector3.zero;
        for (int i = 0; i < SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM; ++i)
        {
            string iconName = null;
            int falseID = mSkillModule.GetEquipSkillID(i);
            if (falseID >= 0 && falseID == skillid)
            {
                targetPos = mActiveAttackComponents.skillBtns[i].transform.position;
                //targetPos = n;
                iconName = mSkillModule.GetSkillIconByID(falseID);
                UIAtlasHelper.SetSpriteImage(mSkillFlyIcon, iconName);
            }
        }

        mFireSkillID = skillid;
        TweenPosition tp = TweenPosition.Begin(mSkillFlyItem, 1.5f, targetPos);
        tp.worldSpace = true;
        tp.from = mSkillFlyPos;
        tp.to = targetPos;
        tp.SetOnFinished(onFlyFinish);
        tp.PlayForward();
    }
    private void onFlyFinish()
    {
        mSkillFlyItem.SetActive(false);

        UpdateNormalSkillIcons();
    }

    private void OnSkillUpdate(EventBase e)
    {
        SkillUIEvent evt = (SkillUIEvent)e;
        OnSkillFly(evt.skillId);
        //解锁新技能
        //
    }
    private void onBossEnterFury(EventBase e)
    {
        UpdateFury(true);
        OnBossFurySprite(true);
    }
    private void onBossLeaveFury(EventBase e)
    {
        UpdateFury(false);
        OnBossFurySprite(false);
    }

    private void UpdateFury(bool fury)
    {
        if (fury)
        {
            NGUITools.SetActive(bossKuangBao, true);
        }
        else
        {
            NGUITools.SetActive(bossKuangBao, false);
        }
    }


    private void OnBossFurySprite(bool show)
    {
        if( show )
        {
            mBossFury.gameObject.SetActive(true);
            mBossFury.alpha = 0.0f;
            TweenAlpha tween = (TweenAlpha)TweenAlpha.Begin(mBossFury.gameObject, 0.5f, 1.0f);
            tween.PlayForward();

            mBossBeginFury = true;
            mBossFuryTime = 3000;
        }else
        {
            mBossFury.gameObject.SetActive(false);
            mBossBeginFury = false;
        }
    }

    private void onKillEvent(EventBase e)
    {
        mKillInterval = GameConfig.KillNumberTime;
        mKillNumber++;
		if(!mIsMonsterFlood)
		{
			OpenComboUI();
			UpdateComboNumber();
		}
     
    }

    void updateBossBloodBar(uint elapsed)
    {
        if (!bossUI.activeSelf)
            return;

        if (mCurBossBloodTotalVal <= 0f && mInitValDone)
        {
            bossUI.SetActive(false);
            return;
        }

        if (Mathf.Approximately(mCurBossBloodTotalVal, mToBossBloodTotalVal))
        {
            return;
        }

        //加血;
        if (mCurBossBloodTotalVal < mToBossBloodTotalVal)
        {
            mCurBossBloodTotalVal = mToBossBloodTotalVal;
            mHpAniBar.value = mToBossBloodTotalVal - (int)mToBossBloodTotalVal;
            return;
        }

        //减血;
        float delta = (float)elapsed / 1000.0f * mBossBloodAniDur;
        
        mCurBossBloodTotalVal -= delta;
        float res = mHpAniBar.value - delta;
        if (res <= 0f)
        {
            resetBossBloodBar();
            //mHpAniFg.color = getBossBloodColorByIndex(mCurBossBloodTotalVal);
        }

        mHpAniBar.value -= delta;
    }

    Color getBossBloodColorByIndex(float val)
    {
        int mVal = (int)val % 5;

        Color tmp = new Color();
        switch (mVal)
        {
            case 0:
                tmp.r = 255f / 255f;
                tmp.g = 58f / 255f;
                tmp.b = 58f / 255f;
                break;
            case 1:
                tmp.r = 206f / 255f;
                tmp.g = 74f / 255f;
                tmp.b = 255f / 255f;
                break;
            case 2:
                tmp.r = 44f / 255f;
                tmp.g = 222f / 255f;
                tmp.b = 255f / 255f;
                break;
            case 3:
                tmp.r = 89f / 255f;
                tmp.g = 255f / 255f;
                tmp.b = 48f / 255f;
                break;
            case 4:
                tmp.r = 255f / 255f;
                tmp.g = 202f / 255f;
                tmp.b = 63f / 255f;
                break;
        }

        return tmp;
    }

    void resetBossBloodBar()
    {
        //mHpAniBar.gameObject.SetActive(true);
        mHpAniBar.gameObject.SetActive(false);
        mHpAniBar.value = 1f;
        mInitValDone = false;
        bossUI.SetActive(false);
    }

    void playBossBloodAni(float to , float duration)
    {
        TweenScrollValue tsv = TweenScrollValue.Begin(mHpAniBar.gameObject, duration, to);

        tsv.PlayForward();
    }

    private void onBossBloodUpdateEvent(EventBase e)
    {
        BossBloodUpdateEvent evt = e as BossBloodUpdateEvent;
        //if (evt.mCurProgress < 1)
        //{
        //    bossUI.SetActive(false);
        //    return;
        //}

        if (mInitValDone && evt.mCurProgress == 0)
        {
            bossUI.SetActive(false);
            return;
        }

        int index = (int)(evt.mCurProgress / evt.mHpUnit);

        hpBarCount.text = "X" + (index + 1).ToString();
        bossLvName.text = "Lv" + evt.mLevel.ToString() + " " + evt.mName;

        UIAtlasHelper.SetSpriteImage(bossicon, evt.mIcon);

        UpdateFury(evt.mFury);

        if (!bossUI.activeSelf)
        {
            bossUI.SetActive(true);
        }

        int foreindex = index % 5;

        for (int i = 0; i < mHpBarList.Count; ++i)
        {
            GameObject obj = mHpBarList[i];
            if (obj == null)
            {
                continue;
            }

            if (i == foreindex)
            {
                //Debug.LogError(bossHpBar + "---" + obj);
                obj.SetActive(true);
                bossHpBar.foregroundWidget = obj.GetComponent<UISprite>();
            }
            else
            {
                obj.SetActive(false);
            }
        }

        for (int i = 0; i < mHpBarBackList.Count; ++i)
        {
            GameObject obj = mHpBarBackList[i];
            if (obj == null)
            {
                continue;
            }

            if (index == 0 || i != foreindex)
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
                bossHpBar.backgroundWidget = obj.GetComponent<UISprite>();
            }
        }

        if (index == 0)
        {
            //bossHpBar.backgroundWidget = null;
            //bossHpBar.backgroundWidget.gameObject.SetActive(false);
            bossHpBar.SetLastBgIsShow(false);
        }

        mToBossBloodTotalVal = foreindex + (float)((float)(evt.mCurProgress % evt.mHpUnit) / (float)evt.mHpUnit);
        float val = mToBossBloodTotalVal / 5f;
        if (!mInitValDone)
        {
            mCurBossBloodTotalVal = mToBossBloodTotalVal;
            bossHpBar.value = val;
            mInitValDone = true;
        }
        //bossHpBar.value = (float)((float)(evt.mCurProgress % evt.mHpUnit) / (float)evt.mHpUnit);

        if (mInitValDone)
        {
            if (val > bossHpBar.value)
            {
                bossHpBar.value = val;
            }
            else
            { 
                TweenScrollValue.Begin(bossHpBar.gameObject, 1f, val).PlayForward();
            }
        }
    }

    private void onGuideTargetEvent(EventBase e)
    {
        UpdateGuideArrow();
    }

    private void onPickTargetEvent(EventBase e)
    {
        UpdateItemArrow(true);
    }

    private void OpenComboUI()
    {
        if (!mMayLianJi)
            return;

        mComboShow = true;
        NGUITools.SetActive(comboUISprite.gameObject, true);
        comboUISprite.alpha = 1.0f;
        if (comAlpha != null)
        {
            comAlpha.enabled = false;
            comAlpha = null;
        }
    }

    private void HideComboUI()
    {
        mComboShow = false;
        comAlpha = TweenAlpha.Begin(comboUISprite.gameObject, 3.0f, 0.0f);
    }

    private void UpdateComboNumber()
    {
        if (comboNumber != null)
        {
            comboNumber.text = mKillNumber.ToString();
            if( !mComboScale )
            {
                comboNumber.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                TweenScale ts = TweenScale.Begin(comboNumber.gameObject, 0.2f, new Vector3(1.0f, 1.0f, 1.0f));
                if (ts != null)
                {
                    ts.method = UITweener.Method.EaseInOut;
                    ts.PlayForward();
                    ts.AddOnFinished(OnScaleFinish);

                    mComboScale = true;
                }
            }
        }
    }

    private void OnScaleFinish()
    {
        mComboScale = false;
        comboNumber.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    private int GetWeaponSkillID()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        WeaponModule weaponModule = ModuleManager.Instance.FindModule<WeaponModule>();
        int weaponid = module.GetSceneUseWeapon();
        return weaponModule.GetWeaponSkillID(weaponid);
    }

    private string GetWeaponSkillIcon()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        WeaponModule weaponModule = ModuleManager.Instance.FindModule<WeaponModule>();
        int weaponid = module.GetSceneUseWeapon();
        return weaponModule.GetWeaponSkillIcon(weaponid);
    }

    private void UpdateWeapon()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        int mainW = module.GetMainWeaponId();
        int subW = module.GetSubWeaponId();

        string bmpn = "";

        int sceneWeaponID = module.GetSceneUseWeapon();

        if (module.HasSubWeapon())
        {
            if (sceneWeaponID == mainW)
            {
//                 bmpn = ItemManager.Instance.getItemBmp(subW);
//                 UIAtlasHelper.SetSpriteImage(mSubWeaponIcon, bmpn);

                bmpn = ItemManager.Instance.getItemBmp(subW);
                UIAtlasHelper.SetSpriteImage(mMainWeaponIcon, bmpn);
            }else
            {
//                 bmpn = ItemManager.Instance.getItemBmp(mainW);
//                 UIAtlasHelper.SetSpriteImage(mSubWeaponIcon, bmpn);

                bmpn = ItemManager.Instance.getItemBmp(mainW);
                UIAtlasHelper.SetSpriteImage(mMainWeaponIcon, bmpn);
            }
            mWeaponUI.SetActive(true);
        }
        else
        {
//             UIAtlasHelper.SetSpriteImage(mSubWeaponIcon, null);
// 
//             bmpn = ItemManager.Instance.getItemBmp(mainW);
            mWeaponUI.SetActive(false);

            UIAtlasHelper.SetSpriteImage(mMainWeaponIcon, null);
        }

        UpdateWeaponSkill();
    }

    private void UpdateCropsIcon()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
        {
            mMainCropsObj.SetActive(false);
            mSubCropsObj.SetActive(false);
            return;
        }
        if (null != PlayerController.Instance.GetMainCropsControlObj())
        {
            mMainCropsObj.SetActive(true);
            InitCropsIcon(module.GetMainCropsId(), mMainCropsObj);
        }
        else
        {
            mMainCropsObj.SetActive(false);
        }

        if (null != PlayerController.Instance.GetSubCropsControlObj())
        {
            mSubCropsObj.SetActive(true);
            InitCropsIcon(module.GetSubCropsId(), mSubCropsObj);
        }
        else
        {
            mSubCropsObj.SetActive(false);
        }
    }

    private void InitCropsIcon(int resid, GameObject obj)
    {
        //佣兵图片
        UISprite icon = ObjectCommon.GetChildComponent<UISprite>(obj, "Sprite");
        //佣兵死亡冷却底图
        UISprite cooldown = ObjectCommon.GetChildComponent<UISprite>(obj, "CoolDown");
        //佣兵复活到计时
        UILabel lb = ObjectCommon.GetChildComponent<UILabel>(obj, "Time");

        CropsTableItem item = DataManager.CropsTable[resid] as CropsTableItem;
        if(null == item)
        {
            obj.SetActive(false);
            return;
        }
        UIAtlasHelper.SetSpriteImage(icon, item.cropsheadpic);

        //佣兵死亡倒计时处理 TODO
        cooldown.gameObject.SetActive(false);
        lb.text = "";
    }

    private void onShootTypeChange(EventBase e)
    {
        UpdateWeaponSkill();
    }

    private void onWeaponSkillUpdate(EventBase e)
    {
        UpdateWeaponSkill();
    }

    private void UpdateWeaponSkill()
    {
        int shoottype = SettingManager.Instance.GetShootType();
        if( shoottype == (int)SHOOT_TYPE.SHOOT_TYPE_NORMAL )
        {
            mWeaponSkillBtn = mActiveAttackComponents.ExtendedWeaponSkillButton;
            mWeaponSkillCoolDown = mActiveAttackComponents.ExtendedWeaponSkillCdSprite;
			mWeaponSkillCoolDownNumber = mActiveAttackComponents.ExtendedWeaponSkillCdLable;

			NGUITools.SetActive(mActiveAttackComponents.HiddenExtendedWeaponSkillButton.gameObject, false);


            if (!mSkillModule.IsWeaponSkillUnlocked())
            {
                mWeaponSkillBtn.gameObject.SetActive(false);
                return;
            }

        }else
        {
			mWeaponSkillBtn = mActiveAttackComponents.HiddenExtendedWeaponSkillButton;
			mWeaponSkillCoolDown = mActiveAttackComponents.HiddenExtendedWeaponSkillCdSprite;
			mWeaponSkillCoolDownNumber = mActiveAttackComponents.HiddenExtendedWeaponSkillCdLable;

			NGUITools.SetActive(mActiveAttackComponents.ExtendedWeaponSkillButton.gameObject, false);
        }

        int skillid = GetWeaponSkillID();
		if (skillid < 0 )
		{
			mWeaponSkillBtn.gameObject.SetActive(false);
			return;
		}
        string imgName = GetWeaponSkillIcon();

		UpdateSkillIcon(mWeaponSkillBtn.gameObject, skillid, imgName);
    }
    private void UpdateItemIcon()
    {
        NGUITools.SetActive(mNormalAttackComponents.itemBtn.gameObject, false);
		NGUITools.SetActive(mTransformedAttackComponents.itemBtn.gameObject, false);
    }

    private void UpdateNormalSkillIcons()
    {
		//List<Pair<int, string>> skillInfo = new List<Pair<int, string>>();
		for (int i = 0; i < SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM; ++i)
		{
			int skillResID = -1;
			string iconName = null;
			int falseID = mSkillModule.GetEquipSkillID(i);
			if (falseID >= 0)
			{
				skillResID = mSkillModule.GetSkillCommonID(falseID);
				iconName = mSkillModule.GetSkillIconByID(falseID);
                UpdateSkillIcon(mActiveAttackComponents.skillBtns[i], skillResID, iconName, (mFireSkillID == falseID));
			}
			else
			{
				ClearSkillIcon(mNormalAttackComponents, i);
			}
		}
    }

	private void UpdateTransformedSkillIcons(List<Pair<uint, string>> skillList)
	{
		NGUITools.SetActive(this.FindChild("TransformedAttack"), skillList != null);
		for (int i = 0; i < SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM; ++i)
			ClearSkillIcon(mTransformedAttackComponents, i);
		
		int maxCount = (skillList == null)
			? 0 : Mathf.Min(skillList.Count, mTransformedAttackComponents.skillBtns.Count);

		for (int i = 0; i < maxCount; ++i)
		{
			UpdateSkillIcon(mTransformedAttackComponents.skillBtns[i], (int)skillList[i].first, skillList[i].second);
		}
	}

	private void UpdateSkillIcon(GameObject obj, int skillResID, string iconName , bool fire = false)
    {
		UIButton btn = obj.GetComponent<UIButton>();
		btn.CustomData = skillResID;

        if (!string.IsNullOrEmpty(iconName))
        {
            UISprite icon = ObjectCommon.GetChildComponent<UISprite>(obj, "Icon");

            UIAtlasHelper.SetSpriteImage(icon, iconName);
        }

        GameObject effect = ObjectCommon.GetChild(obj, "jinengranshao");

        if( effect != null && effect.activeSelf != fire )
        {
            effect.SetActive(fire);
        }


        NGUITools.SetActive(obj, true);
    }

    private void ClearSkillIcon(AttackUIComponents com, int idx)
    {
		if (idx < 0 || idx >= com.skillBtns.Count)
            return;

		GameObject obj = com.skillBtns[idx];

        UIButton btn = obj.GetComponent<UIButton>();
		int id = (int)btn.CustomData;
		if (id >= 0)
		{
			mSkillModule.ResetSkillCD(id);
			btn.CustomData = -1;
		}

        NGUITools.SetActive(obj, false);
    }

    private void onItemClick( GameObject obj )
    {

    }

	private void onTransformedItemClick(GameObject go)
	{ 
	}

    private void onSkillClick(GameObject obj)
    {
        UIButton btn = obj.GetComponent<UIButton>();
        if (btn != null && btn.CustomData != null)
        {
			//int idx = (int)btn.CustomData;
			//if (mSkillModule.UseSkillByIdx(idx))
			//	mUseSkillIdxs.Add(idx);
			if (mSkillModule.UseSkillById((int)btn.CustomData))
				mUseSkillIdxs.Add((int)btn.CustomData);

            GameObject effect = ObjectCommon.GetChild(obj , "jinengranshao");
            if (effect != null && effect.activeSelf)
            {
                effect.SetActive(false);
            }
        }
    }

    private void onRegularAttackBtnPress(GameObject obj, bool isPress)
    {
        int shoottype = SettingManager.Instance.GetShootType();
        if( shoottype == (int)SHOOT_TYPE.SHOOT_TYPE_NORMAL )
        {
            mRegularAttackBtnDown = isPress;
        }
		else if(!isPress)
        {
            onWeaponSkillClick();
        }

    }

	private void onTransformedRegularAttackBtnPress(GameObject go, bool isPress)
	{
		mTransformedRegularAttackBtnDown = isPress;
	}

    public override void Update(uint elapsed)
    {
        if( mView == null )
        {
            return;
        }
        //临时处理 开枪BUG
        if (!mView.activeInHierarchy)
        {
            mRegularAttackBtnDown = false;
        }

        if (mBossBeginFury)
        {
            mBossFuryTime -= (int)elapsed;
            if( mBossFuryTime <= 0 )
            {
                OnBossFurySprite(false);
            }
        }

        if (mReloading)
        {
            mReloadTime -= elapsed;//(Time.unscaledDeltaTime * 1000);

            if (mReloadTime <= 0)
            {
                mReloading = false;
            }

            mReloadSprite.fillAmount = 1.0f - (mReloadTime / mReloadMaxTime);
        }

        if (mWeaponUping)
        {
            mWeaponUpTime -= elapsed;// (Time.unscaledDeltaTime * 1000);

            if (mWeaponUpTime <= 0)
            {
                mWeaponUping = false;
            }

            mWeaponUpProgress.fillAmount = (mWeaponUpTime / mWeaponUpMaxTime);
        }

        UpdateSuperWeapon(elapsed);

        if( mTimeCounter.gameObject.activeSelf )
        {
            mTmpTime = SceneManager.Instance.GetLastTime();

            mTimeCountLabel.text = TimeUtilities.GetTowerCountDown(mTmpTime);

            if (TimeUtilities.GetSecond(mTmpTime) != TimeUtilities.GetSecond(mCurTime))
            {
                mCurTime = mTmpTime;
                PlayTimeSound();
            }
        }
        
        if ((mRegularAttackBtnDown || mTransformedRegularAttackBtnDown) && mSkillModule != null)
        {
            mSkillModule.UseWeaponSkill();

			UIButton button = mActiveAttackComponents.regularAttackBtn.GetComponent<UIButton>();
            if (button != null)
            {
                if (button.gameObject.transform.localScale.x <= 1.05f ||
                    button.gameObject.transform.localScale.x >= 1.16f)
                {
                    mScale = -mScale;
                }
                Vector3 scale = button.gameObject.transform.localScale;
                scale.x += mScale;
                scale.y += mScale;
                scale.z += mScale;
                button.gameObject.transform.localScale = scale;
            }
        }

        if (mComboShow)
        {
            mKillInterval -= Time.unscaledDeltaTime;

            if (mKillInterval <= 0.0f)
            {
                HideComboUI();
                mKillNumber = 0;
            }
        }

        if (comAlpha != null)
        {
            if (comAlpha.enabled == false)
            {
                NGUITools.SetActive(comboUISprite.gameObject, false);
                comAlpha = null;
            }
        }

        //if (bossUI.activeSelf)
        //{
        //    updateBossBloodBar(elapsed);
        //}

        UpdateCoolDown();
        UpdateGuideArrow();

        if (mIsZombieGame)
            UpdateItemArrow();

        if (mIsDouBiMaoGame)
            UpdateItemArrow();
        if (mMainCropsDie)
        {
            if (UpdateMainCropsDieDown(elapsed))
            {
                EventSystem.Instance.PushEvent(new CropsEvent(CropsEvent.MAIN_CROPS_RELIVE_REQUEST));
                mMainCropsDie = false;
            }
        }

        if (mSubCropsDie)
        {
            if (UpdateSubCropsDieDown(elapsed))
            {
                EventSystem.Instance.PushEvent(new CropsEvent(CropsEvent.SUB_CROPS_RELIVE_REQUEST));
                mSubCropsDie = false;
            }
        }
    }

    bool UpdateMainCropsDieDown(uint elapsed)
    {
        //佣兵死亡冷却底图
        UISprite cooldown = ObjectCommon.GetChildComponent<UISprite>(mMainCropsObj, "CoolDown");
        //佣兵复活到计时
        UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mMainCropsObj, "Time");
        bool over = false;

        if (mMainCropsDieCoolDownTime <= elapsed)
        {
            cooldown.gameObject.SetActive(false);
            lb.text = "";
            over = true;
        }
        else
        {
            if (!cooldown.gameObject.activeSelf)
            {
                cooldown.gameObject.SetActive(true);
            }
            lb.text = (mMainCropsDieCoolDownTime / 1000).ToString();
            mMainCropsCoolDownBg.fillAmount = (float)mMainCropsDieCoolDownTime / mMaxReliveTime;
            mMainCropsDieCoolDownTime -= elapsed;
        }
        return over;
    }

    bool UpdateSubCropsDieDown(uint elapsed)
    {
        //佣兵死亡冷却底图
        UISprite cooldown = ObjectCommon.GetChildComponent<UISprite>(mSubCropsObj, "CoolDown");
        //佣兵复活到计时
        UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mSubCropsObj, "Time");
        bool over = false;

        if (mSubCropsDieCoolDownTime <= elapsed)
        {
            cooldown.gameObject.SetActive(false);
            lb.text = "";
            over = true;
        }
        else
        {
            if (!cooldown.gameObject.activeSelf)
            {
                cooldown.gameObject.SetActive(true);
            }
            lb.text = (mSubCropsDieCoolDownTime / 1000).ToString();
            mSubCropsCoolDownBg.fillAmount = (float)mSubCropsDieCoolDownTime / mMaxReliveTime;
            mSubCropsDieCoolDownTime -= elapsed;
        }
        return over;
    }

    bool UpdateCoolDownItem(UIButton btn, UISprite sprite, UILabel label, int resid)
    {
        bool over = false;

        float amount = 0.0f;
        float max = mSkillModule.GetSkillMaxCoolDownByID(resid);
        if (max <= 0.0f)
        {
            amount = 0.0f;
            label.text = "";
        }
        else
        {
            float cur = mSkillModule.GetSkillCoolDownByID(resid);
            amount = cur / max;

            if (amount > 0.0f)
            {
                int cooldown = (int)Mathf.Ceil(cur / 1000.0f);
                label.text = cooldown.ToString();
            }
            else
            {
                label.text = "";
                over = true;
            }
        }

        if (amount <= 0.0f)
        {
            //检测技能 是否缺蓝 缺X
            if (!mSkillModule.CheckSkillCostByID(resid))
            {
                sprite.fillAmount = 1.0f;
            }
            else
            {
                sprite.fillAmount = 0.0f;
            }
        }
        else
        {
            sprite.fillAmount = amount;
        }
        return over;
    }

    void UpdateCoolDown()
    {
        if (mSkillModule == null)
            return;

        int weaponskill = GetWeaponSkillID();
        if( weaponskill >= 0 )
        {
            UpdateCoolDownItem(mWeaponSkillBtn, mWeaponSkillCoolDown, mWeaponSkillCoolDownNumber, weaponskill);
        }

        //更新翻滚冷却

		UpdateCoolDownByComponents(mActiveAttackComponents);
    }

	private void UpdateCoolDownByComponents(AttackUIComponents com)
	{
		for (int i = 0; i < com.skillBtns.Count; ++i)
		{
			GameObject obj = com.skillBtns[i];

			if (obj == null)
				continue;

			UIButton btn = obj.GetComponent<UIButton>();

			int skillResID = (int)btn.CustomData;

			if (skillResID >= 0)
			{
				if (UpdateCoolDownItem(btn, com.skillCoolDown[i], com.skillCoolDownNumber[i], skillResID))
				{
					if (mUseSkillIdxs.Contains(skillResID))
					{
						AnimationManager.Instance.AddSpriteAnimation("jinenglengque", btn.gameObject, 10, 16, false, true);
						mUseSkillIdxs.Remove(skillResID);
					}
				}
			}
		}
	}

    private void UpdateGuideArrow()
    {
        BaseScene scene = SceneManager.Instance.GetCurScene();

        if (scene == null)
            return;

//         if (!typeof(GameScene).IsAssignableFrom(scene.GetType()))
//         {
//             return;
//         }

        GameScene gamescene = scene as GameScene;

        if( gamescene == null )
        {
            return;
        }

        Vector3 dir = new Vector3();
        if (!gamescene.GetGuideTargetDir(ref dir))
        {
			mGuidePanel.SetActive(false);
        }
        else if(mDisplayerGuildArrow)
        {
			mGuidePanel.SetActive(true);
			mGuidePanel.transform.localPosition = new Vector3(dir.x * 160.0f, dir.z * 160.0f, 0.0f);

			dir.y = dir.z;
			dir.z = 0.0f;
			mGuideArrow.transform.localRotation = Quaternion.FromToRotation(new Vector3(0.0f, 1.0f, 0.0f), dir);
        }
    }

    #region 爬塔
    bool isPataGame()
    {
        ChallengeModule module = ModuleManager.Instance.FindModule<ChallengeModule>();
        if (module == null)
            return false;

        return module.IsPataGame;
    }
    #endregion

    #region 打僵尸
    //----------------------生存玩法---打僵尸，--------------------------
    private void UpdateItemArrow(bool updatePickIcon = false)
    {
        BaseScene scene = SceneManager.Instance.GetCurScene();
        if (!typeof(GameScene).IsAssignableFrom(scene.GetType()))
        {
            return;
        }

        GameScene gamescene = scene as GameScene;
        Vector3 dir = new Vector3();
        if (!gamescene.GetPickTargetDir(ref dir))
        {
            mItemArrow.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            mItemArrow.transform.parent.gameObject.SetActive(true);
            mItemArrow.transform.parent.localPosition = new Vector3(dir.x * 220.0f, dir.z * 220.0f, 0.0f);
            mItemArrow.transform.localPosition = new Vector3(dir.x * 35f, dir.z * 35f, 0f);
            mItemArrow.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -Utility.Vector3ToAngles(dir)));

            //			if(updatePickIcon)
            //			{
            //				Pick pick = gamescene.GetPickTarget();
            //				if(pick != null)
            //				{
            ////					mPickItemIcon.spriteName = pick.InstanceID;
            //				}
            //			}
        }
    }

    /// Sets the gear visible.
    /// 手机碎片图标显示;
    void SetGearVisible()
    {
        if (mGearObj == null)
            return;

        mZomebieUIObj.SetActive(mIsZombieGame);

        mItemArrow.SetActive(mIsDouBiMaoGame || mIsZombieGame);

        //mGearObj.SetActive(mIsZombieGame);
        //mItemArrow.transform.parent.gameObject.SetActive(mIsZombieGame);
    }

    bool isZombiesGame()
    {
        ZombiesStageModule zsm = ModuleManager.Instance.FindModule<ZombiesStageModule>();
        if (zsm != null)
            return zsm.IsZombieGame;

        return false;
    }

    bool isDouBiMaoGame()
    {
        DouBiMaoStageModule zsm = ModuleManager.Instance.FindModule<DouBiMaoStageModule>();
        if (zsm != null)
            return zsm.IsDouBiMaoGame;

        return false;
    }


    //挑战本
    bool IsMonsterFlood()
    {
        MonsterFloodModule mfm = ModuleManager.Instance.FindModule<MonsterFloodModule>();
        if (mfm != null)
            return mfm.IsMonsterFlood;

        return false;
    }

    public void onTimerShow(EventBase ev)
    {
        BattleUIEvent bue = ev as BattleUIEvent;
        if (bue == null)
			return;

        if (isPataGame())
        {
			ShowTimer(false);
        }
        else
        {
            bool b = (bool)bue.msg;
			ShowTimer(b);
        }
    }

	private void ShowTimer(bool b)
	{
		mTimeCounter.SetActive(b);

		if (b)
		{
			mCurTime = SceneManager.Instance.GetLastTime();
		}
	}

    void ResetZombieGame()
    {
        //僵尸本不显示拾取金币;
        mGoldCounterLb.transform.parent.gameObject.SetActive(false);

        mGearNumber = 0;
        ignorFirstGoldNumChanged = true;
        mGoldCounterLb.text = "0";
        mGoldNumList.Clear();
        //mTimeCountLabel.fontSize = 16;
        mNeedPlayingSound = false;
        
        updateEnermyNum();
        resetZombieBuff();
    }

    void resetZombieBuff()
    {
        mBuffSliceNum = 0;
        TweenScrollValue tsv = mZombieBar.GetComponent<TweenScrollValue>();
        if(tsv != null)
        {
           // tsv.Destroy();
        }
        mZombieBar.value = 0f;
        mIsInBuffDur = false;
    }

    /// <summary>
    /// 僵尸本儿拾取buff碎片;
    /// </summary>
    /// <param name="ev"></param>
    void onPickZombieBuff(EventBase ev)
    {
        if (mIsInBuffDur)
        {
            //Debug.LogError("不是说不会出现buff持续过程中，继续拾取buff碎片的情况吗？");
            return;
        }

        mBuffSliceNum++;

        int totalNum = (int)ConfigManager.GetVal<int>(ConfigItemKey.ZOMBIE_BUFF_PICK_NUM);
        
        float percent = (float)mBuffSliceNum / (float)totalNum;
        
        TweenScrollValue tsv = TweenScrollValue.Begin(mZombieBar.gameObject, 0.5f, percent);
        tsv.SetOnFinished(onBuffBarTweenFinish);
        tsv.PlayForward();
    }

    void onBuffBarTweenFinish()
    {
        if (Mathf.Approximately(1.0f, mZombieBar.value))
        {
            isZombieBuffFull();
        }
    }

    void isZombieBuffFull()
    {
        uint buffId = (uint)ConfigManager.GetVal<uint>(ConfigItemKey.ZOMBIE_BUFF_ID);
       
        if(!DataManager.BuffTable.ContainsKey(buffId))
        {
            Debug.LogError("僵尸本buffId不存在:" + buffId);
            return ;
        }

        SkillBuffTableItem item = DataManager.BuffTable[buffId] as SkillBuffTableItem;
        if (item == null)
        {
            Debug.LogError("僵尸本BuffId不存在:" + buffId);
            return;
        }
        if (item.lifeMilliseconds <= 0)
        {
            Debug.LogError("僵尸本buffid持续时间不对:" + buffId);
            return;
        }

        if (Mathf.Approximately(mZombieBar.value, 1f))
        { 
            float buffDur = (float)item.lifeMilliseconds / 1000f;

            mIsInBuffDur = true;

            PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
            Player p = PlayerController.Instance.GetControlObj() as Player;
            p.AddSkillEffect(new AttackerAttr(p), SkillEffectType.Buff, buffId);

            TweenScrollValue tsv = TweenScrollValue.Begin(mZombieBar.gameObject, buffDur, 0f);
            tsv.SetOnFinished(onZombieBuffFinish);
            tsv.PlayForward();
        }
    }

    void onZombieBuffFinish()
    {
        mIsInBuffDur = false;
        mBuffSliceNum = 0;
    }

    /// <summary>
    /// mGearNumber的有效范围是[1-3]
    /// </summary>
    public void OnPickGear(EventBase ev)
    {
        if (ev == null) return;
        BattleUIEvent bue = ev as BattleUIEvent;
        if (bue == null) return;

        mGearNumber = ++mGearNumber % 4;

        Vector3 from = (Vector3)bue.msg;
        from = CameraController.Instance.WorldToScreenPoint(from);
        from.z = 0.0f;
        from = WindowManager.current2DCamera.ScreenToWorldPoint(from);

        GameObject go = System.Activator.CreateInstance<GameObject>() as GameObject;
        go.transform.parent = mZombiesCrazySp.transform.parent;
        go.name = "flyGear";

        UISprite gold = go.AddMissingComponent<UISprite>();
        gold.atlas = UIAtlasHelper.GetSpriteAtlas(mPickItemIcon);
        int num = mGearNumber;
        if (num < 1) num = 1;
        if (num > 3) num = 3;
        gold.spriteName = "gear" + num;
        gold.MakePixelPerfect();

        FlyEffect eff = go.AddMissingComponent<FlyEffect>();
        eff.onFinished += GearFlyEffonFinished;
        eff.Play(from, mGearObj.transform.localPosition, mFlyDuration);
    }

    public void OnPickTempMoney(EventBase ev)
    {
        mTempMoneyNum.text = ModuleManager.Instance.FindModule<MonsterFloodModule>().TempMoney.ToString();
    }

    public void OnPickBuff(EventBase ev)
    {
        
    }

    public void OnUpdateMonsterFlood(EventBase ev)
    {
        MonsterFloodModule module = ModuleManager.Instance.FindModule<MonsterFloodModule>();
        mMonsterNum.text = module.mCurNum + "/" + module.TotalNum + StringHelper.GetString("bo");
        PromptUIManager.Instance.AddNewPrompt(mMonsterNum.text+StringHelper.GetString("startnow"));
        mTempMoneyNum.text = ModuleManager.Instance.FindModule<MonsterFloodModule>().TempMoney.ToString();
		mMonsterNumBg.gameObject.SetActive(true);
    }

    void initZombieHint()
    {
        mZombieBarHint.text = StringHelper.GetString("zombie_hint");
    }

    void GearFlyEffonFinished()
    {
        UpdateGearInfo();

        if (mGearNumber >= 3)
        {
            DispatchSkill();
            mGearNumber = 0;

            DisapearGears();
        }
    }

    /// 刷新显示
    void UpdateGearInfo()
    {
        for (int i = 0; i < 3; i++)
        {
            mGearSps[i].gameObject.SetActive(i < mGearNumber);
        }
    }

    void OnPickGold(EventBase ev)
    {
        if (ev == null) return;
        BattleUIEvent bue = ev as BattleUIEvent;
        if (bue == null) return;

        Vector3 from = (Vector3)bue.msg;
        from = CameraController.Instance.WorldToScreenPoint(from);
        from.z = 0.0f;
        from = WindowManager.current2DCamera.ScreenToWorldPoint(from);

        GameObject go = System.Activator.CreateInstance<GameObject>() as GameObject;
        go.transform.parent = mZombiesCrazySp.transform.parent;
        go.name = "flyGold";

        UISprite gold = go.AddMissingComponent<UISprite>();
        gold.atlas = UIAtlasHelper.GetSpriteAtlas(mPickItemIcon);
        gold.spriteName = "goldSp";
        gold.MakePixelPerfect();

        FlyEffect eff = go.AddMissingComponent<FlyEffect>();
        eff.onFinished += eff_onFinished;
        eff.Play(from, mGoldTarget.transform.localPosition, mFlyDuration);

        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm != null)
            mGoldNumList.Add(pdm.GetProceeds(ProceedsType.Money_Game));
    }

    void eff_onFinished()
    {
        UpdateGoldNum();
    }

    T GetListFirst<T>(List<T> list)
    {
        if (list == null || list.Count < 1)
            return default(T);

        return (T)list[0];
    }

    void RemoveListFirst<T>(List<T> list)
    {
        if (list == null || list.Count < 1)
            return;

        list.RemoveAt(0);
    }

    void UpdateGoldNum()
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null)
            return;

        ZombiesStageModule zsm = ModuleManager.Instance.FindModule<ZombiesStageModule>();
        if (zsm == null)
            return;

        uint moneyNum = GetListFirst<uint>(mGoldNumList);
        RemoveListFirst<uint>(mGoldNumList);

        //mGoldCounterLb.text = (moneyNum - zsm.BeginGoldNum) + "";
        if (mScrollNumEff == null)
        {
            mScrollNumEff = mGoldCounterLb.gameObject.AddMissingComponent<ScrollNumEffect>();
        }

        int num = (int)(moneyNum - zsm.BeginGoldNum);
        int oldNum;

        if (ignorFirstGoldNumChanged)
        {
            ignorFirstGoldNumChanged = false;
            oldNum = 0;
            num = 0;
        }
        else
        {
            oldNum = System.Convert.ToInt32(mGoldCounterLb.text);
        }

        mScrollNumEff.Play(oldNum, num);
    }

    void updateEnermyNum()
    {
        setEnermyNum(ZombiesStageModule.GetLastEnermyNum(), ZombiesStageModule.GetTotalEnermyNum());
    }

    void setEnermyNum(uint cur, uint total)
    {
        mEnermyBar.value = (float)cur / (float)total;
        mZombieEnermyLb.text = cur.ToString();
    }

    private void updateEnermyNum(EventBase ev)
    {
        if (ev == null) return;
        BattleUIEvent bue = ev as BattleUIEvent;
        if (bue == null) return;

        setEnermyNum((uint)bue.msg, (uint)bue.msg1);
    }

	private void onProgressOver(EventBase ev)
	{
		BattleUIEvent e = ev as BattleUIEvent;
		if (e == null)
			return;

		mStageProgressText.text = null;
		mProgressAni.gameObject.SetActive(true);
		mGoArrowObj.SetActive(true);

		float angle = (float)e.msg;
		if (angle >= 0)
		{
// 			Vector3 dir = new Vector3(0.0f, 92.0f, 0.0f);
// 
// 			if (!Mathf.Approximately(angle, 0.0f))
// 			{
// 				Quaternion q = new Quaternion();
// 				q.eulerAngles = new Vector3(0.0f, 0.0f, angle);
// 				dir = q * dir;
// 			}
// 
// 			mArrowObj.transform.localPosition = new Vector3(dir.x, dir.y, 0.0f);
// 
// 			dir.Normalize();
// 			mArrowObj.transform.localRotation = Quaternion.FromToRotation(new Vector3(0.0f, 1.0f, 0.0f), dir);
// 
// 			mArrowObj.SetActive(true);
			mGoAni.gameObject.SetActive(true);
			mGoAni.Play();
		}
		else
		{
 			mGoAni.gameObject.SetActive(false);
// 			mArrowObj.SetActive(false);
 		}

		mStageProgressBar.SetActive(false);
	}

	private void InitStageProgress()
	{
		mStageProgressBar.SetActive(true);
		mProgressAni.Reset();
		mProgressAni.gameObject.SetActive(false);
		mGoArrowObj.SetActive(false);
		mGoAni.gameObject.SetActive(false);
		mGoAni.ResetToBeginning();
		mStageProgressObj.SetActive(false);
		mStageProgressText.text = null;
	}
	private void onProgressShow(EventBase ev)
	{
		BattleUIEvent e = ev as BattleUIEvent;
		if (e == null)
			return;

		InitStageProgress();

		mStageProgressObj.SetActive(true);

		onProgressUpdate(ev);
	}

	private void onProgressUpdate(EventBase ev)
	{
		BattleUIEvent e = ev as BattleUIEvent;
		if (e == null)
			return;

		setStageProgress((int)e.msg, (int)e.msg1);
	}

	private void setStageProgress(int cur, int total)
	{
		mStageProgress.value = (float)cur / (float)total;
		mStageProgressText.text = cur.ToString();
	}
	
	private void OnProgressAniFinished(GameObject go)
	{
		go.SetActive(false);
	}

    void SetTimeStyleAndSound(EventBase ev)
    {
        return;

        if (ev == null) return;
        BattleUIEvent bue = ev as BattleUIEvent;
        if (bue == null) return;

        mTimeCountLabel.fontSize = (int)bue.msg;
        //mTimeCounter.SetSize();
        mSoundTimeResId = (int)bue.msg1;
        mNeedPlayingSound = true;
        //mTimeCounter.onPerSecond = PlayTimeSound;
    }

    void PlayTimeSound()
    {
        if (mSoundTimeResId < 0 || !mNeedPlayingSound) return;
        SoundManager.Instance.Play(mSoundTimeResId);
    }

    void SaySomething(EventBase ev)
    {
        if (ev == null) return;
        BattleUIEvent bue = ev as BattleUIEvent;
        if (bue == null) return;

        PromptUIManager.Instance.AddNewPrompt(bue.msg.ToString());
    }

    void OnZombieCrazy(EventBase ev)
    {
        ShowHintImg();
    }

    /// <summary>
    /// 显示僵尸狂暴了图片;
    /// </summary>
    void ShowHintImg()
    {
        mZombiesCrazySp.SetActive(true);
        //		iTween.ShakeScale
    }

    /// <summary>
    /// 积满三个后碎片消失
    /// </summary>
    void DisapearGears()
    {
        UpdateGearInfo();
        //		TweenAlpha.Begin(mGearObj , 0.5f , 0f).PlayForward();
    }

    /// 触发特殊技能;
    void DispatchSkill()
    {
        mSkillModule.UseSkillById(1530);
    }
    #endregion
    private void onWeaponUpEvent(EventBase e)
    {
        ReloadEvent evt = (ReloadEvent)e;

        mWeaponUping = true;
        mWeaponUpTime = mWeaponUpMaxTime = evt.reload_time;

        mWeaponUpProgress.fillAmount = 0.0f;

        UpdateWeaponUp();
    }

	private void onPlayerSkillTransform(EventBase e)
	{
		PlayerSkillTransformEvent evt = (PlayerSkillTransformEvent)e;

		if (evt.mNewSkillList != null)
			mActiveAttackComponents = mTransformedAttackComponents;
		else
			mActiveAttackComponents = mNormalAttackComponents;

		UpdateTransformedSkillIcons(evt.mNewSkillList);
		UpdateWeaponSkill();

		NGUITools.SetActive(this.FindChild("Attack"), evt.mNewSkillList == null);
		mRegularAttackBtnDown = mTransformedRegularAttackBtnDown = false;
	}

    private void onWeaponUpRmoveEvent(EventBase e)
    {
        mWeaponUping = false;
        UpdateWeaponUp();
    }

    private void onBulletChange(EventBase e)
    {
        UpdateBullets();
    }

    private void onSuperWeaponEvent(EventBase e)
    {
        ReloadEvent evt = (ReloadEvent)e;

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if( evt.super_weapon < 0 || evt.reload_time <= 0 )
        {
            mSuperWeaponProBK.SetActive(false);
        }else
        {
            string bmpn = ItemManager.Instance.getItemBmp(evt.super_weapon);
            UIAtlasHelper.SetSpriteImage(mMainWeaponIcon, bmpn);
            mSuperWeaponProBK.SetActive(true);

            mSuperWeapon = true;
            mSuperWeaponTime = evt.reload_time;
            mMaxSuperWeaponTime = evt.reload_time;
        }
        UpdateWeapon();
    }

    private void UpdateSuperWeapon(uint el)
    {
        if( mSuperWeapon )
        {
            mSuperWeaponTime -= (int)el;
            if( mSuperWeaponTime <= 0 )
            {
                mSuperWeapon = false;
                mSuperWeaponTime = 0;
                mSuperWeaponProBK.SetActive(false);
            }
            mSuperWeaponProgress.fillAmount = (float)mSuperWeaponTime / (float)mMaxSuperWeaponTime;
        }
    }

    private void onReloadEvent(EventBase e)
    {
        ReloadEvent evt = (ReloadEvent)e;

        if (evt.reload_time <= 0)
        {
            mReloading = false;
            ReloadEnd();
        }
        else
        {
            mReloading = true;
            mReloadTime = mReloadMaxTime = (float)evt.reload_time;

            Reloading();
        }
    }

    void Reloading()
    {
        NGUITools.SetActive(mReloadProgress.gameObject, true);
        NGUITools.SetActive(mReloadTip.gameObject, true);
        NGUITools.SetActive(mBulletNumber.gameObject, false);
    }

    void ReloadEnd()
    {
        NGUITools.SetActive(mReloadProgress.gameObject, false);
        NGUITools.SetActive(mReloadTip.gameObject, false);
        NGUITools.SetActive(mBulletNumber.gameObject, true);

    }

    //玩家属性改变
    void onPlayerPropertyChanged(EventBase e)
    {
        UpdateProperty();
        UpdateBullets();
    }

    void onCropsPropertyChange(EventBase e)
    {
        UpdateCropsProperty();
    }

    void UpdateBullets()
    {
        Player ply = PlayerController.Instance.GetControlObj() as Player;
        if (ply != null)
		{
			mBulletNumber.text = ply.GetWeaponBullet().ToString() + "/" + ply.GetWeaponMaxBullet().ToString();
			return;
		}

		Ghost ghost = PlayerController.Instance.GetControlObj() as Ghost;
		if(ghost != null)
		{
			mBulletNumber.text = ghost.GetWeaponBullet().ToString() + "/" + ghost.GetWeaponMaxBullet().ToString();
			return;
		}
    }

    void UpdateProperty()
    {
		BattleUnit unit = PlayerController.Instance.GetControlObj() as BattleUnit;
		if (unit == null)
        {
            return;
        }
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();



		mHPNumber.text = unit.GetHP().ToString() + "/" + unit.GetMaxHP().ToString();
		mHPPro.value = (float)unit.GetHP() / (float)unit.GetMaxHP();

		mSPNumber.text = unit.GetMana().ToString() + "/" + unit.GetMaxMana().ToString();
		mSPPro.value = (float)unit.GetMana() / (float)unit.GetMaxMana();

        UIAtlasHelper.SetSpriteImage(mFace, module.GetFace());

    }

    void UpdateCropsProperty()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
        {
            return;
        }

        UISlider sli = ObjectCommon.GetChildComponent<UISlider>(mMainCropsObj, "HPProgress");
        UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mMainCropsObj, "HPProgress/HP");

        if (mMainCropsObj.activeSelf)
        {
            if (null != PlayerController.Instance.GetMainCropsControlObj())
            {
                BattleUnit unit = PlayerController.Instance.GetMainCropsControlObj() as BattleUnit;
                lb.text = unit.GetHP().ToString() + "/" + unit.GetMaxHP().ToString();
                sli.value = (float)unit.GetHP() / (float)unit.GetMaxHP();
                mMainCropsOldHp = unit.GetMaxHP();
            }
            else
            {
                lb.text = "0/" + mMainCropsOldHp;
                sli.value = 0;
            }
        }

        if (mSubCropsObj.activeSelf)
        {
            sli = ObjectCommon.GetChildComponent<UISlider>(mSubCropsObj, "HPProgress");
            lb = ObjectCommon.GetChildComponent<UILabel>(mSubCropsObj, "HPProgress/HP");
            if (null != PlayerController.Instance.GetSubCropsControlObj())
            {
                BattleUnit unit = PlayerController.Instance.GetSubCropsControlObj() as BattleUnit;
                lb.text = unit.GetHP().ToString() + "/" + unit.GetMaxHP().ToString();
                sli.value = (float)unit.GetHP() / (float)unit.GetMaxHP();
                mSubCropsOldHp = unit.GetMaxHP();
            }
            else
            {
                lb.text = "0/" + mSubCropsOldHp;
                sli.value = 0;
            }
        }
    }

    void UpdateInformation()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        mLevel.text = module.GetLevel().ToString();
        mName.text = module.GetName().ToString();
    }

    private void onMainCropsDieHandler(EventBase evt)
    {
        if (mMainCropsDie)
            return;
        GameScene scn = SceneManager.Instance.GetCurScene() as GameScene;
        if (null == scn)
            return;
        SceneTableItem item = scn.GetSceneRes();
        if (null == item)
            return;
        
        mMainCropsDie = scn.MainCropsCanRelive();
        mMainCropsDieCoolDownTime = (uint)item.mCropsReliveCds;
        mMaxReliveTime = (uint)item.mCropsReliveCds;

        //佣兵死亡冷却底图
        UISprite cooldown = ObjectCommon.GetChildComponent<UISprite>(mMainCropsObj, "CoolDown");
        mMainCropsCoolDownBg = cooldown;
        mMainCropsCoolDownBg.fillAmount = 1.0f;
        cooldown.gameObject.SetActive(true);
    }

    private void onSubCropsDieHandler(EventBase evt)
    {
        if (mSubCropsDie)
            return;
        GameScene scn = SceneManager.Instance.GetCurScene() as GameScene;
        if (null == scn)
            return;
        SceneTableItem item = scn.GetSceneRes();
        if (null == item)
            return;
        mSubCropsDie = scn.SubCropsCanRelive();
        mSubCropsDieCoolDownTime = (uint)item.mCropsReliveCds;
        mMaxReliveTime = (uint)item.mCropsReliveCds;

        //佣兵死亡冷却底图
        UISprite cooldown = ObjectCommon.GetChildComponent<UISprite>(mSubCropsObj, "CoolDown");
        mSubCropsCoolDownBg = cooldown;
        mSubCropsCoolDownBg.fillAmount = 1.0f;
        cooldown.gameObject.SetActive(true);
    }

    private void onCropsReliveNoTimeDown(EventBase evt)
    {
        mSubCropsDie = false;
        mMainCropsDie = false;
        if (null != PlayerController.Instance.GetMainCropsControlObj())
        {
            UISprite cooldown = ObjectCommon.GetChildComponent<UISprite>(mMainCropsObj, "CoolDown");
            UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mMainCropsObj, "Time");
            cooldown.fillAmount = 0;
            lb.text = "";
        }

        if (null != PlayerController.Instance.GetSubCropsControlObj())
        {
            UISprite cooldown = ObjectCommon.GetChildComponent<UISprite>(mSubCropsObj, "CoolDown");
            UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mSubCropsObj, "Time");
            cooldown.fillAmount = 0;
            lb.text = "";
        }
    }
}
