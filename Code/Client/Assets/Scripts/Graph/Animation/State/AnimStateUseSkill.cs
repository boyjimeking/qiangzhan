using FantasyEngine;
using System;
using UnityEngine;

/// <summary>
/// 使用技能状态
/// </summary>
public class AnimStateUseSkill : AnimState
{
    private AnimAction Action;
    private string AnimName;
    private float CurrentRotationTime;
    private float EndOfStateTime;
    private Quaternion FinalRotation;
    public bool LookAtTarget;
    private bool RotationOk;
    private float RotationTime;
    private Quaternion StartRotation;

    private bool mReplay = false;
    private bool mActionLoop = false;

    private bool mInitilizeUpdate = true;

    public AnimStateUseSkill(MecanimManager anims, VisualObject owner)
        : base(anims, owner)
    {
    }
    public override bool HandleNewAction(AnimAction action)
    {
        if ((action is AnimActionUseSkill) && (this.Action != null))
        {
            action.SetFailed();

            //Debug.Log("当前" + AnimName + "目标" + (action as AnimActionUseSkill).AnimName);
            if ((action as AnimActionUseSkill).AnimName == AnimName)
            {
                mReplay = true;
            }
            else
            {
                AnimName = (action as AnimActionUseSkill).AnimName;

                SetTransition(AnimName);
            }

            SetFinished(false);
            mActionLoop = (action as AnimActionUseSkill).loop;
            return true;
        }

        if(action is AnimActionDeath)
        {
            //如果是死亡动作，立刻死亡
            action.SetSuccess();
            return false;
        }

        if(action is AnimActionMove)
        {
            action.SetFailed();
            //移动不能打断技能
            return true;
        }

        return false;
    }

    protected override void Initialize(AnimAction action)
    {
        base.Initialize(action);
        Action = action;
        mReplay = false;
        mInitilizeUpdate = true;
        if (Action is AnimActionUseSkill)
        {
            AnimName = (this.Action as AnimActionUseSkill).AnimName;
            mActionLoop = (Action as AnimActionUseSkill).loop;
        }
        if (this.AnimName == null)
        {
            this.Action.SetFailed();
            this.Release();
        }
        else
        {

            AnimatorStateInfo info = mAnimator.Anim.GetCurrentAnimatorStateInfo((int)AnimationLayer.BaseLayer);
            if (info.IsName(AnimName))
                mAnimator.Anim.Play(AnimName, (int)AnimationLayer.BaseLayer, 0);

            SetTransition(AnimName);
        }
    }

    public override void OnActivate(AnimAction action)
    {
        base.OnActivate(action);

    }

    public override void OnDeactivate()
    {
        SetTransition(0);
        mActionLoop = false;
        LookAtTarget = false;
        Action.SetSuccess();
        Action = null;
        base.OnDeactivate();
    }

    public override void Reset()
    {
        LookAtTarget = false;
        Action.SetSuccess();
        mReplay = false;
        base.Reset();
    }

    public override void Update()
    {
        UpdateFinalRotation();

        UpdateAnimation();
    }


    public void UpdateAnimation()
    {
        AnimatorStateInfo info = mAnimator.Anim.GetCurrentAnimatorStateInfo((int)AnimationLayer.BaseLayer);


        /*if (info.IsName(AnimName) && !info.loop && !mActionLoop && info.normalizedTime >= info.length)
        {
            if (mReplay)
            {
                mAnimator.Anim.Play(AnimName, (int)AnimationLayer.BaseLayer, 0);
                mReplay = false;
            }
            else if (!mAnimator.Anim.IsInTransition((int)AnimationLayer.BaseLayer))
            {
                Release();
            }
               
        }*/

		if (info.IsName(AnimName))
		{
			if (mReplay)
			{
				mAnimator.Anim.Play(AnimName, (int)AnimationLayer.BaseLayer, 0);
				mReplay = false;
			}
			else if (!info.loop && !mActionLoop && info.normalizedTime >= info.length)
			{
				Release();
			}
		}
    }
    public override void HandlemAnimatorEvent(MecanimEvent animEvent)
    {
        if (animEvent != null)
        {
            if (animEvent.name == MecanimEvent.MEC_ANIM_END && mAnimator.Property.GetStateHash(AnimName) == animEvent.context.curstate)
            {
                Release();                
            }
             
        }
    }

    private void UpdateFinalRotation()
    {
    }

    public override void Release()
    {
        base.Release();
    }
}

