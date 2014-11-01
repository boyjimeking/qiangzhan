using System;
using System.Collections.Generic;

public class SkillEffectManager
{
	private List<SkillEffect> mContainer = new List<SkillEffect>();
	private BattleUnit mOwner;

	public SkillEffectManager(BattleUnit owner)
	{
		mOwner = owner;
	}

	public void Destroy()
	{
		List<Pair<SkillEffectType, uint>> stillRunningEffects = new List<Pair<SkillEffectType, uint>>();
		ForEverySkillEffect(new SkillUtilities.RecycleAllSkillEffect(stillRunningEffects));

		#region DEBUG
		foreach (var value in stillRunningEffects)
		{
			GameDebug.Log("skilleffect type = " + value.first + ", id = " + value.second + " will be saved");
		}
		#endregion DEBUG
	}

	public BattleUnit Owner { get { return mOwner; } }

	public void UpdateSkillEffects(uint elapsed)
	{
		// 构造临时容器, 更新之中的元素.
		// 因为在Update和Stop一个SkillEffect时, 可能产生新的SkillEffect加入到mContainer中,
		// 因此只遍历当前容器内的元素.
		int count = mContainer.Count;
		for(int i = 0; i < count; ++i)
		{
			SkillEffect effect = mContainer[i];
			if (!effect.IsAwake) continue;

			UpdateRetCode retCode = effect.Update(elapsed);
			if (retCode != UpdateRetCode.Continue)
			{
				// effect从运行状态开始, 经过Update之后, 不在工作, 原因只可能是
				// 时间结束或者在Update中, 被终止(不可能被回收).
				effect.Stop(retCode == UpdateRetCode.Finished 
					? SkillEffectStopReason.Expired : SkillEffectStopReason.Diffused
					);
			}
		}

		mContainer.RemoveAll(x => (!x.IsAwake));
	}

	/// <summary>
	/// 根据参数, 创建技能效果, 并使之开始运行.
	/// </summary>
	/// <returns></returns>
	public ErrorCode CreateSkillEffect(SkillEffectInitParam param)
	{
		SkillEffect effect = SkillDetails.AllocSkillEffect(param);

		if (effect == null)
			return ErrorCode.InvalidParam;

		if (!effect.Initialize(param))
			return ErrorCode.LogicError;

		ErrorCode err = effect.Start(param);

		if (err == ErrorCode.Succeeded)
		{
			if (effect.NeedUpdate)
				mContainer.Add(effect as SkillBuff);
			else
				effect.Stop(SkillEffectStopReason.Expired);
		}

		return err;
	}

	/// <summary>
	/// 查找当前运行的effect中, 类型为type, 且满足pred的effect.
	/// </summary>
	public SkillEffect FindSkillEffectByPredicate(SkillUtilities.SkillEffectMatchPredicate pred)
	{
		return mContainer.Find(pred);
	}

	/// <summary>
	/// <para>对每个技能效果, 调用action.</para>
	/// <para>action, `不支持删除`技能效果容器中的对象, 所有的移除操作都需要在Update中进行.</para>
	/// 可以增加新的效果到技能效果容器的尾部.
	/// </summary>
	public void ForEverySkillEffect(SkillUtilities.SkillEffectAction action)
	{
		// 由于ForEach:
		// Modifying the underlying collection in the body of the Action<T> delegate is not supported and causes undefined behavior.
		// 记录当前容器中元素的个数, 只遍历当前个数的对象.
		ForCurrentObjects(action);
	}

	/// <summary>
	/// 对当期容器中的元素执行操作.
	/// </summary>
	private void ForCurrentObjects(System.Action<SkillEffect> action)
	{
		int count = mContainer.Count;
		for (int i = 0; i < count; ++i)
			action(mContainer[i]);
	}
}
