using UnityEngine;
using System.Collections;
public class BuildInitParam : BattleUnitInitParam
{
    public int build_res_id = -1;

	public bool build_barrier = false;
}
public class BuildObj : BattleUnit
{
    private BuildTableItem mRes = null;

    private uint mDieEffectID = uint.MaxValue;

	private uint mBornEffectID = uint.MaxValue;

	private int mBornEffectTime = 0;

	// 是否阻挡
	private bool mBarrier = false;

	// 延迟0.2秒删出生特效 为了和死亡特效衔接上
	private uint mEffectDelta = 200;

    public BuildObj()
    {

    }

    override public int Type
    {
        get
        {
            return ObjectType.OBJ_BUILD;
        }
    }

    override public bool Init(ObjectInitParam param)
    {
        BuildInitParam buildParam = (BuildInitParam)param;

        if (!DataManager.BuildTable.ContainsKey(buildParam.build_res_id))
        {
            return false;
        }
        mRes = DataManager.BuildTable[buildParam.build_res_id] as BuildTableItem;

        if( mRes == null )
        {
            GameDebug.LogError("未找到buildobj id = " + buildParam.build_res_id.ToString());
            return false;
        }

        mModelResID = mRes.modelId;
        SetLeague(mRes.league);
        mDestroyWaiting = true;
        mMaxWaitDisappearTime = mRes.die_time;
        mMaxDisappearTime = 0.0f;
		mBarrier = buildParam.build_barrier;

		param.init_shape = new SceneShapeParam();
		param.init_shape.mType = mRes.shapeType;
		param.init_shape.mParams.Add(mRes.shapeParam1);
		param.init_shape.mParams.Add(mRes.shapeParam2);

        if (!base.Init(param))
            return false;

        InitProperty();

        return true;
    }

	public override string dbgGetIdentifier()
	{
		return "building " + mRes.resID;
	}

    public override void OnEnterScene(BaseScene scene, uint instanceid)
    {
        if( mRes != null)
        {
			mBornEffectTime = mRes.born_effect_time;
			if(mRes.born_effect >= 0)
			{
				mBornEffectID = scene.CreateEffect(mRes.born_effect, new Vector3(mRes.born_effect_scale_x, mRes.born_effect_scale_y, mRes.born_effect_scale_z), 
					GetPosition(), GetDirection(), null);
			}

			if (mRes.buffID != uint.MaxValue)
				AddBornSkillEffect(new AttackerAttr(this), SkillEffectType.Buff, mRes.buffID);
        }

        base.OnEnterScene(scene, instanceid);
    }

	protected override void onModelLoaded(GameObject obj)
	{
		base.onModelLoaded(obj);
	}

    override public string GetObjectTag()
    {
        return ObjectType.ObjectTagBuild;
    }
    override public void Destroy()
    {
        if (mDieEffectID != uint.MaxValue)
        {
            this.RemoveEffect(mDieEffectID);
        }

		DestroyBornEffect();

        base.Destroy();
    }

    override public bool Update(uint elapsed)
    {
		if (mBornEffectTime > 0)
		{
			mBornEffectTime -= (int)elapsed;

			if (mBornEffectTime <= 0)
			{
				DestroyBornEffect();
			}
			return true;
		}

        if (IsDead())
            return false;
        return base.Update(elapsed);
    }

	public override bool UpdateDestroy(uint elapsed)
	{
		if(!base.UpdateDestroy(elapsed))
		{
			return false;
		}

		if (mEffectDelta > 0)
		{
			mEffectDelta -= elapsed;
			if(mEffectDelta <= 0)
			{
				DestroyBornEffect();
			}
		}

		return true;
	}

    override public uint GetMaterialResourceID()
    {
        if (mRes == null)
            return uint.MaxValue;
        return mRes.materialID;
    }
    private void InitProperty()
    {
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeHP, mRes.hp);
        SetBaseProperty((int)PropertyTypeEnum.PropertyTypeMaxHP, mRes.hp);
    }
    protected override void onDie(AttackerAttr killerAttr, ImpactDamageType impactDamageType)
    {
		AnimActionDeath death = AnimActionFactory.Create(AnimActionFactory.E_Type.Death) as AnimActionDeath;
		death.dieAnim = "Base Layer." + mRes.die_ani;
		GetStateController().DoAction(death);

		DisplayDieEffect();

		//DestroyBornEffect();

		base.onDie(killerAttr, impactDamageType);
    }

// 	public override float GetRadius()
// 	{
// 		return mRes != null ? mRes.radius : base.GetRadius();
// 	}

	override public bool IsBarrier()
	{
		return mBarrier;
	}

	private void DestroyBornEffect()
	{
		if(mBornEffectID != uint.MaxValue)
		{
			RemoveEffect(mBornEffectID);
			mBornEffectID = uint.MaxValue;
		}
	}

	protected override bool SkillEffectImmunity(SkillEffectType type)
	{
		// 免疫位移, buff和硬直.
		return type == SkillEffectType.Displacement || type == SkillEffectType.Buff || type == SkillEffectType.Spasticity;
	}

	private void DisplayDieEffect()
	{
		if (mRes.die_effect < 0)
		{
			return;
		}

		if (string.IsNullOrEmpty(mRes.die_bone))
		{
			mDieEffectID = mScene.CreateEffect(mRes.die_effect, new Vector3(mRes.born_effect_scale_x, mRes.born_effect_scale_y, mRes.born_effect_scale_z),
					GetPosition(), GetDirection(), null);
		}
		else
		{
            mDieEffectID = this.AddEffect((uint)mRes.die_effect, mRes.die_bone, float.NaN);
		}
	}
}
