
public class AIType_14 : CommonAI
{
    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_14(battleUnit);
    }
    public AIType_14(BattleUnit battleUnit)
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

		if(BeginCommand(1000))
        {
            posm.y = 0.0f;
            posn.y = 0.0f;

            float radius = posm.Subtract(posn).Length();

            if (radius >= 0.0f && radius <= BaseAI.GetSkillMaxRangle(mSkill1) * 0.9f)
            {
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill1, BaseAI.MoveMode.MOVE_RUN, 1500));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                AddCommand(new AIGoalWait(this, 500));
                

                int radio = BaseAI.Random(1, 10);
                if (radio <= 5)
                {
                    float curAngle = BaseAI.CalcDirection(posn, posm);

                    int ranAngle = BaseAI.Random(-45, 45);
                    float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                    curAngle = curAngle + angleRadius;
                    float randRadius = BaseAI.Randomf(2, 3);

                    Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);

                    AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                }
            }
            else if (radius > BaseAI.GetSkillMaxRangle(mSkill1) * 0.9f)
            {
                float curAngle = BaseAI.CalcDirection(posn, posm);

                int ranAngle = BaseAI.Random(-45, 45);
                float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                curAngle = curAngle + angleRadius;
                float randRadius = BaseAI.Randomf(2, 3);

                Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);

                AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                AddCommand(new AIGoalApproachTargetLimit(this,mainTargetId,BaseAI.GetSkillMaxRangle(mSkill1) * 0.8f,BaseAI.MoveMode.MOVE_RUN,1500));
            }
        }
    }
};