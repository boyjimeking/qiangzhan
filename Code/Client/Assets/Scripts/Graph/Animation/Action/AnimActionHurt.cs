using System;

public class AnimActionHurt : AnimAction
{
    public string AnimName;

    public AnimActionHurt()
        : base(AnimActionFactory.E_Type.Hurt)
    {
    }
}

