using System.Collections;
using System.Collections.Generic;


public class AIType_35 : CommonAI
{
    private float ratingAngle1 = 10.0f;
    private float ratingAngle2 = 10.0f;
    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_35(battleUnit);
    }
    public AIType_35(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;
        do
        {
            {
                if (null == mRes.param1)
                    break;
                ratingAngle1 = float.Parse(mRes.param1);
            }

            {
                if (null == mRes.param2)
                    break;
                ratingAngle2 = float.Parse(mRes.param2);
            }
        } while (false);
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
        Vector3f posm = BaseAI.GetPosition(mainTargetId);
        Vector3f posn = BaseAI.GetPosition(GetID());

        if (BeginCommand(100))
        {
            float beginAngle = (float)(BaseAI.CalcDirection(posn, posm) * 180.0f / System.Math.PI)  - 70;
            float endAngle = beginAngle + 140;
            Vector3f skillpos = new Vector3f();
            while (beginAngle < endAngle)
            {
                skillpos = BaseAI.get_position_angle_and_distance_position(posn, (float)(beginAngle * System.Math.PI / 180.0f), 2);
                AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, skillpos));
                beginAngle += ratingAngle1;
            }

            posm = BaseAI.GetPosition(mainTargetId);
            posn = BaseAI.GetPosition(GetID());
            beginAngle = (float)(BaseAI.CalcDirection(posn, posm) * 180.0f / System.Math.PI) + 70;
            endAngle = beginAngle - 140;
            skillpos = new Vector3f();
            while (beginAngle > endAngle)
            {
                skillpos = BaseAI.get_position_angle_and_distance_position(posn, (float)(beginAngle * System.Math.PI / 180.0f), 2);
                AddCommand(new AIGoalUseSkillToPosition(this, mSkill2, skillpos));
                beginAngle -= ratingAngle2;
            }
        }
    }

};