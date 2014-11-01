using FantasyEngine;
using System;
using UnityEngine;

/// <summary>
/// 受伤状态,此状态仅位于受伤的状态机下
/// </summary>
public class AnimStateHurt : AnimState
{
    private AnimAction Action;
    private string AnimName;

    private bool mValid = false;

    private int hurthash;


    public AnimStateHurt(MecanimManager anims, VisualObject owner)
        : base(anims, owner)
    {
        //hurthash = Animator.StringToHash("hurt");
    }
    public override bool HandleNewAction(AnimAction action)
    {

        if(action is AnimActionHurt)
        {
            AnimName = (action as AnimActionHurt).AnimName;
            Action = action;

            //if (Owner is Npc)
            //    Debug.Log(AnimName + "被鸡动作");

            SwitchToHurtState(mAnimator.Property.GetStateHash(AnimName),Animator.StringToHash(AnimName));
        }
        return true;
    }

    protected override void Initialize(AnimAction action)
    {
        base.Initialize(action);
        Action = action;
        if (Action is AnimActionHurt)
        {
            AnimName = (this.Action as AnimActionHurt).AnimName;
        }

        if (action == null)
            return;


        if (AnimName == null)
        {
            if(Action != null)
            {
                Action.SetFailed();
                Action = null;
            }

            Release();
        }
        else
        {

            //if(Owner is Npc)
            //    Debug.Log(AnimName + "被鸡动作");
            SwitchToHurtState(mAnimator.Property.GetStateHash(AnimName),Animator.StringToHash(AnimName));
 
        }
    }

    public void SwitchToHurtState(int namehash,int statehash)
    {
        if (mAnimator.Anim.layerCount > (int)AnimationLayer.Hurt)
        {
            mValid = true;

            if (!mAnimator.Property.IsStateExist(namehash))
                return;
          AnimatorStateInfo info =  mAnimator.Anim.GetCurrentAnimatorStateInfo((int)AnimationLayer.Hurt);
          if (info.nameHash == statehash && info.normalizedTime >= info.length)
            {
                mAnimator.Anim.Play(statehash, (int)AnimationLayer.Hurt, 0);
            }
            else
            {
                mAnimator.Anim.SetInteger("hurt",namehash);
            }
            mAnimator.Anim.SetLayerWeight((int)AnimationLayer.Hurt, 1);
        }

       
    }

    public override void OnActivate(AnimAction action)
    {
        base.OnActivate(action);

    }

    public override void OnDeactivate()
    {
        mAnimator.Anim.SetInteger("hurt", 0);
        base.OnDeactivate();
    }

    public override void Reset()
    {

        base.Reset();
    }

    public override void Update()
    {
        if (!mValid)
            return;
        AnimatorStateInfo info = mAnimator.Anim.GetCurrentAnimatorStateInfo((int)AnimationLayer.Hurt);

        if (info.IsName(AnimName) && info.normalizedTime >= info.length)
        {
            Release();
        }


    }

    public override void HandlemAnimatorEvent(MecanimEvent animEvent)
    {
        if (animEvent != null)
        {
            if (animEvent.name == MecanimEvent.MEC_ANIM_END && mAnimator.Property.GetStateHash(AnimName) == animEvent.context.curstate)
                Release();
        }
    }

    public override void Release()
    {
        //Debug.Log(AnimName + " 结束" + Time.frameCount);
        if (mAnimator.Anim.layerCount > (int)AnimationLayer.Hurt)
        {
            mAnimator.Anim.SetLayerWeight((int)AnimationLayer.Hurt, 0);
        }

        mValid = false;
        base.Release();
    }
}

