using System;
using UnityEngine;

public class AnimActionDeath : AnimAction
{
    public string dieAnim;

    public AnimActionDeath() : base(AnimActionFactory.E_Type.Death)
    {
    }
}

