
using System.Collections;
using System.Collections.Generic;

public class AIType_29 : CommonAI
{
    private Vector3f tarPos = new Vector3f(0,0,0);
    bool isArrived = false;
    private List<int> mSkillList1 = new List<int>();

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_29(battleUnit);
    }
    public AIType_29(BattleUnit battleUnit)
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
                if (null == mRes.param1)
                    break;
                string[] arr = mRes.param1.Split(',');
                if (arr.Length < 3)
                    break;

                tarPos = new Vector3f(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
            }

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
        if (!isArrived)
        {
            
        }
        else
        {
            if (BeginCommand(100))
            {
                for (int i = 0; i < mSkillList1.Count; ++i)
                {
                    AddCommand(new AIGoalApproachTarget(this,mainTargetId,BaseAI.GetSkillMaxRangle(mSkillList1[i]) * 0.95f));

                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId , mSkillList1[i]));
                }
            }
        }
    }
};