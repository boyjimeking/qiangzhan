
public class AIType_23 : CommonAI
{
    public int skillslot = 1;
    public AIType_23(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_23(battleUnit);
    }
    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        // ½âÎö²ÎÊı
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

        float radius = posm.Subtract(posn).Length();

        if (BeginCommand(100))
        {
            if (radius > 5.0f)
            {
                if (radius < BaseAI.GetSkillMaxRangle(mSkill1))
                {
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                }
                else
                {
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill1) * 0.95f));
                    return;
                }
            }

            AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill2) * 0.90f));
            AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill2, BaseAI.MoveMode.MOVE_RUN, 5000));
            AddCommand(new AIGoalUseSkillToTarget(this,mainTargetId,mSkill3));
        }
    }
};