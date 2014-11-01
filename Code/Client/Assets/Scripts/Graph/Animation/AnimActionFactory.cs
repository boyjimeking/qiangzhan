using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动作行为工厂
/// </summary>
public static class AnimActionFactory
{
    private static Queue<AnimAction>[] m_UnusedActions = new Queue<AnimAction>[(int)E_Type.Count];

    static AnimActionFactory()
    {
        for (E_Type type = E_Type.Idle; type < E_Type.Count; type += 1)
        {
            m_UnusedActions[(int) type] = new Queue<AnimAction>();
        }
    }

    public static void Clear()
    {
        for (E_Type type = E_Type.Idle; type < E_Type.Count; type += 1)
        {
            m_UnusedActions[(int) type].Clear();
        }
    }

    public static AnimAction Create(E_Type type)
    {
        AnimAction action;
        int index = (int) type;
        if (m_UnusedActions[index].Count > 0)
        {
            action = m_UnusedActions[index].Dequeue();
        }
        else
        {
            switch (type)
            {
                case E_Type.Idle:
                    action = new AnimActionIdle();
                    break;
                case E_Type.PlayAnim:
                      action = new AnimActionPlayAnim();
                    break;
                case E_Type.Move:
                    action = new AnimActionMove();
                    break;
                case E_Type.UseSkill:
                    action = new AnimActionUseSkill();
                    break;
                case E_Type.Death:
                    action = new AnimActionDeath();
                    break;
                case E_Type.Hurt:
                    action = new AnimActionHurt();
                    break;
                default:
                    Debug.LogError("no AgentAction to create");
                    return null;

            }

        }
        action.Reset();
        action.SetActive();
        return action;
    }

    public static void Return(AnimAction action)
    {
        action.SetUnused();
        m_UnusedActions[(int) action.Type].Enqueue(action);
    }

    public enum E_Type
    {
        Idle,  //休闲
        Move,//移动
        PlayAnim,
        PlayIdleAnim,
        UseSkill,//使用技能
        Death,//死亡
        Hurt,//受伤害

        Count
    }
}

