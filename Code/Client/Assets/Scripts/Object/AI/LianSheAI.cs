using UnityEngine;
using System.Collections;

public class LianSheAI : CommonAI
{

    public LianSheAI(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new LianSheAI(battleUnit);
    }
    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        // 解析参数
        return true;
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

        System.Random rand = new System.Random();

        if (BeginCommand(100))
        {
            posm.y = 0.0f;
            posn.y = 0.0f;
            float radius = posm.Subtract(posn).Length();
            if (radius < 4)
            {
                double newPos = System.Math.Atan2(posn.z - posm.z, posn.x - posm.x);
                double curAngle = newPos * 180 / System.Math.PI;

                Vector3f mGunPos = BaseAI.get_position_angle_and_distance_position(posn, (float)curAngle + 180.0f, 5);
                if (BaseAI.SceneMayStraightReach(posn, mGunPos))
                {
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, 1));
                    AddCommand(new AIGoalMoveTo(this, mGunPos, BaseAI.MoveMode.MOVE_RUN));
                }
                else
                {
                    Vector3f skillPos;
                    for (int i = 30; i < 360; i += 30)
                    {

                        skillPos = BaseAI.get_position_angle_and_distance_position(mGunPos, i, 5);
                        if (BaseAI.SceneMayStraightReach(mGunPos, skillPos))
                        {
                            AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                            AddCommand(new AIGoalMoveTo(this, mGunPos, BaseAI.MoveMode.MOVE_RUN));
                            break;
                        }
                    }
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, 2, BaseAI.MoveMode.MOVE_RUN, 1500));
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, 2, BaseAI.MoveMode.MOVE_RUN, 1500));
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, 2, BaseAI.MoveMode.MOVE_RUN, 1500));
                }
            }
            else
            {
                int randomp = BaseAI.Random(1, 10);
                if (randomp <= 6)
                {
                    AddCommand(new AIGoalRandMove(this, posn, 2, 3, -180, 180));
                }
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, 2, BaseAI.MoveMode.MOVE_RUN, 1500));
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, 2, BaseAI.MoveMode.MOVE_RUN, 1500));
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, 2, BaseAI.MoveMode.MOVE_RUN, 1500));
            }
        }
    }
}
