using System;
using UnityEngine;
public class ActionReloadInitParam : ActionInitParam
{
    public int weaponid;
    public ActionReloadInitParam()
    {
        weaponid = -1;
    }

	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeReload; }
	}
}

/// <summary>
/// 换弹中
/// </summary>
public class ActionReload : Action
{
	public class Allocator : ActionAllocator
	{
		public Action Allocate()
		{
			return new ActionReload();
		}
	}

    private int mWeaponResID = -1;

    private int mReloadTime = 0;

    private int mWaitTime = 0;

    private bool mWaiting = false;

    private int mReloadAnimationHashCode = 0;

	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeReload; }
	}

	protected override ErrorCode doStart(ActionInitParam param)
	{
        ActionReloadInitParam reloadParam = (ActionReloadInitParam)(param);
        if( reloadParam == null )
        {
            return ErrorCode.ConfigError;
        }

        mWeaponResID = reloadParam.weaponid;

        if( mWeaponResID < 0 )
        {
            return ErrorCode.ConfigError;
        }

        if( !DataManager.WeaponTable.ContainsKey(mWeaponResID) )
        {
            GameDebug.LogError("ActionReload 未找到武器 id = " + mWeaponResID.ToString());
           return ErrorCode.ConfigError;
        }

        WeaponTableItem item = DataManager.WeaponTable[mWeaponResID] as WeaponTableItem;

        mWaiting = true;
        mWaitTime = (int)item.reload_interval;
        mReloadTime = (int)item.reload_time;

        mOwner.AddActiveFlag(ActiveFlagsDef.DisableSkillUse, true, true);

		return base.doStart(param);
	}

    private void OnBegin()
    {
        AnimActionUseSkill skillAction = AnimActionFactory.Create(AnimActionFactory.E_Type.UseSkill) as AnimActionUseSkill;
        skillAction.AnimName = mOwner.CombineAnimname("%huandan");
        if (mOwner.GetStateController().AnimSet != null)
            mReloadAnimationHashCode = mOwner.GetStateController().AnimSet.GetStateHash(skillAction.AnimName);
        mOwner.GetStateController().DoAction(skillAction);

        mOwner.PlayWeaponAnim(AnimationNameDef.WeaponDefault);
		if(mOwner is Player || ((mOwner is Ghost) && (mOwner as Ghost).IsMainPlayer()))
		{
			ReloadEvent evt = new ReloadEvent(ReloadEvent.RELOAD_EVENT);
			evt.reload_time = mReloadTime;
			EventSystem.Instance.PushEvent(evt);
		}
    }

	protected override UpdateRetCode onUpdate(uint elapsed)
	{
        if (mWaiting)
        {
            mWaitTime -= (int)elapsed;
            if (mWaitTime <= 0)
            {
                OnBegin();
                mWaiting = false; 
            }
            return UpdateRetCode.Continue;
        }

        if (mReloadTime > 0)
        {
            mReloadTime -= (int)elapsed;

            return UpdateRetCode.Continue;
        }

        return UpdateRetCode.Finished;
	}

	protected override ErrorCode doStop(bool finished)
	{
        mOwner.AddWeaponBullet(mOwner.GetWeaponMaxBullet());
        mOwner.AddActiveFlag(ActiveFlagsDef.DisableSkillUse, false, true);
        mOwner.GetStateController().FinishCurrentState(mReloadAnimationHashCode);

		if (mOwner is Player || ((mOwner is Ghost) && (mOwner as Ghost).IsMainPlayer()))
		{
			ReloadEvent evt = new ReloadEvent(ReloadEvent.RELOAD_EVENT);
			evt.reload_time = -1;
			EventSystem.Instance.PushEvent(evt);
		}

		return base.doStop(finished);
	}

	protected override void onStopped(bool finished)
	{
		base.onStopped(finished);
	}
}
