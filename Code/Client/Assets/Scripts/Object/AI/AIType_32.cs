using System.Collections;
using System.Collections.Generic;


public class AIType_32 : CommonAI
{
    private List<Vector3f> posList = new List<Vector3f>();
    private List<int> waitList = new List<int>();
    private int movePos = 0;
    private List<int> mSkillList1 = new List<int>();


    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_32(battleUnit);
    }
    public AIType_32(BattleUnit battleUnit)
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
        } while (false);

        do
        {
            {
                if (null != mRes.param1)
                {
                    string[] pos = mRes.param1.Split(',');
                    posList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                    waitList.Add(int.Parse(pos[3]));
                }

                if (null != mRes.param2)
                {
                    string[] pos = mRes.param2.Split(',');
                    posList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                    waitList.Add(int.Parse(pos[3]));
                }

                if (null != mRes.param3)
                {
                    string[] pos = mRes.param3.Split(',');
                    posList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                    waitList.Add(int.Parse(pos[3]));
                }

                if (null != mRes.param4)
                {
                    string[] pos = mRes.param4.Split(',');
                    posList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                    waitList.Add(int.Parse(pos[3]));
                }

                if (null != mRes.param5)
                {
                    string[] pos = mRes.param5.Split(',');
                    posList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                    waitList.Add(int.Parse(pos[3]));
                }

                if (null != mRes.param6)
                {
                    string[] pos = mRes.param6.Split(',');
                    posList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                    waitList.Add(int.Parse(pos[3]));
                }

                if (null != mRes.param7)
                {
                    string[] pos = mRes.param7.Split(',');
                    posList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                    waitList.Add(int.Parse(pos[3]));
                }

                if (null != mRes.param8)
                {
                    string[] pos = mRes.param8.Split(',');
                    posList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                    waitList.Add(int.Parse(pos[3]));
                }

                if (null != mRes.param9)
                {
                    string[] pos = mRes.param9.Split(',');
                    posList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                    waitList.Add(int.Parse(pos[3]));
                }

                if (null != mRes.param10)
                {
                    string[] pos = mRes.param10.Split(',');
                    posList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                    waitList.Add(int.Parse(pos[3]));
                }
            }
        } while (false);

        // 解析参数
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
        Vector3f posn = BaseAI.GetPosition(GetID());
        posn.y = 0.0f;
        if (posn.Subtract(posList[movePos]).Length() > 1)
        {
            BaseAI.MoveTo(GetID(), posList[movePos]);
        }
        else
        {
            movePos += 1;
            if (movePos > posList.Count - 1)
                movePos = 0;
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

        Vector3f posm = BaseAI.GetPosition(mainTargetId);
        Vector3f posn = BaseAI.GetPosition(GetID());

        if (BeginCommand(100))
        {
            float radius = (float)System.Math.Sqrt((posm.x - posn.x) * (posm.x - posn.x) + (posm.z - posn.z) * (posm.z - posn.z));
            if (radius < 3)
            {

                float newPos = (float)System.Math.Atan2(posn.z - posm.z, posn.x - posm.x);
                float curAngle = (float)(newPos * 180 / System.Math.PI);  //角色当前角度

                Vector3f mGunPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle + 180, 5);

                if (BaseAI.SceneMayStraightReach(posn, mGunPos))
                {
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                    AddCommand(new AIGoalMoveTo(this, mGunPos, BaseAI.MoveMode.MOVE_SCURRY));
                }
                else
                {
                    for (int i = 30; i < 360; i += 30)
                    {
                        Vector3f skillPos = BaseAI.get_position_angle_and_distance_position(posn, i, 5);
                        if (BaseAI.SceneMayStraightReach(posn, skillPos))
                        {
                            AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                            AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                            AddCommand(new AIGoalMoveTo(this, skillPos, BaseAI.MoveMode.MOVE_SCURRY));
                            break;
                        }
                    }

                }
            }
            else if (radius < BaseAI.GetSkillMaxRangle(mSkill2) * 0.95f)
            {
                float curAngle = BaseAI.CalcDirection(posn, posm);
                int ranAngle1 = BaseAI.Random(-90, -60);
                int ranAngle2 = BaseAI.Random(60, 90);
                int ranAngle = BaseAI.Random(1, 3) == 1 ? ranAngle1 : ranAngle2;
                float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                curAngle = curAngle + angleRadius;
                float randRadius = BaseAI.Randomf(4, 7);
                Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);
                AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                if (BaseAI.SceneMayStraightReach(posn, posm))
                {
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill2));
                    AddCommand(new AIGoalWait(this, 1200));
                }
                else
                {
                    for (int i = 30; i <= 360; i += 30)
                    {
                        guPos = BaseAI.get_position_angle_and_distance_position(posn, (float)i, 5.0f);
                        if (BaseAI.SceneMayStraightReach(posn, guPos))
                        {
                            AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                            break;
                        }
                    }
                }
            }
            else
            {
                float curAngle = BaseAI.CalcDirection(posn, posm);
                int ranAngle = BaseAI.Random(-45, 45);
                float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                curAngle = curAngle + angleRadius;
                float randRadius = BaseAI.Randomf(2, 3);
                Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);
                AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                AddCommand(new AIGoalApproachTargetLimit(this, mainTargetId, BaseAI.GetSkillMaxRangle(mSkill2) * 0.95f, BaseAI.MoveMode.MOVE_RUN, 1500));
            }
        }
    }

};