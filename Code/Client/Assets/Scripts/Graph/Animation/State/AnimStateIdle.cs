using FantasyEngine;
using System;
using System.Text;
using UnityEngine;

/// <summary>
/// 空闲状态
/// </summary>
public class AnimStateIdle : AnimState
{

    private IdleStateDef lastState = IdleStateDef.Count;

    private bool lastIsMove = false;
    private bool lastTrans = false;
    private int lastweaponid = -1;
    private MovingType lastMoveType = MovingType.MoveType_Name;

    private bool mInvalidState = false;
    public AnimStateIdle(MecanimManager anims, VisualObject owner)
        : base(anims, owner)
    {
    }

    ///
    public override bool HandleNewAction(AnimAction action)
    {
        if (action is AnimActionIdle)
        {
            action.SetFailed();
            return true;
        }

        if (action is AnimActionUseSkill)
        {
            //if((action as AnimActionUseSkill).AnimName == "xiechi.sheji")
            //{
            //    //Debug.Log("休息-->射击");
            //}
        }

        return false;
    }

    protected override void Initialize(AnimAction action)
    {
        base.Initialize(action);

        mInvalidState = true;
        PlayIdleAnim();

        if (action != null)
        {
            action.SetSuccess();
        }
    }

    public override void OnDeactivate()
    {

    }

    private void PlayIdleAnim()
    {
        /*
       //  * 播放空闲动作
       //  */
        if (Owner.IdleIndex >= IdleStateDef.Count)
            Owner.IdleIndex = IdleStateDef.Rest;


        string statename = null;

        BattleUnit battleUnit = Owner as BattleUnit;


        if (battleUnit != null)
        {
            if (Owner.IdleIndex == lastState &&
                lastMoveType == Owner.GetMovingType()
                && lastIsMove == battleUnit.IsMoveing()
                && lastTrans == battleUnit.IsInviolable()
                && lastweaponid == battleUnit.GetMainWeaponID()
                && !mInvalidState)
            {
                return;
            }
            lastState = Owner.IdleIndex;
            lastMoveType = Owner.GetMovingType();
            lastTrans = battleUnit.IsInviolable();
            lastIsMove = battleUnit.IsMoveing();
			lastweaponid = battleUnit.GetMainWeaponID();

        }
        else
        {
            if (Owner.IdleIndex == lastState && mInvalidState)
            {
                lastState = Owner.IdleIndex;
            }
        }

        mInvalidState = false;

        if (Owner.IdleIndex == IdleStateDef.Rest && battleUnit != null && !battleUnit.IsInviolable())
        {
            bool lowerMove = Owner.GetMovingType() == MovingType.MoveType_Lowwer;
            // 能不能移动
            if (lowerMove && !battleUnit.IsMoveing())
            {
                statename = Owner.CombineAnimname(AnimationNameDef.PrefixZhanliXiuxi);
            }

        }

        if (string.IsNullOrEmpty(statename) || !mAnimator.Property.IsStateExist(statename))
        {
            statename = IdleStateAnimationDef.GetAnimationNameByState(Owner.IdleIndex);
            statename = Owner.CombineAnimname(statename);
        }

        int statehash = mAnimator.Property.GetStateHash(statename);
        if (statehash == 0)
        {
            mInvalidState = true;
            return;
        }
        SetTransition(statehash);
    }

    public override void Reset()
    {
        base.Reset();


        mInvalidState = true;
        lastState = IdleStateDef.Count;
        lastIsMove = false;
        lastTrans = false;
        lastMoveType = MovingType.MoveType_Name;
    }

    public override void Update()
    {

        //播放空闲动作

        PlayIdleAnim();
    }
}

