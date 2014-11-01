using UnityEngine;

public class ActionSpasticityInitParam : ActionQuickMoveInitParam
{
	public SkillSpasticityTableItem spasticityRes;

	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeSpasticity; }
	}
}

/// <summary>
/// 硬直, 进行一段时间的击飞, 一段时间的硬直恢复.
/// </summary>
public class ActionSpasticity : ActionQuickMove
{
	public class Allocator : ActionAllocator
	{
		public Action Allocate()
		{
			return new ActionSpasticity();
		}
	}

	/// <summary>
	/// 击飞的速度.
	/// </summary>
	static readonly float SpasticityBeatbackSpeed = GameConfig.SpasticityBeatbackSpeed;

	/// <summary>
	/// 硬直之后的后摇时间(即硬直恢复时间).
	/// </summary>
	uint spasticityBackswing = uint.MaxValue;
	SkillSpasticityTableItem spasticityRes = null;

	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeSpasticity; }
	}

	/// <summary>
	/// 重新开始硬直过程.
	/// </summary>
	public ErrorCode Restart(ActionSpasticityInitParam param)
	{
		return doStart(param);
	}

	protected override ErrorCode doStart(ActionInitParam param)
	{
		ActionSpasticityInitParam initParam = param as ActionSpasticityInitParam;
		spasticityRes = initParam.spasticityRes;
		spasticityBackswing = uint.MaxValue;
		return base.doStart(param);
	}

	protected override UpdateRetCode onUpdate(uint elapsed)
	{
		if (spasticityBackswing == 0)
			return UpdateRetCode.Finished;

		Vector3 targetPosition = Vector3.zero;

		// 后摇为MaxValue, 表示仍在击飞中.
		if (spasticityBackswing == uint.MaxValue)
		{
			// 击飞结束, 进入后摇时间.
			if (!mController.Update(elapsed, AbsoluteSpeed, out targetPosition))
				spasticityBackswing = spasticityRes.spasticityBackswing;

			if (!mOwner.Scene.IsBarrierRegion(mOwner, targetPosition.x, targetPosition.z))
				mOwner.SetPosition(targetPosition);
		}
		// 更新后摇时间.
		else if (spasticityBackswing > elapsed)
			spasticityBackswing -= elapsed;
		else
		{
			// 后摇时间结束.
			spasticityBackswing = 0;
		}

		return (spasticityBackswing == 0) ? UpdateRetCode.Finished : UpdateRetCode.Continue;
	}

	#region OverrideProperties
	protected override string AnimationName
	{
		get { return AnimationNameDef.JiangZhi; }
	}

	protected override bool LoopAnimation
	{
		get { return true; }
	}

	protected override float AbsoluteSpeed
	{
		get { return SpasticityBeatbackSpeed; }
	}

	protected override bool InterruptSkillUsing
	{
		get { return true; }
	}

	protected override SkillDisplacementType QuickMoveType
	{
		get { return SkillDisplacementType.Beatback; }
	}

	#endregion OverrideProperties
}
