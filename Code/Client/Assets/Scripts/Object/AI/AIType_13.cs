
public class AIType_13 : CommonAI
{
    private int mRandId = BaseAI.Random(1, 3);
    private bool usingSkill = true;

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_13(battleUnit);
    }
    public AIType_13(BattleUnit battleUnit)
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
        usingSkill = true;
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
            if (!usingSkill) 
            {
                mRandId = BaseAI.Random(1, 3);
            }
            posm.y = 0.0f;
            posn.y = 0.0f;

            float radius = posm.Subtract(posn).Length();

            if (radius >= 0.0f && radius <= BaseAI.GetSkillMaxRangle(GetSkillId(mRandId)) * 0.9f)
            {
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

                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, GetSkillId(mRandId), BaseAI.MoveMode.MOVE_RUN, 1500));
                AddCommand(new AIGoalWait(this, 500));
                usingSkill = false;
            }
            else if (radius > BaseAI.GetSkillMaxRangle(GetSkillId(mRandId)) * 0.9f)
            {
                usingSkill = true;
                float curAngle = BaseAI.CalcDirection(posn, posm);

                int ranAngle = BaseAI.Random(-45, 45);
                float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                curAngle = curAngle + angleRadius;
                float randRadius = BaseAI.Randomf(2, 3);

                Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);

                AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                AddCommand(new AIGoalApproachTargetLimit(this,mainTargetId,BaseAI.GetSkillMaxRangle(GetSkillId(mRandId)) * 0.8f,BaseAI.MoveMode.MOVE_RUN,1500));
            }
        }
    }
};