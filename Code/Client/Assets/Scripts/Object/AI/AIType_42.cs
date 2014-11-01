
using System.Collections;
using System.Collections.Generic;

public class AIType_42 : CommonAI
{
    private List<int> mSkillList1 = new List<int>();
    private bool isRanpage = false;
    private uint rageBuffId = 600;
    private int coutskill3 = 30;

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_42(battleUnit);
    }
    public AIType_42(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        do
        {
            {
                int skillId = mSkill1;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);
            }

            {
                int skillId = mSkill2;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);

            }


            {
                int skillId = mSkill3;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);

            }


            {
                int skillId = mSkill4;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);

            }


            {
                int skillId = mSkill5;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);

            }


            {
                int skillId = mSkill6;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);

            }

            {
                int skillId = mSkill7;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);

            }

            {
                int skillId = mSkill8;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);

            }

            {
                int skillId = mSkill9;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);

            }

            {
                int skillId = mSkill10;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);
            }

            {
                int skillId = mSkill11;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);
            }

            {
                int skillId = mSkill12;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);
            }

            {
                int skillId = mSkill13;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);
            }

            {
                int skillId = mSkill14;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);
            }

            {
                int skillId = mSkill15;

                if (skillId < 0)
                    break;

                mSkillList1.Add(skillId);
            }
        } while (false);

        do
        {
            {
                if (null == mRes.param20)
                    break;
                rageBuffId = System.Convert.ToUInt32(mRes.param20);
            }

            {
                if (null == mRes.param1)
                    break;
                coutskill3 = System.Convert.ToInt32(mRes.param1);
            }
        } while (false);
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

        if (BaseAI.GetHpPercent(GetID()) > 0.5f)
        {
            if (BeginCommand(100))
            {
                if (BaseAI.CheckSkillCd(GetID(), mSkill4))
                {
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill4, BaseAI.MoveMode.MOVE_RUN, 5000));
                }
                else
                {
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill1, BaseAI.MoveMode.MOVE_RUN, 50000));
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill2, BaseAI.MoveMode.MOVE_RUN, 50000));
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill3, BaseAI.MoveMode.MOVE_RUN, 50000));
                }
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
                for (int i = 4; i < 10; ++i)
                {
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkillList1[i], BaseAI.MoveMode.MOVE_RUN, 50000));
                }
                for (int i = 9; i >= 4; --i)
                {
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkillList1[i], BaseAI.MoveMode.MOVE_RUN, 50000));
                }

                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill12, BaseAI.MoveMode.MOVE_RUN, 50000));
                for (int i = 0; i < coutskill3; ++i )
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill11, BaseAI.MoveMode.MOVE_RUN, 50000));
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill12, BaseAI.MoveMode.MOVE_RUN, 50000));
                for (int i = 0; i < 3; ++i)
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill13, BaseAI.MoveMode.MOVE_RUN, 50000));
            }
        }
    }
};