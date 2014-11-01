using FantasyEngine;
using System;
using UnityEngine;

/// <summary>
/// 行走状态
/// </summary>
public class AnimStateMove : AnimState
{
    private AnimActionMove Action;
    private string AnimNameBase;
    private string AnimNameDown;
    private string AnimNameUp;
    private float BlendDown;
    private float BlendUp;
    private float MaxSpeed;

    public AnimStateMove(MecanimManager anims, VisualObject owner) : base(anims, owner)
    {
    }

    public override bool HandleNewAction(AnimAction action)
    {
        if (action is AnimActionMove)
        {
            if (Action != null)
            {
                Action.SetSuccess();
            }
            SetFinished(false);
            Initialize(action);
            return true;
        }
        if (action is AnimActionIdle)
        {
            action.SetFailed();
            SetFinished(true);
            return true;
        }
        return false;
    }

    protected override void Initialize(AnimAction action)
    {
        base.Initialize(action);
        Action = action as AnimActionMove;
    }

    public override void OnActivate(AnimAction action)
    {
        base.OnActivate(action);
    }

    public override void OnDeactivate()
    {
        //base.Owner.BlackBoard.MotionType = E_MotionType.None;
        //base.Owner.BlackBoard.MoveDir = Vector3.zero;
        //this.Action.SetSuccess();
        //this.Action = null;
        //if (base.Owner.BlackBoard.AimAnimationsEnabled)
        //{
        //    base.Animation[this.AnimNameUp].weight = 0f;
        //    base.Animation[this.AnimNameDown].weight = 0f;
        //    base.Animation.Stop(this.AnimNameUp);
        //    base.Animation.Stop(this.AnimNameDown);
        //}
        //base.OnDeactivate();
    }

    private void PlayMoveAnim(bool force)
    {
        BattleUnit battleUnit = Owner as BattleUnit;
        if (battleUnit == null)
        {
            Release();
            return;
        }
        bool lowerMove = Owner.GetMovingType() == MovingType.MoveType_Lowwer;
        // 能不能移动
        if (!battleUnit.IsCanMove() || !battleUnit.IsMoveing())
        {
            if (lowerMove)
            {
                StopMove();
                return;//这里处在下半身移动层。。
            }

            Release();
            return;
        }


        if (lowerMove)
        {
            //更新状态

            Speed = battleUnit.GetSpeed();
            Direction = Owner.GetAnimAngle();
            MoveState = (float)Owner.GetMoveState();
        }
        else
        {

            string statename = battleUnit.CombineAnimname(AnimationNameDef.PrefixPao);

            SetTransition(statename);
            //SwitchState(statename, (int)AnimationLayer.BaseLayer);
        }
    }

    public void StopMove()
    {
        if (mAnimator.Anim == null)
            return;

        if (mAnimator.Anim.layerCount <= 1)
            return;
        float weight = mAnimator.Anim.GetLayerWeight((int)AnimationLayer.LowwerBody);
        if (weight > 0.01f)
            mAnimator.Anim.SetLayerWeight((int)AnimationLayer.LowwerBody, 0);
    }
    public float Speed
    {
        set
        {
            mAnimator.Anim.SetFloat("Speed", value);
            if (mAnimator.Anim.layerCount <= 1)
                return;
            mAnimator.Anim.SetLayerWeight((int)AnimationLayer.LowwerBody, 1);
        }
    }
    public float Direction
    {
        set
        {
            mAnimator.Anim.SetFloat("Dirx", (float)Math.Cos(value));
            mAnimator.Anim.SetFloat("Diry", (float)Math.Sin(value));
            if (mAnimator.Anim.layerCount <= 1)
                return;

            float weight = mAnimator.Anim.GetLayerWeight((int)AnimationLayer.LowwerBody);
            if (weight < 1f)
                mAnimator.Anim.SetLayerWeight((int)AnimationLayer.LowwerBody, 1);
        }
    }

    public float MoveState
    {
        set
        {
            mAnimator.Anim.SetFloat("movestate", value);
        }
    }

    public override void Reset()
    {
        //if (base.Owner.BlackBoard.AimAnimationsEnabled)
        //{
        //    base.Animation[this.AnimNameUp].weight = 0f;
        //    base.Animation[this.AnimNameDown].weight = 0f;
        //    base.Animation.Stop(this.AnimNameUp);
        //    base.Animation.Stop(this.AnimNameDown);
        //}
        //this.Action.SetSuccess();
        //this.Action = null;
        //base.Reset();
    }

    public override void Update()
    {
        PlayMoveAnim(false);

    }

}

