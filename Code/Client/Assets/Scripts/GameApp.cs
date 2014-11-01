using UnityEngine;
using System.Collections;

public class GameApp 
{
    private static GameApp msInstance = null;

    //单例初始化
    private EventSystem			mEventSystem = null;
	private SceneManager		mSceneManager = null;
    private WindowManager		mWindowManager = null;
	private DataManager			mDataManager = null;
	private ResourceManager		mResourceManager = null;
	private PlayerDataPool		mPlayerDataPool = null;
	private TouchManager		mToucheManager = null;
    private AIFactory			mAIFactory = null;
    private ItemManager			mItemManage = null;
    private GMHandler			mGMHandler = null;
    private ModuleManager		mModuleManager = null;
    private SoundManager		mSoundManager = null;
	private DropManager			mDropManager = null;
	private ConditionManager	mConditionManager = null;
	private StageDataManager	mStageDataManager = null;
    private BloodUIManager      mBloodUIManager = null;
	private BossBloodUIManager	mBossBloodUIManager = null;
    private PlayerController    mPlayerController = null;
	private YesOrNoBoxManager   mYesOrNoMgr = null;
	private UIEffectManager     mUIEffectMgr = null;
    private InputSystem         mInputSystem = null;
	private StoryManager		mStoryManager = null;
    private PromptUIManager     mPromptManager = null;
    private PaoPaoManager       mPaoPaoManager = null;
    private GuideManager        mGuideManager = null;
    private AnimationManager    mAnimationManager = null;
    private PlayerHeadUIManager mPlayerHeadUIManager = null;
    private UIResourceManager   mUIResourceManager = null;
    private AnnounceItemManager mAnnounceItemManager = null;
    private PopTipManager       mPopTipManager = null;
    private EggManager          mEggManager = null;
    private QuackUIManager      mQuackUIManager = null;
    private ShopManager         mShopManager = null;
    private StrFilterManager    mStrFilterManager = null;
    private MailItemManager     mMailItemManager = null;
    private FightGradeManager   mFightGradeManager = null;
    private SettingManager      mSettingManager = null;
    private ZhushouManager      mZhushouManager = null;
    private OtherDataPool       mOtherDataPool = null;
	private ActivityManager		mActivityManager = null;
    private LoadingManager      mLoadingManager = null;
    private ChargeItemManager   mChargeItemManager = null;
    private ChargeMsgManager    mChargeMsgManager = null;
    private const uint UPDATE_INTERVAL = 30;
    private uint mElapsed = 0;
   

    private GAME_FLOW_ENUM mNextFlow = GAME_FLOW_ENUM.GAME_FLOW_INVAILD;

    private BaseFlow    mCurFlow    = null;
    public static GameApp Instance
    {
        get
        {
            return msInstance;
        }
    }
    public GameApp()
    {
		msInstance = this;

        mEventSystem = new EventSystem();
		mSceneManager = new SceneManager();
		mWindowManager = new WindowManager();
		mDataManager = new DataManager();
		mResourceManager = new ResourceManager();
		mToucheManager = new TouchManager();
        mAIFactory = new AIFactory();
        mItemManage = new ItemManager();
        mGMHandler = new GMHandler();
        mModuleManager = new ModuleManager();
        mSoundManager = new SoundManager();
		mDropManager = new DropManager();
		mConditionManager = new ConditionManager();
		mStageDataManager = new StageDataManager();
        mPlayerController = new PlayerController();
        mBloodUIManager = new BloodUIManager();
		mBossBloodUIManager = new BossBloodUIManager();
        mPlayerDataPool = new PlayerDataPool();
		mYesOrNoMgr = new YesOrNoBoxManager();
		mUIEffectMgr = new UIEffectManager();
        mInputSystem = new InputSystem();
		mStoryManager = new StoryManager();
        mPromptManager = new PromptUIManager();
        mPaoPaoManager = new PaoPaoManager();
        mGuideManager = new GuideManager();
        mAnimationManager = new AnimationManager();
        mPlayerHeadUIManager = new PlayerHeadUIManager();
        mUIResourceManager = new UIResourceManager();
        mAnnounceItemManager = new AnnounceItemManager();
        mPopTipManager = new PopTipManager();
        mQuackUIManager = new QuackUIManager();
        mEggManager = new EggManager();
        mShopManager = new ShopManager();
        mStrFilterManager = new StrFilterManager();
        mFightGradeManager = new FightGradeManager();
        mMailItemManager = new MailItemManager();
        mSettingManager = new SettingManager();
        mZhushouManager = new ZhushouManager();
        mOtherDataPool = new OtherDataPool();
		mActivityManager = new ActivityManager();
        mLoadingManager = new LoadingManager();
        mChargeItemManager = new ChargeItemManager();
        mChargeMsgManager = new ChargeMsgManager();
    }

    public bool Init()
    {

        ApplicationState.Init();

        Net.Instance.SetUrl("http://127.0.0.1:8087");

        new LoginAction();
        new RandomNameAction();
        new CreateRoleAction();
        new EnterGameAction();
        new GMAction();
        new HeartbeatAction();
		new EnterSceneAction();
		new PassStageAction();
		new UnlockStageAction();
		new ReliveAction();
        new SkillAction();
        new BuyWeaponAction();
        new ChangeWeaponAction();
        new ChallengeStageEnterStageAction();
        new StrenWeaponAction();
        new ChallengeStageOverStageAction();
        new ChallengeStageContinueAction();
        new ChallengeStageSweepStageAction();
        new SetPromoteAction();
        new BapFittingsAction();
        new AcceptQuestAction();
        new FinishQuestAction();
		new ArenaBeginAction();
		new ArenaEndAction();
		new ArenaRecordAction();
		new ArenaRefreshAction();
		new ArenaBuyTimesAction();
        new ItemEquipAction();
        new BagOpAction();
        new PromoteDefenceAction();
        new StrenDefenceAction();
        new RisingStarsAction();
        new StoneCombAction();
        new StoneUnInlayAction();
        new StoneInlayAction();
        new SaleDefenceAction();
        new MallAction();
        new QiangLinDanYuReportScoreAction();
        new QiangLinDanYuOverAction();
        new JoinActivityAction();
		new WingActiveAction();
		new WingEquipAction();
        new WingForgeAction();
        new RankingAction();
		new QualifyingBeginAction();
		new QualifyingEndAction();
		new QualifyingRecordAction();
		new QualifyingListAction();
		new QualifyingBuyTimesAction();
		new ShopAction();
		new WingUpdateAction();
        new DailyResetAction();
        new EggAction();
        new ItemSaleAction();
		new SpIncreaseAction();
        new ChallengeStageRankAction();
        new MailOpenedAction();
        new ZoneRewardAction();
        new TitleAction();
        new FashionActiveAction();
        new FashionAddStarAction();
        new FashionEquipAction();
        new MailLoadAction();
        new MailDeleteAction();
        new MailPickAction();
		new YaZhiXieEReportScoreAction();
		new YaZhiXieEOverAction();
		new ZhaoCaiMaoReportScoreAction();
		new ZhaoCaiMaoOverAction();
		new ZhaoCaiMaoPartnerAction();
		new ZhaoCaiMaoRankingAction();
        new CropsBuyAction();
        new SevenStageAction();
        new PlanStateAction();
        new CropsChangeAction();
        new CropsPromoteAction();
        new ViewOtherAction();
        new QueryRechargeResultAction();
        new FirstChargeRewardAction();
        new FundAction();
        new BoxItemAction();
        mWindowManager.Initialize();


        mSettingManager.InitGlobal();

        mPlayerDataPool.Init();

        setNextFlow(GAME_FLOW_ENUM.GAME_FLOW_VERIFY);

        return true;
    }

    public void Term()
    {
        mEventSystem = null;
    }

    public void Update(uint elapsed)
    {
        mElapsed += elapsed;
        if (mElapsed < UPDATE_INTERVAL)
            return;

        elapsed = mElapsed;

        mElapsed = 0;

        if (mEventSystem != null)
            mEventSystem.Update();
		if( mSceneManager != null )
            mSceneManager.Update(elapsed);
        if (mWindowManager != null)
            mWindowManager.Update(elapsed);
        if (mSoundManager != null)
        {
            mSoundManager.Update(elapsed);
        }
        if( mAnimationManager != null )
        {
            mAnimationManager.Update();
        }
        if (mInputSystem != null)
        {
            mInputSystem.Update();
        }
		if (mBossBloodUIManager != null)
		{
            mBossBloodUIManager.Update(elapsed);
		}

        if (mPromptManager != null)
        {
            mPromptManager.Update(elapsed);
        }
        if (mPopTipManager != null)
        {
            mPopTipManager.Update(elapsed);
        }
        if( mGuideManager != null )
        {
            mGuideManager.Update(elapsed);
        }

        if (mEggManager != null)
        {
            mEggManager.Update(elapsed);
        }

        if (mQuackUIManager != null)
        {
            mQuackUIManager.Update(elapsed);
        }

        if (mShopManager != null)
        {
            mShopManager.Update(elapsed);
        }

        if( mModuleManager != null )
        {
            mModuleManager.Update(elapsed);
        }

        if( mNextFlow != GAME_FLOW_ENUM.GAME_FLOW_INVAILD )
        {
            if( mCurFlow != null )
            {
                mCurFlow.Term();
                mCurFlow = null;
            }
			GAME_FLOW_ENUM curFlowEnum = mNextFlow;
			mNextFlow = GAME_FLOW_ENUM.GAME_FLOW_INVAILD;
			
			mCurFlow = createNextFlow(curFlowEnum);
        }

        if( mCurFlow != null )
        {
            FLOW_EXIT_CODE exitCode = mCurFlow.Update( elapsed);
            if( exitCode != FLOW_EXIT_CODE.FLOW_EXIT_CODE_NONE )
            {
                onFlowExit(mCurFlow, exitCode);
            }
        }
    }


    public void setNextFlow(GAME_FLOW_ENUM flow)
    {
        mNextFlow = flow;
    }

    private BaseFlow createNextFlow(GAME_FLOW_ENUM flow)
    {
        BaseFlow nextFlow = null;
        switch( flow )
        {
            case GAME_FLOW_ENUM.GAME_FLOW_VERIFY:
                {
                    nextFlow = new VerifyFlow();
                }break;
            case GAME_FLOW_ENUM.GAME_FLOW_LOGIN:
                {
                    nextFlow = new LoginFlow();
                }break;
            case GAME_FLOW_ENUM.GAME_FLOW_MAIN:
                {
                    nextFlow = new MainFlow();
                }break;
        }

        if( nextFlow != null )
        {
            nextFlow.Init();
        }

        return nextFlow;
    }

    private void onFlowExit(BaseFlow flow,FLOW_EXIT_CODE exitCode)
    {
        GAME_FLOW_ENUM flowType = flow.GetFlowEnum();
        if( exitCode == FLOW_EXIT_CODE.FLOW_EXIT_CODE_NEXT )
        {
            if (flowType == GAME_FLOW_ENUM.GAME_FLOW_VERIFY)
            {
                setNextFlow(GAME_FLOW_ENUM.GAME_FLOW_LOGIN);
            }
            else if (flowType == GAME_FLOW_ENUM.GAME_FLOW_LOGIN)
            {
                setNextFlow(GAME_FLOW_ENUM.GAME_FLOW_MAIN);
            }
            else if (flowType == GAME_FLOW_ENUM.GAME_FLOW_MAIN)
            {
                setNextFlow(GAME_FLOW_ENUM.GAME_FLOW_LOGIN);
            }
        }
    }

    public BaseFlow GetCurFlow()
    {
        return mCurFlow;
    }

    public void ReLogin()
    {
        MainFlow flow = mCurFlow as MainFlow;
        if (flow == null)
            return;

        flow.BackToLogin();
    }

    public void PlatformNotify()
    {
        LoginFlow flow = mCurFlow as LoginFlow;
        if (flow != null)
        {
            flow.PlatformLogin();
            return;
        }

        MainFlow mflow = mCurFlow as MainFlow;
        if (mflow == null)
            return;

        if (!PlatformSDK.PlatformLoginSucceed)
        {
            mflow.BackToLogin();
        }
        else
        {// 更新票据到服务器 xeixin

        }
    }

    public void OnBuyGameCoinsNeedLogin(string param)
    {
        MainFlow mflow = mCurFlow as MainFlow;
        if(mflow == null)
            return;

        mflow.OnBuyGameCoinsNeedLogin(param);
    }

    public void OnBuyGameCoinsRst(string param)
    {
        MainFlow mflow = mCurFlow as MainFlow;
        if (mflow == null)
            return;

        mflow.OnBuyGameCoinsRst(param);
    }
}
