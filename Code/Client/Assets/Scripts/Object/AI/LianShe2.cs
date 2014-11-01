
public class LianShe2 : CommonAI
{
    public LianShe2(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new LianShe2(battleUnit);
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


        if (BeginCommand(100)) 
        {
            posm.y = 0.0f;
            posn.y = 0.0f;

            float radius = posm.Subtract(posn).Length();

            if (radius < 4.0f)
            {
                float newPos = (float)System.Math.Atan2(posn.z - posm.z, posn.x - posm.x);
                float curAngle = (float)(newPos * 180 / System.Math.PI);  //角色当前角度

                Vector3f mGunPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle + 180, 5);
                if (BaseAI.SceneMayStraightReach(posn, mGunPos))
                {
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                    AddCommand(new AIGoalMoveTo(this, mGunPos, BaseAI.MoveMode.MOVE_SCURRY));
                }
                else
                {
                    for (int i = 30; i <= 360; i += 30)
                    {
                        mGunPos = BaseAI.get_position_angle_and_distance_position(posn, (float)i, 5.0f);
                        if (scene_postion_arrive(posn, mGunPos))
                        {
                            AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                            AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                            AddCommand(new AIGoalMoveTo(this, mGunPos, BaseAI.MoveMode.MOVE_SCURRY));
                            break;
                        }
                    }
                }
            }
            else if (radius < BaseAI.GetSkillMaxRangle(mSkill2) * 0.9f)
            {
                if (BaseAI.SceneMayStraightReach(posn, posm))
                {
                    AddCommand(new AIGoalUseSkillToPosition(this, mSkill2, posm));
                    AddCommand(new AIGoalUseSkillToPosition(this, mSkill2, posm));
                    AddCommand(new AIGoalUseSkillToPosition(this, mSkill2, posm));
                    AddCommand(new AIGoalWait(this, 500));

                    float curAngle = BaseAI.CalcDirection(posn, posm);
                    int ranAngle1 = BaseAI.Random(-90, -60);
                    int ranAngle2 = BaseAI.Random(60, 90);
                    int ranAngle = BaseAI.Random(1, 3) == 1 ? ranAngle1 : ranAngle2;
                    float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                    curAngle = curAngle + angleRadius;
                    float randRadius = BaseAI.Randomf(2, 3);
                    Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);
                    AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                }
                else
                {
                    for (int i = 30; i <= 360; i += 30)
                    {
                        Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, (float)i, 5.0f);
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
                float curAngle = BaseAI.CalcDirection(posn, posm);
                int ranAngle = BaseAI.Random(-45, 45);
                float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                curAngle = curAngle + angleRadius;
                float randRadius = BaseAI.Randomf(2, 3);
                Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);
                AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                AddCommand(new AIGoalApproachTargetLimit(this,mainTargetId,BaseAI.GetSkillMaxRangle(mSkill2) * 0.8f,BaseAI.MoveMode.MOVE_RUN,1500));
            }
        }
    }
};