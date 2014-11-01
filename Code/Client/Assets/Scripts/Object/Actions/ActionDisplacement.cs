using System.Collections.Generic;
using UnityEngine;

public class ActionDisplacementInitParam : ActionQuickMoveInitParam
{
	public SkillDisplacementTableItem displacementResource;
	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeDisplacement; }
	}
}

/// <summary>
/// 位移动作. 由技能发起, 进行突进, 牵引, 击退等操作.
/// </summary>
public class ActionDisplacement : ActionQuickMove
{
	public class Allocator : ActionAllocator
	{
		public Action Allocate()
		{
			return new ActionDisplacement();
		}
	}

	SkillDisplacementTableItem mDisplacementResource = null;

	// 只可以冲撞一个单位一次!
	HashSet<uint> mHitted = new HashSet<uint>();

	TargetSelectionTableItem mTargetSelectionOnExecute = null;

	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeDisplacement; }
	}

	protected override ErrorCode doStart(ActionInitParam param)
	{
		ErrorCode err = base.doStart(param);
		if (err != ErrorCode.Succeeded)
			return err;

		ActionDisplacementInitParam displacementInit = param as ActionDisplacementInitParam;

		mDisplacementResource = displacementInit.displacementResource;

		if (mDisplacementResource.leagueSelectionOnExecute != LeagueSelection.None && 
			mDisplacementResource.skillEffect2OthersOnExecute != uint.MaxValue)
			mTargetSelectionOnExecute = new TargetSelectionTableItem()
			{
				resID = -1,
				desc = "displacement hit",
				leagueSel = mDisplacementResource.leagueSelectionOnExecute,
				maxTargetCount = uint.MaxValue,
				shape = ShapeType.ShapeType_Rect,
				RectLength = mDisplacementResource.radiusOnCollide * 2
			};

		return ErrorCode.Succeeded;
	}

	protected override void onStopped(bool finished)
	{
		base.onStopped(finished);

		if (finished)
		{
			// 结束时给自己的效果.
			SkillDetails.AddSkillEffectByResource(mAttackerAttr, mOwner, mDisplacementResource.skillEffect2SelfOnArrive);

			// 结束时给目标的效果.
			SkillDetails.SelectTargetAndAddSkillEffect(mAttackerAttr,
				mOwner.GetPosition(), mOwner.GetDirection(), mDisplacementResource.targetSelectionOnArrive,
				mDisplacementResource.skillEffect2OthersOnArrive);

			// 在位移目标的周围创建.
			SkillDetails.CreateCreationAround(mAttackerAttr, mDisplacementResource.creationOnArrive, mOwner.GetPosition(), mOwner.GetDirection());
		}
	}

	protected override UpdateRetCode onUpdate(uint elapsed)
	{
		Vector3 oldPosition = mOwner.GetPosition();

		UpdateRetCode ret = base.onUpdate(elapsed);

		if (ret != UpdateRetCode.Aborted)
		{
			if (mTargetSelectionOnExecute != null)
				displacementCollide(oldPosition, mQuickMoveDirection, Utility.Distance2D(mOwner.GetPosition(), oldPosition));
		}

		return ret;
	}

	/// <summary>
	/// 处理使用者从startPositon, 沿direction方向位移distance距离的碰撞.
	/// </summary>
	private void displacementCollide(Vector3 startPosition, Vector3 direction, float distance)
	{
		mTargetSelectionOnExecute.RectWidth = distance;

		// 需要取得当前碰撞矩形的中点, 作为参数传入SelectTargets.
		float radDirection = Utility.Vector3ToRadian(direction);

		SkillDetails.SelectTargetAndAddSkillEffect(mAttackerAttr,
			Utility.MoveVector3Towards(startPosition, radDirection, distance / 2f),
			radDirection, mTargetSelectionOnExecute, mDisplacementResource.skillEffect2OthersOnExecute,
			checkCollide
			);
	}

	private bool checkCollide(BattleUnit target, params object[] p)
	{
		if (!mHitted.Contains(target.InstanceID))
		{
			mHitted.Add(target.InstanceID);
			return true;
		}
		return false;
	}

	#region OverrideProperties
	protected override float AbsoluteSpeed
	{
		get { return Mathf.Abs(mDisplacementResource.speed); }
	}

	protected override string AnimationName
	{
		get { return mDisplacementResource.animationName; }
	}

	protected override bool LoopAnimation
	{
		get { return mDisplacementResource.loopAnimation; }
	}

	protected override uint EffectID
	{
		get { return mDisplacementResource._3DEffectIdOnExecute; }
	}

	protected override string EffectBindPoint
	{
		get { return mDisplacementResource._3DEffectBindpointOnExecute; }
	}

	protected override bool LoopEffect
	{
		get { return mDisplacementResource.loop3DEffectOnExecute; }
	}

	protected override bool InterruptSkillUsing
	{
		get { return mDisplacementResource.interruptSkillUsing; }
	}

	protected override SkillDisplacementType QuickMoveType
	{
		get { return mDisplacementResource.displacementType; }
	}

	#endregion OverrideProperties
}
