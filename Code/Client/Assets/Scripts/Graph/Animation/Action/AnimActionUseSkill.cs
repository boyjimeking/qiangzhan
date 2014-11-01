using System;

public class AnimActionUseSkill : AnimAction
{
    public string AnimName;
    public bool loop = false;
    public AnimActionUseSkill()
        : base(AnimActionFactory.E_Type.UseSkill)
    {
    }
}

