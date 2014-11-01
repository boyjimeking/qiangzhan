
public class AIType_9 : CommonAI
{
    public AIType_9(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_9(battleUnit);
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
		
        System.Random rand = new System.Random();

		if(BeginCommand(100))
        {
		    posm.y = 0.0f;
            posn.y = 0.0f;

			float radius = posm.Subtract(posn).Length();

            if (radius >= 0.0f && radius <= BaseAI.GetSkillMaxRangle(mSkill1) + 2.0f) 
		    {
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this,mainTargetId,mSkill1,BaseAI.MoveMode.MOVE_RUN,1500));
            }
			else if(radius > BaseAI.GetSkillMaxRangle(mSkill1) + 2.0f)
            {
				float curAngle = BaseAI.get_direction(GetID(), mainTargetId);
											
				int ranAngle = BaseAI.Random(-45, 45);
				float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
				curAngle = curAngle + angleRadius;					
				int randRadius = BaseAI.Random(2, 4);
						
				Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);
				
                AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                AddCommand(new AIGoalApproachTargetLimit(this,mainTargetId,BaseAI.GetSkillMaxRangle(mSkill1),BaseAI.MoveMode.MOVE_RUN,1500));
            }
        }
    }

};