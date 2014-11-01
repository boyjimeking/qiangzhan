using FantasyEngine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 动画的有限状态机
/// </summary>
public abstract class AnimFSM
{
    protected MecanimManager AnimEngine;
    protected Dictionary<AnimActionFactory.E_Type, AnimState> AnimStates = new Dictionary<AnimActionFactory.E_Type, AnimState>(0);
    protected AnimState DefaultAnimState;
    protected AnimState NextAnimState;
    protected VisualObject Owner;

    public AnimFSM(MecanimManager anims, VisualObject owner)
    {
        AnimEngine = anims;
        Owner = owner;
    }

    public virtual void Activate()
    {
        CurrentAnimState = DefaultAnimState;
        CurrentAnimState.OnActivate(null);
        NextAnimState = null;
    }

    public bool DoAction(AnimAction action)
    {
        if (CurrentAnimState.HandleNewAction(action))
        {
            /*
             *当前状态处理了该行为
             */
            NextAnimState = null;
            return true;
        }
        if (AnimStates.ContainsKey(action.Type))
        {
            /*
             * 切换到该行为对应的状态
             */
            NextAnimState = AnimStates[action.Type];
            SwitchToNewStage(action);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 初始化状态机
    /// </summary>
    public virtual void Initialize()
    {
    }

    public void Reset()
    {
        if (!CurrentAnimState.IsFinished())
        {
            CurrentAnimState.SetFinished(true);
            CurrentAnimState.Reset();
        }
    }

    protected void SwitchToNewStage(AnimAction action)
    {
        if (NextAnimState != null)
        {
            CurrentAnimState.Release();
            CurrentAnimState.OnDeactivate();
            CurrentAnimState = NextAnimState;
            CurrentAnimState.OnActivate(action);
            NextAnimState = null;
        }
    }

    public void HandleMecAnimEvent(MecanimEvent animEvent)
    {
        CurrentAnimState.HandlemAnimatorEvent(animEvent);
    }

    public void UpdateAnimStates()
    {

        AnimEngine.Update();

        if (CurrentAnimState.IsFinished())
        {
            CurrentAnimState.OnDeactivate();
            CurrentAnimState = DefaultAnimState;
            CurrentAnimState.OnActivate(null);
        }
        CurrentAnimState.Update();
    }

    public AnimState CurrentAnimState { get; private set; }
    public AnimState DefaultState
    {
        get
        {
            return DefaultAnimState;
        }
    }
}

