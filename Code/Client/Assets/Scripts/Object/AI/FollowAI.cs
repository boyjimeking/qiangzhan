using System.Collections;
using System.Collections.Generic;


public class FollowAI : CommonAI
{
    private float mMaxDistacne = 5;
    private float mOffsetDistacne = 2;

    private List<int> mSkillList = new List<int>();
    private List<uint> mSkillWaitList = new List<uint>();

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new FollowAI(battleUnit);
    }
    public FollowAI(BattleUnit battleUnit)
        : base(battleUnit,true)
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

        do
        {
            if (null == mRes.param11)
                break;
            mMaxDistacne = System.Convert.ToInt32(mRes.param11);
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
        uint followId = PlayerController.Instance.GetControl();

        Vector3f pos = BaseAI.GetPosition(followId);

        float distance = BaseAI.GetObjectDistance(GetID(), followId);

        if (distance >= mMaxDistacne)
        {
            RemoveCommand();
            BaseAI.SetObjectPos(GetID(), pos);
        }
        else if (distance <= 3)
        {
            if (BeginCommand(100))
            {
                float rand = BaseAI.Randomf(1, 10);

                if (rand <= 5)
                {
                    AddCommand(new AIGoalRandMove(this, pos, 0.5f, 1.5f, 0, 90));

                }
                else if (rand <= 10)
                {
                    AddCommand(new AIGoalStand(this, 1000, 2500));
                }
            }
        }
        else
        {
            if (BeginCommand(200))
            {
                AddCommand(new AIGoalApproachTarget(this, followId, 2));
            }
        }	
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

        if (BeginCommand(100))
        {
            for (int i = 0; i < mSkillList.Count; i++)
            {
                AddCommand(new AIGoalSkillToTargetRange(this, mainTargetId, mSkillList[i], BaseAI.MoveMode.MOVE_RUN));
                AddCommand(new AIGoalLookAtTarget(this, mainTargetId, mSkillWaitList[i]));
            }
        }
    }

};