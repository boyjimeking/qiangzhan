using System.Collections;
using System.Collections.Generic;
public class AIType_22 : CommonAI
{
    private List<int> mSkillList1 = new List<int>();
    private List<uint> mSkillWaitList1 = new List<uint>();
    private float minRadius = 0;
    private float maxRadius = 0;
    public AIType_22(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_22(battleUnit);
    }
    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        do
        {
            {
                int skillId = mSkill1;
                string waitTime = mRes.param1;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }

            {
                int skillId = mSkill2;
                string waitTime = mRes.param2;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }


            {
                int skillId = mSkill3;
                string waitTime = mRes.param3;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }


            {
                int skillId = mSkill4;
                string waitTime = mRes.param4;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }


            {
                int skillId = mSkill5;
                string waitTime = mRes.param5;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }

            {
                int skillId = mSkill6;
                string waitTime = mRes.param6;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }

            {
                int skillId = mSkill7;
                string waitTime = mRes.param7;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }

            {
                int skillId = mSkill8;
                string waitTime = mRes.param8;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }

            {
                int skillId = mSkill9;
                string waitTime = mRes.param9;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }

            {
                int skillId = mSkill10;
                string waitTime = mRes.param10;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }

            {
                int skillId = mSkill11;
                string waitTime = mRes.param11;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }

            {
                int skillId = mSkill12;
                string waitTime = mRes.param12;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }

            {
                int skillId = mSkill13;
                string waitTime = mRes.param13;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }

            {
                int skillId = mSkill14;
                string waitTime = mRes.param14;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);

            }

            {
                int skillId = mSkill15;
                string waitTime = mRes.param15;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList1.Add(skillId);
                mSkillWaitList1.Add((uint)wait);
            }

            {
                if (null == mRes.param16)
                    break;
                minRadius = float.Parse(mRes.param16);
            }

            {
                if (null == mRes.param17)
                    break;
                maxRadius = float.Parse(mRes.param17);
            }
        } while (false);
        // ½âÎö²ÎÊý
        return true;
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
        if (BeginCommand(100))
        {
            for (int i = 0; i < mSkillList1.Count; ++i)
            {
                AddCommand(new AIGoalRandMoveTime(this, posm, 4, 8, 0, 360, mSkillWaitList1[i]));
                AddCommand(new AIGoalUseSkillToTarget(this,mainTargetId,mSkillList1[i]));
            }
        }
    }

};