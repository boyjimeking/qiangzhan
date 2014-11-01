
public class PaoDaZhuang : CommonAI
{
    private int staicskillgroup1 = 1;
    private int staicskillgroup2 = 2;
    private int staicskillgroup;
    private float saoshe_2_beginangle = 0;
    private float saoshe_2_endangle = 0;
    private float saoshe_2_zhengudu = 16;
    private float curangle_2 = 0;
    private bool saoshe_ing = false;
    private int static_1 = 1;
    private int static_2 = 2;
    private int static_3 = 3;
    private int static_4 = 4;
    private int _static;
    private int countbegin = 0;
    private int countend = 5;
    private int count;
    private bool isRange = false;
    private uint rageBuffId = 0;

    public PaoDaZhuang(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new PaoDaZhuang(battleUnit);
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
        // ½âÎö²ÎÊý
        return true;
    }
    public override void OnEnterCombat()
    {
        isRange = false;
        staicskillgroup = staicskillgroup1;
        _static = static_3;
        count = countbegin;
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

        System.Random rand = new System.Random();

        if (BaseAI.GetHpPercent(GetID()) > 0.5f)
        {
            if (BeginCommand(100))
            {
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill1, BaseAI.MoveMode.MOVE_RUN, 5000));
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill2, BaseAI.MoveMode.MOVE_RUN, 5000));
            }
        }
        else
        {
            if (!isRange)
            {
                BaseAI.UseSkillEffect(GetID(), SkillEffectType.Buff, rageBuffId);
                isRange = !isRange;
                RemoveCommand();
                BaseAI.FlyIntoaRage(GetID());
            }

            if (BeginCommand(1000))
            {
                if (staicskillgroup == staicskillgroup1)
                {
                    if (_static == static_1)
                    {
                        if (count >= countend)
                        {
                            count = countbegin;
                            countend = BaseAI.Random(2, 3);
                            _static = (BaseAI.Random(3, 5) == 3 ? static_3 : static_4);
                            staicskillgroup = staicskillgroup2;
                            return;
                        }
                        else
                        {
                            AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, posm));
                            count += 1;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 10; ++i)
                        {
                            AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill3));
                        }
                        _static = (BaseAI.Random(3, 5) == 3 ? static_3 : static_4);
                        staicskillgroup = staicskillgroup2;
                    }
                }
                else
                {
                    /*if (_static == static_3)
                    {
                        AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill3) * 0.95f));
                        for (int i = 0; i < 5; ++i)
                        {
                            AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));
                        }
                        _static = (BaseAI.Random(1, 3) == 1 ? static_1 : static_2);
                        staicskillgroup = staicskillgroup1;
                    }
                    else
                    {*/
                    AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill2) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                    _static = (BaseAI.Random(1, 3) == 1 ? static_1 : static_2);
                    staicskillgroup = staicskillgroup1;
                    //}
                }
            }
        }
    }
};