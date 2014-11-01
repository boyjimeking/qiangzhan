
public class QuanDaZhuang : CommonAI
{
    private bool isRanpage = false;
    private int[] ySkill = new int[5];
    private uint rageBuffId = 0;
    public QuanDaZhuang(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new QuanDaZhuang(battleUnit);
    }
    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;
        ySkill[1] = mRes.skillslot1;
        ySkill[2] = mRes.skillslot2;
        ySkill[3] = mRes.skillslot3;
        ySkill[4] = mRes.skillslot4;

        do
        {
            {
                if (null == mRes.param20)
                    break;
                rageBuffId = System.Convert.ToUInt32(mRes.param20);
            }
        } while (false);
        // ½âÎö²ÎÊý
        return true;
    }
    public override void OnEnterCombat()
    {
        isRanpage = false;
    }

    public override void OnExitCombat()
    {
        BaseAI.LeaveFromaRange(GetID());
    }

    public override void OnUpdateCombat(uint elapsed)
    {
        uint mainTargetId = GetCurrentTargetId();

        Vector3f posm = BaseAI.GetPosition(mainTargetId);
        Vector3f posn = BaseAI.GetPosition(GetID());

        posm.y = 0.0f;
        posn.y = 0.0f;

        if (BaseAI.GetHpPercent(GetID()) > 0.5f)
        {
            if (BeginCommand(100))
            {
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill1, BaseAI.MoveMode.MOVE_RUN, 5000));
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill3, BaseAI.MoveMode.MOVE_RUN, 5000));
            }

        }
        else
        {
            if (!isRanpage)
            {
                BaseAI.UseSkillEffect(GetID(), SkillEffectType.Buff, rageBuffId);
                isRanpage = !isRanpage;
                RemoveCommand();
                BaseAI.FlyIntoaRage(GetID());
            }
            if (BeginCommand(1000))
            {
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill5));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill5));

                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill2) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
            }
        }
    }
};