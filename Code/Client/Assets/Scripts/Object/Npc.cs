using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class NpcInitParam : RoleInitParam
{
	public int npc_res_id = -1;
	public uint lifeTime = uint.MaxValue;
	public AttackerAttr summonerAttr = new AttackerAttr(null);
    public int talk_id = -1;
}

public class Npc : Role 
{
	protected NPCTableItem mRes = null;

    private BloodNode mBloodNode = null;

	private uint mLifeTime = uint.MaxValue;

    private uint mBornEffectID = uint.MaxValue;

    private int mBornEffectTime = 0;

    private int mBornAniTime = 0;

    private int mTalkTime = int.MaxValue;
    private int mContinued = int.MaxValue;
    private int mInterval = int.MaxValue;

    private string[] mFightingTalks = null;
    private string[] mNormalTalks = null;

    private int mTalkID = -1;

    private int cdTime = 0;

    private int mCryTime = 0;

	private AttackerAttr mSummonerAttr;

	public Npc()
	{
        
	}
	override public bool Init(ObjectInitParam param)
	{
		NpcInitParam npcParam = (NpcInitParam)param;

		if (!DataManager.NPCTable.ContainsKey(npcParam.npc_res_id))
		{
			return false;
		}
		mRes = DataManager.NPCTable[npcParam.npc_res_id] as NPCTableItem;
		mModelResID = mRes.model;
		
		if (!base.Init(param))
			return false;

		mBattleUintAI = AIFactory.Instance.CreateAIObject(this, mRes.ai);
		if (mBattleUintAI == null)
			return false;

        if( npcParam.talk_id >= 0 )
        {
            mTalkID = npcParam.talk_id;
        }
        else
        {
            mTalkID = mRes.talkID;
        }

        InitTalk();

		SetLeague(mRes.league);

        mDestroyWaiting = true;
        mMaxDisappearTime = mRes.DisappearTime;
        mMaxWaitDisappearTime = mRes.WaitDisappearTime;

		mLifeTime = npcParam.lifeTime;

		mSummonerAttr = npcParam.summonerAttr;

        InitProperty();

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

    private void InitProperty()
    {
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeHP, mRes.defaultHP);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMaxHP, mRes.defaultHP);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMana, mRes.defaultEnergy);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMaxMana, mRes.defaultEnergy);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeDamage, mRes.defaultDamage);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeCrticalLV, mRes.defaultCrticalLV);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeDefance, mRes.defaultDamageReduce);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeSpeed, mRes.movespeed);
    }

    private void PlayBornAnimation()
    {
        if (mRes == null)
            return;

        if (string.IsNullOrEmpty(mRes.bornAni) || mRes.bornAniTime == uint.MaxValue)
            return;

        AnimActionPlayAnim action = AnimActionFactory.Create(AnimActionFactory.E_Type.PlayAnim) as AnimActionPlayAnim;


        action.AnimName = CombineAnimname("%"+mRes.bornAni);

        GetStateController().DoAction(action);

        mBornAniTime = (int)mRes.bornAniTime;
    }

    private void OnPreEnterScene(BaseScene scene, uint instanceid)
    {
        if( mRes == null )
        {
            return;
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
            "in Npc::AddBornBuff"
            );

        if (mRes.bornBuff_1 != uint.MaxValue)
            ErrorHandler.Parse(
            AddBornSkillEffect(myAttr, SkillEffectType.Buff, mRes.bornBuff_1),
            "in Npc::AddBornBuff"
            );
    }

    public override void OnEnterScene(BaseScene scene, uint instanceid)
    {
        this.Scene = scene;
        this.InstanceID = instanceid;


        //如果有出生特效....就
        if (mRes != null && mRes.bornEffect != uint.MaxValue)
        {
            mBornEffectTime = (int)mRes.bornEffectTime;
            mBornEffectID = scene.CreateEffect((int)mRes.bornEffect, Vector3.one, GetPosition(), GetDirection());

            if (mBornEffectID != uint.MaxValue)
            {
                return;
            }
        }

        OnPreEnterScene(scene , instanceid);

        base.OnEnterScene(scene, instanceid);
    }
	protected override void onModelLoaded(GameObject obj)
	{
		base.onModelLoaded(obj);
        PlayBornAnimation();
		
        System.Random counter = new System.Random((Time.renderedFrameCount + InstanceID).GetHashCode());
        mRes.cryInternal = counter.Next(mRes.cryInternal - (int)(mRes.cryInternal * 0.5), mRes.cryInternal + (int)(mRes.cryInternal * 0.5));
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
        OnPreEnterScene(this.Scene, this.InstanceID);

        base.OnEnterScene(this.Scene, this.InstanceID);
    }

	override public bool Update(uint elapsed)
	{
        if (mBornEffectTime > 0 )
        {
            mBornEffectTime -= (int)elapsed;

            if( mBornEffectTime <= 0 )
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
        if (0 > cdTime )
        {
            cdTime = 0;
        }

        if (IsDead())
        {
            //死了状态机也得更新
            UpdateMecanim(elapsed);

            return false;
        }

		if (mLifeTime != uint.MaxValue)
		{
			if (mLifeTime > elapsed)
				mLifeTime -= elapsed;
			else
			{
				mLifeTime = uint.MaxValue;
				// NPC生命时间结束, 视为被系统回收.
				Die(new AttackerAttr(null));
				return false;
			}
		}

        if (mBloodNode != null )
        {
            Vector3 headPos = this.GetBonePositionByName("head");
            if (headPos != Vector3.zero)
            {
                headPos = CameraController.Instance.WorldToScreenPoint(headPos);
                headPos.z = 0.0f;
                mBloodNode.Update(headPos,this.GetHP() , this.GetMaxHP());
            }
        }

        UpdateTalk(elapsed);
	    UpdateCrySound(elapsed);

		bool baseUpdate = base.Update(elapsed);
		//如果死了 等待多少秒后 删除
		return baseUpdate;
	}
	protected override void onDie(AttackerAttr killerAttr, ImpactDamageType impactDamageType)
    {
        if (mRes.bossHpUnit > 0)
        {
            BossBloodUIManager.Instance.ChangeHp(mRes.name, mRes.headicon, GetLevel(), 0, GetMaxHP(),
                mRes.bossHpUnit, IsFury());

			GameScene scene = mScene as GameScene;
			if(scene != null)
			{
				scene.BossDeadPos = GetPosition();
			}
        }

        if( mBloodNode != null )
        {
            mBloodNode.Hide();
        }
        
        base.onDie(killerAttr, impactDamageType);
    }

    protected override void OnEnterFury()
    {
        if (mRes.bossHpUnit > 0)
        {
            BossBloodUpdateEvent evt = new BossBloodUpdateEvent(BossBloodUpdateEvent.BOSS_ENTER_FURY);
            EventSystem.Instance.PushEvent(evt);
        }
    }

    protected override void OnLeaveFury()
    {   
        if (mRes.bossHpUnit > 0)
        {
            BossBloodUpdateEvent evt = new BossBloodUpdateEvent(BossBloodUpdateEvent.BOSS_LEAVE_FURY);
            EventSystem.Instance.PushEvent(evt);
        }
    }

	protected override void onDamage(DamageInfo damage, AttackerAttr attackerAttr)
	{
		if(mRes.bossHpUnit > 0)
		{
			int curHp = GetHP() < 0 ? 0 : GetHP();
			BossBloodUIManager.Instance.ChangeHp(mRes.name, mRes.headicon, GetLevel(), curHp, GetMaxHP(),
                mRes.bossHpUnit, IsFury());
		}

        if( mBloodNode != null && !IsDead() && damage.Value < 0)
        {
            mBloodNode.Show();
        }

        Vector3 headPos = this.GetBonePositionByName("head");
        if (headPos != Vector3.zero)
        {
            headPos = CameraController.Instance.WorldToScreenPoint(headPos);
            headPos.z = 0.0f;

            BattleUIEvent evt = new BattleUIEvent(BattleUIEvent.BATTLE_UI_DAMAGE);
            evt.pos = headPos;
            evt.damage = damage;
			evt.dead = IsDead();

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
        System.Random counter = new System.Random((Time.renderedFrameCount+InstanceID*10).GetHashCode());
        int prob = counter.Next(0,100);
        mCryTime = counter.Next(mRes.cryInternal - (int)(mRes.cryInternal * 0.5), mRes.cryInternal + (int)(mRes.cryInternal * 0.5));
       
        if (prob < mRes.crySoundProp)
        {
            //GameDebug.Log(InstanceID + "----" + prob + "mRes.cryInternal");
            return mRes.crySound;
        }
        return -1;
        
    }
    //NPC没有弹药说
    override public int GetWeaponMaxBullet()
    {
        return -1;
    }

    public override uint AddEffect(uint resId, Vector3 pos, float dir = Single.NaN)
    {
        return base.AddEffect(resId, pos, dir);
    }

    public override void HpDamageAward(uint objtarget, int time)
	{
		if(mRes == null)
		{
			return;
		}

        if (uint.MaxValue == objtarget)
        {
            return;
        }

        if ( 0 == cdTime)
            
            cdTime = time * 1000;
        else
            return;

		// 掉货币
		if(mRes.dropMoney > 0 && mRes.dropMoneyWeight > 0)
		{
			int rand = Random.Range(0, DropManager.MAX_WEIGHT);
			if(mRes.dropMoneyWeight > rand)
            {
                List<PickInitParam> paramList = new List<PickInitParam>();
                if (SceneObjManager.CreatePickInitParam(Pick.PickType.MONEY, mRes.dropMoneyPickId, mRes.dropMoney, GetPosition(), GetDirection(), out paramList, true, Pick.FlyType.FLY_OUT, false))
                {
                    foreach (PickInitParam param in paramList)
                    {
                        mScene.CreateSprite(param);
                    }
                }
			}
		}

		// 掉buff
		if(mRes.buffDropBoxId >= 0)
		{
			List<PickInitParam> paramList = new List<PickInitParam>();
			if (SceneObjManager.CreatePickInitParam(Pick.PickType.BUFF, -1, mRes.buffDropBoxId, GetPosition(), GetDirection(), out paramList, true, Pick.FlyType.FLY_OUT, true))
			{
				foreach (PickInitParam param in paramList)
				{
					mScene.CreateSprite(param);
				}
			}
		}

		// 掉道具
		if(mRes.itemDropBoxId >= 0)
		{
			if(mRes.isDropOnGround > 0)
			{// 掉地上

				List<PickInitParam> paramList = new List<PickInitParam>();
				if (SceneObjManager.CreatePickInitParam(Pick.PickType.ITEM, -1, mRes.itemDropBoxId, GetPosition(), GetDirection(), out paramList, true, Pick.FlyType.FLY_OUT, true))
				{
					foreach (PickInitParam param in paramList)
					{
						mScene.CreateSprite(param);
					}
				}
			}
			else
			{
				ArrayList itemList = new ArrayList();
				if (DropManager.Instance.GenerateDropBox(mRes.itemDropBoxId, out itemList))
				{
					foreach (DropBoxItem item in itemList)
					{
                        ItemTableItem itemres = ItemManager.GetItemRes(item.itemid);
                        if (itemres == null)
                            continue;

						ObjectBase obj = PlayerController.Instance.GetControlObj();
						if (obj != null)
						{
							PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
							if (pdm != null)
							{
								//pdm.CreateItemUnreal(item.itemid, PackageType.Pack_Bag);
							}
						}
					}
				}
			}
		}
	}


    protected override void DieAward(uint killerid)
    {
        if (mRes == null)
        {
            return;
        }

        // 掉货币
        if (mRes.dropMoney > 0 && mRes.dropMoneyWeight > 0)
        {
            int rand = Random.Range(0, DropManager.MAX_WEIGHT);
            if (mRes.dropMoneyWeight > rand)
            {
                if (mRes.dropMoneyPickId < 0 || !DataManager.PickTable.ContainsKey(mRes.dropMoneyPickId))
                {
                    if (killerid != uint.MaxValue)
                    {
                        ObjectBase obj = PlayerController.Instance.GetControlObj();
                        if (obj != null && obj.InstanceID == killerid)
                        {
                            PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
                            if (pdm != null)
                            {
                              //  pdm.ChangeProceeds(ProceedsType.Money_Game, mRes.dropMoney);
                            }
                        }
                    }
                }
                else
                {// 掉进场景

                    List<PickInitParam> paramList = new List<PickInitParam>();
					if (SceneObjManager.CreatePickInitParam(Pick.PickType.MONEY, mRes.dropMoneyPickId, mRes.dropMoney, GetPosition(), GetDirection(), out paramList, true, Pick.FlyType.FLY_OUT, false))
                    {
                        foreach (PickInitParam param in paramList)
                        {
                            mScene.CreateSprite(param);
                        }
                    }
                }
            }
        }

        // 掉buff
        if (mRes.buffDropBoxId >= 0)
        {
            List<PickInitParam> paramList = new List<PickInitParam>();
			if (SceneObjManager.CreatePickInitParam(Pick.PickType.BUFF, -1, mRes.buffDropBoxId, GetPosition(), GetDirection(), out paramList, true, Pick.FlyType.FLY_OUT, true))
            {
                foreach (PickInitParam param in paramList)
                {
                    mScene.CreateSprite(param);
                }
            }
        }

        // 掉道具
        if (mRes.itemDropBoxId >= 0)
        {
            if (mRes.isDropOnGround > 0)
            {// 掉地上

                List<PickInitParam> paramList = new List<PickInitParam>();
				if (SceneObjManager.CreatePickInitParam(Pick.PickType.ITEM, -1, mRes.itemDropBoxId, GetPosition(), GetDirection(), out paramList, true, Pick.FlyType.FLY_OUT, true))
                {
                    foreach (PickInitParam param in paramList)
                    {
                        mScene.CreateSprite(param);
                    }
                }
            }
//             else
//             {
//                 ArrayList itemList = new ArrayList();
//                 if (DropManager.Instance.GenerateDropBox(mRes.itemDropBoxId, out itemList))
//                 {
//                     foreach (DropBoxItem item in itemList)
//                     {
//                         ItemTableItem itemres = ItemManager.GetItemRes(item.itemid);
//                         if (itemres == null)
//                             continue;
// 
//                         if (killerid != uint.MaxValue)
//                         {
//                             ObjectBase obj = PlayerController.Instance.GetControlObj();
//                             if (obj != null && obj.InstanceID == killerid)
//                             {
//                                 PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
//                                 if (pdm != null)
//                                 {
//                                     //pdm.CreateItemUnreal(item.itemid, PackageType.Pack_Bag);
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
        }
    }

	override public bool IsGuildTarget()
	{
		return true;
	}

    public int GetQiangLinDanYuScore()
    {
        return mRes != null ? mRes.qiangLinDanYuScore : 0;
    }

	public uint GetYaZhiXieEScore()
	{
		return mRes != null ? mRes.yaZhiXieEScore : 0;
	}

    private void UpdateCrySound(uint elapsed)
    {
        mCryTime -= (int) elapsed;
        if (mCryTime <= 0)
        {
            int cryId = GetCrySound();
            if (cryId == -1) return;
            SoundManager.Instance.Play(cryId);
            mCryTime = mRes.cryInternal;
        }
      
    }

    private void InitTalk()
    {
        if (mTalkID < 0)
        {
            return;
        }
        if (!DataManager.NpcTalkTable.ContainsKey(mTalkID))
        {
            return;
        }

        NpcTalkTableItem item = DataManager.NpcTalkTable[mTalkID] as NpcTalkTableItem;

        if( !string.IsNullOrEmpty(item.talk1) )
            mNormalTalks = item.talk1.Split(new char[] { '|' });
        if (!string.IsNullOrEmpty(item.talk2))
            mFightingTalks = item.talk2.Split(new char[] { '|' });

        mTalkTime = item.Interval;
        mContinued = item.Continued;
        mInterval = item.Interval;
    }

    private void UpdateTalk(uint elapsed)
    {
        if (mTalkID < 0)
        {
            return;
        }
        if (mNormalTalks == null && mFightingTalks == null)
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
            string[] talks = null;
            if (this.IsFighting())
            {
                talks = mFightingTalks;
            }else
            {
                talks = mNormalTalks;
            }

            if (talks == null || talks.Length <= 0)
                return;

            int idx = Random.Range(0, talks.Length);

            ShowTalk(talks[idx], mContinued * 1000);

            mTalkTime = mInterval * 1000 + mContinued * 1000;
        }
    }
}

