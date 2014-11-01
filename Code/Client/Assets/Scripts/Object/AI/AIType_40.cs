
using System.Collections;
using System.Collections.Generic;

public class AIType_40 : CommonAI
{
    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_40(battleUnit);
    }

    public AIType_40(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        // 解析参数
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

	    if(BeginCommand(100))
        {
            float radius = (float)System.Math.Sqrt((posm.x - posn.x) * (posm.x - posn.x) + (posm.z - posn.z) * (posm.z - posn.z));
            if (radius >= 4)
            {
                int randomselect = BaseAI.Random(1, 11);
                if (randomselect < 5)
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill1, BaseAI.MoveMode.MOVE_RUN,5000));
                else
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill2, BaseAI.MoveMode.MOVE_RUN, 5000));
                float newPos = (float)System.Math.Atan2(posn.z - posm.z, posn.x - posm.x);
                float curAngle = (float)(newPos * 180 / System.Math.PI);  //角色当前角度

                Vector3f mGunPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle + 180, 5);

                if (scene_postion_arrive(posn, mGunPos))
                {
                    AddCommand(new AIGoalUseSkillToPosition(this, mSkill4, mGunPos));
                }
                else
                {
                    for (int i = 30; i < 360; i += 30)
                    {
                        Vector3f skillPos = BaseAI.get_position_angle_and_distance_position(posn, i, 5);
                        if (scene_may_straight_reach(posn, skillPos))
                        {
                            AddCommand(new AIGoalUseSkillToPosition(this, mSkill4, mGunPos));
                            break;
                        }
                    }

                }
            }
            else
            {
                int randomselect = BaseAI.Random(1, 11);
                if (randomselect < 4)
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill3, BaseAI.MoveMode.MOVE_RUN, 5000));

                float newPos = (float)System.Math.Atan2(posn.z - posm.z, posn.x - posm.x);
                float curAngle = (float)(newPos * 180 / System.Math.PI);  //角色当前角度

                Vector3f mGunPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle + 180, 5);

                if (scene_postion_arrive(posn, mGunPos))
                {
                    AddCommand(new AIGoalUseSkillToPosition(this, mSkill4, mGunPos));
                }
                else
                {
                    for (int i = 30; i < 360; i += 30)
                    {
                        Vector3f skillPos = BaseAI.get_position_angle_and_distance_position(posn, i, 5);
                        if (scene_may_straight_reach(posn, skillPos))
                        {
                            AddCommand(new AIGoalUseSkillToPosition(this, mSkill4, mGunPos));
                            break;
                        }
                    }

                }
            }
	    }
    }

};