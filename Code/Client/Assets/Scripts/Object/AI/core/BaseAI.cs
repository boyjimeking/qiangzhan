
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseAI
{
    protected BattleUnit mOwner = null;
    protected uint mId = 0xFFFFFFFF;
    protected AITableItem mRes = null;

    protected int mSkill1  = -1;
    protected int mSkill2  = -1;
    protected int mSkill3 = -1;
    protected int mSkill4 = -1;
    protected int mSkill5 = -1;
    protected int mSkill6 = -1;
    protected int mSkill7 = -1;
    protected int mSkill8 = -1;
    protected int mSkill9 = -1;
    protected int mSkill10 = -1;
    protected int mSkill11 = -1;
    protected int mSkill12 = -1;
    protected int mSkill13 = -1;
    protected int mSkill14 = -1;
    protected int mSkill15 = -1;

    public enum MoveMode : int
    {
        MOVE_INVALID = -1,

        /** 跑 */
        MOVE_RUN = 0,

        /** 走 */
        MOVE_WALK = 1,

        /** 急跑 */
        MOVE_SCURRY = 2,

        MOVE_MAOYAO_RUN = 3,
    };

    public BaseAI(BattleUnit owner)
    {
        mOwner = owner;

        if(mOwner != null)
        {
            mId = mOwner.InstanceID;
        }
    }

    public virtual bool Init(int aiId)
    {
        if (!DataManager.AITable.ContainsKey(aiId))
            return false;

        AITableItem aiItem = DataManager.AITable[aiId] as AITableItem;
        if (aiItem == null)
            return false;

        mRes = aiItem;

        mSkill1 = mRes.skillslot1;
        mSkill2 = mRes.skillslot2;
        mSkill3 = mRes.skillslot3;
        mSkill4 = mRes.skillslot4;
        mSkill5 = mRes.skillslot5;
        mSkill6 = mRes.skillslot6;
        mSkill7 = mRes.skillslot7;
        mSkill8 = mRes.skillslot8;
        mSkill9 = mRes.skillslot9;
        mSkill10 = mRes.skillslot10;
        mSkill11 = mRes.skillslot11;
        mSkill12 = mRes.skillslot12;
        mSkill13 = mRes.skillslot13;
        mSkill14 = mRes.skillslot14;
        mSkill15 = mRes.skillslot15;
        return true;
    }

    public virtual void Destory()
    {

    }

    public virtual void Update(uint elapsed)
    {
    }

    public BattleUnit GetOwner()
    {
        return mOwner;
    }

    public uint GetID()
    {
        if (mOwner == null)
            return 0xFFFFFFFF;

        return mOwner.InstanceID;
    }

    public virtual void OnBeHit(uint whoId, float value)
    {
    }

    public virtual void OnBeImpact(uint whoId, bool good)
    {
    }

    public virtual bool IsEnemy(uint targetId)
    {
        return BaseAI.IsEnemy(GetID(), targetId);    
    }

    public int GetSkillId(int slot)
    {
        if (mRes == null)
            return -1;

        if (slot <= 0 || slot > 15)
            return -1;

        switch(slot)
        {
            case 1:
				return mSkill1;
            case 2:
                return mSkill2;
            case 3:
                return mSkill3;
            case 4:
                return mSkill4;
            case 5:
                return mSkill5;
            case 6:
                return mSkill6;
            case 7:
                return mSkill7;
            case 8:
                return mSkill8;
            case 9:
                return mSkill9;
            case 10:
                return mSkill10;
            case 11:
                return mSkill11;
            case 12:
                return mSkill12;
            case 13:
                return mSkill13;
            case 14:
                return mSkill14;
            case 15:
                return mSkill15;
            default:
                return -1;
        }
    }

    static public bool CheckSkillCd(uint objId,int skillid)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return false;

        BattleUnit battleUnit = (BattleUnit)obj;

        if ((int)battleUnit.GetSkillCD(skillid) == 0)
            return true;
        else
            return false;
    }

    static public bool CheckSkillCost(uint objId, int skillid)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return false;

        BattleUnit battleUnit = (BattleUnit)obj;

        if (battleUnit.CheckSkillCost(skillid) == ErrorCode.InsufficientMana)
            return false;
        else
            return true;
    }

    static public float GetDirection(uint objId)
    {
         BaseScene scn = SceneManager.Instance.GetCurScene();
         if (scn == null)
             return 0.0f;

         ObjectBase obj = scn.FindObject(objId);
         if (obj == null)
             return 0.0f;

         return obj.GetDirection();
    }

    static public Vector3f GetMoveTargetPosition(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return new Vector3f();

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return new Vector3f();

        BattleUnit battleUnit = (BattleUnit)obj;
        Vector3f movPos = battleUnit.GetMoveTargetPos();
  
        return movPos;
    }

    static public Vector3f GetPosition(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return new Vector3f();

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null)
            return new Vector3f();

        return obj.GetPosition3f();
    }

    static public bool MoveTo(uint objId, Vector3f tarPos)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return false;

        BattleUnit battleUnit = (BattleUnit)obj;
        return battleUnit.MoveTo(tarPos);
    }

    static public void StopMove(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return;

        BattleUnit battleUnit = (BattleUnit)obj;
        battleUnit.StopMove();
    }

    static public bool UseSkillToTarget(uint objId, int skillId, uint targetId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return false;

        ObjectBase tarObj = scn.FindObject(targetId);
        if (tarObj == null || !ObjectType.IsBattleUnit(tarObj.Type))
            return false;

        BattleUnit battleUnit = (BattleUnit)obj;
        BattleUnit tarUnit = (BattleUnit)tarObj;
        battleUnit.StopMove();
        return ( battleUnit.UseSkill(skillId, tarUnit.GetPosition()) == ErrorCode.Succeeded );
    }

    static public bool UseSkillToPosition(uint objId, int skillId, Vector3f tarPosition)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return false;

        BattleUnit battleUnit = (BattleUnit)obj;
        //battleUnit.StopMove();
		return (battleUnit.UseSkill(skillId, new UnityEngine.Vector3(tarPosition.x, tarPosition.y, tarPosition.z)) == ErrorCode.Succeeded);
    }

    static public bool UseSkillToDirection(uint objId, int skillId, float dir)
    {
        // todo : 需要处理 方向没起作用
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return false;

        BattleUnit battleUnit = (BattleUnit)obj;
        battleUnit.StopMove();
		return (battleUnit.UseSkill(skillId, Vector3.zero) == ErrorCode.Succeeded);
    }

    static public bool IsUseSkill(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return false;

        BattleUnit battleUnit = (BattleUnit)obj;
        return battleUnit.IsSkillUsing();
    }

    static public bool Say(uint objId, int talkId)
    {
        return true;
    }

    static public void SetMoveMode(uint objId, MoveMode mode)
    {

    }

    static public bool isMoving(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return false;

        BattleUnit battleUnit = (BattleUnit)obj;
        return battleUnit.IsMoveing();
    }

    static public bool SceneMayStraightReach(Vector3f src, Vector3f tar)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        Vector3 farthest;
        return !scn.TestLineBlock(new Vector3(src.x, src.y, src.z), new Vector3(tar.x, tar.y, tar.z), true, true, out farthest);
    }

    static public void DestoryObject(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null)
            return;

        obj.Disappear();
    }

    static public void LookAt(uint objId, Vector3f tar)
    {

    }

    static public bool IsValid(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        return obj != null;
    }

    static public bool IsAlive(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return false;

        BattleUnit battleUnit = (BattleUnit)obj;
        return battleUnit.isAlive();
    }

    static public bool IsDeath(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return false;

        BattleUnit battleUnit = (BattleUnit)obj;
        return battleUnit.IsDead();
    }

    static public bool IsEnemy(uint objId, uint tarId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return false;

        ObjectBase tarObj = scn.FindObject(tarId);
        if (tarObj == null || !ObjectType.IsBattleUnit(tarObj.Type))
            return false;

        BattleUnit battleUnit = (BattleUnit)obj;
        BattleUnit tarUnit = (BattleUnit)tarObj;

        return battleUnit.IsEnemy(tarUnit);
    }

    static public bool IsVisable(uint objId, uint tarId)
    {
        return true;
    }

    static public void StopAllSkill(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return;

        BattleUnit battleUnit = (BattleUnit)obj;
        battleUnit.StopAllSkills();
    }

    static public void TurnTo(uint objId, Vector3f tarPos)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return;

        BattleUnit battleUnit = (BattleUnit)obj;
        battleUnit.TurnTo(tarPos);
    }

    static public void Turn(uint objId, float angle)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return;

        BattleUnit battleUnit = (BattleUnit)obj;
        battleUnit.Turn(angle);
    }

    static public Vector3f GetHomePosition(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return new Vector3f();

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null || !ObjectType.IsBattleUnit(obj.Type))
            return new Vector3f();

        BattleUnit battleUnit = (BattleUnit)obj;
        return battleUnit.GetHomePosition();
    }

    static public float CalcDirection(Vector3f s, Vector3f e)
    {
        UnityEngine.Vector3 sv = new UnityEngine.Vector3(s.x, 0.0f, s.z);
        UnityEngine.Vector3 ev = new UnityEngine.Vector3(e.x, 0.0f, e.z);
        
        return Utility.Vector3ToRadian(ev - sv);
    }

    static public float GetSkillMaxRangle(int skillId)
    {
        if (!DataManager.SkillCommonTable.ContainsKey(skillId))
            return 0.0f;

        SkillCommonTableItem item = DataManager.SkillCommonTable[skillId] as SkillCommonTableItem;
        if (item == null)
            return 0.0f;

        return item.maxRange;
    }

    static public float GetSkillMinRangle(int skillId)
    {
        if (!DataManager.SkillCommonTable.ContainsKey(skillId))
            return 0.0f;

        SkillCommonTableItem item = DataManager.SkillCommonTable[skillId] as SkillCommonTableItem;
        if (item == null)
            return 0.0f;

        return item.minRange;
    }

    static public int Random(int min, int max)
    {
        if( min > max )
        {
            int tmp = min;
            min = max;
            max = min;
        }

        return UnityEngine.Random.Range(min, max);
    }

    static public float Randomf(float min, float max)
    {
        if (min > max)
        {
            float tmp = min;
            min = max;
            max = min;
        }

        return UnityEngine.Random.Range(min, max);
    }

    static public Vector3f get_position_angle_and_distance_position(Vector3f pos, float angle, float distance)
    {
	    float x = pos.x + (float)System.Math.Sin(angle) * distance;
	    float z = pos.z + (float)System.Math.Cos(angle) * distance;

        return new Vector3f(x, 0.0f, z);
    }

    static public float get_direction(uint sourceId, uint targetId)
    {
        Vector3f sp = BaseAI.GetPosition(sourceId);
        Vector3f ep = BaseAI.GetPosition(targetId);

        return BaseAI.CalcDirection(sp, ep);
    }

    public bool scene_postion_arrive(Vector3f source, Vector3f target)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

		return scn.FindPath(mOwner, new Vector2f(source.x, source.z), new Vector2f(target.x, target.z)) != null;
    }

    public bool scene_may_straight_reach(Vector3f source, Vector3f target)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

		List<Vector2f> rst = scn.FindPath(mOwner, new Vector2f(source.x, source.z), new Vector2f(target.x, target.z));
        if (rst == null)
            return false;

        return rst.Count == 2;
    }

    static public float GetObjectDistance(uint srcId, uint descId)
    {
        Vector3f p1 = GetPosition(srcId);

        Vector3f p2 = GetPosition(descId);

        Vector3f d = p1.Subtract(p2);
        return (float)System.Math.Sqrt(d.x * d.x + d.z * d.z); 
    }

    static public void SetObjectPos(uint objId, Vector3f pos)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null)
            return;

        obj.SetPosition3f(pos);
    }

    static public void PlayAni(uint objId, string aniName)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null)
            return;

        VisualObject vObj = obj as VisualObject;
        if (vObj == null)
            return;

        AnimActionPlayAnim action =  AnimActionFactory.Create(AnimActionFactory.E_Type.PlayAnim) as AnimActionPlayAnim;
        action.AnimName = aniName;

        vObj.GetStateController().DoAction(action);
        return;
    }

    static public void UseSkillEffect(uint objId, SkillEffectType type, uint resID)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return ;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null)
            return ;

        BattleUnit npc = (BattleUnit)obj;
        npc.AddSkillEffect(new AttackerAttr(npc),type,resID);
    }

    static public float GetHpPercent(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return 0.0f;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null)
            return 0.0f;

        BattleUnit npc = (BattleUnit)obj;
        return (float)npc.GetHP() / npc.GetMaxHP();
    }

    static public void FlyIntoaRage(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return ;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null)
            return ;

        BattleUnit npc = (BattleUnit)obj;
        npc.FlyIntoaRage();
    }

    static public void LeaveFromaRange(uint objId) 
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null)
            return;

        BattleUnit npc = (BattleUnit)obj;
        npc.LeaveFromaRange();
    }

    static public bool IsPlayer(uint objId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null)
            return false;

        return ObjectType.IsPlayer(obj.Type);
    }

    static public bool AddBuffByResId(uint objId, uint target, int buffResId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        BattleUnit tar = scn.FindObject(target) as BattleUnit;
        if (tar == null)
            return false;

        BattleUnit sender = scn.FindObject(objId) as BattleUnit;
        if (sender == null)
            return false;

        return tar.AddSkillEffect(new AttackerAttr(sender), SkillEffectType.Buff, (uint)buffResId) == ErrorCode.Succeeded;
    }

    static public bool RemoveBuffByResId(uint objId, int buffResId)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        BattleUnit unit = scn.FindObject(objId) as BattleUnit;
        if (unit == null)
            return false;

        return unit.RemoveSkillBuffByResID((uint)buffResId) == ErrorCode.Succeeded;
    }

    static public void PromptUI(string info)
    {
        PromptUIManager.Instance.AddNewPrompt(info);
    }

    static public bool CanMoveTo(uint objId, Vector3f srcPos, Vector3f tarPos)
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        ObjectBase obj = scn.FindObject(objId);
        if (obj == null)
            return false;

        return scn.FindPath(obj, new Vector2f(srcPos.x, srcPos.z), new Vector2f(tarPos.x, tarPos.z)) != null;
    }
};