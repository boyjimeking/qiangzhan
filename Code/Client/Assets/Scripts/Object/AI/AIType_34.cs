using System.Collections;
using System.Collections.Generic;


public class AIType_34 : CommonAI
{
    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_34(battleUnit);
    }
    public AIType_34(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        // ½âÎö²ÎÊý
        return true;
    }

    public override void OnEnterIdle()
    {
    }

    public override void OnExitIdle()
    {
    }

    public override void OnUpdateIdle(uint elapsed)
    {
       
    }

    public override void OnEnterCombat()
    {
    }

    public override void OnExitCombat()
    {
    }

    public override void OnUpdateCombat(uint elapsed)
    {


        uint mainTargetId = GetCurrentTargetId();
        Vector3f posn = BaseAI.GetPosition(GetID());
        Vector3f posm = BaseAI.GetPosition(mainTargetId);
        float curAngle = BaseAI.CalcDirection(posm, posn);

        if (BeginCommand(100))
        {
            Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle - 40, 2.0f);
            AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, guPos));
            guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle + 40, 2);
            AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, guPos));
        }
    }

};