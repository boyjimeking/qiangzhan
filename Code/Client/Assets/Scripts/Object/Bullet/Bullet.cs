using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FantasyEngine;

public class BulletInitParam : ObjectInitParam
{
	public uint resID = uint.MaxValue;
	public Vector3 startPosition = Vector3.zero;
	public Vector3 targetPosition = Vector3.zero;

	public AttackerAttr firerAttr;

	public uint createDelay = 0;
}

/// <summary>
/// <para>子弹的基类.</para>
/// </summary>
public abstract class Bullet : ObjectBase 
{
	protected enum BulletState : int
	{
		Invalid = -1,
		Ready,
		Flying,
		Arrived,
	}

	protected BulletTableItem mBulletResource = null;

	private ParticleVisual mVisual = null;

	private BulletState mState = BulletState.Invalid;

	// 子弹只可以命中一个单位一次!
	protected HashSet<uint> mHitted = new HashSet<uint>();

	private TargetSelectionTableItem mFlyTargetSelection = null;

    protected BulletInitParam mBulletParam = null;

	// 子弹模型延迟创造时间.
	private uint mCreateDelay = 0;

	/// 预警特效.
	private uint mAlertEffect = uint.MaxValue;

	/// <summary>
	/// 子弹的飞行和爆炸分别由mFlyController和mExplodeController控制, 子弹的类型只是指定了二者的一些组合方式.
	/// </summary>
	private BulletFlyController mFlyController = null;
	private BulletExplodeController mExplodeController = null;

	protected AttackerAttr mFirerAttr;

	public override bool Init(ObjectInitParam param)
	{
		if (!base.Init(param))
			return false;

		mBulletParam = (BulletInitParam)param;

		if ((mBulletResource = DataManager.BulletTable[mBulletParam.resID] as BulletTableItem) == null)
		{
			SkillUtilities.ResourceNotFound("bullet", mBulletParam.resID);
			return false;
		}

		mCreateDelay = mBulletParam.createDelay;

		mFirerAttr = mBulletParam.firerAttr;

		FlySpeed = mBulletResource.flySpeed;

		StartPosition = mBulletParam.startPosition;
		TargetPosition = mBulletParam.targetPosition;

		Vector3 dir3d = TargetPosition - StartPosition;
		dir3d.y = 0;

		if (dir3d == Vector3.zero)
		{
			// 起始点与终点位置重叠, 取开火者当前朝向.
			BattleUnit firer = mFirerAttr.CheckedAttackerObject();
			float alternative = (firer != null) ? firer.GetDirection() : mFirerAttr.AttackerDirection;
			dir3d = Utility.RadianToVector3(alternative);
		}

		FlyDirection = Quaternion.LookRotation(dir3d);
		SetRotation(FlyDirection);

		AccelerateDelay = mBulletResource.accelerateDelay;

		mFlyTargetSelection = new TargetSelectionTableItem()
		{
			resID = -1,
			desc = "bullet hit",
			leagueSel = mBulletResource.leagueSelection,
			//maxTargetCount = mBulletResource.flyThroughCount - (uint)mHitted.Count,
			shape = ShapeType.ShapeType_Rect,
			RectLength = mBulletResource.radiusOnCollide * 2
		};

		SetPosition(StartPosition);

		return true;
	}

	/// <summary>
	/// 子弹进入准备阶段前, 检测初始的碰撞和阻挡, 从而决定子弹究竟进入何种状态.
	/// </summary>
	private BulletState readyCheck()
	{
		BulletState state = BulletState.Ready;

		// 如果开火者的位置, 不在子弹的飞行方向上, 这样检测会不准确.
		// 检测开火者的位置到子弹起始点之间的阻挡.
		Vector3 farthest = Vector3.zero;
		if (Scene.TestLineBlock(mFirerAttr.AttackerPosition, StartPosition, 
			true, CheckLowBlockOnFlying, out farthest))
		{
			// 存在阻挡, 目标点截断至farthest.
			// 表示子弹产生就会出现阻挡, 因此子弹不会产生, 直接爆炸.
			state = BulletState.Arrived;
		}

		// 检测开火者的位置到子弹起始点之间的碰撞.
		// (如果开火者的位置, 不在子弹的飞行方向上, 这样检测会不准确)
		if (CheckFlyHit && FlyHit(mFirerAttr.AttackerPosition, farthest))
			state = BulletState.Arrived;

		// 如果没有延迟时间, 那么直接进入飞行状态.
		if (state != BulletState.Arrived && mCreateDelay == 0)
			state = BulletState.Flying;

		return state;
	}

	/// <summary>
	/// 在准备状态进行更新, 经过子弹延迟创建时间之后, 创建子弹.
	/// </summary>
	/// <param name="elapsed"></param>
	/// <returns></returns>
	private bool readyUpdate(uint elapsed)
	{
		if (mCreateDelay > elapsed)
			mCreateDelay -= elapsed;
		else
		{
			mCreateDelay = 0;
			switch2State(BulletState.Flying);
		}
		return true;
	}

    //进入场景后
	public override void OnEnterScene(BaseScene scene, uint instanceid)
	{
		base.OnEnterScene(scene, instanceid);

		try
		{
			mFlyController = initFlyController();
			mExplodeController = initExplodeController();
		}
		catch (System.Exception exception)
		{
			switch2State(BulletState.Invalid);
			GameDebug.LogError(exception);
			return;
		}

		// 检查从开火者位置到枪口的碰撞等.
		BulletState newState = readyCheck();

		// 根据子弹的飞行控制器, 确定目标点.
		if (newState != BulletState.Arrived)
			verifyRange(mBulletParam);

		// 
		switch2State(newState);

		// 
		mFlyController.Launch();
	}
	/// <summary>
	/// <para>初始化飞行控制器.</para>
	/// 可能抛出ArgumentException.
	/// </summary>
	protected abstract BulletFlyController initFlyController();

	/// <summary>
	/// <para>初始化爆炸控制器.</para>
	/// 可能抛出ArgumentException.
	/// </summary>
	protected abstract BulletExplodeController initExplodeController();

	public override void Destroy()
	{

        BehaviourUtil.StopCoroutine(generateVisual((int)mBulletResource.bulletFigureID));

		if (mAlertEffect != uint.MaxValue)
			Scene.RemoveEffect(mAlertEffect);

        if (mVisual != null)
        {
           ParticleVisual.DestroyParticle(mVisual);
           mVisual = null;

        }

		mFlyController = null;

		mExplodeController = null;

		base.Destroy();
	}

	public override bool Update(uint elapsed)
	{
        base.Update(elapsed);

        bool ret = true;
		switch (mState)
		{
			case BulletState.Ready:
				{
					ret = readyUpdate(elapsed);
				}
				break;
			case BulletState.Flying:
				{
					if (!mFlyController.FlyUpdate(elapsed))
						switch2State(BulletState.Arrived);
				}
				break;
			case BulletState.Arrived:
				{
					mExplodeController.Explode();
					ret = false;
				}
				break;
			case BulletState.Invalid:
				{
					GameDebug.LogError("bullet state is invalid");
					ret = false;
				}
				break;
		}

        return ret;
	}

	protected override void OnChangePosition(Vector3 oldPos, Vector3 curPos)
	{
		if (mVisual != null && mVisual.Visual != null)
		{
            Transform t = mVisual.VisualTransform;
			t.position = curPos;
		}

		base.OnChangePosition(oldPos, curPos);
	}

	protected void switch2State(BulletState newState)
	{
		if (mState != newState)
		{
			mState = newState;

			if (mState == BulletState.Flying)
			{
				onStartFlying();
			}
		}
	}

	private void onStartFlying()
	{
		BehaviourUtil.StartCoroutine(generateVisual((int)mBulletResource.bulletFigureID));

		uint effectID = mBulletResource.effectOnTargetPosition;
		if (effectID != uint.MaxValue)
		{
			mAlertEffect = Scene.CreateEffect((int)effectID, Vector3.one, AlertEffectPosition, GetDirection());
		}
	}

	/// <summary>
	/// 预警特效位置.
	/// </summary>
	protected virtual Vector3 AlertEffectPosition 
	{
		get {
			Vector3 targetPosition = TargetPosition;
			targetPosition.y = Scene.GetHeight(targetPosition.x, targetPosition.z) + 0.05f;
			return targetPosition;
		}
	}

	/// <summary>
	/// 检查单位可否被命中.
	/// </summary>
	private bool checkHitObject(BattleUnit obj, params object[] p)
	{
		if (!mHitted.Contains(obj.InstanceID))
		{
			mHitted.Add(obj.InstanceID);
			return true;
		}
		return false;
	}


	private IEnumerator generateVisual(int effectID)
	{
		if (mVisual != null)
		{
            ParticleVisual.DestroyParticle(mVisual);
			mVisual = null;
		}

        EffectTableItem res = DataManager.EffectTable[effectID] as EffectTableItem;
        if( res != null )
        {
            mVisual = ParticleVisual.CreateParticle(res.effect_name, null);
            if (mVisual == null)
                yield break;
            mVisual.Scale = res.scale;

            bool needWaitForSomeTime = !mVisual.IsCompleteOrDestroy;
            //在这里处理缩放数值
            while (mVisual != null && !mVisual.IsCompleteOrDestroy)
                yield return 1;

            if (mVisual == null || mVisual.IsDestroy)
                yield break;

            mVisual.Visual.SetActive(false);

            SetRotation(DefaultRotation);
            OnChangePosition(GetPosition(), GetPosition());

            //if (needWaitForSomeTime)
            //    yield return 10;

            if (mVisual != null && mVisual.Visual != null)
                mVisual.Visual.SetActive(true);

        }
    }

    /// <summary>
	/// 确定目标点的位置, 检查子弹初始位置和目标点之间的阻挡.
	/// </summary>
	private void verifyRange(BulletInitParam param)
	{
		//如果存在高/低阻挡, 那么截断至最远点.
		if (mFlyController.FlyStraight)
		{
			Vector3 targetPosition = Vector3.zero;
			if (Scene.TestLineBlock(StartPosition, TargetPosition, true, CheckLowBlockOnFlying, out targetPosition))
			{
				targetPosition.y = TargetPosition.y;
				TargetPosition = targetPosition;
			}
		}

		LeftRange = TotalRange = Utility.Distance2D(TargetPosition, StartPosition);
	}
	
	/// <summary>
	///  处理子弹由pos向前飞行dist时的碰撞, 返回子弹是否需要爆炸.
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="dist"></param>
	/// <returns></returns>
	public bool FlyHit(Vector3 pos, float dist, float dirRadian)
    {
		if (Utility.isZero(dist))
			return false;

		mFlyTargetSelection.maxTargetCount = mBulletResource.flyHitCount - (uint)mHitted.Count;
		mFlyTargetSelection.RectWidth = dist;

		Vector3 graphCenter = Utility.MoveVector3Towards(GetPosition(), dirRadian, dist / 2f);

		uint flyCollideCount = collideAt(graphCenter, dirRadian, mFlyTargetSelection, mBulletResource.skilleffectOnFlyCollide);
		return flyCollideCount >= mFlyTargetSelection.maxTargetCount;
    }

	/// <summary>
	/// 从graphCenter, 沿dirRadian方向, 通过targetSel选择目标, 并给选择到的目标添加skillEffectID标识的技能效果.
	/// </summary>
	/// <returns>命中目标个数</returns>
	private uint collideAt(Vector3 graphCenter, float dirRadian, TargetSelectionTableItem targetSel, uint skillEffectID)
	{
		ArrayList targets = SkillUtilities.SelectTargets(mFirerAttr, graphCenter, dirRadian, targetSel);
		SkillUtilities.FilterTargetsBy(targets, checkHitObject);

		AttackerAttr other = mFirerAttr;
		other.SetEffectStartLocation(graphCenter, dirRadian);

		foreach (BattleUnit t in targets)
		{
			SkillDetails.AddSkillEffectByResource(other, t, skillEffectID);
		}

		return (uint)targets.Count;
	}

	protected bool FlyHit(Vector3 from, Vector3 to)
	{
		return FlyHit(from, Utility.Distance2D(from, to), Utility.Vector3ToRadian(to - from));
	}

    public Quaternion GetRotation()
    {
		if (mVisual != null && mVisual.Visual != null)
		{
            return mVisual.VisualTransform.rotation;
		}

		Quaternion q = new Quaternion();
		q.eulerAngles = new Vector3(0f, GetDirection() * Mathf.Rad2Deg, 0f);
		return q;
    }

	/// <summary>
	/// 设置子弹的朝向, 该朝向为子弹的模型的朝向.
	/// 不影响子弹的2D方向上的飞行朝向, 该朝向由FlyDirection控制.
	/// </summary>
	/// <param name="q"></param>
    public void SetRotation(Quaternion q)
    {
        if (mVisual != null && mVisual.Visual != null)
        {
            mVisual.VisualTransform.rotation = q;
        }

		SetDirection(q.eulerAngles.y * Mathf.Deg2Rad);
    }

	#region Properties

	/// <summary>
	/// 子弹模型创建之后的默认朝向.
	/// </summary>
	protected virtual Quaternion DefaultRotation
	{
		get { return Quaternion.LookRotation(TargetPosition - StartPosition); }
	}

	/// <summary>
	/// 飞行时是否检测低阻挡.
	/// </summary>
	protected virtual bool CheckLowBlockOnFlying { get { return true; } }

	/// <summary>
	/// 飞行时是否可以命中.
	/// </summary>
	protected virtual bool CheckFlyHit { get { return true; } }

	public float FlySpeed { get; set; }

	public float Accelerate { get { return mBulletResource.accelerateSpeed; } }

	public uint AccelerateDelay { get; set; }

	public float LeftRange { get; set; }

	public float TotalRange { get; set; }

	/// <summary>
	/// 子弹2D方向上的飞行方向.
	/// </summary>
	public Quaternion FlyDirection { get; set; }

	public AttackerAttr FirerAttr { get { return mFirerAttr; } }

	public uint CreationOnArrive { get { return mBulletResource.creationOnArrive; } }

	public uint CurrentHittedCount { get { return (uint)mHitted.Count; } }

	public uint MaxHitCount { get { return mBulletResource.flyHitCount; } }

	public uint ExplodeEffect { get { return mBulletResource.threeDEffectOnArrive; } }

	public uint TargetSelectionOnArrive { get { return mBulletResource.targetSelectionOnArrive; } }

	public uint SkillEffectOnArrive { get { return mBulletResource.skilleffectOnArrive; } }

	public Vector3 StartPosition { get; set; }

	public Vector3 TargetPosition { get; set; }

	#endregion Properties
}
