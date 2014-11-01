using UnityEngine;
using System.Collections;
public class PickInitParam : VisualObjectInitParam
{
	public Pick.PickType pick_type = Pick.PickType.INVALID;
	public Pick.FlyType fly_type = Pick.FlyType.FLY_OUT;

	public int pick_res_id = -1;
	public int content = -1;
	public bool random_pos = false;
}
public class Pick : VisualObject
{
	// 拾取物类型
	public enum PickType : int
	{
		INVALID = -1,
		BUFF = 0,
		ITEM = 1,
		MONEY = 2,
        SUPER_WEAPON = 3,

		TYPE_COUNT
	}

	// 飞行类型
	public enum FlyType : int
	{
		FLY_OUT = 0,	// 抛出
		DROP_DOWN = 1,	// 坠落
	}

	protected PickTableItem mRes = null;

	// 是否浮动
	private bool mNeedFloat = false;

	// 是否旋转
	private bool mNeedRotation = false;

	// 浮动Y上限
	private float mMaxFloatY;

	// 浮动Y下限
	private float mMinFloatY;

	// 浮动速度
	private float mFloatSpeed;

	// 旋转速度
	private float mRotation;

	// 存在时间
	private uint mAliveTime = 0;

// 	// 碰触CD
// 	private uint mTouchTime = 0;

	// 销毁计时
	private uint mDestroyTime = 0;

	// 掉落0.5秒后才可拾取
	private const uint mProtectedTime = 500;

// 	// 碰触检查间隔0.2秒
// 	private const uint mTouchProtectedTime = 200;
// 
// 	// 拾取0.5秒后销毁
// 	private const uint mDestroyProtectedTime = 500;

	// 内容物
	private int mContent = -1;

	// 拾取物类型
	private PickType mPickType = PickType.INVALID;

	// 飞行类型
	private FlyType mFlyType = FlyType.FLY_OUT;

//	// 是否抛物中
//	private bool mIsFlying = false;
	
	// 抛物方向
	private Vector3 mFlyDir = Vector3.zero;

	// 飞行速度
	private float mFlySpeed = 3.0f;

	// 重力加速度
	private static readonly float G = GameConfig.GravitationalAcceleration;

	// 抛物高度速度
	private float mSpeedY = 15.0f;

	// 追踪速度
	private float mFollowSpeed = 6.0f;

	// 追踪速度Y
	private float mFollowSpeedY = 6.0f;

	// 追踪加速度
	private float mFollowAcc = 20.0f;

	// 拾取者
	private Player mPicker = null;

	// 出生特效
	private uint mPickEffectID = uint.MaxValue;

	// 当前状态
	private PickState mState = PickState.Invalid;

    //是否可拾取
    public bool mIsPickable = true;

	// 是否随机位置
	private bool mRandomPos = true;

	// 状态
	private enum PickState : int
	{
		// 未创建或已销毁
		Invalid = -1,

		// 抛物中
		FlyOut = 0,

		// 旋转中 等待拾取
		Rotate = 1,

		// 追踪中
		Follow = 2,
	}

	public Pick()
	{
        
	}
	override public bool Init(ObjectInitParam param)
	{
		PickInitParam pickParam = (PickInitParam)param;

		if (!DataManager.PickTable.ContainsKey(pickParam.pick_res_id))
		{
			GameDebug.LogError("pick表中不存在ID: " + pickParam.pick_res_id);
			return false;
		}

		mRes = DataManager.PickTable[pickParam.pick_res_id] as PickTableItem;
		mModelResID = mRes.modelId;
		mContent = pickParam.content;
		mPickType = pickParam.pick_type;
		mFlyType = pickParam.fly_type;
		mRandomPos = pickParam.random_pos;
		
		if (!base.Init(param))
			return false;

        InitProperty();

		return true;

	}

	public override string dbgGetIdentifier()
	{
		return "pick: " + mRes.resID;
	}

    override public int Type
    {
        get
        {
            return ObjectType.OBJ_PICK;
        }
    }

    private void InitProperty()
    {
        if(mRes == null)
		{
			return;
		}

		if(mRes.moveDist > float.Epsilon && mRes.moveSpeed > float.Epsilon)
		{
			mMinFloatY = mPosition.y;
			mMaxFloatY = mPosition.y + mRes.moveDist;
			mFloatSpeed = mRes.moveSpeed;

			mNeedFloat = true;
		}

		if(mRes.rotateAngle > float.Epsilon)
		{
			mRotation = mRes.rotateAngle * Mathf.Deg2Rad;

			mNeedRotation = true;
		}

		mDestroyWaiting = true;
    }

	// 出生特效
	private void DisplayBornParticle()
	{
		if(mRes == null || mRes.dropParticleId < 0)
		{
			return;
		}

		if(!DataManager.EffectTable.ContainsKey(mRes.dropParticleId))
		{
			return;
		}

		if(string.IsNullOrEmpty(mRes.dropParticleBone))
		{
            mPickEffectID = AddEffect((uint)mRes.dropParticleId, Vector3.zero, float.NaN);
		}
		else
		{
			mPickEffectID = AddEffect((uint)mRes.dropParticleId, mRes.dropParticleBone, 0f);
		}
	}

	// 触碰特效
	private void DisplayTouchParticle()
	{
		if (mRes == null || mRes.pickParticleId < 0)
		{
			return;
		}

		if (!DataManager.EffectTable.ContainsKey(mRes.pickParticleId))
		{
			return;
		}


        //Pick完后 Pick实例从场景删除 所以特效不能挂在Pick上 不然会跟着删除

		this.Scene.CreateEffect(mRes.pickParticleId, Vector3.one, this.GetPosition());

// 		if(string.IsNullOrEmpty(mRes.pickParticleBone))
// 		{
// 			AddEffect((uint)mRes.pickParticleId, Vector3.zero);
// 		}
// 		else
// 		{
// 			AddEffect((uint)mRes.pickParticleId, mRes.pickParticleBone);
// 		}
	}

	private void AddPlayerPickParticle()
	{
		if (mPicker == null || mPicker.IsDead() || mRes == null || mRes.playerParticleId < 0)
		{
			return;
		}

		if(!DataManager.EffectTable.ContainsKey(mRes.playerParticleId))
		{
			return;
		}

		if(string.IsNullOrEmpty(mRes.playerParticleBone))
		{
			mPicker.AddEffect((uint)mRes.playerParticleId, Vector3.zero, float.NaN);
		}
		else
		{
			mPicker.AddEffect((uint)mRes.playerParticleId, mRes.playerParticleBone, 0f);
		}
	}

	override public int GetObjectLayer()
	{
		return (int)ObjectLayerType.ObjectLayerObjects;
	}

	override public string GetObjectTag()
	{
		return ObjectType.ObjectTagPick;
	}

	override public bool Update(uint elapsed)
	{
		if(!base.Update(elapsed))
		{
			return false;
		}

		if(mDestroyTime > 0)
		{
			return false;
		}

		switch(mState)
		{
			case PickState.Invalid:
				{

				}
				break;
			case PickState.FlyOut:
				{
					UpdateFlying(elapsed);
				}
				break;
			case PickState.Rotate:
				{
					mAliveTime += elapsed;
					if (mRes.keepTime > 0 && mAliveTime >= mRes.keepTime)
					{
						return false;
					}

					if (mNeedFloat)
					{
						UpdateFloat(elapsed);
					}

					if (mNeedRotation)
					{
						UpdateRotation(elapsed);
					}

					if (mAliveTime < mProtectedTime)
					{
						return true;
					}

					UpdateTouch();
				}
				break;
			case PickState.Follow:
				{
					if(!UpdateFollow(elapsed))
					{
						return false;
					}
				}
				break;
			default:
				return false;
		}

		return true;
	}

	public override bool UpdateDestroy(uint elapsed)
    {
// 		mDestroyTime += elapsed;
// 		if (mDestroyTime > mDestroyProtectedTime)
// 		{
// 			//todo.渐隐 等接口
// 			return false;
// 		}
// 
//         return true;

		return false;
    }

	// 上下浮动
	private void UpdateFloat(uint elapsed)
	{
		Vector3 pos = GetPosition();

		float newY = pos.y + elapsed * mFloatSpeed;

		if(newY > mMaxFloatY)
		{
			newY = mMaxFloatY;
			mFloatSpeed = -mFloatSpeed;
		}
		else if(newY < mMinFloatY)
		{
			newY = mMinFloatY;
			mFloatSpeed = -mFloatSpeed;
		}

		pos.y = newY;
		SetPosition(pos);
	}

	// 旋转
	private void UpdateRotation(uint elapsed)
	{
		float dir = GetDirection() + elapsed * mRotation;
		if(dir > 2 * Mathf.PI)
		{
			dir -= 2 * Mathf.PI;
		}

		SetDirection(dir);
	}

	// 抛物丢出
	private void UpdateFlying(uint elapsed)
	{
		float seconds = elapsed / 1000f;

		mFlySpeed += elapsed * 5 / 1000f;

		float moveDistance = seconds * mFlySpeed + 5 * seconds * seconds * 0.5f;

		if(mFlyType == FlyType.FLY_OUT)
		{
			//Vector3 forward为zero时会有Log报出
			Quaternion rotation = new Quaternion();
			if (!mFlyDir.Equals(Vector3.zero))
				rotation = Quaternion.LookRotation(mFlyDir);
			Vector3 nextPosition = (GetPosition() + (rotation * Vector3.forward) * moveDistance);

			float heightOffset = mSpeedY * seconds - seconds * seconds * G * 0.5f;

			mSpeedY -= seconds * G * 4.5f;

			nextPosition.y += heightOffset;

			if (!mScene.IsInWalkableRegion(nextPosition.x, nextPosition.z))
			{
				mAliveTime = 0;
				mState = PickState.Rotate;
				return;
			}

			float horizon = mScene.GetHeight(nextPosition.x, nextPosition.z);

			if (nextPosition.y <= horizon)
			{
				nextPosition.y = horizon;
				mAliveTime = 0;
				mState = PickState.Rotate;
			}

			SetPosition(nextPosition);
		}
		else if(mFlyType == FlyType.DROP_DOWN)
		{
			Vector3 nextPosition = new Vector3(mPosition.x, mPosition.y - moveDistance, mPosition.z);
			float limitY = mScene.GetHeight(nextPosition.x, nextPosition.z);
			if (nextPosition.y < limitY)
			{
				nextPosition.y = limitY;
				mAliveTime = 0;
				mState = PickState.Rotate;
			}

			SetPosition(nextPosition);
		}
	}

	// 碰撞
	private void UpdateTouch()
	{
		ObjectBase obj = PlayerController.Instance.GetControlObj();
		if(!typeof(Player).IsAssignableFrom(obj.GetType()))
		{
			return;
		}

	    if (!mIsPickable) return;

		Player player = obj as Player;
		if (player.CanPick(mPickType, mContent) 
			&& Utility.Distance2D(player.GetPosition(), this.GetPosition()) < (player.PickRadius + this.GetRadius()))
		{
			OnTouch(player);
		}
	}

	// 追踪
	private bool UpdateFollow(uint elapsed)
	{
		if(mPicker == null || mPicker.IsDead())
		{
			return false;
		}

		if(Utility.Distance2D(mPicker.GetPosition(), this.GetPosition()) < 0.5f)
		{
			OnPickUp();
			mState = PickState.Invalid;
			return false;
		}

		float seconds = elapsed / 1000f;
		mFollowSpeed += elapsed * mFollowAcc / 1000f;

		float moveDistance = seconds * mFollowSpeed + mFollowAcc * seconds * seconds;

		Vector3 dir = mPicker.GetPosition() - GetPosition();

		Quaternion rotation = Quaternion.LookRotation(dir);
		Vector3 nextPosition = (GetPosition() + (rotation * Vector3.forward) * moveDistance);

		float offsetY = mPicker.GetPosition().y - nextPosition.y + 2.0f;
		if(offsetY > float.Epsilon || offsetY < -float.Epsilon)
		{
			nextPosition.y += mFollowSpeedY * seconds;
		}

		float horizon = mScene.GetHeight(nextPosition.x, nextPosition.z);

		SetPosition(nextPosition);

		return true;
	}

	// 碰到 开始追踪
	private void OnTouch(Player player)
	{
		mPicker = player;

		DisplayTouchParticle();

		mState = PickState.Follow;
	}

	// 拾取结束
	private void OnPickUp()
	{
		AddPlayerPickParticle();

		AddPickerEffect();

		AddPickerBuff();

		PlayPickSound();

		PayAward();

		mScene.OnPick(this, mPicker);

        if (mPickType == PickType.SUPER_WEAPON)
        {
            mPicker.OnPickSuperWeapon(mContent);
        }
	}

	// 播放拾取音效
	private void PlayPickSound()
	{
		SoundManager.Instance.Play(mRes.pickSoundId);
	}

	// 给奖励
	private void PayAward()
	{
		PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
		if(pdm == null)
		{
			return;
		}

		if(mPickType == PickType.MONEY)
		{
			if(mContent > 0)
			{
                //pdm.ChangeProceeds(ProceedsType.Money_Game, mContent);
			}
		}
		else if(mPickType == PickType.ITEM)
		{
			//pdm.CreateItemUnreal(mContent, PackageType.Pack_Bag);
		}
	}

	// 加effect
	private void AddPickerEffect()
	{
		if (mPicker == null || mPicker.IsDead() || mRes.skillEffect2Picker == uint.MaxValue)
		{
			return;
		}

		SkillDetails.AddSkillEffectByResource(new AttackerAttr(mPicker), mPicker, mRes.skillEffect2Picker);
	}

	private void AddPickerBuff()
	{
		if (mPicker == null || mPicker.IsDead() || string.IsNullOrEmpty(mRes.skillBuff2Picker))
		{
			return;
		}

		string[] bufflist = mRes.skillBuff2Picker.Split('|');
		int idx = Random.Range(0, bufflist.Length);
		uint buffid = System.Convert.ToUInt32(bufflist[idx]);

		mPicker.AddSkillEffect(new AttackerAttr(mPicker), SkillEffectType.Buff, buffid);
	}

    override public void Destroy()
    {
        if (mPickEffectID != uint.MaxValue)
        {
            this.RemoveEffect(mPickEffectID);
        }
        base.Destroy();
    }

	public override void OnEnterScene(BaseScene scene, uint instanceid)
	{
		base.OnEnterScene(scene, instanceid);

		if(mRes == null)
		{
			return;
		}

		// 不飞出
		if(mRes.flyOut < 1)
		{
			if(mRandomPos)
			{
				Vector3 pos = new Vector3(mPosition.x + Random.RandomRange(-1.0f, 1.0f), 0.0f, mPosition.z + Random.RandomRange(-1.0f, 1.0f));
				pos.y = mScene.GetHeight(pos.x, pos.z);
				SetPosition(pos);
			}

			mState = PickState.Rotate;
		}
		else
		{
			// 坠落
			if(mFlyType == FlyType.DROP_DOWN)
			{
				if (mRandomPos)
				{
					Vector3 pos = new Vector3(mPosition.x + Random.RandomRange(-1.0f, 1.0f), 0.0f, mPosition.z + Random.RandomRange(-1.0f, 1.0f));
					pos.y = mScene.GetHeight(pos.x, pos.z) + 4.0f;
					SetPosition(pos);
				}
				else
				{
					Vector3 pos = new Vector3(mPosition.x, mScene.GetHeight(mPosition.x, mPosition.z) + 4.0f, mPosition.z);
					SetPosition(pos);
				}
			}
			else // 抛出
			{
				if (mRandomPos)
				{
					mFlyDir = new Vector3(Random.RandomRange(-1.0f, 1.0f), 0.0f, Random.RandomRange(-1.0f, 1.0f));
				}
			}
			
			mState = PickState.FlyOut;
		}
	}

	override protected void onModelLoaded(GameObject obj)
	{
		base.onModelLoaded(obj);

		DisplayBornParticle();
	}

	public override float GetRadius()
	{
		return mRes != null ? mRes.radius : base.GetRadius();
	}

	public PickTableItem GetCurPickTableItem()
	{
		return mRes;
	}
}

