using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public abstract class CommonAI : BattleUnitAI
{
    private class Timer
    {
        private uint mMinValue = 0;
        private uint mMaxValue = 0;
        private uint mCurValue = 0;
        
        public Timer(uint minValue, uint maxValue)
        {
            mMinValue = minValue;
            mMaxValue = maxValue;

            Reset();
        }

        public void Tick(uint elapsed)
        {
            if (mCurValue < elapsed)
                mCurValue = 0;
            else
                mCurValue -= elapsed;
        }

        public bool IsTrigger()
        {
            return mCurValue == 0;
        }

        public void Reset()
        {
            mCurValue = (uint)UnityEngine.Random.Range((int)mMinValue, (int)mMaxValue);
        }
        
        public uint LeftTime()
        {
            return mCurValue;
        }
    };

    private AIStateIdle   mStateIdle   = null;
    private AIStateGoHome mStateGoHome = null;
    private AIStateCombat mStateCombat = null;
    private bool mFlowIdle = false;

    private AIState mStateCurrent = null;

    private List<uint> mEnemyList = new List<uint>();   // 敌人列表

    private Vector3f mCombatStartPos = new Vector3f();  // 最近一次战斗开始的位置

    private uint mSearchTime = uint.MaxValue;

    private Dictionary<uint, Timer> mTimerList = new Dictionary<uint, Timer>();

    //private uint mCurTargetId = uint.MaxValue;

    public CommonAI(BattleUnit battleUnit,bool flowIdle = false) : base(battleUnit)
    {
        mStateIdle      = new AIStateIdle(this);
        mStateGoHome    = new AIStateGoHome(this);
        mStateCombat    = new AIStateCombat(this);

        mFlowIdle = flowIdle;
        mStateCurrent   = null;
    }

    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        // 解析idle状态默认行为
        AIGoal goal = CommonAI.ParseGoalXML(this, mRes.commonIdle);
        if(goal != null)
        {
            mStateIdle.AddCommand(goal);
        }
       
        return true;
    }

    public override void Update(uint elapsed)
    {
        base.Update(elapsed);

        foreach(KeyValuePair<uint, Timer> pair in mTimerList)
        {
            pair.Value.Tick(elapsed);
        }

        mSearchTime += elapsed;

        if(mStateCurrent == null)
        {
            ChangeState(mStateIdle);
        }

        if(mStateCurrent == mStateIdle)
        {
            SearchEnemy();
         
            if(GetEnemyCount() > 0)
            {
                mCombatStartPos = BaseAI.GetPosition(GetID());
                ChangeState(mStateCombat);
            }

        }
        else if(mStateCurrent == mStateCombat)
        {
            SearchEnemy();
            CheckEnemy();

            if (GetEnemyCount() <= 0)
            {
                if (mFlowIdle)
                    ChangeState(mStateIdle);
                else
                    ChangeState(mStateGoHome);
            }

        }
        else if(mStateCurrent == mStateGoHome)
        {

        }

        if(!mStateCurrent.Update(elapsed))
        {
            ChangeState(mStateIdle); 
        }
    }

    public override void OnBeHit(uint whoId, float value)
    {
        if (mStateCurrent == mStateGoHome)
            return;

        if (!IsEnemy(whoId))
            return;

        AddEnemy(whoId);
    }

    public override void OnBeImpact(uint whoId, bool good)
    {
        if (mStateCurrent == mStateGoHome)
            return;

        if(!good)
        {
            AddEnemy(whoId);
        }
    }

    public override bool IsEnemy(uint targetId)
    {
        if (!base.IsEnemy(targetId))
            return false;

        if (mRes != null && mRes.checkway != 0)
        {
            Vector3f pos = BaseAI.GetPosition(GetID());
            Vector3f tarPos = BaseAI.GetPosition(targetId);

            return BaseAI.CanMoveTo(GetID(), pos, tarPos);
        }

        return true;
    }

    public int GetEnemyCount()
    {
        return mEnemyList.Count;
    }

    public void RemoveAllEnemy()
    {
        mEnemyList.Clear();
    }

    public void AddEnemy(uint objId)
    {
        if (mEnemyList.IndexOf(objId) >= 0)
            return;

        mEnemyList.Add(objId);
    }

    public void RemoveEnemy(uint objId)
    {
        if (mEnemyList.IndexOf(objId) < 0)
            return;

        mEnemyList.Remove(objId);
    }

    public void CheckEnemy()
    {
        BaseScene scn = mOwner.Scene;
        if (scn == null)
            return;

        List<uint> removeList = new List<uint>();
        foreach(uint id in mEnemyList)
        {
            if (!BaseAI.IsValid(id))
                removeList.Add(id);
            else if (!IsEnemy(id))
                removeList.Add(id);
            else if (!BaseAI.IsAlive(id))
                removeList.Add(id);
            else if (BaseAI.IsDeath(id))
                removeList.Add(id);
            else if (!BaseAI.IsVisable(GetID(), id))
                removeList.Add(id);
            else
            {
                Vector3f combatPos = GetCombatStartPosition();
                combatPos.y = 0.0f;

                Vector3f enemyPos = BaseAI.GetPosition(id);
                enemyPos.y = 0.0f;

                if((combatPos - enemyPos).Length() > mRes.chaseDist)
                {
                    removeList.Add(id);
                }
            }
        }

        foreach(uint id in removeList)
        {
            RemoveEnemy(id);
        }
    }

    public void SearchEnemy()
    {
        if (mSearchTime < 500)
            return;

        mSearchTime = 0;

        BaseScene scn = mOwner.Scene;
        if (scn == null)
            return;

        Vector3f pos = BaseAI.GetPosition(GetID());

        ArrayList enemyList = scn.SearchBattleUnit(new Vector2f(pos.x, pos.z), mRes.searchEnemyRadius);
        if (enemyList == null)
            return;

        for (int i = 0; i < enemyList.Count; i++)
        {
            BattleUnit battleUnit = enemyList[i] as BattleUnit;
            if (battleUnit == null)
                continue;

            uint id = battleUnit.InstanceID;
            if (GetID() == id)
                continue;

            if (!IsEnemy(id))
                continue;

            if (!BaseAI.IsAlive(id))
                continue;

            if (!BaseAI.IsVisable(GetID(), id))
                continue;

            if (BaseAI.IsDeath(id))
                continue;

            AddEnemy(id);
        }
    }

    public void ChangeState(AIState newState)
    {
        if (newState == null)
            return;

        if (mStateCurrent == newState)
            return;

        if (mStateCurrent != null)
            mStateCurrent.Exit();

        mStateCurrent = newState;

        mStateCurrent.Enter();
    }

    public Vector3f GetCombatStartPosition()
    {
        return mCombatStartPos;
    }

    public virtual void OnEnterIdle()
    {
    }

    public virtual void OnExitIdle()
    {
    }

    public virtual void OnUpdateIdle(uint elapsed)
    {
    }

    public virtual void OnEnterCombat()
    {
    }

    public virtual void OnExitCombat()
    {
    }

    public virtual void OnUpdateCombat(uint elapsed)
    {
    }

    public bool BeginCommand(int priority)
    {
        if (mStateCurrent == null)
            return false;

        if (BaseAI.IsUseSkill(GetID()))
            return false;

        return mStateCurrent.BeginCommand(priority);
    }

    public bool AddCommand(AIGoal goal)
    {
        if (mStateCurrent == null)
            return false;

        mStateCurrent.AddCommand(goal);
        return true;
    }

    public bool EmptyCommand()
    {
        if (mStateCurrent == null)
            return false;

        return mStateCurrent.EmptyCommand();
    }

    public bool RemoveCommand()
    {
        if (mStateCurrent == null)
            return false;

        mStateCurrent.RemoveCommand();
        return true;
    }

    public uint GetCurrentTargetId()
    {
        if (mEnemyList.Count == 0)
            return uint.MaxValue;

        Vector3f posn = BaseAI.GetPosition(GetID());
        uint mEnemyId = uint.MaxValue;
        float mMinDis = float.MaxValue;
        for (int i = 0; i < mEnemyList.Count; ++i)
        { 
            Vector3f enemypos = BaseAI.GetPosition(mEnemyList[i]);
            float dis = posn.Subtract(enemypos).Length();
            if (dis < mMinDis)
            {
                mEnemyId = mEnemyList[i];
                mMinDis = dis;
            }
        }
        return mEnemyId;
    }

    static public AIGoal ParseGoalXML(CommonAI ai, string xmlData)
    {
        if (xmlData == null)
            return null;

        if (xmlData.Length == 0)
            return null;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlData);

        XmlNode node = xmlDoc.FirstChild;
        if (node.Name != "root")
            return null;

        bool loop = node.Attributes["loop"] != null ? System.Convert.ToBoolean(node.Attributes["loop"].Value) : false;
        AIGoalCompositeGoal rootGoal = new AIGoalCompositeGoal(ai, loop);

        XmlNodeList nodeList = node.ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            AIGoal goal = ParseGoalNode(ai, nodeList[i]);
            if (goal == null)
                return null;

            rootGoal.AddSubGoal(goal);
        }

        return rootGoal;
    }

    static public AIGoal ParseGoalNode(CommonAI ai, XmlNode node)
    {
        if (ai == null)
            return null;

        if (node == null)
            return null;

        if(node.Name == "walk")
        {
            if (node.Attributes["x"] == null ||
                node.Attributes["y"] == null ||
                node.Attributes["z"] == null)
                return null;

            float x = System.Convert.ToSingle(node.Attributes["x"].Value);
            float y = System.Convert.ToSingle(node.Attributes["y"].Value);
            float z = System.Convert.ToSingle(node.Attributes["z"].Value);

            Vector3f tarPos = new Vector3f(x, y, z);
            return new AIGoalWalk(ai, tarPos);
        }
        else if(node.Name == "run")
        {
            if (node.Attributes["x"] == null ||
                node.Attributes["y"] == null ||
                node.Attributes["z"] == null)
                return null;

            float x = System.Convert.ToSingle(node.Attributes["x"].Value);
            float y = System.Convert.ToSingle(node.Attributes["y"].Value);
            float z = System.Convert.ToSingle(node.Attributes["z"].Value);

            Vector3f tarPos = new Vector3f(x, y, z);
            return new AIGoalRun(ai, tarPos);
        }
        else if(node.Name == "wait")
        {
            if (node.Attributes["time"] == null)
                return null;

            uint millisecond = System.Convert.ToUInt32(node.Attributes["time"].Value);
            return new AIGoalWait(ai, millisecond);
        }
        else if(node.Name == "useskilltoposition")
        {
            if (node.Attributes["x"] == null ||
               node.Attributes["y"] == null ||
               node.Attributes["z"] == null ||
                node.Attributes["skillid"] == null)
                return null;

            float x = System.Convert.ToSingle(node.Attributes["x"].Value);
            float y = System.Convert.ToSingle(node.Attributes["y"].Value);
            float z = System.Convert.ToSingle(node.Attributes["z"].Value);
            int skillId = System.Convert.ToInt32(node.Attributes["skillid"].Value);

            Vector3f tarPos = new Vector3f(x, y, z);
            return new AIGoalUseSkillToPosition(ai, skillId, tarPos);
        }
        else if(node.Name == "say")
        {
            if (node.Attributes["talkid"] == null)
                return null;

            int talkid = System.Convert.ToInt32(node.Attributes["talkid"].Value);
            return new AIGoalSay(ai, talkid);

        }
        else if(node.Name == "randmoveradius")
        {
            if (node.Attributes["minr"] == null ||
                node.Attributes["maxr"] == null)
                return null;

            float minRadius = System.Convert.ToSingle(node.Attributes["minr"].Value);
            float maxRadius = System.Convert.ToSingle(node.Attributes["maxr"].Value);

            return new AIGoalRandMoveRadius(ai, minRadius, maxRadius);
        }
        else if(node.Name == "stand")
        {
            if (node.Attributes["mintime"] == null ||
                node.Attributes["maxtime"] == null)
                return null;

            uint minMillisecond = System.Convert.ToUInt32(node.Attributes["mintime"].Value);
            uint maxMillisecond = System.Convert.ToUInt32(node.Attributes["maxtime"].Value);
            return new AIGoalStand(ai, minMillisecond, maxMillisecond);
        }
        else if(node.Name == "destoryself")
        {
            return new AIGoalDestorySelf(ai);
        }
        else if (node.Name == "turnto")
        {
            if (node.Attributes["x"] == null ||
              node.Attributes["y"] == null ||
              node.Attributes["z"] == null)
                return null;

            float x = System.Convert.ToSingle(node.Attributes["x"].Value);
            float y = System.Convert.ToSingle(node.Attributes["y"].Value);
            float z = System.Convert.ToSingle(node.Attributes["z"].Value);

            Vector3f tarPos = new Vector3f(x, y, z);
            return new AIGoalTurnToPosition(ai, tarPos);
        }
        else if(node.Name == "ani")
        {
            if (node.Attributes["name"] == null)
                return null;

            string name = node.Attributes["name"].Value;
            return new AIGoalPlayAni(ai, name);
        }

        return null;
    }

    public bool IsTimerTrigger(uint id, uint minValue, uint maxValue)
    {
        if(mTimerList.ContainsKey(id))
        {
            return mTimerList[id].IsTrigger();
        }
        else
        {
            mTimerList.Add(id, new Timer(minValue, maxValue));
            return false;
        }
    }

    public void ResetTimer(uint id)
    {
        if(!mTimerList.ContainsKey(id))
            return;

        mTimerList[id].Reset();
    }

    public void ClearTimer()
    {
        mTimerList.Clear();
    }

    public uint TimerLeftMillisecond(uint id)
    {
        if (mTimerList.ContainsKey(id))
            return mTimerList[id].LeftTime();

        return uint.MaxValue;
    }

    public uint GetEnemyPlayerId()
    {
        for(int i = 0; i < mEnemyList.Count; i++)
        {
            if(BaseAI.IsPlayer(mEnemyList[i]))
            {
                return mEnemyList[i];
            }
        }

        return uint.MaxValue;
    }

    public uint GetNearestEnemyId()
    {
        float nearest_distance = float.MaxValue;
        uint enemyid = uint.MaxValue;
 
        for (int i = 0; i < mEnemyList.Count; i++)
        {
            float dist = BaseAI.GetObjectDistance(GetID(), mEnemyList[i]);
            if(dist < nearest_distance)
            {
                nearest_distance = dist;
                enemyid = mEnemyList[i];
            }

        }

        return enemyid;
    }
};