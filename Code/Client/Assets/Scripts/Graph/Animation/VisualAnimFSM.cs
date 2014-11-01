using FantasyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 可见物体的动画状态机
/// </summary>
public class VisualAnimFSM : AnimFSM
{

    public VisualAnimFSM(MecanimManager anims, VisualObject owner)
        : base(anims,owner)
    {

    }
    public override void Initialize()
    {
        /*
         * 初始化属于可见物体的状态机
         */
        AnimStates.Add(AnimActionFactory.E_Type.Idle, new AnimStateIdle(AnimEngine, Owner));
        AnimStates.Add(AnimActionFactory.E_Type.PlayAnim, new AnimStatePlayAnim(AnimEngine, Owner));
        AnimStates.Add(AnimActionFactory.E_Type.Move, new AnimStateMove(AnimEngine, Owner));
        AnimStates.Add(AnimActionFactory.E_Type.UseSkill, new AnimStateUseSkill(AnimEngine, Owner));
        AnimStates.Add(AnimActionFactory.E_Type.Death, new AnimStateDeath(AnimEngine, Owner));


        //设置默认的动画状态
        DefaultAnimState = AnimStates[AnimActionFactory.E_Type.Idle];

        if (AnimEngine == null)
            return;

        AnimEngine.eventListener += HandleMecAnimEvent;

    }
}

