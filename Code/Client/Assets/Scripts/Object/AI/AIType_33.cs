using System.Collections;
using System.Collections.Generic;


public class AIType_33 : CommonAI
{
    private List<Vector3f> guPosList = new List<Vector3f>();
    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_33(battleUnit);
    }
    public AIType_33(BattleUnit battleUnit)
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
                if (null != mRes.param1)
                {
                    string[] pos = mRes.param1.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param2)
                {
                    string[] pos = mRes.param2.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param3)
                {
                    string[] pos = mRes.param3.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param4)
                {
                    string[] pos = mRes.param4.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param5)
                {
                    string[] pos = mRes.param5.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param6)
                {
                    string[] pos = mRes.param6.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param7)
                {
                    string[] pos = mRes.param7.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param8)
                {
                    string[] pos = mRes.param8.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param9)
                {
                    string[] pos = mRes.param9.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param10)
                {
                    string[] pos = mRes.param10.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param11)
                {
                    string[] pos = mRes.param11.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param12)
                {
                    string[] pos = mRes.param12.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param13)
                {
                    string[] pos = mRes.param13.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param14)
                {
                    string[] pos = mRes.param14.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }

            {
                if (null != mRes.param15)
                {
                    string[] pos = mRes.param15.Split(',');
                    guPosList.Add(new Vector3f(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
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
        Vector3f posn = BaseAI.GetPosition(GetID());
        Vector3f posm = BaseAI.GetPosition(mainTargetId);

        if (BeginCommand(100))
        {
            for (int i = 0; i < guPosList.Count; ++i)
            {
                AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, guPosList[i]));
            }

            for (int i = guPosList.Count - 1; i >= 0; --i)
            {
                if(-1 == mSkill2)
                    AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, guPosList[i]));
                else
                    AddCommand(new AIGoalUseSkillToPosition(this, mSkill2, guPosList[i]));
            }
        }
    }

};