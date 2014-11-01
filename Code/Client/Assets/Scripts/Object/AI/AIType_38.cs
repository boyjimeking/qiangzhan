
using System.Collections;
using System.Collections.Generic;

/*
 *  进入战斗 旋转param1指定的角度， 释放技能槽1的技能， 删除自己
 */
public class AIType_38 : CommonAI
{
    private float mTurnAngle = 0.0f;
    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_38(battleUnit);
    }
    public AIType_38(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        mTurnAngle = System.Convert.ToSingle(mRes.param1);

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
        if(BeginCommand(100))
        {
            AddCommand(new AIGoalTurn(this, mTurnAngle));
            AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, BaseAI.GetPosition(GetID())));
            AddCommand(new AIGoalDestorySelf(this));
        }
    }
};