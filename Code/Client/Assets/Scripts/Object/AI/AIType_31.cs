using System.Collections;
using System.Collections.Generic;
public class AIType_31 : CommonAI
{

    private Vector3f tarPos = new Vector3f(0, 0, 0);
    bool isArrived = false;
    private List<int> mSkillList1 = new List<int>();
    public AIType_31(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_31(battleUnit);
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
        Vector3f posn = BaseAI.GetPosition(GetID());
        Vector3f posm = BaseAI.GetPosition(mainTargetId);

        if (BeginCommand(100))
        {
            for (int i = 0; i < mSkillList1.Count; ++i)
            {
                float newPos = (float)System.Math.Atan2(posn.z - posm.z, posn.x - posm.x);
                float curAngle = (float)(newPos * 180 / System.Math.PI);  //角色当前角度

                Vector3f mGunPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle + 180, 5);
                if (BaseAI.SceneMayStraightReach(posn, mGunPos))
                {
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkillList1[i]));
                    AddCommand(new AIGoalMoveTo(this, mGunPos, BaseAI.MoveMode.MOVE_SCURRY));
                }
                else
                {
                    for (int j = 30; j <= 360; j += 30)
                    {
                        mGunPos = BaseAI.get_position_angle_and_distance_position(posn, (float)j, 5.0f);
                        if (scene_postion_arrive(posn, mGunPos))
                        {
                            AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkillList1[i]));
                            AddCommand(new AIGoalMoveTo(this, mGunPos, BaseAI.MoveMode.MOVE_SCURRY));
                            break;
                        }
                    }
                }
            }
        }
    }
};