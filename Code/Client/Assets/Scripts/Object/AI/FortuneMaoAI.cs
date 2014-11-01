
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

public class FortuneMaoAI : CommonAI
{
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

    private uint mStateTime     = 0;
    private double mMoneyValue   = 0;            // 当前money值, 当money值大于等于 100 时会掉一个金币
    private bool mFindJieYao    = false;        // 是否已找到解药
    private int mGoldCount      = 0;            // 已获得的 金币数量
    private double mAngerValue   = 0;            // 当前怒气值, 当努气值大于等于100时，会进入狂暴状态

    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new FortuneMaoAI(battleUnit);
    }

    public FortuneMaoAI(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        try
        {
            mGoldPickId1 = System.Convert.ToInt32(mRes.param1);
            mDamageToMoney  = System.Convert.ToDouble(mRes.param2);
        }
        catch(Exception e)    
        {
            GameDebug.LogError(e.Message);
            return false;
        }

        return true;
    }

    public override void OnBeHit(uint whoId, float value)
    {
        base.OnBeHit(whoId, value);

        SetAngerValue(mAngerValue + value * mDamageToAnger);

        mMoneyValue += value * mDamageToMoney;
        while(mMoneyValue > MAX_MONEY_VALUE)
        {
            mMoneyValue -= MAX_MONEY_VALUE;
            CreateGold();
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

        //MaoStageUpdageAngerEvent e = new MaoStageUpdageAngerEvent();
        //e.Value = (float)(mAngerValue / MAX_ANGER_VALUE);
        //EventSystem.Instance.PushEvent(e);
    }

    private void CreateGold()
    {
        // 创建金币 pick
        Vector3f pos = BaseAI.GetPosition(GetID()); ;

        PickInitParam param = new PickInitParam();
        param.pick_res_id = mGoldPickId1;
        param.init_dir = 0;
        param.init_pos = new UnityEngine.Vector3(pos.x, pos.y, pos.z);

        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return;

        scn.CreateSprite(param);
    }

    public override void OnEnterCombat()
    {
    }

    public override void OnExitCombat()
    {
    }

    public override void OnUpdateCombat(uint elapsed)
    {
        
    }

    private void OnPicked(EventBase e)
    {
        FindPickEvent ev = e as FindPickEvent;
        if (ev == null)
            return;

        if (!BaseAI.IsPlayer((uint)ev.OwnerId))
            return;
        OnGetGold(ev.Position);
    }

    private void OnGetGold(Vector3 pickpos)
    {
        MaoStageUpdateGoldEvent e = new MaoStageUpdateGoldEvent();
        e.CurrentGold = mGoldCount;
        e.TotalGold = (int)mMaxGoldCount;
        e.PickPos = pickpos;

        EventSystem.Instance.PushEvent(e);
    }

    public override void Destory()
    {
        base.Destory();
    }
};