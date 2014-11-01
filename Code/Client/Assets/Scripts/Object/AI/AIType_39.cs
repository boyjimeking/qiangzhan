
using System.Collections;
using System.Collections.Generic;

public class AIType_39 : CommonAI
{

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_39(battleUnit);
    }
    public AIType_39(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

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
        if (BeginCommand(100))
        {
            Vector3f tarPos = BaseAI.GetPosition(GetNearestEnemyId());
            AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, tarPos));
        }
    }
};