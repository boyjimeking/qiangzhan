using FantasyEngine;
using System;
using UnityEngine;

/// <summary>
/// 播放动画状态
/// </summary>
public class AnimStatePlayAnim : AnimState
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

    public AnimStatePlayAnim(MecanimManager anims, VisualObject owner)
        : base(anims, owner)
    {
    }
    public override bool HandleNewAction(AnimAction action)
    {
        if ((action is AnimActionPlayAnim) && (this.Action != null))
        {
            if ((action as AnimActionPlayAnim).AnimName == AnimName)
            {
                mReplay = true;
            }
            else
            {
                AnimName = (action as AnimActionPlayAnim).AnimName;
                mReplay = false;
            }

            action.SetFailed();
            return true;
        }
        return false;
    }

    protected override void Initialize(AnimAction action)
    {
        base.Initialize(action);
        Action = action;
        if (Action is AnimActionPlayAnim)
        {
            this.AnimName = (this.Action as AnimActionPlayAnim).AnimName;
            this.LookAtTarget = false;
        }
   

        if (this.AnimName == null)
        {
            this.Action.SetFailed();
            this.Action = null;
            this.Release();
        }
        else
        {
            this.RotationOk = true;

            SwitchState(this.AnimName, (int)AnimationLayer.BaseLayer);
            //CrossFade(this.AnimName, 0.1f, PlayMode.StopAll);
            //if (base.Animation[this.AnimName].wrapMode == WrapMode.Loop)
            //{
            //    this.EndOfStateTime = 100000f + Time.timeSinceLevelLoad;
            //}
            //else
            //{
            //    this.EndOfStateTime = (base.Animation[this.AnimName].length + Time.timeSinceLevelLoad) - 0.3f;
            //}
        }
    }

    public override void OnActivate(AnimAction action)
    {
        base.OnActivate(action);

    }

    public override void OnDeactivate()
    {
        LookAtTarget = false;
        Action.SetSuccess();
        Action = null;
        base.OnDeactivate();
    }

    public override void Reset()
    {
        LookAtTarget = false;
        Action.SetSuccess();
        Action = null;
        base.Reset();
    }

    public override void Update()
    {
        UpdateFinalRotation();
        SwitchState(this.AnimName, (int)AnimationLayer.BaseLayer);
        AnimatorStateInfo info = mAnimator.Anim.GetCurrentAnimatorStateInfo((int)AnimationLayer.BaseLayer);
        AnimatorStateInfo nextinfo = mAnimator.Anim.GetNextAnimatorStateInfo((int)AnimationLayer.BaseLayer);

        if (info.IsName(AnimName) && !info.loop && Mathf.FloorToInt(info.normalizedTime) >= 1 && !mAnimator.Anim.IsInTransition((int)AnimationLayer.BaseLayer))
        {
            if (mReplay)
            {
                mAnimator.Anim.Play(AnimName, (int)AnimationLayer.BaseLayer, 0);
                mReplay = false;
            }
            else
            {
                Release();
            }
        }
    

    }

    public override void HandlemAnimatorEvent(MecanimEvent animEvent)
    {
        GameDebug.Log(animEvent.name);
        if(animEvent != null)
        {
            if (mAnimator.Property.GetStateHash(AnimName) == animEvent.context.curstate)
                Release();
        }
    }

    private void UpdateFinalRotation()
    {
    }
}

