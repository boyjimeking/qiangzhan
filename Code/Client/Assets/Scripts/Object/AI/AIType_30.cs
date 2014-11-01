
public class AIType_30 : CommonAI
{
    private bool isRanpage = false;
    private bool isRanpageAgain = false;
    private int[] ySkill = new int[5];
    private uint rageBuffId = 0;
    public AIType_30(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_30(battleUnit);
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
        isRanpageAgain = false;
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

        float radius = posn.Subtract(posm).Length();

        if (BaseAI.GetHpPercent(GetID()) > 0.7f)
        {
            if (BeginCommand(100))
            {
                if (radius < 4.0f)
                {
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill1) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                }
                else
                {
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill2) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                }
            }
        }
        else if (BaseAI.GetHpPercent(GetID()) > 0.4f)
        {
            if (!isRanpage)
            {
                BaseAI.LeaveFromaRange(GetID());
                isRanpage = !isRanpage;
                RemoveCommand();
                BaseAI.FlyIntoaRage(GetID());
            }
            if (BeginCommand(1000))
            {
                if (radius < 4.0f)
                {
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill1) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                }
                else
                {
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill3) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill3));

                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill5) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill5));

                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill2) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                }
            }
        }
        else
        {
            if (!isRanpageAgain)
            {
                BaseAI.UseSkillEffect(GetID(), SkillEffectType.Buff, rageBuffId);
                isRanpageAgain = !isRanpageAgain;
                RemoveCommand();
                BaseAI.FlyIntoaRage(GetID());
            }

            if (BeginCommand(10000))
            {
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill3) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill3));
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill4) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill4) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));

                posm = BaseAI.GetPosition(mainTargetId);
                posn = BaseAI.GetPosition(GetID());

                radius = posn.Subtract(posm).Length();
                if (radius < 3.0f)
                {
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill1) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                }
                else if (radius < 6.0f)
                {
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill6) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill6));
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill7) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill7));
                }
                else
                {
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill6) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill6));
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill7) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill7));

                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill3) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill3));
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill4) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill4) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));
                }
            }
        }
    }
};