public abstract class ActionInitParam
{
	public abstract ActionTypeDef ActionType { get; }
}

public abstract class Action
{
    protected BattleUnit mOwner;

	public abstract ActionTypeDef ActionType { get; }

    /// <summary>
    /// 该action是否在执行.
    /// </summary>
	public bool IsRunning {
		get;
		private set;
	}

    public Action()
    {
		IsRunning = false;
    }

    public BattleUnit Owner
    {
        set { mOwner = value; }
    }

    public ErrorCode Start(ActionInitParam param)
    {
        ErrorCode ret = canStart(param);

        if (ret != ErrorCode.Succeeded)
            return ret;

        if (IsRunning)
            Stop(false);

		IsRunning = false;

        ErrorCode err = doStart(param);

        if (err != ErrorCode.Succeeded)
            return err;

		IsRunning = true;

        onStarted();

        return ErrorCode.Succeeded;
    }

    /// <summary>
    /// 结束一个Action.
    /// Finished_killed为true表示自然停止, 为false表示被强制终止.
    /// </summary>
    public ErrorCode Stop(bool Finished_killed)
    {
        if (!IsRunning)
            return ErrorCode.Succeeded;

		IsRunning = false;
        ErrorCode err = doStop(Finished_killed);
        if (err != ErrorCode.Succeeded)
            return err;

        onStopped(Finished_killed);
        return ErrorCode.Succeeded;
    }

    public UpdateRetCode Update(uint elapsed)
    {
        return IsRunning ? onUpdate(elapsed) : UpdateRetCode.Aborted;
    }

	protected virtual UpdateRetCode onUpdate(uint elapsed)
    {
        return UpdateRetCode.Continue;
    }

    protected virtual ErrorCode canStart(ActionInitParam param)
    {
        return ErrorCode.Succeeded;
    }

    protected virtual ErrorCode doStart(ActionInitParam param)
    {
        return ErrorCode.Succeeded;
    }

    protected virtual void onStarted() { }

    protected virtual ErrorCode doStop(bool finished)
    {
        return ErrorCode.Succeeded;
    }

    protected virtual void onStopped(bool finished) { }

    /// <summary>
    /// 活动状态发生变化时, 通知每个action. 返回该action是否可以继续.
    /// 如果返回false, 该Action会被管理器停止.
    /// 尽量不要在每个Action的Update里轮询.
    /// </summary>
    /// <param name="flagName"></param>
    /// <returns></returns>
    public virtual bool OnActiveFlagsStateChanged(ActiveFlagsDef flagName, bool increased)
    {
        return true;
    }
}
