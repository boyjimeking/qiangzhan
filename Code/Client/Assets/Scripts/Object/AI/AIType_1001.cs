// Ö°Òµ1µÄAI

public class AIType_1001 : CommonAI
{
    enum BATTLE_STATE : int
    {
        SHEJI = 0,
        COMBAT = 1,
    }

    private int mWeaponSkillId = -1;
    private BATTLE_STATE mState = BATTLE_STATE.SHEJI;
    private uint mStateTime = 0;
    private int mFireTime = 0;



    public AIType_1001(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }


    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_1001(battleUnit);
    }
    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        if (!(mOwner is Ghost))
            return false;

        Ghost ghost = mOwner as Ghost;

        SkillModule module = ModuleManager.Instance.FindModule<SkillModule>();
        if (module == null)
            return false;

        mWeaponSkillId = mOwner.GetWeaponSkillID();

        return true;
    }
    public override void OnEnterCombat()
    {
        mState = BATTLE_STATE.SHEJI;
        SetFireTime();
    }

    public override void OnExitCombat()
    {
        BaseAI.LeaveFromaRange(GetID());
    }

    public override void OnUpdateCombat(uint elapsed)
    {
        switch (mState)
        {
            case BATTLE_STATE.SHEJI:
                {
                    uint mainTargetId = GetCurrentTargetId();
                    Vector3f posm = BaseAI.GetPosition(mainTargetId);
                    Vector3f posn = BaseAI.GetPosition(GetID());
                    float radius = posm.Subtract(posn).Length();

                    if (radius > BaseAI.GetSkillMaxRangle(mWeaponSkillId) * 0.5f)
                    {
                        if (BeginCommand(1000))
                            AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(mWeaponSkillId) * 0.45f));
                        break;
                    }
                    mStateTime += elapsed;
                    if (GetStateTime() < GetFireTime())
                    {
                        if (BeginCommand(1000))
                            AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mWeaponSkillId));
                    }
                    else
                    {
                        ChangeState(BATTLE_STATE.COMBAT);
                    }
                }
                break;
            case BATTLE_STATE.COMBAT:
                {
                    if (!CombatChoice())
                    {
                        MoveChoice();
                    }
                    ChangeState(BATTLE_STATE.SHEJI);
                    break;
                }
        }
    }

    private void ChangeState(BATTLE_STATE eState)
    {
        if (mState == eState)
            return;

        mState = eState;
        SetStateTime();
        SetFireTime();
    }

    private uint GetStateTime()
    {
        return mStateTime;
    }

    private void SetStateTime()
    {
        mStateTime = 0;
    }

    private bool CombatChoice()
    {
        uint mainTargetId = GetCurrentTargetId();
        int choice = UnityEngine.Random.Range(1, 3);
        if (choice == 1 && BaseAI.CheckSkillCd(GetID(), mSkill2) && BaseAI.CheckSkillCost(GetID(), mSkill2))
        {
            if (BeginCommand(2000))
            {
                AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkill2, BaseAI.MoveMode.MOVE_RUN, 30000));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill3));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));
            }
            return true;
        }
        else if (choice == 2 && BaseAI.CheckSkillCd(GetID(), mSkill4) && BaseAI.CheckSkillCost(GetID(), mSkill4))
        {
            if (BeginCommand(2000))
            {
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill1));
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill4));
            }
            return true;
        }
        else if (choice == 3 && BaseAI.CheckSkillCd(GetID(), mSkill5) && BaseAI.CheckSkillCost(GetID(), mSkill5))
        {
            if (BeginCommand(2000))
            {
                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, mSkill5));
            }
            return true;
        }

        return false;

    }

    private bool MoveChoice()
    {
        uint mainTargetId = GetCurrentTargetId();
        Vector3f posm = BaseAI.GetPosition(mainTargetId);
        Vector3f posn = BaseAI.GetPosition(GetID());
        int choice = UnityEngine.Random.Range(1, 4);

        switch (choice)
        {
            case 1:
                {
                    if (BeginCommand(3000))
                    {
                        float ranAngle = BaseAI.Randomf(1, 3) == 1 ? BaseAI.Randomf(-30, -50) : BaseAI.Randomf(30, 50);
                        float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                        float curAngle = BaseAI.CalcDirection(posn, posm) + angleRadius;
                        float randRadius = BaseAI.Randomf(2, 3);
                        Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);
                        if (BaseAI.SceneMayStraightReach(posn, guPos))
                        {
                            AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, guPos));
                        }
                        else
                        {
                            for (int i = 30; i <= 360; i += 30)
                            {
                                guPos = BaseAI.get_position_angle_and_distance_position(posn, (float)i, 3.0f);
                                if (BaseAI.SceneMayStraightReach(posn, guPos))
                                {
                                    AddCommand(new AIGoalUseSkillToPosition(this, mSkill1, guPos));
                                    break;
                                }
                            }
                        }
                    }
                }
                return true;
            case 2:
                {
                    if (BeginCommand(3000))
                    {
                        float ranAngle = BaseAI.Randomf(-60, 60);
                        float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
                        float curAngle = BaseAI.CalcDirection(posn, posm) + angleRadius;
                        float randRadius = BaseAI.Randomf(2, 4);
                        Vector3f guPos = BaseAI.get_position_angle_and_distance_position(posn, curAngle, randRadius);
                        if (BaseAI.SceneMayStraightReach(posn, guPos))
                        {
                            AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                        }
                        else
                        {
                            for (int i = 30; i <= 360; i += 30)
                            {
                                guPos = BaseAI.get_position_angle_and_distance_position(posn, (float)i, 3.0f);
                                if (BaseAI.SceneMayStraightReach(posn, guPos))
                                {
                                    AddCommand(new AIGoalMoveTo(this, guPos, BaseAI.MoveMode.MOVE_RUN));
                                    break;
                                }
                            }
                        }
                    }
                }
                return true;
            default:
                return false;
        }
    }

    private void SetFireTime()
    {
        mFireTime = BaseAI.Random(1500, 3000);
    }

    private int GetFireTime()
    {
        return mFireTime;
    }
};