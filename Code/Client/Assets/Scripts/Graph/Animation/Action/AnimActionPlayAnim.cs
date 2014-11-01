using System;

public class AnimActionPlayAnim : AnimAction
{
    public string AnimName;

    public AnimActionPlayAnim()
        : base(AnimActionFactory.E_Type.PlayAnim)
    {
    }
}

