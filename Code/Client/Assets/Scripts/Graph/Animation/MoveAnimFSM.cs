using FantasyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 移动动画的状态机
/// </summary>
public class MoveAnimFSM : AnimFSM
{
    public MoveAnimFSM(MecanimManager anims, VisualObject owner)
        : base(anims,owner)
    {

    }

    public override void Initialize()
    {
        AnimStates.Add(AnimActionFactory.E_Type.Move, new AnimStateMove(AnimEngine, Owner));

        DefaultAnimState = AnimStates[AnimActionFactory.E_Type.Move];
        AnimEngine.eventListener += HandleMecAnimEvent;
    }
}

