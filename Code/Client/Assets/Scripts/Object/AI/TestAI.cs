
public class TestAI : CommonAI
{
    private float mApproachMin = 0.0f;
    private float mApproachMax = 0.0f;
    private uint mLimitTime = 0;
    private int mUseSkillCount = 1;


    public TestAI(BattleUnit battleUnit) : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new TestAI(battleUnit);
    }
    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        mApproachMin = System.Convert.ToSingle(mRes.param2);
        mApproachMax = System.Convert.ToSingle(mRes.param3);
        mLimitTime = System.Convert.ToUInt32(mRes.param4);
        mUseSkillCount = System.Convert.ToInt32(mRes.param5);

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
        if (BeginCommand(1000))
        {
            System.Random rand = new System.Random();

            float distance = rand.Next((int)(mApproachMin * 100), (int)(mApproachMax * 100)) / 100.0f;
            AddCommand(new AIGoalApproachTargetLimit(this, GetCurrentTargetId(), distance, BaseAI.MoveMode.MOVE_RUN, mLimitTime));
            for (int i = 0; i < mUseSkillCount; i++)
            {
                AddCommand(new AIGoalTurnToTarget(this, GetCurrentTargetId()));
                AddCommand(new AIGoalUseSkillToTarget(this, GetCurrentTargetId(), GetSkillId(1)));
            }
        }
    }

};