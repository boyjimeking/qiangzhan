using System.Collections;
using System.Collections.Generic;

public class AIType_47 : CommonAI
{
    private List<int> mSkillList = new List<int>();
    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_47(battleUnit);
    }
    public AIType_47(BattleUnit battleUnit)
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

                mSkillList.Add(skillId);

            }

            {
                int skillId = mSkill2;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);

            }


            {
                int skillId = mSkill3;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);

            }


            {
                int skillId = mSkill4;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);

            }


            {
                int skillId = mSkill5;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);

            }
        } while (false);

        do
        {
            {
                int skillId = mSkill6;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);

            }


            {
                int skillId = mSkill7;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);

            }


            {
                int skillId = mSkill8;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);

            }


            {
                int skillId = mSkill9;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);

            }


            {
                int skillId = mSkill10;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);
            }
        } while (false);

        do
        {
            {
                int skillId = mSkill11;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);

            }

            {
                int skillId = mSkill12;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);

            }

            {
                int skillId = mSkill13;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);

            }

            {
                int skillId = mSkill14;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);

            }

            {
                int skillId = mSkill15;

                if (skillId < 0)
                    break;

                mSkillList.Add(skillId);
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
        Vector3f posn = BaseAI.GetPosition(GetID());

        if (BeginCommand(100))
        {
            for (int i = 0; i < mSkillList.Count; ++i)
            { 
                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkillList[i]) * 0.9f));
                AddCommand(new AIGoalUseSkillToTarget(this,mainTargetId,mSkillList[i]));
                float curAngle = BaseAI.CalcDirection(posn, posm);
                int ranAngle1 = BaseAI.Random(-90, -60);
                int ranAngle2 = BaseAI.Random(60, 90);
                int ranAngle = BaseAI.Random(1, 3) == 1 ? ranAngle1 : ranAngle2;
                float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                curAngle = curAngle + angleRadius;
                float randRadius = BaseAI.Randomf(2, 5);
                Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);

                if (BaseAI.SceneMayStraightReach(posn, guPos))
                {
                    AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                }
                else
                {
                    for (int j = 30; j <= 360; j += 30)
                    {
                        guPos = BaseAI.get_position_angle_and_distance_position(posn, (float)j, 5.0f);
                        if (BaseAI.SceneMayStraightReach(posn, guPos))
                        {
                            AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                            break;
                        }
                    }
                }
            }
        }
    }
};