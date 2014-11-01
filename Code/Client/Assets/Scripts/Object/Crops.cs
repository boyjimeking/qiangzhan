using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class CropsInitParam : RoleInitParam
{
    public int crops_res_id = -1;
    public uint lifeTime = uint.MaxValue;
    public AttackerAttr summonerAttr = new AttackerAttr(null);
    public int talk_id = -1;
    public LeagueDef league = LeagueDef.InvalidLeague;
    public CropsItemInfo cropsinfo = null;
}

public class Crops : Role
{
    protected NPCTableItem mRes = null;

    private BloodNode mBloodNode = null;

    private uint mBornEffectID = uint.MaxValue;

    private int mBornEffectTime = 0;

    private int mBornAniTime = 0;

    private int mTalkTime = 0;

    private int mTalkID = -1;

    private int cdTime = 0;

    private int mCryTime = 0;

    private AttackerAttr mSummonerAttr;

    public int resid = int.MaxValue;
    

    public Crops()
    {

    }
    override public bool Init(ObjectInitParam param)
    {
        CropsInitParam cropsParam = (CropsInitParam)param;

        if (!DataManager.NPCTable.ContainsKey(cropsParam.crops_res_id))
        {
            return false;
        }
        mRes = DataManager.NPCTable[cropsParam.crops_res_id] as NPCTableItem;
        mModelResID = mRes.model;

        if (!base.Init(param))
            return false;

        resid = mRes.resID;
        mBattleUintAI = AIFactory.Instance.CreateAIObject(this, mRes.ai);
        if (mBattleUintAI == null)
            return false;

        if (cropsParam.talk_id >= 0)
        {
            mTalkID = cropsParam.talk_id;
        }
        else
        {
            mTalkID = mRes.talkID;
        }

        if (cropsParam.league != LeagueDef.InvalidLeague)
        {
            SetLeague(cropsParam.league);
        }
        else
        {
            SetLeague(mRes.league);
        }

        mDestroyWaiting = true;
        mMaxDisappearTime = mRes.DisappearTime;
        mMaxWaitDisappearTime = mRes.WaitDisappearTime;

        mSummonerAttr = cropsParam.summonerAttr;

        InitProperty(cropsParam);

        if (mRes.bossHpUnit < 0 && mRes.showHp)
        {
            mBloodNode = BloodUIManager.Instance.CreateBloodUI();
        }

        GetCrySound();
        return true;
    }

    public override uint SummonerID
    {
        get { return mSummonerAttr.AttackerID; }
    }

    public override string SummonerName
    {
        get { return mSummonerAttr.AttackerName; }
    }

    public override uint SummonerLevel
    {
        get { return mSummonerAttr.AttackerLevel; }
    }

    override public int Type
    {
        get
        {
            return ObjectType.OBJ_NPC;
        }
    }

    public override string dbgGetIdentifier()
    {
        return "npc: " + mRes.resID;
    }

    private void InitProperty(CropsInitParam param)
    {
        if (null == param.cropsinfo)
            return;
        CropsModule module = ModuleManager.Instance.FindModule<CropsModule>();
        if (null == module)
            return;
        float hp = 0.0f;
        float damage = 0.0f;
        float crits = 0.0f;
        float defence = 0.0f;
        float energy = 0.0f;

        module.GetProperty(param.cropsinfo.mCropsId, param.cropsinfo.mCropsStarslv, ref hp, ref damage, ref crits, ref defence, ref energy);

        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeHP, mRes.defaultHP + hp);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMaxHP, mRes.defaultHP + hp);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMana, mRes.defaultEnergy + energy);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMaxMana, mRes.defaultEnergy + energy);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeDamage, mRes.defaultDamage + damage);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeCrticalLV, mRes.defaultCrticalLV + crits);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeDefance, mRes.defaultDamageReduce + defence);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeSpeed, mRes.movespeed);
    }

    private void PlayBornAnimation()
    {
        if (mRes == null)
            return;

        if (string.IsNullOrEmpty(mRes.bornAni) || mRes.bornAniTime == uint.MaxValue)
            return;

        AnimActionPlayAnim action = AnimActionFactory.Create(AnimActionFactory.E_Type.PlayAnim) as AnimActionPlayAnim;


        action.AnimName = CombineAnimname("%" + mRes.bornAni);

        GetStateController().DoAction(action);

        mBornAniTime = (int)mRes.bornAniTime;
    }

    public void PlayLeisureAnimation()
    {
        AnimActionPlayAnim action = AnimActionFactory.Create(AnimActionFactory.E_Type.PlayAnim) as AnimActionPlayAnim;
        action.AnimName = CombineAnimname("%xiuxian");
        GetStateController().DoAction(action);

        //GetStateController().DoAction(AnimActionFactory.E_Type.PlayIdleAnim);
    }

    public override void OnEnterScene(BaseScene scene, uint instanceid)
    {
        this.Scene = scene;
        this.InstanceID = instanceid;

        if (mRes != null)
        {
            if (mRes.bornEffect != uint.MaxValue)
            {
                mBornEffectTime = (int)mRes.bornEffectTime;
                mBornEffectID = scene.CreateEffect((int)mRes.bornEffect, Vector3.one, GetPosition(), GetDirection());
            }

            if (mRes.bossHpUnit > 0)
            {
                CameraController.Instance.ShakeCamera();
            }

            // NPC使用自身的属性添加出生buff.
            AttackerAttr myAttr = new AttackerAttr(this);

            if (mRes.bornBuff_0 != uint.MaxValue)
                ErrorHandler.Parse(
                AddBornSkillEffect(myAttr, SkillEffectType.Buff, mRes.bornBuff_0),
                "in Crops::AddBornBuff"
                );

            if (mRes.bornBuff_1 != uint.MaxValue)
                ErrorHandler.Parse(
                AddBornSkillEffect(myAttr, SkillEffectType.Buff, mRes.bornBuff_1),
                "in Crops::AddBornBuff"
                );

			if(Scene.getType() != SceneType.SceneType_Qualifying && Scene.getType() != SceneType.SceneType_Arena)
			{
				ErrorHandler.Parse(
				AddBornSkillEffect(myAttr, SkillEffectType.Buff, GameConfig.CropHaloBuffId),
				"in Crops::AddBornHalo"
				);
			}
		}

        base.OnEnterScene(scene, instanceid);
    }
    protected override void onModelLoaded(GameObject obj)
    {
        base.onModelLoaded(obj);
        PlayBornAnimation();

        System.Random counter = new System.Random((Time.renderedFrameCount + InstanceID).GetHashCode());
        mRes.cryInternal = counter.Next(mRes.cryInternal - (int)(mRes.cryInternal * 0.5), mRes.cryInternal + (int)(mRes.cryInternal * 0.5));

		Scene.OnSpriteModelLoaded(mInstanceID);
    }
    override protected bool IsBorning()
    {
        return mBornAniTime > 0;
    }
    override public int GetObjectLayer()
    {
        return ObjectLayerMask.GetLayer(ObjectLayerMask.NPC);
    }

    override public string GetObjectTag()
    {
        return ObjectType.ObjectTagNPC;
    }

    public override float GetRadius()
    {
        return mRes != null ? mRes.radius : base.GetRadius();
    }

    public override uint GetMaterialResourceID()
    {
        return mRes != null ? mRes.materialID : uint.MaxValue;
    }

    public override int GetMainWeaponID()
    {
        return mRes != null ? mRes.weaponid : base.GetMainWeaponID();
    }

    private void OnBornEffectEnd()
    {
        base.OnEnterScene(this.Scene, this.InstanceID);
    }

    override public bool Update(uint elapsed)
    {
        if (mBornEffectTime > 0)
        {
            mBornEffectTime -= (int)elapsed;

            if (mBornEffectTime <= 0)
            {
                OnBornEffectEnd();
            }
            return true;
        }

        if (mBornAniTime > 0)
        {
            mBornAniTime -= (int)elapsed;
        }


        cdTime -= (int)elapsed;
        if (0 > cdTime)
        {
            cdTime = 0;
        }

        if (IsDead())
        {
            //死了状态机也得更新
            UpdateMecanim(elapsed);

            /*GameScene scn = SceneManager.Instance.GetCurScene() as GameScene;

            if (scn == null)
                return false;

            if (scn.GetCropsCanRelive())
                return true;*/

            return true;
        }

        if (mBloodNode != null)
        {
            Vector3 headPos = this.GetBonePositionByName("head");
            if (headPos != Vector3.zero)
            {
                headPos = CameraController.Instance.WorldToScreenPoint(headPos);
                headPos.z = 0.0f;
                mBloodNode.Update(headPos, this.GetHP(), this.GetMaxHP());
            }
        }

        UpdateTalk(elapsed);
        UpdateCrySound(elapsed);

        return base.Update(elapsed);
    }
    protected override void onDie(AttackerAttr killerAttr, ImpactDamageType impactDamageType)
    {
        base.onDie(killerAttr, impactDamageType);
        if (mBloodNode != null)
        {
            mBloodNode.Hide();
        }
        int xx = GetHP();
        
        Scene.OnCropsDie();
    }

    protected override void onDamage(DamageInfo damage, AttackerAttr attackerAttr)
    {
        if (mBloodNode != null && !IsDead() && damage.Value < 0)
        {
            mBloodNode.Show();
        }
        Vector3 headPos = this.GetBonePositionByName("head");
        if (headPos != Vector3.zero)
        {
            headPos = CameraController.Instance.WorldToScreenPoint(headPos);
            headPos.z = 0.0f;

            BattleUIEvent evt = new BattleUIEvent(BattleUIEvent.BATTLE_UI_PLAYER_DAMAGE);
            evt.pos = headPos;
            evt.damage = damage;
            EventSystem.Instance.PushEvent(evt);
        }

        base.onDamage(damage, attackerAttr);
    }

    public override MovingType GetMovingType()
    {
        return mRes != null ? (MovingType)mRes.movetype : base.GetMovingType();
    }
    override public void Destroy()
    {
        if (mBloodNode != null)
        {
            BloodUIManager.Instance.ReleaseBloodUI(mBloodNode);
            mBloodNode = null;
        }
        if (mBornEffectID != uint.MaxValue)
            Scene.RemoveEffect(mBornEffectID);
        base.Destroy();
    }

    public override uint GetLevel()
    {
        return mRes != null ? mRes.level : 1;
    }

    public override string GetName()
    {
        return mRes != null ? mRes.name : base.GetName();
    }

    public override int GetDeadSound()
    {
        int prob = Random.Range(0, 100);
        if (prob < mRes.deadSoundProb)
        {
            return mRes.DeadSound;
        }
        return -1;

    }

    public override string GetDeathAnimation()
    {
        if (mRes != null && !string.IsNullOrEmpty(mRes.deathAnimation))
        {
            //2014-10-13 14:02 韩柳青:死亡动作跟着枪械走
            string[] array = mRes.deathAnimation.Split('|');
            return CombineAnimname(array[UnityEngine.Random.Range(0, array.Length)]);
        }

        return base.GetDeathAnimation();
    }

    public override int GetWalkSound()
    {
        return mRes.walkSound;
    }

    public override int GetCrySound()
    {
        System.Random counter = new System.Random((Time.renderedFrameCount + InstanceID * 10).GetHashCode());
        int prob = counter.Next(0, 100);
        mCryTime = counter.Next(mRes.cryInternal - (int)(mRes.cryInternal * 0.5), mRes.cryInternal + (int)(mRes.cryInternal * 0.5));

        if (prob < mRes.crySoundProp)
        {
            //GameDebug.Log(InstanceID + "----" + prob + "mRes.cryInternal");
            return mRes.crySound;
        }
        return -1;

    }
    //佣兵没有弹药说
    override public int GetWeaponMaxBullet()
    {
        return -1;
    }

    public override uint AddEffect(uint resId, Vector3 pos, float dir = Single.NaN)
    {
        return base.AddEffect(resId, pos, dir);
    }

    override public bool IsGuildTarget()
    {
        return true;
    }

    protected override void onPropertyChanged(int id, float oldValue, float newValue)
    {
        base.onPropertyChanged(id, oldValue, newValue);

        PropertyEvent evt = new PropertyEvent(PropertyEvent.CROPS_PROPERTY_CHANGE);
        EventSystem.Instance.PushEvent(evt);
    }

    private void UpdateCrySound(uint elapsed)
    {
        mCryTime -= (int)elapsed;
        if (mCryTime <= 0)
        {
            int cryId = GetCrySound();
            if (cryId == -1) return;
            SoundManager.Instance.Play(cryId);
            mCryTime = mRes.cryInternal;
        }

    }

    private void UpdateTalk(uint elapsed)
    {
        if (mTalkID < 0)
        {
            return;
        }

        if (this.IsDead())
        {
            return;
        }

        mTalkTime -= (int)elapsed;
        if (mTalkTime <= 0)
        {

            if (DataManager.NpcTalkTable.ContainsKey(mTalkID))
            {
                NpcTalkTableItem item = DataManager.NpcTalkTable[mTalkID] as NpcTalkTableItem;

                string talk_str = item.talk1;
                if (this.IsFighting())
                {
                    talk_str = item.talk2;
                }

                if (string.IsNullOrEmpty(talk_str))
                    return;
                string[] talks = talk_str.Split(new char[] { '|' });

                if (talks.Length <= 0)
                {
                    return;
                }

                int idx = Random.Range(0, talks.Length);

                ShowTalk(talks[idx], item.Continued * 1000);

                mTalkTime = item.Interval * 1000 + item.Continued * 1000;
            }
        }
    }

}

