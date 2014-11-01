
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
skill 1 - 5 非狂暴时技能
skill 6 - 10 狂暴时技能

param1 最大狂暴时间 ms
param2 boss狂暴技能
param3 player变小猫buffid
param4 boss狂暴buffid
param5 金币pickid 猫创建,狂爆时创建,场景创建
param6 解药pickid
param7 每点伤害转化money值 (浮点)， 当money为100，掉一金币
param8 每点伤害转化怒气值 (浮点）， 当怒气为100, 狂暴
param9 休息计时器最小间隔 ms
param10 休息计时器最大间隔 ms
param11 最小休息时间 ms
param12 最大休息时间 ms
param13 非狂暴时使用技能计时器 最小间隔 ms
param14 非狂暴时使用技能计时器 最大间隔 ms
param15 游动的最小半径
param16 游动的最大半径
param17 解药出生位置.如 0.0,1.0;2.0,3.0;4.0,5.0  三个点， 如果不添解药出生在boss脚下
param18 金币总数
param19 狂暴时创建金币位置 如 0.0,1.0;2.0,3.0;4.0,5.0  三个点
*/

public class MaoAI : CommonAI
{
    enum BATTLE_STATE : int
    {
        STATE_1 = 0,        
        STATE_2 = 1,  
        STATE_3 = 2, 
        STATE_4 = 3,
        STATE_5 = 4,
        STATE_6 = 5,
    }

    private const float MAX_ANGER_VALUE = 100;
    private const float MAX_MONEY_VALUE = 100;

    private uint mMaxGoldCount = 80;

    private uint mFrenzyMaxTime     = 150000;                       // 最长狂暴时间
    private List<int> mSkillList1   = new List<int>();              // 非狂暴时的技能列表
    private List<int> mSkillList2   = new List<int>();              // 狂暴时的技能列表
    private List<Vector3f> mJieYaoPosition = new List<Vector3f>();  // 解药可以出生的位置，每次会随机出一个位置,进行解药的创建
    private int mSkillId = -1;                                      // 使自己进入狂暴状态的技能id
    private int mBuffIdToPlayer = -1;                               // 使player变在小猫的buff_id 
    private int mBuffIdToBoss = -1; 
    private int mGoldPickId1     = 1;                                // 金币pick resid 用于猫掉
    private int mGoldPickId2     = 1;                                // 金币pick resid 用于狂暴时创建
    private int mGoldPickId3     = 1;                                // 金币pick resid 用于场景创建
    private int mJieYaoPickId   = 2;                                // 解药pick resid 
    private double mDamageToMoney = 0.5f;                            // 每点伤害转化的money值
    private double mDamageToAnger = 0.2f;                            // 每点伤害转化的anger值
    private uint mTimerStandIntervalMin;
    private uint mTimerStandIntervalMax;
    private uint mStandTimeMin;
    private uint mStandTimeMax;
    private uint mTimerUseSkillIntervalMin;
    private uint mTimerUseSkillIntervalMax;
    private float mMoveRadiusMin;
    private float mMoveRadiusMax;
    private List<Vector3f> mSceneGoldPosition = new List<Vector3f>();


    private bool mIsFighting = false;
    private BATTLE_STATE mState = BATTLE_STATE.STATE_1;
    private uint mStateTime     = 0;
    private double mMoneyValue   = 0;            // 当前money值, 当money值大于等于 100 时会掉一个金币
    private bool mFindJieYao    = false;        // 是否已找到解药
    private int mGoldCount      = 0;            // 已获得的 金币数量
    private double mAngerValue   = 0;            // 当前怒气值, 当努气值大于等于100时，会进入狂暴状态

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new MaoAI(battleUnit);
    }

    public MaoAI(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        for(int i = 1; i <= 5; i++)
        {
            int skillId = GetSkillId(i);
            if (skillId < 0)
                continue;

            mSkillList1.Add(skillId);
        }
      
        for(int i = 6; i <= 10; i++)
        {
            int skillId = GetSkillId(i);
            if (skillId < 0)
                continue;

            mSkillList2.Add(skillId);
        }

        try
        {
            mFrenzyMaxTime  = System.Convert.ToUInt32(mRes.param1);
            mSkillId        = System.Convert.ToInt32(mRes.param2);
            mBuffIdToPlayer = System.Convert.ToInt32(mRes.param3);
            mBuffIdToBoss   = System.Convert.ToInt32(mRes.param4);
            mJieYaoPickId   = System.Convert.ToInt32(mRes.param6);
            mDamageToMoney  = System.Convert.ToDouble(mRes.param7);
            mDamageToAnger  = System.Convert.ToDouble(mRes.param8);
            mTimerStandIntervalMin = System.Convert.ToUInt32(mRes.param9); 
            mTimerStandIntervalMax = System.Convert.ToUInt32(mRes.param10);
            mStandTimeMin = System.Convert.ToUInt32(mRes.param11);
            mStandTimeMax = System.Convert.ToUInt32(mRes.param12);
            mTimerUseSkillIntervalMin = System.Convert.ToUInt32(mRes.param13);
            mTimerUseSkillIntervalMax = System.Convert.ToUInt32(mRes.param14);
            mMoveRadiusMin = System.Convert.ToSingle(mRes.param15);
            mMoveRadiusMax = System.Convert.ToSingle(mRes.param16);
            mMaxGoldCount = System.Convert.ToUInt32(mRes.param18);
        
            if(mRes.param5 != null)
            {
                string[] pos = mRes.param5.Split(new char[] { ',' });
                if (pos.Length != 3)
                {
                    GameDebug.LogError("猫AI 参数param5错误，正确格式为：1,2,3");
                    return false;
                }

                mGoldPickId1 = System.Convert.ToInt32(pos[0]);
                mGoldPickId2 = System.Convert.ToInt32(pos[1]);
                mGoldPickId3 = System.Convert.ToInt32(pos[2]);
            }

            if(mRes.param17 != null)
            {
                string[] strPosition = mRes.param17.Split(new char[] {';'});
                for(int i = 0; i < strPosition.Length; i++)
                {
                    string [] strVertexs = strPosition[i].Split(new char[] { ',' });
                    if(strVertexs.Length != 2)
                    {
                        GameDebug.LogError("猫AI 参数param17错误，正确格式为：0.0,1.0;2.0,3.0;4.0,5.0");
                        return false;
                    }

                    mJieYaoPosition.Add(new Vector3f(System.Convert.ToSingle(strVertexs[0]), 0.0f, System.Convert.ToSingle(strVertexs[1])));
                }
            }

            if(mRes.param19 != null)
            {
                string[] strPosition = mRes.param19.Split(new char[] { ';' });
                for (int i = 0; i < strPosition.Length; i++)
                {
                    string[] strVertexs = strPosition[i].Split(new char[] { ',' });
                    if (strVertexs.Length != 2)
                    {
                        GameDebug.LogError("猫AI 参数param19错误，正确格式为：0.0,1.0;2.0,3.0;4.0,5.0");
                        return false;
                    }

                    mSceneGoldPosition.Add(new Vector3f(System.Convert.ToSingle(strVertexs[0]), 0.0f, System.Convert.ToSingle(strVertexs[1])));
                }
            }
        }
        catch(Exception e)    
        {
            GameDebug.LogError(e.Message);
            return false;
        }

        return true;
    }

    public override void Destory()
    {
        EventSystem.Instance.removeEventListener(FindPickEvent.FIND_PICK_BOX, OnPicked);

        base.Destory();
    }

    public override void OnBeHit(uint whoId, float value)
    {
        base.OnBeHit(whoId, value);

        if (!mIsFighting)
            return;

        if (mState != BATTLE_STATE.STATE_1)
            return;

        SetAngerValue(mAngerValue + value * mDamageToAnger);

        mMoneyValue += value * mDamageToMoney;
        while(mMoneyValue > MAX_MONEY_VALUE)
        {
            mMoneyValue -= MAX_MONEY_VALUE;
            CreateGold();
        }

        MaoStageUpdateTargetPosEvent e = new MaoStageUpdateTargetPosEvent();
        e.CatPos = mOwner.GetPosition();
        EventSystem.Instance.PushEvent(e);


        mStateTime += 32;

        switch (mState)
        {
            case BATTLE_STATE.STATE_1:
                {
                    if (mAngerValue >= MAX_ANGER_VALUE)
                    {
                        ChangeState(BATTLE_STATE.STATE_2);
                    }
                    else
                    {
                        if (IsTimerTrigger(1, mTimerStandIntervalMin, mTimerStandIntervalMax))
                        {
                            if (BeginCommand(1000))
                            {
                                AddCommand(new AIGoalStand(this, mStandTimeMin, mStandTimeMax));
                            }

                            ResetTimer(1);
                        }

                        if (IsTimerTrigger(0, mTimerUseSkillIntervalMin, mTimerUseSkillIntervalMax))
                        {
                            if (BeginCommand(9000))
                            {
                                if (mSkillList1.Count > 0)
                                {
                                    int index = BaseAI.Random(0, mSkillList1.Count);
                                    int skillid = mSkillList1[index];

                                    AddCommand(new AIGoalUseSkillToTarget(this, GetID(), skillid));
                                }

                                ResetTimer(0);
                            }
                        }
                    }
                }
                break;
            case BATTLE_STATE.STATE_2:
                {
                    if (BeginCommand(10000))
                    {
                        OnCrazy(true);

                        AddCommand(new AIGoalAddBuff(this, GetEnemyPlayerId(), mBuffIdToPlayer));   // 把角色变身，不能攻击
                        AddCommand(new AIGoalAddBuff(this, GetID(), mBuffIdToBoss));
                        AddCommand(new AIGoalUseSkillToTarget(this, GetID(), mSkillId));            // 给自己添加狂暴效果
                        ChangeState(BATTLE_STATE.STATE_3);
                    }
                }
                break;
            case BATTLE_STATE.STATE_3:
                {
                    if (EmptyCommand())
                    {
                        CreateJieYao();
                        ChangeState(BATTLE_STATE.STATE_4);
                    }
                }
                break;
            case BATTLE_STATE.STATE_4:
                {
                    if (mStateTime > mFrenzyMaxTime || mFindJieYao)
                    {
                        ChangeState(BATTLE_STATE.STATE_5);
                    }
                    else
                    {
                        float v = MAX_ANGER_VALUE / mFrenzyMaxTime * 32;
                        if (mAngerValue > v)
                        {
                            SetAngerValue(mAngerValue - v);
                        }
                        else
                        {
                            SetAngerValue(0);
                        }

                        if (BeginCommand(100))
                        {
                            // 进行战斗
                            if (mSkillList2.Count > 0)
                            {
                                int index = BaseAI.Random(0, mSkillList2.Count);
                                int skillid = mSkillList2[index];

                                uint mainTargetId = GetCurrentTargetId();
                                AddCommand(new AIGoalApproachTarget(this, mainTargetId, BaseAI.GetSkillMaxRangle(skillid) * 0.95f));
                                AddCommand(new AIGoalUseSkillToTarget(this, mainTargetId, skillid));
                            }
                        }
                    }
                }
                break;
            case BATTLE_STATE.STATE_5:
                {
                    mFindJieYao = false;
                    if (BeginCommand(10000))
                    {
                        AddCommand(new AIGoalRemoveBuff(this, GetID(), mBuffIdToBoss));               // 移除狂暴效果
                        AddCommand(new AIGoalRemoveBuff(this, GetEnemyPlayerId(), mBuffIdToPlayer));  // 把角色变为正常 
                        ChangeState(BATTLE_STATE.STATE_6);
                    }
                }
                break;
            case BATTLE_STATE.STATE_6:
                {
                    if (EmptyCommand())
                    {
                        OnCrazy(false);

                        SetAngerValue(0);
                        ChangeState(BATTLE_STATE.STATE_1);
                    }
                }
                break;
            default:
                break;
        }

        if (TimerLeftMillisecond(0) > 1000 && TimerLeftMillisecond(1) > 1000)
        {
            if (BeginCommand(200))
            {
                AddCommand(new AIGoalRun(this, CaleMovePosition()));
            }
        }
    }

    private void SetAngerValue(double value)
    {
        double oldValue = mAngerValue;

        if(value > MAX_ANGER_VALUE)
        {
            mAngerValue = MAX_ANGER_VALUE;
        }
        else if(value < 0.0f)
        {
            mAngerValue = 0.0f;
        }
        else
        {
            mAngerValue = value;
        }

//         MaoStageUpdageAngerEvent e = new MaoStageUpdageAngerEvent();
//         e.Value = (float)(mAngerValue / MAX_ANGER_VALUE);
//         EventSystem.Instance.PushEvent(e);
    }

    private void CreateGold()
    {
        // 创建金币 pick
        Vector3f pos = BaseAI.GetPosition(GetID());

        PickInitParam param = new PickInitParam();
        param.pick_res_id = mGoldPickId1;
        param.init_dir = 0;
        param.init_pos = new UnityEngine.Vector3(pos.x, pos.y, pos.z);

        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return;

        scn.CreateSprite(param);
    }

    private void OnPicked(EventBase e)
    {
        FindPickEvent ev = e as FindPickEvent;
        if (ev == null)
            return;

        if (!BaseAI.IsPlayer((uint)ev.OwnerId))
            return;

        if(ev.PickResId == mJieYaoPickId)
        {
            OnGetJieYao(ev.Position);
        }
    }

    private void OnGetJieYao(Vector3 pickpos)
    {
        mFindJieYao = true;
    }

    private void CreateJieYao()
    {
        Vector3f pos = BaseAI.GetPosition(GetID());

        if(mJieYaoPosition.Count > 0)
        {
            int index = BaseAI.Random(0, mJieYaoPosition.Count - 1);
            pos = mJieYaoPosition[index];
        }

        PickInitParam param = new PickInitParam();
        param.pick_res_id = mJieYaoPickId;
        param.init_dir = 0;
        param.init_pos = new UnityEngine.Vector3(pos.x, pos.y, pos.z);

        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return;

        scn.CreateSprite(param);
    }

    public override void OnEnterCombat()
    {
        mIsFighting = true;
        mState = BATTLE_STATE.STATE_1;
        mAngerValue = 0;
        mStateTime  = 0;
        mMoneyValue = 0;
        mFindJieYao = false;

        // 事件可能会引起内存泄露，需要处理
        EventSystem.Instance.addEventListener(FindPickEvent.FIND_PICK_BOX, OnPicked);
    }

    public override void OnExitCombat()
    {
        mIsFighting = false;
        mState = BATTLE_STATE.STATE_1;
        mAngerValue = 0;
        mStateTime  = 0;
        mMoneyValue = 0;

        EventSystem.Instance.removeEventListener(FindPickEvent.FIND_PICK_BOX, OnPicked);
    }

    private void ChangeState(BATTLE_STATE toState)
    {
        if (mState == toState)
            return;

        mStateTime = 0;
        mState = toState;
    }

    private Vector3f CaleMovePosition()
    {
        Vector3f tarPos = new Vector3f();

        Vector3f pos = BaseAI.GetPosition(GetID());
        float dir = BaseAI.GetDirection(GetID());

        for(int j = 0; j < 10; j++)
        {
            for(int i = 0; i < 2; i++)
            {
                float toDir = dir + BaseAI.Randomf(-20 * j, 20 * j) * UnityEngine.Mathf.Deg2Rad;
                tarPos.x = (float)(pos.x + Math.Sin(toDir) * 10);
                tarPos.z = (float)(pos.z + Math.Cos(toDir) * 10);

                if (BaseAI.CanMoveTo(GetID(), pos, tarPos))
                    return tarPos;
            }
        }
        return tarPos;
    }

    public override void OnUpdateCombat(uint elapsed)
    {
        
    }

    private void OnCrazy(bool crazy)
    {
//         MaoStageCrazyEvent e = new MaoStageCrazyEvent();
//         e.Crazy = crazy;
//         EventSystem.Instance.PushEvent(e);

        if(crazy)
        {
            foreach(Vector3f pos in mSceneGoldPosition)
            {
                PickInitParam param = new PickInitParam();
                param.pick_res_id = mGoldPickId2;
                param.init_dir = 0;
                param.init_pos = new UnityEngine.Vector3(pos.x, pos.y, pos.z);

                BaseScene scn = SceneManager.Instance.GetCurScene();
                if (scn == null)
                    return;

                scn.CreateSprite(param);
            }
        }
    }

};