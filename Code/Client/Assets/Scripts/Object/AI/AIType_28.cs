
using System.Collections;
using System.Collections.Generic;

public class AIType_28 : CommonAI
{
    private Vector3f tarPos = new Vector3f(0,0,0);

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_28(battleUnit);
    }
    public AIType_28(BattleUnit battleUnit)
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

                tarPos = new Vector3f(float.Parse(arr[0]),float.Parse(arr[1]),float.Parse(arr[2]));
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

        if (BeginCommand(100))
        {
            AddCommand(new AIGoalUseSkillToPosition(this,mSkill1,tarPos));
        }
    }
};