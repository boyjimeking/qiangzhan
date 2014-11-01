
public class AIType_25 : CommonAI
{
    private bool isRanpage = false;
    private float ratingAngle = 10.0f;
    private uint rageBuffId = 0;
    public AIType_25(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_25(battleUnit);
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
                ratingAngle = float.Parse(mRes.param1);
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
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill3) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill3));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));
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
                float beginAngle = BaseAI.get_direction(GetID(), mainTargetId) * 180;
                float endAngle = beginAngle + 360;
                Vector3f skillpos = new Vector3f();
                while (beginAngle < endAngle)
                {
                    skillpos = BaseAI.get_position_angle_and_distance_position(posn, (float)(beginAngle * System.Math.PI / 180.0f), 8);
                    AddCommand(new AIGoalUseSkillToPosition(this, mSkill5, skillpos));
                    beginAngle += ratingAngle;
                }

                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill6));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill7));

                for (int i = 0; i < 3; ++i)
                {
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill8));
                }

                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill9) * 0.95f));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill9));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill10));
            }
        }
    }
};