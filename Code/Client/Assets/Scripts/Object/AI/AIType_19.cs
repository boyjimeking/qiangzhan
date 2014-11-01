
public class AIType_19 : CommonAI
{
    private float turnAngle = 10.0f;
    private float turnStartAngle = 0.0f;
    private float turnEndAngle = 360.0f;
    private bool turnDone = true;
    public AIType_19(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_19(battleUnit);
    }
    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        do
        {
            if (null != mRes.param1)
                turnAngle = (float)System.Convert.ToDouble(mRes.param1);
        } while (false);
        // ½âÎö²ÎÊý
        return true;
    }

    public override void OnEnterIdle()
    {
    }

    public override void OnUpdateIdle(uint elapsed)
    {
        Vector3f posn = BaseAI.GetPosition(GetID());

        if (turnStartAngle > turnEndAngle)
            turnStartAngle = 0.0f;
        Vector3f skillpos = new Vector3f();
        skillpos = BaseAI.get_position_angle_and_distance_position(posn, (float)(turnStartAngle * System.Math.PI / 180.0f), 8);
        BaseAI.UseSkillToPosition(GetID(), mSkill1, skillpos);
        turnStartAngle += turnAngle;
    }

    public override void OnExitIdle()
    {
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