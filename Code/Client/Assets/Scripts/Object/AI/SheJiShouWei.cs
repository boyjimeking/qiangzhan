
public class SheJiShouWei : CommonAI
{
    private bool isRampage = false;
    private uint rageBuffId = 0;
    public SheJiShouWei(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new SheJiShouWei(battleUnit);
    }
    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        do
        {
            {
                if (null == mRes.param20)
                    break;
                rageBuffId = System.Convert.ToUInt32(mRes.param20);
            }
        } while (false);
        // Ω‚Œˆ≤Œ ˝
        return true;
    }
    public override void OnEnterCombat()
    {
        isRampage = false;
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

        if (BaseAI.GetHpPercent(GetID()) > 0.5f)
        {
            if (BeginCommand(100))
            {
                //¡¨…‰
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill1) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                AddCommand(new AIGoalStand(this, 1000, 1000));
                //…¢…‰
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill2) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                AddCommand(new AIGoalStand(this, 1000, 1000));
                   
            }
        }
        else
        {
            if (!isRampage)
            {
                BaseAI.UseSkillEffect(GetID(), SkillEffectType.Buff, rageBuffId);
                RemoveCommand();
                BaseAI.FlyIntoaRage(GetID());
                isRampage = true;
            }
            if (BeginCommand(1000))
            {
                //º§π‚
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill3) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill3));

                //∫·œÚ∑‚À¯-∫‰’®
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill4) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));
                AddCommand(new AIGoalWait(this,200));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));
                AddCommand(new AIGoalWait(this, 200));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));
                AddCommand(new AIGoalWait(this, 200));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));
                AddCommand(new AIGoalStand(this, 1000, 1000));

                //øÒ±©∑‚À¯
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill6) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill6));
                AddCommand(new AIGoalStand(this, 1000, 1000));
                //øÒ±©…¢…‰
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill7) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill7));
                AddCommand(new AIGoalStand(this, 2000, 2000));
            }
        }
    }
};