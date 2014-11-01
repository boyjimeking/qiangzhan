using System;

/// <summary>
/// 动画行为
/// </summary>
public class AnimAction
{
    public E_State Status;
    public AnimActionFactory.E_Type Type;

    public AnimAction(AnimActionFactory.E_Type type)
    {
        Type = type;
    }

    public bool IsActive()
    {
        return (Status == E_State.E_ACTIVE);
    }

    public bool IsFailed()
    {
        return (Status == E_State.E_FAILED);
    }

    public bool IsSuccess()
    {
        return (Status == E_State.E_SUCCESS);
    }

    public bool IsUnused()
    {
        return (Status == E_State.E_UNUSED);
    }

    public virtual void Reset()
    {
    }

    public void SetActive()
    {
        Status = E_State.E_ACTIVE;
    }

    public void SetFailed()
    {
        Status = E_State.E_FAILED;
    }

    public void SetSuccess()
    {
        Status = E_State.E_SUCCESS;
    }

    public void SetUnused()
    {
        Status = E_State.E_UNUSED;
    }

    public enum E_State
    {
        E_ACTIVE,//激活
        E_SUCCESS,//成功
        E_FAILED,//失败
        E_UNUSED//未使用
    }
}

