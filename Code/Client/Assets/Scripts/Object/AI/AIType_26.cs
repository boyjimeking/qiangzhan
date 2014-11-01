
public class AIType_26 : CommonAI
{
    private bool isRanpage = false;
    private uint rageBuffId = 0;
    private uint mCount = 0;
    public AIType_26(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_26(battleUnit);
    }
    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        do
        {
            {
                if (null == mRes.param1)
                    break;
                mCount = System.Convert.ToUInt32(mRes.param1);
            }

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
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill1) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill2) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
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
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill2) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill3));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));

                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill5) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill5));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill6));

                if (0 != mCount)
                {
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill7) * 0.95f));
                    for (int i = 0; i < mCount; ++i)
                    {
                        AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill7));
                    }
                }
            }
        }
    }
};