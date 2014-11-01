using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static public class SkillUtilities
{
	/// <summary>
	/// 检查skill的CD时间, 返回该技能是否在CD中.
	/// </summary>
	public static bool CheckSkillCd(BattleUnitSkill skill)
	{
		return skill.skillRes != null && skill.CdMilliseconds == 0;
	}

	public static ErrorCode CheckDistance(BattleUnitSkill skill, Vector3 startPosition, Vector3 targetPosition)
	{
		if (skill.skillRes == null)
			return ErrorCode.InvalidParam;

		Vector3 v3 = targetPosition - startPosition;
		v3.y = 0;

		float magnitudeSquared = v3.sqrMagnitude;

		if (magnitudeSquared < skill.skillRes.minRange * skill.skillRes.minRange)
			return ErrorCode.TooClose;
		else if (magnitudeSquared > skill.skillRes.maxRange * skill.skillRes.maxRange)
			return ErrorCode.TooFar;

		return ErrorCode.Succeeded;
	}

	public static ErrorCode CheckCost(BattleUnit user, BattleUnitSkill skill)
	{
		if (skill.skillRes == null)
			return ErrorCode.InvalidParam;

		if (user.GetPropertyValue((int)PropertyTypeEnum.PropertyTypeMana) < skill.skillRes.manaCost)
			return ErrorCode.InsufficientMana;

		if (user.GetWeaponBullet() < skill.skillRes.bulletCost)
			return ErrorCode.InsufficientBullet;

		return ErrorCode.Succeeded;
	}

	/// <summary>
	/// 检查skill是否启用, CD时间, 消耗, 距离这些技能基本信息从而决定该技能可否使用.
	/// </summary>
	public static ErrorCode CheckSkillGeneralInfo(BattleUnit user, BattleUnitSkill skill, Vector3 startPosition, Vector3 targetPosition)
	{
		ErrorCode err = CheckSkillGeneralInfo(user, skill);
		if (err != ErrorCode.Succeeded)
			return err;
		return CheckDistance(skill, startPosition, targetPosition);
	}

	/// <summary>
	/// 检查skill是否启用, CD时间, 消耗这些技能基本信息<b>(与另一重载的区别在于该方法不检查距离)</b>从而决定该技能可否使用.
	/// </summary>
	public static ErrorCode CheckSkillGeneralInfo(BattleUnit user, BattleUnitSkill skill)
	{
		if (!skill.enabled)
			return ErrorCode.SkillDisabled;

		if (!CheckSkillCd(skill))
			return ErrorCode.CoolingDown;

		return CheckCost(user, skill);
	}

	/// <summary>
	/// 检查技能使用者的当前状态, 可否使用skill标识的技能, 并返回错误码.
	/// </summary>
	public static ErrorCode CheckUserSkillUsingState(BattleUnitActionCenter actionCenter, BattleUnitSkill skill)
	{
		ActionSkill currentAs = actionCenter.GetActionByType(ActionTypeDef.ActionTypeSkill) as ActionSkill;

		if (currentAs == null) return ErrorCode.Succeeded;

		// 当前正在进行技能动作.
		// 执行相同的技能.
		if (currentAs.SameSkill(skill))
		{
			// 正在使用阶段则无法被打断(因为是相同的技能, 不存在非普通攻击打断普通攻击的情况).
			if (currentAs.InUsingState)
				return ErrorCode.SkillAlreadyUsing;
			// 当前技能正在准备, 如果不能切换, 那么返回错误.
			else if (!currentAs.Interruptable)
				return ErrorCode.SkillUninterruptable;
		}
		else	// 其他技能使用.
		{
			// 普通攻击, 在任何情况下都被打断.
			if (currentAs.IsRegularAttack)
			{
			}
			// 非普通攻击, 在准备阶段. 检查它可否被打断.
			else if (currentAs.InChargingState)
			{
				if (!currentAs.Interruptable)
					return ErrorCode.SkillUninterruptable;
			}
			// 非普通攻击, 在使用阶段.
			else
			{
				return ErrorCode.SkillAlreadyUsing;
			}

			// 当前的技能被打断.
			actionCenter.RemoveActionByType(ActionTypeDef.ActionTypeSkill);
			currentAs = null;
		}

		return ErrorCode.Succeeded;
	}

	/// <summary>
	/// 是否为控制标记.
	/// </summary>
	public static bool IsStunFlag(ActiveFlagsDef flag)
	{
		return flag >= ActiveFlagsDef.DisableMovement && flag < ActiveFlagsDef.StunImmunity;
	}

	/// <summary>
	/// <para>解析属性.</para>
	/// 多组属性之间, 用'|'分割. 属性以及操作用'+'或者'-'分割, 分别表示该属性增加或者减少指定值.
	/// </summary>
	public static List<Pair<int, int>> ParseProperties(string properties)
	{
		if (string.IsNullOrEmpty(properties))
			return null;

		List<Pair<int, int>> result = new List<Pair<int, int>>();
		string[] group = properties.Split('|');
		foreach (string p in group)
		{
			int sep = p.IndexOfAny(new char[] { '+', '-' });
			int propID = System.Convert.ToInt32(p.Substring(0, sep));
			int addValue = System.Convert.ToInt32(p.Substring(sep));
			result.Add(SkillUtilities.MakePair(propID, addValue));
		}

		return result;
	}

	public static List<Pair<uint, string>> ParseSkillTransform(string skillTransform)
	{
		if (string.IsNullOrEmpty(skillTransform))
			return null;

		List<Pair<uint, string>> result = new List<Pair<uint, string>>();
		string[] group = skillTransform.Split('|');
		foreach (string p in group)
		{
			string[] pair = p.Split('&');
			uint skillResID = System.Convert.ToUInt32(pair[0]);
			string iconName = (pair.Length > 1) ? pair[1] : null;
			result.Add(MakePair(skillResID, iconName));
		}

		return result;
	}

	/// <summary>
	/// 根据目标选择参数, 以centerPosition为中心选择目标.
	/// </summary>
	/// <param name="attackerAttr">攻击者的数据</param>
	/// <param name="centerPosition">选择的中心点</param>
	/// <param name="attackerDirection">攻击者的方向(对于矩形, 扇形时用到)</param>
	/// <param name="res">目标选择的资源</param>
	/// <returns>目标集合, 不会为null.</returns>
	public static ArrayList SelectTargets(AttackerAttr attackerAttr, Vector3 centerPosition, float attackerDirection, TargetSelectionTableItem targetSelRes)
	{
		BaseScene scn = SceneManager.Instance.GetCurScene();
		if (scn == null)
			return null;

        SceneShapeRect selRect = null;
		ArrayList result = null;
        SceneShape shape = null;
		switch (targetSelRes.shape)
		{// 假设目标最大半径为5.0f米
			case ShapeType.ShapeType_Round:
                {
                    float radius = targetSelRes.CircleRadius * 2.0f + 5.0f;
                    selRect = new SceneShapeRect(new Vector2(centerPosition.x, centerPosition.z), radius, radius); 
                    shape = new SceneShapeRound(new Vector2(centerPosition.x, centerPosition.z), targetSelRes.CircleRadius);
                }
				break;
            case ShapeType.ShapeType_Rect:
                {
                    float radius = targetSelRes.RectLength + targetSelRes.RectWidth + 5.0f;
                    selRect = new SceneShapeRect(new Vector2(centerPosition.x, centerPosition.z), radius, radius);
				    SceneShapeRect rect = new SceneShapeRect(new Vector2(centerPosition.x, centerPosition.z), targetSelRes.RectLength, targetSelRes.RectWidth);
				    shape = SceneShapeUtilities.rotate(rect, new Vector2(centerPosition.x, centerPosition.z), attackerDirection * Mathf.Rad2Deg);
                }
                break;
			case ShapeType.ShapeType_Invalid:
				break;
			default:
				ResourceInvalidParam("targetselection", (uint)targetSelRes.resID, "形状");
				break;
		}

        if (shape != null && selRect != null)
        {
            ArrayList lst = scn.SearchObject(selRect, ObjectType.OBJ_SEARCH_BATTLEUNIT);
            if(lst != null && lst.Count > 0)
            {
                result = new ArrayList();
                for(int i = 0; i < lst.Count; i++)
                {
                    ObjectBase obj = lst[i] as ObjectBase;
                    if (obj == null)
                        continue;

                    if(shape.intersect(obj.GetShape()))
                    {
                        result.Add(obj);
                    }
                }
            }
        }

		if (result == null)
			result = new ArrayList();

		// 根据阵营筛选.
		FilterTargetsBy(result, filterTargetsByLeague, attackerAttr, targetSelRes.leagueSel);

		// 最多只能选择maxTargetCount个单位.
		RandomSampling(result, targetSelRes.maxTargetCount);

		return result;
	}

	static bool filterTargetsByLeague(BattleUnit target, params object[] param)
	{ 
		return LeagueManager.IsDisiredTarget((AttackerAttr)param[0], target, (LeagueSelection)param[1]);
	}

	/// <summary>
	/// 通过filter以及param, 将container中不满足的对象删除.
	/// </summary>
	public static void FilterTargetsBy(ArrayList container, FilterTarget filter, params object[] param)
	{
		int firstUnmatched = 0;

		// firstUnmatched及之后的所有battleunit都无效.
		for (int i = 0; i < container.Count; ++i) {
			BattleUnit target = (BattleUnit)container[i];
			if (filter(target, param))
			{
				container[firstUnmatched++] = container[i];
			}
		}
		
		// 移除该位置及之后的所有battleunit.
		container.RemoveRange(firstUnmatched, container.Count - firstUnmatched);
	}

	/// <summary>
	/// 从container中, 随机选取desired个单位.
	/// </summary>
	public static void RandomSampling(ArrayList container, uint desired)
	{
		if (container.Count > desired)
			Sampling(container, (int)desired);
	}

	/// <summary>
	/// 在container中随机选取desired个元素.
	/// </summary>
	static void Sampling(ArrayList container, int desired)
	{
		for (int i = 0; i < desired; ++i)
		{
			int r = UnityEngine.Random.Range(i, container.Count);
			if (r != i)
			{
				var tmp = container[i];
				container[i] = container[r];
				container[r] = tmp;
			}
		}

		container.RemoveRange(desired, container.Count - desired);
	}

	/// <summary>
	/// 蓄水池抽样, 从N个元素中选取m个(N > m), 使最终的m个元素每一个被选取的概率都是m/N.
	/// </summary>
	/// <remarks>
	/// 较Sampling更复杂, 但该方法无需知道container的元素个数, container无需支持随机访问(如链表),
	/// 只要可以取得下一个元素即可.
	/// </remarks>
	static List<T> ReservoirSampling<T>(LinkedList<T> container, int desired)
	{
		LinkedListNode<T> node = container.First;
		List<T> result = new List<T>(new T[] { node.Value });

		int index = 1;
		for (; index < desired; ++index)
		{
			node = node.Next;
			result.Add(node.Value);
		}

		do
		{
			node = node.Next;

			int r = UnityEngine.Random.Range(0, ++index);
			if (r < desired)
				result[r] = node.Value;

		} while (node != container.Last);

		return result;
	}

	/// <summary>
	/// 传入概率值[0, 100], 返回这次随机尝试是否成功.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static bool Random100(uint value)
	{
		// UnityEngine.Random.Range(int, int):
		// Returns a random integer number between min [inclusive] and max [exclusive].
		return UnityEngine.Random.Range(0, 100) < value;
	}
	
	/// <summary>
	/// 判断某一skilleffect是否是有害的.
	/// </summary>
	/// <param name="type">skilleffect的类型</param>
	/// <param name="resID">skilleffect的资源ID</param>
	/// <returns></returns>
	public static bool IsHarmfulEffect(SkillEffectType type, uint resID)
	{
		switch (type)
		{ 
			case SkillEffectType.Buff:
				SkillBuffTableItem resBuff = DataManager.BuffTable[resID] as SkillBuffTableItem;
				return resBuff == null || resBuff.harmful;
				
			case SkillEffectType.Impact:
				SkillImpactTableItem resImpact = DataManager.ImpactTable[resID] as SkillImpactTableItem;
				return resImpact == null || resImpact.harmful;

			case SkillEffectType.Displacement:
				SkillDisplacementTableItem resDisplacement = DataManager.DisplacementTable[resID] as SkillDisplacementTableItem;
				return resDisplacement == null || resDisplacement.harmful;
			case SkillEffectType.Spasticity:
				return true;
			default:
				ErrorHandler.Parse(ErrorCode.LogicError, "invalid skill effect type: " + type);
				break;
		}

		return true;
	}

	/// <summary>
	/// 表格tableName中的条目resID不存在.
	/// </summary>
	public static void ResourceNotFound(string tableName, uint resID)
	{
		string message = "\"" + tableName + "\"中的项[" + resID + "]不存在";
		ErrorHandler.Parse(ErrorCode.ConfigError, message);
	}

	public static void ResourceInvalidParam(string tableName, uint resID, string fieldName, string desc = "")
	{
		String errMessage = ("\"" + tableName + "\"" + "中的项[" + resID + "]: \"" + fieldName + "\"上的值无效");
		if(!string.IsNullOrEmpty(desc))
			errMessage += ": " + desc;
		ErrorHandler.Parse(ErrorCode.ConfigError, errMessage);
	}

	/// <summary>
	/// 由fromTableName的fromTableID条目指向的toTableName的toTableID条目不存在.
	/// </summary>
	public static void ResourceBadLink(string fromTableName, uint fromTableID, string toTableName, uint toTableID)
	{
		ErrorHandler.Parse(ErrorCode.ConfigError,
			("由\"" + fromTableName + "\"[" + fromTableID + "]指向的\"" + toTableName + "\"[" + toTableID + "]不存在"));
	}

	/// <summary>
	/// 返回该target是否满足条件.
	/// </summary>
	public delegate bool FilterTarget(BattleUnit target, params object[] param);

	public static Pair<T1, T2> MakePair<T1, T2>(T1 first, T2 second)
	{
		return new Pair<T1, T2>(first, second);
	}

	/// <summary>
	/// 查找/删除skilleffect时使用.
	/// </summary>
	public abstract class SkillEffectMatchPredicate
	{
		public static implicit operator Predicate<SkillEffect>(SkillEffectMatchPredicate pred)
		{
			return pred.match;
		}

		protected abstract bool match(SkillEffect effect);
	}

	/// <summary>
	/// 根据资源ID查找BUFF.
	/// </summary>
	public class FindBuffByResource : SkillEffectMatchPredicate
	{
		uint BuffResID = uint.MaxValue;
		public FindBuffByResource(uint resID)
		{
			BuffResID = resID;
		}

		protected override bool match(SkillEffect effect)
		{
			return effect.Type == SkillEffectType.Buff && effect.ResID == BuffResID;
		}
	}

	/// <summary>
	/// 对一个skilleffect执行操作(操作不能删除skilleffect中的元素).
	/// </summary>
	public abstract class SkillEffectAction
	{
		protected abstract void takeAction(SkillEffect effect);

		public static implicit operator System.Action<SkillEffect>(SkillEffectAction action)
		{
			return action.takeAction;
		}
	}

	/// <summary>
	/// 移除skilleffect.
	/// </summary>
	public abstract class SkillEffectKiller : SkillEffectAction
	{
		protected SkillEffectStopReason StopReason { get; private set; }

		public SkillEffectKiller(SkillEffectStopReason reason)
		{
			StopReason = reason;
		}
	}

	/// <summary>
	/// 当skilleffect的目标的状态发生变化时, 检查skilleffect是否需要被移除.
	/// </summary>
	public class KillSkillEffectOnOwnerEvent : SkillEffectKiller
	{
		SkillEffectOwnerEventDef mOwnerEvent = SkillEffectOwnerEventDef.Invalid;
		public KillSkillEffectOnOwnerEvent(SkillEffectOwnerEventDef e)
			: base(SkillEffectStopReason.Recycled)
		{
			mOwnerEvent = e;
		}

		protected override void takeAction(SkillEffect effect)
		{
			if (effect.NeedRemoveOnOwnerEvent(mOwnerEvent))
				effect.Stop(StopReason);
		}
	}

	/// <summary>
	/// 根据资源ID, 删除buff.
	/// </summary>
	public class KillSkillBuffByResource : SkillEffectKiller
	{
		uint ResID = uint.MaxValue;
		public KillSkillBuffByResource(uint resID)
			: base(SkillEffectStopReason.Diffused)
		{
			ResID = resID;
		}

		protected override void takeAction(SkillEffect effect)
		{
			if(effect.Type == SkillEffectType.Buff && effect.ResID == ResID)
				effect.Stop(StopReason);
		}
	}

	/// <summary>
	/// 根据BUFF组ID, 删除buff.
	/// </summary>
	public class KillSkillBuffByGroup : SkillEffectKiller
	{
		uint mGroupID = uint.MaxValue;
		public KillSkillBuffByGroup(uint groupID)
			: base(SkillEffectStopReason.Diffused)
		{
			mGroupID = groupID;
		}

		protected override void takeAction(SkillEffect effect)
		{
			SkillBuff buff = effect as SkillBuff;
			if (buff != null && buff.SameGroup(mGroupID))
				buff.Stop(StopReason);
		}
	}

	/// <summary>
	/// 根据BUFF的增减益类型, 删除buff.
	/// </summary>
	public class KillHarmfulSkillBuff : SkillEffectKiller
	{
		bool mHarmful = true;
		public KillHarmfulSkillBuff(bool harmful)
			: base(SkillEffectStopReason.Diffused)
		{
			mHarmful = harmful;
		}

		protected override void takeAction(SkillEffect effect)
		{
			if (IsHarmfulEffect(effect.Type, effect.ResID) == mHarmful)
				effect.Stop(StopReason);
		}
	}

	/// <summary>
	/// 移除所有带有控制的buff.
	/// </summary>
	public class KillSkillBuffWithStunEffect : SkillEffectKiller
	{
		public KillSkillBuffWithStunEffect()
			: base(SkillEffectStopReason.Diffused)
		{
		}

		protected override void takeAction(SkillEffect effect)
		{
			SkillBuff buff = effect as SkillBuff;
			if (buff != null && buff.HasStunEffect)
				effect.Stop(StopReason);
		}
	}

	/// <summary>
	/// 移除所有与当前互斥类型互斥的buff.
	/// </summary>
	public class KillMutuallyExclusiveSkillBuff : SkillEffectKiller
	{ 
		uint mMutex = uint.MaxValue;
		public KillMutuallyExclusiveSkillBuff(uint mutex)
			: base(SkillEffectStopReason.Diffused)
		{
		}

		protected override void takeAction(SkillEffect effect)
		{
			SkillBuff buff = effect as SkillBuff;
			if (buff != null && buff.IsMutuallyExclusive(mMutex))
				effect.Stop(StopReason);
		}
	}

	/// <summary>
	/// 回收所有的skilleffect的资源.
	/// </summary>
	public class RecycleAllSkillEffect : SkillEffectKiller
	{
		List<Pair<SkillEffectType, uint>> mStillRunning = null;

		/// <param name="container">传出仍在执行中的skilleffect的类型和资源ID.</param>
		public RecycleAllSkillEffect(List<Pair<SkillEffectType, uint>> container)
			: base(SkillEffectStopReason.Recycled)
		{
			mStillRunning = container;
		}

		protected override void takeAction(SkillEffect effect)
		{
			if (effect.IsAwake)
				mStillRunning.Add(SkillUtilities.MakePair(effect.Type, effect.ResID));

			effect.Stop(StopReason);
		}
	}

	/// <summary>
	/// 将目标点控制在技能的minRange和maxRange之间.
	/// </summary>
	public static Vector3 RoundTargetPosition(float minRange, float maxRange, Vector3 startPosition, Vector3 targetPosition)
	{
		Vector3 v3 = targetPosition - startPosition;
		v3.y = 0f;

		float lengthSquared = v3.sqrMagnitude;
		
		if (lengthSquared < minRange * minRange)
		{
			v3.Normalize();
			v3 *= minRange;
		}
		else if (lengthSquared > maxRange * maxRange)
		{
			v3.Normalize();
			v3 *= maxRange;
		}

		return v3 + startPosition;
	}

	public delegate void TaskHandler(object param);

	public class Task
	{
		/// <summary>
		/// 任务的触发时间.
		/// </summary>
		public uint TimeTrigger { get; private set; }

		/// <summary>
		/// 初始化一个任务, 任务在timeTrigger毫秒后, 调用taskHandler方法, 以taskParam作为参数.
		/// </summary>
		public Task(uint timeTrigger, TaskHandler taskHandler, object taskParam)
		{
			TimeTrigger = timeTrigger;
			handler = taskHandler;
			param = taskParam;
		}

		// 触发该任务.
		public void Invoke() { if (handler != null) handler(param); }
		
		// 任务触发时的回调.
		TaskHandler handler;

		object param;
	}

	/// <summary>
	/// 根据时间, 加入任务队列, 在manager更新时, 会检查任务的时间, 并触发任务.
	/// </summary>
	public class TaskManager
	{
		// 任务的队列, 按照时间从晚到早的顺序排列.
		List<Task> mTaskContainer = new List<Task>();
		uint mElapsed = 0;
		bool mIsRunning = false;

		// 避免TaskManager在Update时被修改.
		bool mLocked = false;

		/// 所有任务正常完成时的回调.
		public delegate void FinishedCallback();
		FinishedCallback mOnTaskFinished = null;

		/// <summary>
		/// <para>所有任务正常结束后, 调用callback方法.</para>
		/// 在调用callback时, manager已经停止, 需要重新start.
		/// </summary>
		public TaskManager(FinishedCallback callback = null)
		{
			mOnTaskFinished = callback;
		}

		public void Start()
		{
			mIsRunning = true;
			mElapsed = 0;
			//Update(0);
		}
		
		/// <summary>
		/// 停止管理器, 并清空任务列表.
		/// </summary>
		public void Stop()
		{
			if (mLocked) {
				ErrorHandler.Parse(ErrorCode.ConfigError, "failed to stop TaskManager, TaskContainer is Locked");
				return;
			}

			mIsRunning = false;
			mElapsed = 0;
			mTaskContainer.Clear();
		}

		public void AddTask(Task task)
		{
			if (mLocked) {
				ErrorHandler.Parse(ErrorCode.LogicError, "failed to add task, TaskContainer is Locked");
				return;
			}

			if (mIsRunning) {
				ErrorHandler.Parse(ErrorCode.LogicError, "you must stop TaskManager before you add a new task");
				return;
			}

			int pos = findFirstEarlierTask(mTaskContainer, task.TimeTrigger);
			mTaskContainer.Insert(pos, task);
		}

		public uint TaskCount { get { return (uint)mTaskContainer.Count; } }

		public void Update(uint elapsed)
		{
			if (!mIsRunning) return;

			mElapsed += elapsed;

			// 第一个触发时间不晚于mElapsed的任务的索引.
			// 由于列表按照时间由晚到早的顺序排列, pos之后的任务也必然不早于pos位置处的任务.
			int pos = findFirstEarlierTask(mTaskContainer, mElapsed);

			mLocked = true;
			// 依次将他们触发(注意, Invoke时, 不能修改mTaskManager).
			for (int i = pos; i < mTaskContainer.Count; ++i)
				mTaskContainer[i].Invoke();
			mLocked = false;

			// 移除触发过的任务.
			mTaskContainer.RemoveRange(pos, mTaskContainer.Count - pos);
			
			// 所有任务都已完成.
			if (mTaskContainer.Count == 0)
			{
				Stop();
				if (mOnTaskFinished != null)
					mOnTaskFinished();
			}
		}

		/// <summary>
		/// 由于任务列表是有序的, 通过二分查找, 找到第一个触发时间不晚于time的任务的索引.
		/// </summary>
		private int findFirstEarlierTask(List<Task> container, uint time)
		{
			int first = 0;
			for (int count = container.Count; count > 0; )
			{
				int step = count / 2;
				int mid = first + step;
				if (container[mid].TimeTrigger <= time)
					count = step;
				else
				{
					first = mid + 1;
					count -= (step + 1);
				}
			}

			return first;
		}
	}

	/// <summary>
	/// 快速移动的位移控制器.
	/// </summary>
	public class QuickMoveController
	{
		/// <summary>
		/// 开始位置.
		/// </summary>
		Vector3 startPosition;

		/// <summary>
		/// 方向(已单位化).
		/// </summary>
		Vector3 normalizedDirection;

		/// <summary>
		/// 剩余的快速移动距离.
		/// </summary>
		float leftDistance;

		public QuickMoveController(Vector3 start, Vector3 dir, float s)
		{
			startPosition = start;
			normalizedDirection = dir.normalized;
			leftDistance = s;
		}

		/// <summary>
		/// 更新下一帧的位置.
		/// </summary>
		/// <returns>返回false, 表示快速移动已经结束</returns>
		public bool Update(uint elapsed, float speed, out Vector3 targetPosition)
		{
			bool unfinished = true;

			float distance = elapsed * speed / 1000f;

			// 最大只能继续leftDistance距离.
			leftDistance -= distance;
			if (leftDistance < 0f)
			{
				distance += leftDistance;
				unfinished = false;
			}

			startPosition = normalizedDirection * distance + startPosition;

			targetPosition = startPosition;

			return unfinished;
		}
	}
}
