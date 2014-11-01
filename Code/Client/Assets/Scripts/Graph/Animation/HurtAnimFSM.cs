using FantasyEngine;
using System;

/// <summary>
/// 移动动画的状态机
/// </summary>
public class HurtAnimFSM : AnimFSM
{
    public HurtAnimFSM(MecanimManager anims, VisualObject owner)
        : base(anims, owner)
    {

    }

    public override void Initialize()
    {
        AnimStates.Add(AnimActionFactory.E_Type.Hurt, new AnimStateHurt(AnimEngine, Owner));

        DefaultAnimState = AnimStates[AnimActionFactory.E_Type.Hurt];
        AnimEngine.eventListener += HandleMecAnimEvent;
    }
}

