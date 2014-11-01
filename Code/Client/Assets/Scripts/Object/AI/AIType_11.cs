
public class AIType_11 : CommonAI
{
    public AIType_11(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_11(battleUnit);
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
            float radius = posm.Subtract(posn).Length();
            if (radius <= 2.0f) 
            {
                if (BaseAI.SceneMayStraightReach(posn, posm))
                {
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                    AddCommand(new AIGoalWait(this, 1000));
                }
                else
                {
                    for (int i = 30; i <= 360; i += 30)
                    {
                        Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, (float)i, 5.0f);
                        if (BaseAI.SceneMayStraightReach(posn, guPos))
                        {
                            AddCommand(new AIGoalMoveTo(this,guPos,BaseAI.MoveMode.MOVE_RUN));
                            break;
                        }
                    }
                }
            }
            else if (radius > 1.6f && radius <= 6.6f)
            {
                int radio = BaseAI.Random(1, 10);
                if (radio < 7)
                {
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill2, BaseAI.MoveMode.MOVE_RUN, 1500));
                }
                else
                {
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill1, BaseAI.MoveMode.MOVE_RUN, 1500));
                }
            }
            else
            {
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill1, BaseAI.MoveMode.MOVE_RUN, 1500));
            }
        }
    }
};