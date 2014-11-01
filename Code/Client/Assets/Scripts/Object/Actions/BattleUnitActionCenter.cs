using System;
using System.Collections.Generic;
using UnityEngine;

public interface ActionAllocator
{ 
	Action Allocate();
}

class BattleUnitActionFactory
{
	ActionAllocator[] allocators = new ActionAllocator[(int)ActionTypeDef.ActionTypeCount];
	static BattleUnitActionFactory instance = new BattleUnitActionFactory();

	static public BattleUnitActionFactory Instance { get { return instance; } }

	BattleUnitActionFactory()
	{
		allocators[(int)ActionTypeDef.ActionTypeIdle] = new ActionIdle.Allocator();
		allocators[(int)ActionTypeDef.ActionTypeMove] = new ActionMove.Allocator();
		allocators[(int)ActionTypeDef.ActionTypeSkill] = new ActionSkill.Allocator();
		allocators[(int)ActionTypeDef.ActionTypeSpasticity] = new ActionSpasticity.Allocator();
		allocators[(int)ActionTypeDef.ActionTypeDisplacement] = new ActionDisplacement.Allocator();
		allocators[(int)ActionTypeDef.ActionTypeDie] = new ActionDie.Allocator();
		allocators[(int)ActionTypeDef.ActionTypeReload] = new ActionReload.Allocator();
	}

	public Action Allocate(ActionTypeDef type)
	{
		if (type >= ActionTypeDef.ActionTypeCount)
		{
			ErrorHandler.Parse(ErrorCode.LogicError, "invalid action type");
			return null;
		}

		return allocators[(int)type].Allocate();
	}
}

public class BattleUnitActionCenter
{
	BattleUnit mActionOwner = null;

	/// <summary>
	/// ÿ�����͵�Action, ֻ������һ���������е�ʵ��.
	/// </summary>
	Action[] mActionContainer = new Action[(int)ActionTypeDef.ActionTypeCount];

	/// <summary>
	/// ����������һֱ����Running״̬, ��ʹOwner�Ѿ�����.
	/// ��Ҫ������¼Owner��ս��״̬(����/��ս/��Ϣ), ������Ownerû������Action, ������ʱ,
	/// ���ݵ�ǰ״̬���Ŷ���(��Щ�����������ȼ��ܵ͵Ķ���).
	/// </summary>
	ActionIdle IdleAction
	{
		get { return mActionContainer[(int)ActionTypeDef.ActionTypeIdle] as ActionIdle; }
		set { mActionContainer[(int)ActionTypeDef.ActionTypeIdle] = value; }
	}

    bool isFighting = false;

	public BattleUnitActionCenter(BattleUnit user)
	{
		mActionOwner = user;
		IdleAction = BattleUnitActionFactory.Instance.Allocate(ActionTypeDef.ActionTypeIdle) as ActionIdle;
		IdleAction.Owner = mActionOwner;
		IdleAction.Start(new ActionIdleInitParam());
	}

	public void Destroy()
	{
		for (ActionTypeDef type = ActionTypeDef.ActionTypeIdle; type < ActionTypeDef.ActionTypeCount; ++type)
		{
			Action action = GetActionByType(type);
			if (action != null)
				action.Stop(false);
		}
	}

	public void EnterFightState()
	{
		if (IdleAction.IsRunning)
			IdleAction.EnterFightState();

        isFighting = true;
	}

    public bool IsFighting()
    {
        return isFighting;
    }

	/// <summary>
	/// ����һ��Action, ���ҽ������뵽Update������.
	/// </summary>
	/// ?
	public ErrorCode StartAction(ActionInitParam param)
	{
        if (param == null)
            return ErrorCode.InvalidParam;

        if (param.GetType() == typeof(ActionMoveInitParam))
        {
            ActionMove actionMove = GetActionByType(ActionTypeDef.ActionTypeMove) as ActionMove;
            if (actionMove != null)
            {
                return actionMove.Restart(param);
            }
        }

		if (GetActionByType(param.ActionType) != null)
		{
			ErrorHandler.Parse(ErrorCode.ConfigError, "only one " + param.ActionType + " can run");
			return ErrorCode.ConfigError;
		}

		Action action = BattleUnitActionFactory.Instance.Allocate(param.ActionType);

        if (action == null)
            return ErrorCode.LogicError;

		action.Owner = mActionOwner;

		ErrorCode err = action.Start(param);
		if (err == ErrorCode.Succeeded)
		{
			mActionContainer[(int)param.ActionType] = action;
			IdleAction.SetActive(false);
		}

		return err;
	}

	public BattleUnit Owner
	{
		get { return mActionOwner; }
	}

	/// <summary>
	/// ����ÿ��Action, Action��ʵ���������ͳһ���������.
	/// </summary>
	public void UpdateActions(uint elapsed)
	{
		for (ActionTypeDef type = ActionTypeDef.ActionTypeIdle; type < ActionTypeDef.ActionTypeCount; ++type)
		{
			Action action = mActionContainer[(int)type];
			if (action == null)
				continue;

			if (!action.IsRunning)
			{
				mActionContainer[(int)type] = null;
				continue;
			}

			UpdateRetCode ret = action.Update(elapsed);
			if (ret != UpdateRetCode.Continue)
			{
				action.Stop((ret == UpdateRetCode.Finished));
				mActionContainer[(int)type] = null;
			}
		}
	}

	/// <summary>
	/// Owner�Ķ�̬��Ƿ����仯, ֪ͨÿ��Action, �Ӷ���֪��ЩAction��Щ��Ҫ����ֹ.
	/// </summary>
	/// <param name="flagName">��̬��Ǳ�ʶ</param>
	public void OnActiveFlagsStateChanged(ActiveFlagsDef flagName, bool increased)
	{
		for (ActionTypeDef type = ActionTypeDef.ActionTypeIdle; type < ActionTypeDef.ActionTypeCount; ++type)
		{
			Action action = GetActionByType(type);
			if (action == null)
				continue;

			if (!action.OnActiveFlagsStateChanged(flagName, increased))
				action.Stop(false);
		}
   
		if (flagName == ActiveFlagsDef.IsDead)
			IdleAction.SetActive(Owner.isAlive());
	}

	/// <summary>
	/// ɾ������ָ�����͵�action(ֻ����ActionΪstopped, ʵ�ʵ���������ͳһ��Update����).
	/// </summary>
	public void RemoveActionByType(ActionTypeDef type)
	{
		Action action = GetActionByType(type);
		if (action != null)
			action.Stop(false);
	}

	/// <summary>
	/// ��ȡһ�����͵Ļ״̬��action.
	/// </summary>
	/// <param name="T"></param>
	/// <returns></returns>
	public Action GetActionByType(ActionTypeDef type)
	{
		Action result = mActionContainer[(int)type];
		if (result != null && !result.IsRunning)
			result = null;
		return result;
	}
}
