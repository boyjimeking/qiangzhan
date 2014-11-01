using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//玩家类
public class PlayerInitParam : RoleInitParam
{
    public PlayerData player_data;
    //初始属性
    public PropertyOperation init_property;
}
public class Player : Role 
{
    protected PlayerTableItem mRes = null;

	private PlayerDataModule mModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

    private int mUseWeaponID = -1;

    private int mLastUseWeaponID = -1;

    private int mSuperWeaponID = -1;
    private int mSuperWeaponTime = -1;

    private BattleUnit mAutoAimTarget = null;

    private HeadNode mHeadNode = null;


    private Dictionary<int, int> mOverBullet = new Dictionary<int, int>();
    //剩余弹药数
    //private int     mOverBullet =  0;

    private bool    mWaitReload = false;

	float mHpRegenerated = 0f;
	float mManaRegenerated = 0f;

	uint mRestoreInterval = 1000;

	// 玩家的等级改变时, 更新二者的值.
	float mHpRegRate = 0f;
	float mManaRegRate = 0f;

    private int mLevel = -1;

	// 拾取半径
	private float mPickRadius = 3.0f;

    private uint mDailyResetTimer = 0;
	private uint mSpIncreaseTimer = 0;

    override public bool Init(ObjectInitParam param)
    {
        PlayerInitParam plyParam = (PlayerInitParam)param;

        if (!DataManager.PlayerTable.ContainsKey(plyParam.player_data.resId))
        {
            return false;
        }
        mRes = DataManager.PlayerTable[plyParam.player_data.resId] as PlayerTableItem;
		mModelResID = mRes.model;
        mUseWeaponID = plyParam.player_data.main_weaponId;

        if (!base.Init(param))
            return false;

        mLevel = plyParam.player_data.level;

        ApplyProperty(plyParam.init_property);

        mHeadNode = PlayerHeadUIManager.Instance.CreatePlayerHeadUI();
        mHeadNode.SetName(GetName());
        mHeadNode.SetTitle(GetTitleImg());
        mHeadNode.Show();
		SetLeague(LeagueDef.Red);
        return true;
    }

	public override string dbgGetIdentifier()
	{
		return "player: " + mRes.resID;
	}

	public override string GetName()
	{
		return mModule.GetName();
	}

    public string GetTitleImg()
    {
        return TitleModule.GetTitleImgById(mModule.GetCurTitle());
    }

    public void RefreshTitle()
    {
        mHeadNode.SetTitle(GetTitleImg());
    }

    override public int Type
    {
        get
        {
            return ObjectType.OBJ_PLAYER;
        }
    }

    public void ApplyProperty(PropertyOperation data)
	{
        LevelTableItem levelRes = DataManager.LevelTable[mLevel] as LevelTableItem;
        if( levelRes == null )
        {
            GameDebug.LogError("level.txt 配置错误");
            return;
        }

        mHpRegRate = levelRes.hpRegRate;
        mManaRegRate = levelRes.manaRegRate;

        foreach (KeyValuePair<int, float> v in data.values)
        {
            SetBaseProperty(v.Key, v.Value);
        }
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeHP, this.GetMaxHP());
//         SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMaxHP, levelRes.maxhp);
//         SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMana, levelRes.energy);
//         SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMaxMana, levelRes.energy);
//         SetBaseProperty((int)PropertyTypeEnum.PropertyTypeDamage, levelRes.damage);
//         SetBaseProperty((int)PropertyTypeEnum.PropertyTypeCrticalLV, levelRes.crticalLV);
//         SetBaseProperty((int)PropertyTypeEnum.PropertyTypeDefance, levelRes.damageReduce);
//         SetBaseProperty((int)PropertyTypeEnum.PropertyTypeSpeed, mRes.speed);
	}

	private void AddWingSkillBuff()
	{
		WingData wingData = PlayerDataPool.Instance.MainData.mWingData;
		if(mScene.getType() == SceneType.SceneType_City) return;
		for(int i = 0;i < wingData.wingItems.Count; ++i)
		{
			WingLevelTableItem levelRes = wingData.GetLevelRes(i);
			if(levelRes != null && levelRes.buffid != -1)
			{
				AddSkillEffect( new AttackerAttr(this),SkillEffectType.Buff,System.Convert.ToUInt32(levelRes.buffid));
				GameDebug.Log("添加翅膀buff:" + levelRes.buffid);
			}
		}
		
	}

    public int GetEquipWing()
    {
        WingData wingData = PlayerDataPool.Instance.MainData.mWingData;
        return wingData.mWearId;
    }

	private void AddWingModel()
	{
		WingData wingData = PlayerDataPool.Instance.MainData.mWingData;
	    if (wingData.mWearId != -1)
	    {
            WingEquip(wingData.mWearId, 0);
            
	    }
        
	}

	public void WingEquip(int wingid ,int action)
	{
		WingData wingData = PlayerDataPool.Instance.MainData.mWingData;
		
		WingItemData itemData = wingData.GetWingItemDataById(wingid);
		WingCommonTableItem commonRes = DataManager.WingCommonTable[wingid] as WingCommonTableItem;
	    string modelName = WingModule.GetModelName(wingid, itemData.level);
	    int wingeffectId = WingModule.GetEffectId(wingid, (int) itemData.level);
		if(action == 0)
		{
			RemoveAttach(AttachMountType.Wing);
			GameDebug.Log("装备翅膀");
			ChangeAttach(AttachMountType.Wing,AssetConfig.WeaponPath + modelName,commonRes.modelSlot);
		    if (wingeffectId != -1)
		    {
                AddEffect((uint)wingeffectId, null, float.NaN, AttachMountType.Wing);
		    }
           
		}
		else
		{
			GameDebug.Log("卸载翅膀");
			RemoveAttach(AttachMountType.Wing);
		}

	}

   
    public void AddFashionModel()
    {
       PlayerFashionData data = PlayerDataPool.Instance.MainData.mFashion;
        if (data == null) return;
        if (data.head_id != -1)
        {
            ChangeFashion(data.head_id,1);
        }

        if (data.upper_body_id != -1)
        {
            ChangeFashion(data.upper_body_id,1);
        }

        if (data.lower_body_id != -1)
        {
            ChangeFashion(data.lower_body_id,1);
        }
    }


	override public int GetMainWeaponID()
    {
        return mUseWeaponID;
    }

    public bool SceneChangeWeapon(int weaponid)
    {
        if( HasSuperWeapon() )
        {
            return false;
        }
        return ChangeUseWeapon(weaponid);
    }
	private bool ChangeUseWeapon(int weaponid)
	{
		if (CanChangeWeapon())
		{
			mUseWeaponID = weaponid;
			ChangeWeapon(mUseWeaponID);

			return true;
		}

		return false;
	}

	public override bool ChangeSkill(int[] skillContainer)
	{
		return false;
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
		return (uint)mModule.GetLevel();
	}

    public override MovingType GetMovingType()
    {
        if (mActiveFlagsContainer[ActiveFlagsDef.Transformed] != 0)
        {
            return MovingType.MoveType_Name;
        }
        return MovingType.MoveType_Lowwer;
    }

	override public string GetObjectTag()
	{
		return ObjectType.ObjectTagPlayer;
	}

    public override int GetWalkSound()
    {
        return 17;
    }
	override public bool Update(uint elapsed)
	{
        //如果是战斗场景. 切状态允许战斗 就自动瞄准

        if (mWaitReload)
        {
            ReloadBullet();
            mWaitReload = false;
        }

        if (mSuperWeaponID >= 0)
        {
            mSuperWeaponTime -= (int)elapsed;
            if( mSuperWeaponTime <= 0 )
            {
                UnEquipSuperWeapon();
            }
        }


        int shoottype = SettingManager.Instance.GetShootType();

        if (shoottype == (int)SHOOT_TYPE.SHOOT_TYPE_AUTO)
        {
                 AutoFire();
        }

        //正在使用技能的时候 锁定一个目标
        if (/*IsCurSkillAutoAim()*/ mAutoAimTarget != null && !IsSkillUsing())
        {
            mAutoAimTarget = null;
        }
		if (!IsDead())
			updateRestore(elapsed);

        updateNamePos();
		//if (Environment.Operation == 0)
		//	AutoFire(elapsed);

        UpdateDailyReset(elapsed);
		UpdateSpIncrease(elapsed);
        UpdateLevelUp(elapsed);
        return base.Update(elapsed);
	}

    public void AutoFire()
    {
        if (IsDead())
            return;
        int skillid = GetWeaponSkillID();

        SkillCommonTableItem skillRes = DataManager.SkillCommonTable[skillid] as SkillCommonTableItem;
        if (skillRes == null)
        {
            return;
        }

        Vector3 pos = GetAimTargetPos();
        if( pos == Vector3.zero )
        {
            return ;
        }

		UseSkill(skillid, pos);
    }

    override protected void OnChangeWeapon()
    {
        if( mWeaopnRes == null )
            return ;

        if (!mOverBullet.ContainsKey( mWeaopnRes.id ))
        {
            mOverBullet.Add(mWeaopnRes.id, mWeaopnRes.max_bullet);
        }

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
        if( mWeaopnRes == null )
        {
            return 0;
        }
        if( !mOverBullet.ContainsKey( mWeaopnRes.id ) )
        {
            return 0;
        }

        return mOverBullet[mWeaopnRes.id];
    }
    //消耗弹药
    override public void CostWeaponBullet(int cost)
    {
        if( mWeaopnRes == null )
        {
            return;
        }
        if (!mOverBullet.ContainsKey(mWeaopnRes.id))
        {
            return;
        }

        int cur = mOverBullet[mWeaopnRes.id];

        cur -= cost;
        if (cur <= 0)
        {
            cur = 0;
            mWaitReload = true;
        }
        mOverBullet[mWeaopnRes.id] = cur;

        OnBulletNumChange();
    }
    override public void AddWeaponBullet(int add)
    {
        if (mWeaopnRes == null)
        {
            return;
        }
        if (!mOverBullet.ContainsKey(mWeaopnRes.id))
        {
            return;
        }

        int cur = mOverBullet[mWeaopnRes.id];

        cur += add;

        if (cur < 0)
            cur = 0;
        if (cur >= GetWeaponMaxBullet())
            cur = GetWeaponMaxBullet();

        mOverBullet[mWeaopnRes.id] = cur;

        OnBulletNumChange();
    }

    private void OnBulletNumChange()
    {
        EventSystem.Instance.PushEvent(new ReloadEvent(ReloadEvent.BULLET_CHANGE_EVENT));
    }
    public void AutoAimEnemy()
    {
        if (IsDead())
            return ;
        if (!IsCanRotation())
            return;
        mAutoAimTarget = Scene.SearchAutoAimEnemy(this, 10);
		// 只需获取到自动锁敌的目标, 进而得到目标位置.
		// 调整玩家朝向, 在技能确定可以使用时, 才进行.
		//if (mAutoAimTarget == null)
		//{
		//	return;
		//}
		//this.SetDirection(Utility.Angle2D(mAutoAimTarget.GetPosition(), this.GetPosition()) * Mathf.Deg2Rad);
    }

    public bool IsAutoAim()
    {
        return (mAutoAimTarget != null);
    }

    public override Vector3 GetAimTargetPos()
    {
        //if (mAutoAimTarget == null)
        {
            AutoAimEnemy();
        }
        if( mAutoAimTarget != null )
            return mAutoAimTarget.GetPosition();
        return Vector3.zero;
    }

    override public bool IsCanMoveRotation()
    {
        return !IsAutoAim();
    }
    public override float GetAnimAngle()
    {
        float moveDir = this.GetMoveDirection();
        float myDir = this.GetDirection();

        return myDir - moveDir + Mathf.PI/2;
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

    void updateNamePos()
    {
        if (mHeadNode != null)
        {
            Vector3 headPos = this.GetBonePositionByName("head");
            if (headPos != Vector3.zero)
            {
                headPos = CameraController.Instance.WorldToScreenPoint(headPos);
                headPos.Set(headPos.x, headPos.y + 10f, 0f);
                //headPos.z = 0.0f;
                mHeadNode.Update(headPos);
            }
        }
    }

	/// <summary>
	/// 更新恢复(血量和蓝量).
	/// </summary>
	/// <param name="elapsed"></param>
	void updateRestore(uint elapsed)
	{
		if (mRestoreInterval > elapsed)
		{
			mRestoreInterval -= elapsed;
			return;
		}

		mRestoreInterval = 1000;

		if (GetHP() < GetMaxHP())
		{
			mHpRegenerated += mHpRegRate;
			int hp = (int)mHpRegenerated;
			if (hp != 0)
			{
				ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeHP, hp);
				mHpRegenerated -= hp;
			}
		}
		if (GetMana() < GetMaxMana())
		{
			mManaRegenerated += mManaRegRate;
			int mana = (int)mManaRegenerated;
			if (mana != 0)
			{
				ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeMana, mana);
				mManaRegenerated -= mana;
			}
		}
	}

    protected override void onPropertyChanged(int id, float oldValue, float newValue)
    {
        base.onPropertyChanged(id, oldValue, newValue);

        //判定是否为当前玩家自己
        PropertyEvent evt = new PropertyEvent(PropertyEvent.FIGHT_PROPERTY_CHANGE);
        EventSystem.Instance.PushEvent(evt);
    }

    protected override void OnKillOther(AttackerAttr attr, BattleUnit theDead)
    {
		base.OnKillOther(attr, theDead);

        BattleUIEvent evt = new BattleUIEvent(BattleUIEvent.BATTLE_UI_KILL_OTHER);
        EventSystem.Instance.PushEvent(evt);
    }

	protected override void onDie(AttackerAttr killerAttr, ImpactDamageType impactDamageType)
	{
		base.onDie(killerAttr, impactDamageType);

		mScene.OnMainPlayerDie();

        if (mHeadNode != null)
        {
            mHeadNode.Hide();
        }
	}

    protected override void onDamage(DamageInfo damage, AttackerAttr attackerAttr)
    {
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

	protected override void onManaChanged(int delta)
	{
		Vector3 headPos = this.GetBonePositionByName("head");
		if (headPos != Vector3.zero)
		{
			headPos = CameraController.Instance.WorldToScreenPoint(headPos);
			headPos.z = 0.0f;

			BattleUIEvent evt = new BattleUIEvent(BattleUIEvent.BATTLE_UI_PLAYER_MANA_CHANGED);
			evt.pos = headPos;
			evt.deltaMana = delta;
			EventSystem.Instance.PushEvent(evt);
		}

		base.onManaChanged(delta);
	}
    override public void Destroy()
    {
        if (mHeadNode != null)
        {
            PlayerHeadUIManager.Instance.ReleasePlayerHeadUI(mHeadNode);
            mHeadNode = null;
        }
        base.Destroy();
    }

	// 拾取半径
	public float PickRadius
	{
		get
		{
			if (HasMagneticEffect())
				return mPickRadius;

			return 0.5f;
		}
		//set
		//{
		//	mPickRadius = PickRadius;
		//}
	}

	protected override void onModelLoaded (GameObject obj)
	{
		base.onModelLoaded (obj);
		Scene.OnSpriteModelLoaded(mInstanceID);
	


	}

    private void UpdateDailyReset(uint elapsed)
    {
        if(TimeUtilities.GetNow() > PlayerDataPool.Instance.MainData.next_daily_resettime * 1000)
        {
            if(mDailyResetTimer > elapsed)
            {
                mDailyResetTimer -= elapsed;
            }
            else
            {
                mDailyResetTimer = 0;
            }

            if (mDailyResetTimer > 0)
                return;

            mDailyResetTimer = 5000;
            Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_DAILY_RESET, null);
        }
        else
        {
            mDailyResetTimer = 0;
        }


    }

	private void UpdateSpIncrease(uint elapsed)
	{
		if (mModule.GetSP() < mModule.GetSPMax())
		{
			if (TimeUtilities.GetUtcNowSeconds() > PlayerDataPool.Instance.MainData.sp_next_inc_time)
			{
				if (mSpIncreaseTimer > elapsed)
				{
					mSpIncreaseTimer -= elapsed;
				}
				else
				{
					mSpIncreaseTimer = 0;
				}

				if (mSpIncreaseTimer > 0)
					return;

				mSpIncreaseTimer = 5000;
				Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_SP_INCREASE, null);
			}
			else
			{
				mSpIncreaseTimer = 0;
			}
		}
	}

    private void UpdateLevelUp(uint elapsed)
    {
        if (mScene == null)
            return;

        if (!SceneManager.Instance.IsCurSceneType(SceneType.SceneType_City))
            return;

        if (PlayerDataPool.Instance.MainData.mLevelUp.Count <= 0)
            return;

        int level = -1;
        for(int i = 0; i < PlayerDataPool.Instance.MainData.mLevelUp.Count; i++)
        {
            level = PlayerDataPool.Instance.MainData.mLevelUp[i];
            OnLevelUp(PlayerDataPool.Instance.MainData.mLevelUp[i]);
        }
        PlayerDataPool.Instance.MainData.mLevelUp.Clear();

        mScene.CreateEffect(GameConfig.LevelUpEffectID, Vector3.one, GetPosition());

        CameraController.Instance.ShakeCamera(GameConfig.LevelUpShakeCameraAmount, GameConfig.LevelUpShakeCameraTime);

        if(level > 0)
        {
            UILevelUpInitParam param = new UILevelUpInitParam();
            param.Level = (uint)level;

            WindowManager.Instance.QueueOpenUI("levelup", param);
        }
        
    }

    private void OnLevelUp(int toLevel)
    {
		LevelTableItem levelRes = DataManager.LevelTable[toLevel] as LevelTableItem;
		if (levelRes == null)
		{
			ErrorHandler.Parse(ErrorCode.ConfigError, "level.txt不存在等级" + toLevel);
			return;
		}

		mHpRegRate = levelRes.hpRegRate;
		mManaRegRate = levelRes.manaRegRate;
    }

	public override ErrorCode SkillTransform(IEnumerable<Pair<uint, string>> newSkills)
	{
		PlayerSkillTransformEvent e = new PlayerSkillTransformEvent(newSkills);
		EventSystem.Instance.PushEvent(e);
		return ErrorCode.Succeeded;
	}

    public override void OnEnterScene(BaseScene scene, uint instanceid)
    {
        base.OnEnterScene(scene, instanceid);
        AddWingSkillBuff();
        AddWingModel();
        AddFashionModel();

    }

    public void OnPickSuperWeapon(int superid)
    {
        if (!DataManager.SuperWeaponTable.ContainsKey(superid))
        {
            return;
        }
        SuperWeaponTableItem item = DataManager.SuperWeaponTable[superid] as SuperWeaponTableItem;

        if( item == null )
        {
            return;
        }

        mSuperWeaponID = item.weaponid;
        mSuperWeaponTime = item.lifetime;
        EquipSuperWeapon();
    }

    public bool HasSuperWeapon()
    {
        return mSuperWeaponID >= 0;
    }

    public void EquipSuperWeapon()
    {
        if( mLastUseWeaponID < 0 )
        {
            mLastUseWeaponID = mUseWeaponID;
        }
        ChangeUseWeapon(mSuperWeaponID);

        ReloadEvent evt = new ReloadEvent(ReloadEvent.SUPER_WEAPON_EVENT);
        evt.reload_time = mSuperWeaponTime;
        evt.super_weapon = mSuperWeaponID;
        EventSystem.Instance.PushEvent(evt);

    }

	public bool CanPick(Pick.PickType type, int content)
	{
		if (type == Pick.PickType.SUPER_WEAPON && !CanChangeWeapon())
			return false;

		return true;
	}

    public void UnEquipSuperWeapon()
    {
        if( mLastUseWeaponID < 0 )
        {
            return ;
        }
        ChangeUseWeapon(mLastUseWeaponID);

        mLastUseWeaponID = -1;
        mSuperWeaponID = -1;

        ReloadEvent evt = new ReloadEvent(ReloadEvent.SUPER_WEAPON_EVENT);
        evt.reload_time = -1;
        evt.super_weapon = -1;
        EventSystem.Instance.PushEvent(evt);
    }
}
