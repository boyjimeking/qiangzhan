
public class QuanPao : CommonAI
{
    public QuanPao(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new QuanPao(battleUnit);
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
            AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill1, BaseAI.MoveMode.MOVE_RUN, 1000));
            AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill2, BaseAI.MoveMode.MOVE_RUN, 1000));
        }
    }

};