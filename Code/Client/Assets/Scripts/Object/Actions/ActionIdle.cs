public class ActionIdleInitParam : ActionInitParam
{
	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeIdle; }
	}
}

/// <summary>
/// 空闲. 指battleunit在没有其他所有动作时, 默认进入的动作.
/// 空闲时, 可以播放被击/备战/休息动作.
/// </summary>
public class ActionIdle : Action
{
	public class Allocator : ActionAllocator
	{
		public Action Allocate()
		{
			return new ActionIdle();
		}
	}

	IdleStateDef mState = IdleStateDef.Invalid;

	/// <summary>
	/// 每个状态对应的时间.
	/// </summary>
	static readonly uint[] mStateDuration = new uint[(int)IdleStateDef.Count] { 0, 3000, uint.MaxValue };

	/// <summary>
	/// 当前状态经历了多少时间.
	/// </summary>
	uint mStateTimer = 0;

	/// <summary>
	/// 表示当前的单位是否在空闲状态.
	/// </summary>
	bool mActive = false;

	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeIdle; }
	}

	protected override ErrorCode doStart(ActionInitParam param)
	{
		mActive = true;

		toState(IdleStateDef.Rest);

		return base.doStart(param);
	}

	private void toState(IdleStateDef state)
	{
		mState = state;
		mStateTimer = 0;

        if(mOwner.GetStateController() != null)
        {
            //mOwner.GetStateControl()._tempIdleState = (int)mState;

            mOwner.IdleIndex = mState;
        }
		if (Active)
			playAnimationByCurrentState();
	}

	private void playAnimationByCurrentState()
	{
        mOwner.GetStateController().DoAction(AnimActionFactory.E_Type.Idle);
        //mOwner.PlayAnimation(AnimActionFactory.E_Type.Idle);
	}

	public void SetActive(bool f)
	{
		mActive = f;
		onActive(f);
	}

	private void onActive(bool f)
	{
		if (f) playAnimationByCurrentState();
	}

	public void EnterFightState()
	{
		toState(IdleStateDef.Fight);
	}

	public bool Active { get { return mActive; } }

	protected override UpdateRetCode onUpdate(uint elapsed)
	{
		if (Active && (mStateTimer += elapsed) >= mStateDuration[(uint)mState])
		{
			toState(++mState);
		}

		return base.onUpdate(elapsed);
	}
}
