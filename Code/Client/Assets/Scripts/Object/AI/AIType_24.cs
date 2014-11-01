
public class AIType_24 : CommonAI
{
    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_24(battleUnit);
    }
    public AIType_24(BattleUnit battleUnit)
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
            float radius = (float)System.Math.Sqrt((posm.x - posn.x) * (posm.x - posn.x) + (posm.z - posn.z) * (posm.z - posn.z));
            if (radius < 4.5f)
            {
                float curAngle = BaseAI.CalcDirection(posn, posm);
                int ranAngle1 = BaseAI.Random(-90, -60);
                int ranAngle2 = BaseAI.Random(60, 90);
                int ranAngle = BaseAI.Random(1, 3) == 1 ? ranAngle1 : ranAngle2;
                float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                curAngle = curAngle + angleRadius;
                float randRadius = BaseAI.Randomf(3, 4);
                Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);
                AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                if (BaseAI.SceneMayStraightReach(posn, posm))
                {
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                }
                else
                {
                    for (int i = 30; i <= 360; i += 30)
                    {
                        guPos = BaseAI.get_position_angle_and_distance_position(posn, (float)i, 5.0f);
                        if (BaseAI.SceneMayStraightReach(posn, guPos))
                        {
                            AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                            break;
                        }
                    }
                }
            }
            else
            {
                if (radius < BaseAI.GetSkillMaxRangle(mSkill2))
                    AddCommand(new AIGoalUseSkillToTarget(this,mainTargetId,mSkill2));
                float curAngle = BaseAI.CalcDirection(posn, posm);
                int ranAngle = BaseAI.Random(-45, 45);
                float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                curAngle = curAngle + angleRadius;
                float randRadius = BaseAI.Randomf(3, 4);
                Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);
                AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
            }
        }
    }
}