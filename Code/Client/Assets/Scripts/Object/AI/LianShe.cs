
public class LianShe : CommonAI
{
    public int skillslot = 1;
    public LianShe(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new LianShe(battleUnit);
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
        skillslot = 1;
    }

    public override void OnExitCombat()
    {
    }

    public override void OnUpdateCombat(uint elapsed)
    {
		uint mainTargetId = GetCurrentTargetId(); 
		
		Vector3f posm = BaseAI.GetPosition(mainTargetId);
		Vector3f posn = BaseAI.GetPosition(GetID());

        posm.y = 0.0f;
        posn.y = 0.0f;

        float radius = posm.Subtract(posn).Length();

        if (radius >= 20.0f)
        {
            if (BeginCommand(1000))
            {
                for (skillslot = 1; skillslot <= 4; ++skillslot)
                {
                    if (skillslot == 4)
                    {
                        AddCommand(new AIGoalUseSkillToTargetRangeTime(this,mainTargetId,mSkill2,BaseAI.MoveMode.MOVE_RUN,1500));
                    }
                    else
                    {
                        AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill1, BaseAI.MoveMode.MOVE_RUN, 1500));
                    }
                }
            }
        }

        if (radius >= 0.0f && radius < 20.0f)
        {
            if (BeginCommand(1000))
            {
                if (skillslot == 4)
                {
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill2, BaseAI.MoveMode.MOVE_RUN, 1500));
                }
                else
                {
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill1, BaseAI.MoveMode.MOVE_RUN, 1500));
                }
            }
        }
    }
};