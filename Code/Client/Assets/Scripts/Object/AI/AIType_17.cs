
public class AIType_17 : CommonAI
{
    private float rad = 0.0f;
    private float dir = 0.0f;

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_17(battleUnit);
    }
    public AIType_17(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        rad = (float)System.Convert.ToDouble(mRes.param1);
        dir = (float)System.Convert.ToDouble(mRes.param2);
        // 解析参数
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

		if(BeginCommand(1000))
        {
            float radius = posm.Subtract(posn).Length();
            if (radius <= rad)
            {
                float newPos = (float)System.Math.Atan2(posn.z - posm.z, posn.x - posm.x);
                float curAngle = (float)(newPos * 180 / System.Math.PI);  //角色当前角度

                Vector3f mGunPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle + 180, dir);
                if (BaseAI.SceneMayStraightReach(posn, mGunPos))
                {
                    AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                    AddCommand(new AIGoalMoveTo(this, mGunPos, BaseAI.MoveMode.MOVE_SCURRY));
                }
                else
                {
                    for (int i = 30; i <= 360; i += 30)
                    {
                        mGunPos = BaseAI.get_position_angle_and_distance_position(posn, (float)i, 11.0f);
                        if (scene_postion_arrive(posn, mGunPos))
                        {
                            AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                            AddCommand(new AIGoalMoveTo(this, mGunPos, BaseAI.MoveMode.MOVE_SCURRY));
                            break;
                        }
                    }
                }
            }
        }
    }
};