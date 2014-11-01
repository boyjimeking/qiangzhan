
using System.Collections;
using System.Collections.Generic;

public class AIType_15 : CommonAI
{
    private float mMaxDistacne = 5;
    private List<int> mSkillList = new List<int>();
    private List<uint> mSkillWaitList = new List<uint>();

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_15(battleUnit);
    }
    public AIType_15(BattleUnit battleUnit)
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
                string waitTime = mRes.param1;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList.Add(skillId);
                mSkillWaitList.Add((uint)wait);

            }

            {
                int skillId = mSkill2;
                string waitTime = mRes.param2;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList.Add(skillId);
                mSkillWaitList.Add((uint)wait);

            }


            {
                int skillId = mSkill3;
                string waitTime = mRes.param3;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList.Add(skillId);
                mSkillWaitList.Add((uint)wait);

            }


            {
                int skillId = mSkill4;
                string waitTime = mRes.param4;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList.Add(skillId);
                mSkillWaitList.Add((uint)wait);

            }


            {
                int skillId = mSkill5;
                string waitTime = mRes.param5;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList.Add(skillId);
                mSkillWaitList.Add((uint)wait);

            }


            {
                int skillId = mSkill6;
                string waitTime = mRes.param6;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList.Add(skillId);
                mSkillWaitList.Add((uint)wait);

            }


            {
                int skillId = mSkill7;
                string waitTime = mRes.param7;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList.Add(skillId);
                mSkillWaitList.Add((uint)wait);

            }


            {
                int skillId = mSkill8;
                string waitTime = mRes.param8;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList.Add(skillId);
                mSkillWaitList.Add((uint)wait);

            }


            {
                int skillId = mSkill9;
                string waitTime = mRes.param9;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList.Add(skillId);
                mSkillWaitList.Add((uint)wait);

            }


            {
                int skillId = mSkill10;
                string waitTime = mRes.param10;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList.Add(skillId);
                mSkillWaitList.Add((uint)wait);

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
	    if(BeginCommand(100))
        {
            for(int i = 0; i < mSkillList.Count; i++)
            {
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkillList[i], BaseAI.MoveMode.MOVE_RUN, 2000));
            }
	    }
    }

};