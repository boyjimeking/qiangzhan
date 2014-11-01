
using System.Collections;
using System.Collections.Generic;

public class AIType_27 : CommonAI
{
    private float mMaxDistacne = 5;
    private List<int> mSkillList1 = new List<int>();

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_27(battleUnit);
    }
    public AIType_27(BattleUnit battleUnit)
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

        if (BeginCommand(100))
        {
            float radius = posn.Subtract(posm).Length();
            for (int i = 0; i < mSkillList1.Count; ++i)
            {
                if (radius > BaseAI.GetSkillMaxRangle(mSkillList1[i]))
                {
                    AddCommand(new AIGoalApproachTarget(this,mainTargetId,BaseAI.GetSkillMaxRangle(mSkillList1[i]) * 0.95f));
                    AddCommand(new AIGoalUseSkillToTarget(this,mainTargetId,mSkillList1[i]));
                }
            }
        }
    }
};