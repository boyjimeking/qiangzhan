
public class SanShe : CommonAI
{
    public SanShe(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new SanShe(battleUnit);
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

        if (BeginCommand(10000))
        {
            float radius = posm.Subtract(posn).Length();
            float curAngle = BaseAI.CalcDirection(posn, posm);
            int ranAngle = 0;
            if (radius <= 3.0f)
            {
                ranAngle = BaseAI.Random(120, 240);
                float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                curAngle = curAngle + angleRadius;
                float randRadius = BaseAI.Randomf(2, 3);
                Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);
                if (BaseAI.SceneMayStraightReach(posn, guPos))
                {
                    AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, guPos));
                }
                else
                {
                    for (int i = 30; i <= 360; i += 30)
                    {
                        guPos = BaseAI.get_position_angle_and_distance_position(posn, (float)i, 5.0f);
                        if (BaseAI.SceneMayStraightReach(posn, guPos))
                        {
                            AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, guPos));
                            break;
                        }
                    }
                }
                AddCommand(new AIGoalUseSkillToPosition(this, mSkill2, posm));
            }
            else if (radius > 3.0f && radius <= 8.0f)
            {
                ranAngle = BaseAI.Random(1, 3) == 1 ? BaseAI.Random(-60, -30) : BaseAI.Random(30, 60);
                float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                curAngle = curAngle + angleRadius;
                float randRadius = BaseAI.Randomf(2, 3);
                Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);
                if (BaseAI.SceneMayStraightReach(posn, guPos))
                {
                    AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, guPos));
                }
                else
                {
                    for (int i = 30; i <= 360; i += 30)
                    {
                        guPos = BaseAI.get_position_angle_and_distance_position(posn, (float)i, 5.0f);
                        if (BaseAI.SceneMayStraightReach(posn, guPos))
                        {
                            AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, guPos));
                            break;
                        }
                    }
                }
                AddCommand(new AIGoalUseSkillToPosition(this, mSkill2, posm));     
            }
            else
            {
                if (radius <= BaseAI.GetSkillMaxRangle(mSkill2) * 0.95f)
                {
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                }
                ranAngle = BaseAI.Random(-15, 15);
                float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                curAngle = curAngle + angleRadius;
                float randRadius = BaseAI.Randomf(2, 3);
                Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);
                AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, guPos));
            }
        }
    }

};