using FantasyEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// 动画状态
/// </summary>
public class AnimState
{
    protected MecanimManager mAnimator;
    private bool m_Finished = true;

    /// <summary>
    /// 所有者
    /// </summary>
    protected VisualObject Owner;
    private float PlayInjuryTime;

    public AnimState(MecanimManager anims, VisualObject owner)
    {
        mAnimator = anims;
        Owner = owner;

    }
    /// <summary>
    /// 切换状态机状态
    /// </summary>
    /// <param name="statename"></param>
    /// <param name="layer"></param>
    protected void SwitchState(string statename,int layer)
    {
        SwitchState(mAnimator.Property.GetStateHash(statename), layer);
    }

    protected void SwitchState(int namehash, int layer)
    {
        mAnimator.Anim.SetInteger("state", namehash);
    }

    protected void SetTransition(string statename)
    {

      SetTransition( mAnimator.Property.GetStateHash(statename));

    }

    protected void SetTransition(int statehash)
    {
        mAnimator.Anim.SetInteger("state", statehash);
    }
    public virtual void HandlemAnimatorEvent(MecanimEvent animEvent)
    {
    }

    /// <summary>
    /// 处理新行为的接收
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual bool HandleNewAction(AnimAction action)
    {
        return false;
    }

    protected virtual void Initialize(AnimAction action)
    {
    }

    public virtual bool IsFinished()
    {
        return m_Finished;
    }

    protected bool IsGroundThere(Vector3 pos)
    {
        return Physics.Raycast(pos + Vector3.up, -Vector3.up, (float)5f, 0x4000);
    }
    public virtual void OnActivate(AnimAction action)
    {
        SetFinished(false);
        Initialize(action);
    }

    public virtual void OnDeactivate()
    {
    }

    public virtual void Release()
    {
        SetFinished(true);

    }

    public virtual void Reset()
    {
    }

    public virtual void SetFinished(bool finished)
    {
        this.m_Finished = finished;
    }
    public virtual void Update()
    {
    }

    public enum E_AnimEvent
    {
        Loop
    }
}

