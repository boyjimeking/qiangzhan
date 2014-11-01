using UnityEngine;
using System.Collections;

public class GameSceneInitParam : BaseSceneInitParam
{
}

public class GameScene : BaseScene 
{
    // 游戏逻辑阶段
    private SceneLogicState mLogicState = SceneLogicState.SceneLogicState_Invalid;

    // 游戏逻辑阶段计时
    protected uint mLogicRunTime = 0;

	// 结果
	protected int mResult = -1;

    // 结算
    protected SceneBalanceComponent mBalanceComponent = null;

	// 复活
	protected SceneReliveComponent mReliveComponent = null;

    // 佣兵复活
    private CropsReliveComponent mCropsReliveComponent = null;

	// 战斗指引目标
	private BattleUnit mGuideTarget = null;

	/// <summary>
	/// 凋落物拾取指引目标
	/// </summary>
	private Pick mPickTarget = null;

    protected bool mShowPickGuide = false;

	// 最后一个Boss死亡的位置
	private Vector3 mBossDeadPos = Vector3.zero;

	// 是否通关结算
	protected bool mPassed = false;

	// 最大准备时间
	protected uint mMaxReadyTime = uint.MaxValue;

	// 最大游戏时间
	protected uint mMaxLogicTime = uint.MaxValue;

	// 战斗界面Module
	protected BattleUIModule mBattleUIModule = ModuleManager.Instance.FindModule<BattleUIModule>();

    public bool IsShowPickGuide
    {
        get
        {
            return mShowPickGuide;
        }
        set
        {
            mShowPickGuide = value;
        }
    }

	public Vector3 BossDeadPos
	{
		get
		{
			return mBossDeadPos;
		}
		set
		{
			mBossDeadPos = new Vector3(value.x, value.y, value.z);
		}
	}

    public GameScene()
    {

    }

	override public bool Init( BaseSceneInitParam param )
	{
		if( !base.Init(param) )	
		   return false;

		RemoveAllActionFlag();
        mCropsReliveComponent = new CropsReliveComponent(this);
		mMaxReadyTime = mSceneRes.mReadyTime;
		mMaxLogicTime = mSceneRes.mLogicTime;

		RemoveAllActionFlag();
		
		return true;
	}

    override public bool LogicUpdate(uint elapsed)
    {
        if(!base.LogicUpdate(elapsed))
        {
            return false;
        }

        mLogicRunTime += elapsed;

        SceneLogicResult ret = SceneLogicResult.SceneLogicResult_Continue;
        switch(mLogicState)
        {
            case SceneLogicState.SceneLogicState_Invalid:
                {
					ret = InitLogicState();
                }
                break;
            case SceneLogicState.SceneLogicState_Ready:
                {
                    ret = ReadyStateUpdate(elapsed);
                }
                break;
            case SceneLogicState.SceneLogicState_Working:
                {
                    ret = WorkingStateUpdate(elapsed);
                }
                break;
            case SceneLogicState.SceneLogicState_Closing:
                {
                    ret = ClosingStateUpdate(elapsed);
                }
                break;
			case SceneLogicState.SceneLogicState_Destroy:
				{
					ret = DestroyStateUpdate(elapsed);
				}
				break;
            default:
                break;
        }

        return DisposeLogicResult(ret);
    }

    private bool DisposeLogicResult(SceneLogicResult ret)
    {
        switch(ret)
        {
            case SceneLogicResult.SceneLogicResult_Destroy:
                {
                    return false;
                }
                break;
            case SceneLogicResult.SceneLogicResult_Continue:
                {
                    return true;
                }
                break;
            case SceneLogicResult.SceneLogicResult_NextState:
                {
                    int next = (int)mLogicState + 1;

					if (!System.Enum.IsDefined(typeof(SceneLogicState), next))
						return false;

					ChangeState((SceneLogicState)next);
                }
                break;
            default:
                break;
        }
        return true;
    }

	private SceneLogicResult InitLogicState()
	{
		OnGameStart();

		mLogicRunTime = 0;

		return SceneLogicResult.SceneLogicResult_NextState;
	}

    private SceneLogicResult ReadyStateUpdate(uint elapsed)
    {
		OnReadyUpdate(elapsed);

		if (mMaxReadyTime == 0 || mMaxReadyTime == uint.MaxValue)
			return SceneLogicResult.SceneLogicResult_NextState;

		if (mLogicRunTime > mMaxReadyTime)
		{
			mLogicRunTime -= mMaxReadyTime;
			return SceneLogicResult.SceneLogicResult_NextState;
		}

        return SceneLogicResult.SceneLogicResult_Continue;
    }

    private SceneLogicResult WorkingStateUpdate(uint elapsed)
    {
		OnWorkingUpdate(elapsed);

		if (mMaxLogicTime == 0 || mMaxLogicTime == uint.MaxValue)
			return SceneLogicResult.SceneLogicResult_Continue;

		if (mLogicRunTime > mMaxLogicTime)
		{
			mLogicRunTime -= mMaxLogicTime;
			return SceneLogicResult.SceneLogicResult_NextState;
		}

		return SceneLogicResult.SceneLogicResult_Continue;
    }

    private SceneLogicResult ClosingStateUpdate(uint elapsed)
    {
		DestroyCurGrowthTrigger();

		return SceneLogicResult.SceneLogicResult_NextState;
    }

	private SceneLogicResult DestroyStateUpdate(uint elapsed)
	{
		return SceneLogicResult.SceneLogicResult_Continue;
	}

    virtual protected void OnGameStart()
    {
        mScriptSystem.TriggerEvent("onGameStart", null);
    }

	virtual protected void OnReadyUpdate(uint elapsed)
	{

	}

	virtual protected void OnWorkingUpdate(uint elapsed)
	{
		UpdateGuideTarget();

		if (mShowPickGuide)
			UpdatePickTarget();
	}

	protected void ChangeState(SceneLogicState state)
	{
		if(mLogicState != state)
		{
			mLogicState = state;

			OnStateChanged(state);
		}
	}

	private void OnStateChanged(SceneLogicState state)
	{
		switch(state)
		{
			case SceneLogicState.SceneLogicState_Ready:
				{
					OnStateChangeToReady();
				}
				break;
			case SceneLogicState.SceneLogicState_Working:
				{
					OnStateChangeToWorking();
				}
				break;
			case SceneLogicState.SceneLogicState_Closing:
				{
					OnStateChangeToClosing();
				}
				break;
			case SceneLogicState.SceneLogicState_Destroy:
				{
					OnStateChangeToDestroy();
				}
				break;
			default:
				break;
		}
	}

	virtual protected void OnStateChangeToReady()
	{
		RemoveAllActionFlag();
	}

	virtual protected void OnStateChangeToWorking()
	{
		AddAllActionFlag();
	}

	virtual protected void OnStateChangeToClosing()
	{
		PlayerController.Instance.SetWudi(true);

		if (mBalanceComponent != null)
		{
			mBalanceComponent.Balance();
		}
	}

	virtual protected void OnStateChangeToDestroy()
	{

	}

	override public bool isPassed()
	{
		return mPassed;
	}

	override sealed public void pass()
	{
		if (isPassed())
			return;

		mPassed = true;

		ChangeState(SceneLogicState.SceneLogicState_Closing);
	}

	virtual public void SetResult(int result)
	{
		mResult = result;
	}

	public int GetResult()
	{
		return mResult;
	}

	virtual public uint GetSceneScore()
	{
		return uint.MaxValue;
	}

	virtual public StageGrade GetSceneGrade()
	{
		return StageGrade.StageGrade_Invalid;
	}

	override public bool isSafeScene()
	{
		return false;
	}

	public uint GetLogicRunTime()
	{
		return mLogicRunTime;
	}

    public void ResetLogicRunTime()
    {
        mLogicRunTime = 0;
    }

	public void ResetLogicState()
	{
		mPassed = false;
		ResetLogicRunTime();
		ChangeState(SceneLogicState.SceneLogicState_Ready);
	}

	private void UpdateGuideTarget()
	{
		ObjectBase owner = GetOwner();
		if (owner == null)
		{
			return;
		}

		BattleUnit target = SearchGuideTarget(owner);
		if(target == mGuideTarget)
		{
			return;
		}

		mGuideTarget = target;
		EventSystem.Instance.PushEvent(new GuideTargetEvent(GuideTargetEvent.GUIDE_TARGET_CHANGED));
	}

	// 搜索指引目标
	private BattleUnit SearchGuideTarget(ObjectBase owner)
	{
		if (mSceneObjManager == null)
		{
			return null;
		}

		return mSceneObjManager.SearchGuideTarget(owner);
	}

	// 指引目标方位
	public bool GetGuideTargetDir(ref Vector3 dir)
	{
		ObjectBase owner = GetOwner();
		if (owner == null)
		{
			return false;
		}

		if(mGuideTarget == null)
		{
			return false;
		}

		Vector3 srcPos = mGuideTarget.GetPosition() - owner.GetPosition();
		srcPos.y = 0.0f;

		dir = Utility.RotateVectorByAngle(srcPos, -CameraController.Instance.CurCamera.transform.localEulerAngles.y, Vector3.zero);
		dir.Normalize();

		return true;
	}

	void UpdatePickTarget()
	{
		ObjectBase owner = GetOwner();
		if (owner == null)
		{
			return;
		}
		
		Pick target = SearchPickTarget(owner);
		if(target == mPickTarget)
		{
			return;
		}
		
		mPickTarget = target;
		EventSystem.Instance.PushEvent(new GuideTargetEvent(GuideTargetEvent.PICK_TARGET_CHANGED));
	}

	/// <summary>
	/// 搜索道具目标
	/// </summary>
	/// <returns>The pick target.</returns>
	/// <param name="owner">Owner.</param>
	private Pick SearchPickTarget(ObjectBase owner)
	{
		if (mSceneObjManager == null)
		{
			return null;
		}
		
		return mSceneObjManager.SearchPickTarget(owner);
	}

	/// <summary>
	/// 指引道具的方位;
	/// </summary>
	/// <returns><c>true</c>, if pick target dir was gotten, <c>false</c> otherwise.</returns>
	/// <param name="dir">Dir.</param>
	public bool GetPickTargetDir(ref Vector3 dir)
	{
		ObjectBase owner = GetOwner();
		if (owner == null)
		{
			return false;
		}
		
		if(mPickTarget == null)
		{
			return false;
		}
		
		dir = mPickTarget.GetPosition() - owner.GetPosition();
		dir.y = 0.0f;
		dir.Normalize();
		return true;
	}

	public Pick GetPickTarget()
	{
		return mPickTarget;
	}

	public override void Destroy()
	{
		if(mReliveComponent != null)
		{
			mReliveComponent.Destroy();
		}

		if(mBalanceComponent != null)
		{
			mBalanceComponent.Destroy();
		}

		mGuideTarget = null;

		mPickTarget = null;

		base.Destroy();
	}

    protected override void OnSceneInited()
    {
        WindowManager.Instance.EnterFlow(UI_FLOW_TYPE.UI_FLOW_BATTLE);

        base.OnSceneInited();
    }

	override protected void OpenUI()
	{
		base.OpenUI();

		UIBattleFormInitParam initParam = new UIBattleFormInitParam();
		initParam.DisplayLianJi = MayDisplayLianJi();
		initParam.DisplayerGuideArrow = MayDisplayGuideArrow();
		initParam.DisplayController = true;
		WindowManager.Instance.OpenUI("battle", initParam);
	}

    protected override void OnSceneDestroy()
    {
        WindowManager.Instance.LeaveFlow(UI_FLOW_TYPE.UI_FLOW_BATTLE);

        base.OnSceneDestroy();
    }

	override protected void CloseUI()
	{
		base.CloseUI();

		WindowManager.Instance.CloseUI("battle");
	}

    protected virtual bool MayDisplayLianJi()
    {
        return true;
    }

    protected virtual bool MayDisplayGuideArrow()
    {
        return true;
    }

	public bool IsWorkingState()
	{
		return mLogicState == SceneLogicState.SceneLogicState_Working;
	}

    public override void OnCropsDie()
    {
        base.OnCropsDie();

        if (mCropsReliveComponent != null)
        {
            mCropsReliveComponent.requestRelive();
        }
    }

    public void ResetCropsReliveTimes()
    {
        var ssrc = mCropsReliveComponent as CropsReliveComponent;
        ssrc.ResetReliveTime();
    }

    public bool GetCropsCanRelive()
    {
        return mCropsReliveComponent.CropsCanRelive();
    }

    public bool MainCropsCanRelive()
    {
        return mCropsReliveComponent.MainCropsCanRelive();
    }

    public bool SubCropsCanRelive()
    {
        return mCropsReliveComponent.SubCrosCanRelive();
    }
}
