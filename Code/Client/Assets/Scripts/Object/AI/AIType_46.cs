
using System.Collections;
using System.Collections.Generic;

public class AIType_46 : CommonAI
{
    private List<int> mSkillList = new List<int>();
    private List<int> mParamList = new List<int>();

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new AIType_46(battleUnit);
    }
    public AIType_46(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        do
        {
            int skillId = mSkill1;
            string waitTime = mRes.param1;

            if (waitTime == null || waitTime.Length == 0)
                break;

            int isEnemy = System.Convert.ToInt32(waitTime);
            if (isEnemy < 0)
                break;

            mSkillList.Add(skillId);
            mParamList.Add(isEnemy);

        } while (false);

        do
        {
            int skillId = mSkill2;
            string waitTime = mRes.param2;

            if (waitTime == null || waitTime.Length == 0)
                break;

            int isEnemy = System.Convert.ToInt32(waitTime);
            if (isEnemy < 0)
                break;

            mSkillList.Add(skillId);
            mParamList.Add(isEnemy);

        } while (false);

        do
        {
            int skillId = mSkill3;
            string waitTime = mRes.param3;

            if (waitTime == null || waitTime.Length == 0)
                break;

            int wait = System.Convert.ToInt32(waitTime);
            if (wait < 0)
                break;

            int isEnemy = System.Convert.ToInt32(waitTime);
            if (isEnemy < 0)
                break;

            mSkillList.Add(skillId);
            mParamList.Add(isEnemy);
        } while (false);

        do
        {
            int skillId = mSkill4;
            string waitTime = mRes.param4;

            if (waitTime == null || waitTime.Length == 0)
                break;

            int wait = System.Convert.ToInt32(waitTime);
            if (wait < 0)
                break;

            int isEnemy = System.Convert.ToInt32(waitTime);
            if (isEnemy < 0)
                break;

            mSkillList.Add(skillId);
            mParamList.Add(isEnemy);
        } while (false);

        do
        {
            int skillId = mSkill5;
            string waitTime = mRes.param5;

            if (waitTime == null || waitTime.Length == 0)
                break;

            int wait = System.Convert.ToInt32(waitTime);
            if (wait < 0)
                break;

            int isEnemy = System.Convert.ToInt32(waitTime);
            if (isEnemy < 0)
                break;

            mSkillList.Add(skillId);
            mParamList.Add(isEnemy);
        } while (false);

        do
        {
            int skillId = mSkill6;
            string waitTime = mRes.param6;

            if (waitTime == null || waitTime.Length == 0)
                break;

            int wait = System.Convert.ToInt32(waitTime);
            if (wait < 0)
                break;

            int isEnemy = System.Convert.ToInt32(waitTime);
            if (isEnemy < 0)
                break;

            mSkillList.Add(skillId);
            mParamList.Add(isEnemy);
        } while (false);

        do
        {
            int skillId = mSkill7;
            string waitTime = mRes.param7;

            if (waitTime == null || waitTime.Length == 0)
                break;

            int wait = System.Convert.ToInt32(waitTime);
            if (wait < 0)
                break;

            int isEnemy = System.Convert.ToInt32(waitTime);
            if (isEnemy < 0)
                break;

            mSkillList.Add(skillId);
            mParamList.Add(isEnemy);
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
        uint followId = PlayerController.Instance.GetControl();

        Vector3f pos = BaseAI.GetPosition(followId);

        float distance = BaseAI.GetObjectDistance(GetID(), followId);

        if (distance >= 15)
        {
            RemoveCommand();
            BaseAI.SetObjectPos(GetID(), pos);
        }
        else if (distance <= 3)
        {
            if (BeginCommand(100))
            {
                float rand = BaseAI.Randomf(1, 10);

                if (rand <= 5)
                {
                    AddCommand(new AIGoalRandMove(this, pos, 0.5f, 1.5f, 0, 90));

                }
                else if (rand <= 10)
                {
                    AddCommand(new AIGoalStand(this, 2000, 3500));
                    //AddCommand(new AIGoalPlayAni(this, weaponitem.ani_pre + "%xiuxian"));
                   // AddCommand(new AIGoalPlayAniByFun(this, battlecrops.PlayLeisureAnimation));
                    //AddCommand(new AIGoalStand(this, 1000, 2500));
                }
            }
        }
        else
        {
            if (BeginCommand(200))
            {
                AddCommand(new AIGoalApproachTarget(this, followId, 2));
            }
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
        uint mCurCtrlID = PlayerController.Instance.GetControl();
        Vector3f pos = BaseAI.GetPosition(mCurCtrlID);
        float distance = BaseAI.GetObjectDistance(GetID(), mCurCtrlID);

        if (distance >= 10)
        {
            RemoveCommand();
            if (BeginCommand(2000))
            {
                RemoveAllEnemy();
                AddCommand(new AIGoalApproachTarget(this, mCurCtrlID, 2));
            }
            return;
        }

        if (BeginCommand(1000))
        {
            for (int i = 0; i < mSkillList.Count; ++i)
            {
                if (0 != mParamList[i])
                {
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this,mCurCtrlID,mSkillList[i],BaseAI.MoveMode.MOVE_RUN,5000));
                }
                else
                {
                    AddCommand(new AIGoalUseSkillToTargetRangeTime(this, mainTargetId, mSkillList[i], BaseAI.MoveMode.MOVE_RUN, 5000));
                }
            }
        }
    }
};