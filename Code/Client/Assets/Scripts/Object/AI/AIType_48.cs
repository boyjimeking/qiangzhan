
using System.Collections;
using System.Collections.Generic;

public class AIType_48 : CommonAI
{
    private bool isRanpage = false;
    private uint rageBuffId = 0;
    private float mMaxDistacne = 5;
    private List<int> mSkillList1 = new List<int>();
    private List<int> mSkillList2 = new List<int>();
    private List<uint> mSkillWaitList1 = new List<uint>();
    private List<uint> mSkillWaitList2 = new List<uint>();
    private bool canMove = true;

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_48(battleUnit);
    }
    public AIType_48(BattleUnit battleUnit)
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
        } while (false);

        do
        {
            {
                int skillId = mSkill6;
                string waitTime = mRes.param6;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList2.Add(skillId);
                mSkillWaitList2.Add((uint)wait);

            }


            {
                int skillId = mSkill7;
                string waitTime = mRes.param7;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList2.Add(skillId);
                mSkillWaitList2.Add((uint)wait);

            }


            {
                int skillId = mSkill8;
                string waitTime = mRes.param8;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList2.Add(skillId);
                mSkillWaitList2.Add((uint)wait);

            }


            {
                int skillId = mSkill9;
                string waitTime = mRes.param9;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList2.Add(skillId);
                mSkillWaitList2.Add((uint)wait);

            }


            {
                int skillId = mSkill10;
                string waitTime = mRes.param10;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList2.Add(skillId);
                mSkillWaitList2.Add((uint)wait);

            }
        } while (false);

        do
        {
            {
                int skillId = mSkill11;
                string waitTime = mRes.param11;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList2.Add(skillId);
                mSkillWaitList2.Add((uint)wait);

            }

            {
                int skillId = mSkill12;
                string waitTime = mRes.param12;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList2.Add(skillId);
                mSkillWaitList2.Add((uint)wait);

            }

            {
                int skillId = mSkill13;
                string waitTime = mRes.param13;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList2.Add(skillId);
                mSkillWaitList2.Add((uint)wait);

            }

            {
                int skillId = mSkill14;
                string waitTime = mRes.param14;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList2.Add(skillId);
                mSkillWaitList2.Add((uint)wait);

            }

            {
                int skillId = mSkill15;
                string waitTime = mRes.param15;

                if (skillId < 0 || waitTime == null || waitTime.Length == 0)
                    break;

                int wait = System.Convert.ToInt32(waitTime);
                if (wait < 0)
                    break;

                mSkillList2.Add(skillId);
                mSkillWaitList2.Add((uint)wait);

            }
        } while (false);


        do
        {
            {
                if (null == mRes.param16)
                    break;
                canMove = System.Convert.ToBoolean(mRes.param16);
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
        if (canMove)
        {
            if (BaseAI.GetHpPercent(GetID()) > 0.5f)
            {
                if (BeginCommand(100))
                {
                    for (int i = 0; i < mSkillList1.Count; i++)
                    {
                        AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkillList1[i]) * 0.95f));
                        float dist = posn.Subtract(posn).Length();
                        if (dist <= BaseAI.GetSkillMaxRangle(mSkillList1[i]))
                        {
                            if (BaseAI.SceneMayStraightReach(posn, posm))
                                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkillList1[i]));
                            else
                            {
                                for (int j = 30; j <= 360; j += 30)
                                {
                                    Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, (float)j, 5.0f);
                                    if (BaseAI.SceneMayStraightReach(posn, guPos))
                                    {
                                        AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                                        break;
                                    }
                                }
                            }
                            AddCommand(new AIGoalWait(this, mSkillWaitList1[i]));
                        }
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
                    for (int i = 0; i < mSkillList2.Count; i++)
                    {
                        AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkillList2[i]) * 0.95f));
                        float dist = posn.Subtract(posn).Length();
                        if (dist <= BaseAI.GetSkillMaxRangle(mSkillList2[i]))
                        {
                            if (BaseAI.SceneMayStraightReach(posn, posm))
                                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkillList2[i]));
                            else
                            {
                                for (int j = 30; j <= 360; j += 30)
                                {
                                    Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, (float)j, 5.0f);
                                    if (BaseAI.SceneMayStraightReach(posn, guPos))
                                    {
                                        AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                                        break;
                                    }
                                }
                            }
                            //AddCommand(new AIGoalMoveTlimit(this, mainTargetId, mSkillWaitList2[i], BaseAI.MoveMode.MOVE_RUN));
                            AddCommand(new AIGoalWait(this, mSkillWaitList2[i]));
                        }
                    }
                }
            }
        }
        else
        {
            if (BaseAI.GetHpPercent(GetID()) < 0.4f)
            {
                if (!isRanpage)
                {
                    isRanpage = !isRanpage;
                    RemoveCommand();
                    BaseAI.FlyIntoaRage(GetID());
                    BaseAI.UseSkillEffect(GetID(), SkillEffectType.Buff, rageBuffId);
                }

                if (BeginCommand(1000))
                {
                    for (int i = 0; i < mSkillList2.Count; i++)
                    {
                        if (BaseAI.SceneMayStraightReach(posn, posm))
                        {
                            AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkillList2[i]));
                            AddCommand(new AIGoalWait(this, mSkillWaitList2[i]));
                        }
                    }
                }
            }
            else
            {
                if (BeginCommand(100))
                {
                    for (int i = 0; i < mSkillList1.Count; i++)
                    {
                        if (BaseAI.SceneMayStraightReach(posn, posm))
                        {
                            AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkillList1[i]));
                            AddCommand(new AIGoalWait(this, mSkillWaitList1[i]));
                        }
                    }
                }
            }
        }
    }
};