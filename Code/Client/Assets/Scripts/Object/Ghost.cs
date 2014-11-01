using UnityEngine;
using System.Collections;

// 玩家镜像类

public class GhostInitParam : RoleInitParam
{
	public GhostData ghost_data = new GhostData();

	public LeagueDef league = LeagueDef.Blue;

	public bool main_player = false;
}

public class Ghost : Role
{
	private PlayerTableItem mRes = null;

	private BloodNode mBloodNode = null;

	private GhostData mGhostData = null;

	private int mMainWeaponID = -1;

	private int mOverBullet = 0;

	private bool mWaitReload = false;

	private bool mMainPlayer = false;

    override public bool Init(ObjectInitParam param)
    {
		GhostInitParam ghostParam = (GhostInitParam)param;

		mGhostData = ghostParam.ghost_data;

		if (!DataManager.PlayerTable.ContainsKey(mGhostData.resId))
        {
            return false;
        }

		mRes = DataManager.PlayerTable[mGhostData.resId] as PlayerTableItem;

		mModelResID = mRes.model;
		mMainWeaponID = mGhostData.main_weaponId;
		mMainPlayer = ghostParam.main_player;

        if (!base.Init(param))
            return false;

		InitProperty(mGhostData);

		SetLeague(ghostParam.league);
       
        return true;
    }

	public override string dbgGetIdentifier()
	{
		return "ghost: " + mRes.resID;
	}

	public override string GetName()
	{
		return mGhostData.name;
	}

    override public int Type
    {
        get
        {
            return ObjectType.OBJ_GHOST;
        }
    }

	public bool IsMainPlayer()
	{
		return mMainPlayer;
	}

    private void InitProperty(GhostData data)
	{
        LevelTableItem levelRes = DataManager.LevelTable[data.level] as LevelTableItem;
        if( levelRes == null )
        {
            GameDebug.LogError("level.txt 配置错误");
            return;
        }

        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeHP, levelRes.maxhp);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMaxHP, levelRes.maxhp);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMana, levelRes.energy);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMaxMana, levelRes.energy);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeDamage, levelRes.damage);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeCrticalLV, levelRes.crticalLV);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeDefance, levelRes.damageReduce);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeSpeed, mRes.speed);

// 		mHpRegRate = levelRes.hpRegRate;
// 		mManaRegRate = levelRes.manaRegRate;
	}

	override public int GetMainWeaponID()
    {
        return mMainWeaponID;
    }

    override public uint GetMaterialResourceID()
    {
        if (mRes == null)
            return uint.MaxValue;
        return mRes.materialID;
    }

	override public int GetObjectLayer()
	{
        return ObjectLayerMask.GetLayer(ObjectLayerMask.Player);
	}

	public override float GetRadius()
	{
		return mRes != null ? mRes.radius : base.GetRadius();
	}

	public override uint GetLevel()
	{
		return (uint)mGhostData.level;
	}

    public override MovingType GetMovingType()
    {
        return MovingType.MoveType_Lowwer;
    }

	override public string GetObjectTag()
	{
		return ObjectType.ObjectTagGhost;
	}

    public override int GetWalkSound()
    {
        return 17;
    }
	override public bool Update(uint elapsed)
	{
        if (mWaitReload)
        {
            ReloadBullet();
            mWaitReload = false;
        }

        return base.Update(elapsed);
	}

    override protected void OnChangeWeapon()
    {
        if( mWeaopnRes == null )
            return ;
        mOverBullet = mWeaopnRes.max_bullet;

        OnBulletNumChange();
    }
    //获取当前武器的总弹药数量
    override public int GetWeaponMaxBullet()
    {
        if (mWeaopnRes == null)
            return -1;
        return mWeaopnRes.max_bullet;
    }

    //获取当前武器 剩余弹药
    override public int GetWeaponBullet()
    {
        return mOverBullet;
    }
    //消耗弹药
    override public void CostWeaponBullet(int cost)
    {
        mOverBullet -= cost;
        if (mOverBullet <= 0)
        {
            mOverBullet = 0;
            mWaitReload = true;
        }
        OnBulletNumChange();
    }
    override public void AddWeaponBullet(int cost)
    {
        mOverBullet += cost;
        if (mOverBullet < 0)
            mOverBullet = 0;
        if (mOverBullet >= GetWeaponMaxBullet())
            mOverBullet = GetWeaponMaxBullet();

        OnBulletNumChange();
    }

	private void OnBulletNumChange()
	{
		if(mMainPlayer)
			EventSystem.Instance.PushEvent(new ReloadEvent(ReloadEvent.BULLET_CHANGE_EVENT));
	}

	override protected RoleAnimationDir GetAnimationDirection()
	{
		RoleAnimationDir dir = RoleAnimationDir.AnimationDir_Front;
		if( IsMoveing() )
		{
            float moveDir = this.GetMoveDirection();
			float myDir = this.GetDirection();

			float r = myDir - moveDir;
			
			if (r > Mathf.PI)
			{
				//				r = -(r - Math.PI);
				r = 2 * Mathf.PI - r;
			}
			else if (r < -Mathf.PI)
			{
				r = (Mathf.PI * 2.0f + r);
			}
			if (r < 0)
			{
				//
				// 枪的方向在移动方向的左边
				//
				r = -r;
				if (r < 0.25 * Mathf.PI)
					dir = RoleAnimationDir.AnimationDir_Front;
				else if (r < 0.75 * Mathf.PI)
					dir = RoleAnimationDir.AnimationDir_Right;
				else
					dir = RoleAnimationDir.AnimationDir_Back;
			}
			else
			{
				//
				// 枪的方向在移动方向的右边
				//
				if (r < 0.25 * Mathf.PI)
					dir = RoleAnimationDir.AnimationDir_Front;
				else if (r < 0.75 * Mathf.PI)
					dir = RoleAnimationDir.AnimationDir_Left;
				else
					dir = RoleAnimationDir.AnimationDir_Back;
			}
			
			//GameDebug.Log( "moveDir :" + moveDir.ToString() + "  myDir :" + myDir.ToString() );
		}

		return dir;
	}

//     void updateNamePos()
//     {
//         if (mHeadNode != null)
//         {
//             Vector3 headPos = this.GetBonePositionByName("head");
//             if (headPos != Vector3.zero)
//             {
//                 headPos = CameraController.Instance.WorldToScreenPoint(headPos);
//                 headPos.Set(headPos.x, headPos.y + 10f, 0f);
//                 //headPos.z = 0.0f;
//                 mHeadNode.Update(headPos);
//             }
//         }
//     }

// 	/// <summary>
// 	/// 更新恢复(血量和蓝量).
// 	/// </summary>
// 	/// <param name="elapsed"></param>
// 	void updateRestore(uint elapsed)
// 	{
// 		if (mRestoreInterval > elapsed)
// 		{
// 			mRestoreInterval -= elapsed;
// 			return;
// 		}
// 
// 		mRestoreInterval = 1000;
// 
// 		if (GetHP() < GetMaxHP())
// 		{
// 			mHpRegenerated += mHpRegRate;
// 			int hp = (int)mHpRegenerated;
// 			if (hp != 0)
// 			{
// 				ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeHP, hp);
// 				mHpRegenerated -= hp;
// 			}
// 		}
// 		if (GetMana() < GetMaxMana())
// 		{
// 			mManaRegenerated += mManaRegRate;
// 			int mana = (int)mManaRegenerated;
// 			if (mana != 0)
// 			{
// 				ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeMana, mana);
// 				mManaRegenerated -= mana;
// 			}
// 		}
// 	}

    protected override void onPropertyChanged(int id, float oldValue, float newValue)
    {
        base.onPropertyChanged(id, oldValue, newValue);

		if(mMainPlayer)
			EventSystem.Instance.PushEvent(new PropertyEvent(PropertyEvent.GHOST_FIGHT_PROPERTY_CHANGE));
    }

    protected override void OnKillOther(AttackerAttr attr, BattleUnit theDead)
    {
		base.OnKillOther(attr, theDead);
    }

	protected override void onDie(AttackerAttr killerAttr, ImpactDamageType impactDamageType)
	{
		if (!mMainPlayer)
			BossBloodUIManager.Instance.ChangeHp(mGhostData.name, "touxiang:head" + mGhostData.resId.ToString(), GetLevel(), 0, GetMaxHP(), GetMaxHP(), IsFury());

		base.onDie(killerAttr, impactDamageType);
	}

    protected override void onDamage(DamageInfo damage, AttackerAttr attackerAttr)
    {
		if (!mMainPlayer)
		{
			int curHp = GetHP() < 0 ? 0 : GetHP();
			BossBloodUIManager.Instance.ChangeHp(mGhostData.name, "touxiang:head" + mGhostData.resId.ToString(), GetLevel(), curHp, GetMaxHP(),
				GetMaxHP(), IsFury());
		}

		Vector3 headPos = this.GetBonePositionByName("head");
		if (headPos != Vector3.zero)
		{
			headPos = CameraController.Instance.WorldToScreenPoint(headPos);
			headPos.z = 0.0f;

			BattleUIEvent evt = new BattleUIEvent(BattleUIEvent.BATTLE_UI_GHOST_DAMAGE);
			evt.pos = headPos;
			evt.damage = damage;
			evt.dead = IsDead();

			EventSystem.Instance.PushEvent(evt);
		}

		base.onDamage(damage, attackerAttr);
    }

	protected override void onManaChanged(int delta)
	{


		base.onManaChanged(delta);
	}
    override public void Destroy()
    {
        base.Destroy();
    }

	override protected void onModelLoaded(GameObject obj)
	{
		base.onModelLoaded(obj);

		GameScene gamescene = Scene as GameScene;
		if (gamescene != null && gamescene.IsActionFlagAllowed(SceneActionFlag.SceneActionFlag_Ai))//IsWorkingState())
		{
			CreateGhostAI();
		}

		Scene.OnSpriteModelLoaded(mInstanceID);
	   
	}

    private void AddWingModel()
    {
        if (mGhostData.mWingData.mWearId != -1)
        {
            WingEquip(mGhostData.mWingData.mWearId,0);
        }
    }

    private void AddFashionModel()
    {
        if (mGhostData.mFashionData.head_id != -1)
        {
            ChangeFashion(mGhostData.mFashionData.head_id,1);
        }

        if (mGhostData.mFashionData.upper_body_id != -1)
        {
            ChangeFashion(mGhostData.mFashionData.upper_body_id,1);
        }

        if (mGhostData.mFashionData.lower_body_id != -1)
        {
            ChangeFashion(mGhostData.mFashionData.lower_body_id,1);
        }
    }

    private void ChangeFashion(int fashionid, int action)
    {
        FashionTableItem fashion_res = DataManager.FashionTable[fashionid] as FashionTableItem;
        PartModelTableItem part_res = DataManager.PartModelTable[fashion_res.model] as PartModelTableItem;
        if (part_res == null)
        {
            GameDebug.LogError("fashion 表里没有 partsmodel表的id =" + fashion_res.model);
            return;
        }
        if (action == 1)
        {

            mVisual.ChangeElment(part_res.solt, AssetConfig.ModelPath + "Role/" + part_res.file + AssetConfig.AssetSuffix, null);
        }

        if (action == 2)
        {
            // mVisual.ChangeElment(part_res.solt,null,null);
        }
    }

    private void AddWingSkillBuff()
    {
        WingData wingData = mGhostData.mWingData;
        if (mScene.getType() == SceneType.SceneType_City) return;
        for (int i = 0; i < wingData.wingItems.Count; ++i)
        {
            WingLevelTableItem levelRes = wingData.GetLevelRes(i);
            if (levelRes != null)
            {
                AddSkillEffect(new AttackerAttr(this), SkillEffectType.Buff, System.Convert.ToUInt32(levelRes.buffid));
                GameDebug.Log("添加翅膀buff:" + levelRes.buffid);
            }
        }
    }

    public void WingEquip(int wingid, int action)
    {
        WingItemData itemData = mGhostData.mWingData.GetWingItemDataById(wingid);
        WingCommonTableItem commonRes = DataManager.WingCommonTable[wingid] as WingCommonTableItem;
        string modelName = WingModule.GetModelName(wingid, itemData.level);
        int wingeffectId = WingModule.GetEffectId(wingid, (int) itemData.level);
        if (action == 0)
        {
            RemoveAttach(AttachMountType.Wing);
            GameDebug.Log("装备翅膀");
            ChangeAttach(AttachMountType.Wing, AssetConfig.WeaponPath + modelName, commonRes.modelSlot);
            if (wingeffectId != -1)
            {
                AddEffect((uint)wingeffectId, null, float.NaN,AttachMountType.Wing);
            }

        }
        else
        {
            GameDebug.Log("卸载翅膀");
            RemoveAttach(AttachMountType.Wing);
        }

    }

	public void CreateGhostAI()
	{
		if (mRes != null && mBattleUintAI == null)
		{
            int mAiId = 0;
            int roleLv = PlayerDataPool.Instance.MainData.level;
            if (roleLv <= 25)
            {
                mAiId = 0;
            }
            else if (roleLv <= 39)
            {
                mAiId = 1;
            }
            else if (roleLv <= 49)
            {
                mAiId = 2;
            }
            else if (roleLv <= 50)
            {
                mAiId = Random.Range(3, 4);
            }
            else
            {
                mAiId = Random.Range(5, 7);
            }
			if (Scene.getType() == SceneType.SceneType_Arena)
			{
                if (null == mRes.arenaAI)
                    return;
                string[] array = mRes.arenaAI.Split('|');
                if (mAiId >= array.Length)
                    return;
                
				mBattleUintAI = AIFactory.Instance.CreateAIObject(this, System.Convert.ToInt32(array[mAiId]));
			}
			else if (Scene.getType() == SceneType.SceneType_Qualifying)
			{
                if (null == mRes.rankAI)
                    return;
                string[] array = mRes.rankAI.Split('|');
                if (mAiId >= array.Length)
                    return;

                mBattleUintAI = AIFactory.Instance.CreateAIObject(this, System.Convert.ToInt32(array[mAiId]));
			}
			else if(Scene.getType() == SceneType.SceneType_ZhaoCaiMao)
			{
				if (null == mRes.zcmAI)
					return;
				string[] array = mRes.rankAI.Split('|');
				if (mAiId >= array.Length)
					return;

				mBattleUintAI = AIFactory.Instance.CreateAIObject(this, System.Convert.ToInt32(array[mAiId]));
			}
		}
	}

	public int GetSkillId(int idx)
	{
		if(mGhostData == null || mGhostData.skillData == null)
		{
			return -1;
		}

		if(idx < 0 || idx >= SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM)
		{
			return -1;
		}

		return mGhostData.skillData.skills[idx];
	}

    public override float GetAnimAngle()
    {
        float moveDir = this.GetMoveDirection();
        float myDir = this.GetDirection();

        return myDir - moveDir + Mathf.PI / 2;
    }

    public override void OnEnterScene(BaseScene scene, uint instanceid)
    {
        base.OnEnterScene(scene, instanceid);
        AddWingSkillBuff();
        AddWingModel();
        AddFashionModel();
    }
}
