
public class AIType_21 : CommonAI
{
    public AIType_21(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_21(battleUnit);
    }
    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        // ½âÎö²ÎÊý
        return true;
    }

    public override void OnEnterIdle()
    {
    }

    public override void OnExitIdle()
    {
    }

    public override void OnUpdateIdle(uint elapsed)
    {
        uint playerId = PlayerController.Instance.GetControl();
        Vector3f posn = BaseAI.GetPosition(GetID());

        float direction = BaseAI.get_direction(playerId,GetID());
        Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, direction, 3.5f);
        BaseAI.UseSkillToPosition(GetID(), mSkill1, guPos);
    }

    public override void OnEnterCombat()
    {
    }

    public override void OnExitCombat()
    {
    }

    public override void OnUpdateCombat(uint elapsed)
    {
		
    }

};