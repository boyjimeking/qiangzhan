
using System.Collections;
using System.Collections.Generic;

public class AIType_41 : CommonAI
{
    private List<int> mSkillList1 = new List<int>();
    private List<int> mParamList1 = new List<int>();
    private List<int> mParamList2 = new List<int>();

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_41(battleUnit);
    }
    public AIType_41(BattleUnit battleUnit)
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

        do
        {
            {
                if (mRes.param1 == null)
                    break;

                string[] arr = mRes.param1.Split(',');

                mParamList1.Add(int.Parse(arr[0]));

                for (int i = 1; i < arr.Length; ++i)
                    mParamList2.Add(int.Parse(arr[i]));
            }

            {
                if (mRes.param2 == null)
                    break;

                string[] arr = mRes.param2.Split(',');

                mParamList1.Add(int.Parse(arr[0]));

                for (int i = 1; i < arr.Length; ++i)
                    mParamList2.Add(int.Parse(arr[i]));
            }

            {
                if (mRes.param3 == null)
                    break;

                string[] arr = mRes.param3.Split(',');

                mParamList1.Add(int.Parse(arr[0]));

                for (int i = 1; i < arr.Length; ++i)
                    mParamList2.Add(int.Parse(arr[i]));
            }

            {
                if (mRes.param4 == null)
                    break;

                string[] arr = mRes.param4.Split(',');

                mParamList1.Add(int.Parse(arr[0]));

                for (int i = 1; i < arr.Length; ++i)
                    mParamList2.Add(int.Parse(arr[i]));
            }

            {
                if (mRes.param5 == null)
                    break;

                string[] arr = mRes.param5.Split(',');

                mParamList1.Add(int.Parse(arr[0]));

                for (int i = 1; i < arr.Length; ++i)
                    mParamList2.Add(int.Parse(arr[i]));
            }

            {
                if (mRes.param6 == null)
                    break;

                string[] arr = mRes.param6.Split(',');

                mParamList1.Add(int.Parse(arr[0]));

                for (int i = 1; i < arr.Length; ++i)
                    mParamList2.Add(int.Parse(arr[i]));
            }

            {
                if (mRes.param7 == null)
                    break;

                string[] arr = mRes.param7.Split(',');

                mParamList1.Add(int.Parse(arr[0]));

                for (int i = 1; i < arr.Length; ++i)
                    mParamList2.Add(int.Parse(arr[i]));
            }

            {
                if (mRes.param8 == null)
                    break;

                string[] arr = mRes.param8.Split(',');

                mParamList1.Add(int.Parse(arr[0]));

                for (int i = 1; i < arr.Length; ++i)
                    mParamList2.Add(int.Parse(arr[i]));
            }

            {
                if (mRes.param9 == null)
                    break;

                string[] arr = mRes.param9.Split(',');

                mParamList1.Add(int.Parse(arr[0]));

                for (int i = 1; i < arr.Length; ++i)
                    mParamList2.Add(int.Parse(arr[i]));
            }

            {
                if (mRes.param10 == null)
                    break;

                string[] arr = mRes.param10.Split(',');

                mParamList1.Add(int.Parse(arr[0]));

                for (int i = 1; i < arr.Length; ++i)
                    mParamList2.Add(int.Parse(arr[i]));
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

    }

    public override void OnUpdateCombat(uint elapsed)
    {
        uint mainTargetId = GetCurrentTargetId();
        Vector3f posm = BaseAI.GetPosition(mainTargetId);
        Vector3f posn = BaseAI.GetPosition(GetID());

        if (BeginCommand(100))
        {
            for (int i = mSkillList1.Count - 1; i >= 0 ; --i)
            {
                if (BaseAI.CheckSkillCd(GetID(),mSkillList1[i]))
                {
                    if (i == 0)
                    {
                        for (int j = 0; j < mParamList1[i]; ++j)
                        {
                            AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkillList1[i], BaseAI.MoveMode.MOVE_RUN, 5000));
                        }
                        break;
                    }
                    else
                    {
                        for (int j = 0; j < mParamList1[i]; ++j)
                        {
                            AddCommand(new AIGoalUseSkillToTargetRangeTime(this,mainTargetId,mSkillList1[i],BaseAI.MoveMode.MOVE_RUN,5000));
                        }
                        break;
                    }
                }
            }
        }
    }
};