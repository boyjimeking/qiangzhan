using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using FantasyEngine;



//角色单元
public class RoleInitParam : BattleUnitInitParam
{

}
public abstract class Role : BattleUnit
{
    private PaoPaoNode mPaoPao = null;

    protected RoleState mRoleState = RoleState.RoleState_Invaild;

    //private RoleAnimationDir mLastAnimationDir = RoleAnimationDir.AnimationDir_Front;

    private AttachMent mWeaponAttach = null;
    protected WeaponTableItem mWeaopnRes = null;


    private MeshVisual mWeaponVisual = null;

    private uint mLastWeaponBuffID = uint.MaxValue;


    private uint mWaveWingTime = 0;

    override public bool Init(ObjectInitParam param)
    {
        if (!base.Init(param))
            return false;
        //RoleInitParam roleParam = (RoleInitParam)param;

        mHasShadow = true;
        return true;
    }

    protected override void DestroyVisual()
    {
        //清除挂接的特效的关联，特效就不会随着物体的删除而删除
        foreach (ParticleAttachMent attach in mAttachParticles)
        {
            if (attach != null)
            {
                attach.parent = null;
                ParticleItem item = SceneManager.Instance.GetCurScene().GetParticleManager().GetParticle(attach.particleid);
                if (item != null && item.visual != null && item.visual.Visual != null)
                {
                    item.visual.VisualTransform.parent = null;
                    item.visual.Visual.SetActive(false);

                }

            }
        }

        ///挂接的物体也摘除下来
       foreach(AttachMent attach in mAttachMents)
       {
           if (attach != null)
           {
               attach.parent = null;
               if (attach.visual != null && attach.visual.Visual != null)
               {
                   attach.visual.VisualTransform.parent = null;
                   attach.visual.Visual.SetActive(false);

               }

           }
       }

        base.DestroyVisual();
    }

	/// <summary>
	/// 对玩家, 只改变模型; 其他单位模型和武器技能都被改变.
	/// </summary>
	public override bool ChangeWeapon(int weaponID)
	{
		if (!CanChangeWeapon() || mActiveFlagsContainer[ActiveFlagsDef.DisableChangeWeaponModel] != 0)
		{
			return false;
		}

		if (weaponID == -1)
			return false;

		//测试武器
		if (!DataManager.WeaponTable.ContainsKey(weaponID))
		{
			GameDebug.LogError(dbgGetIdentifier() + " not find weapon id=" + weaponID.ToString());
			return false;
		}

		if (mWeaponAttach != null)
		{
			DetachVisual(mWeaponAttach);
			mWeaponAttach.visual.Destroy();
			mWeaponAttach = null;


		}
		RemoveAttach(AttachMountType.Weapon);
		if (mWeaponVisual != null)
		{
			mWeaponVisual.Destroy();
			mWeaponVisual = null;
		}

		if (mLastWeaponBuffID != uint.MaxValue)
		{
			ErrorHandler.Parse(
				RemoveSkillBuffByResID(mLastWeaponBuffID),
				"failed to remove skill buff on skill stopped"
				);
			mLastWeaponBuffID = uint.MaxValue;
		}

		mWeaopnRes = DataManager.WeaponTable[weaponID] as WeaponTableItem;

		mWeaponVisual = new MeshVisual();
		if (string.IsNullOrEmpty(mWeaopnRes.modelname))
			onWeaponVisualSucess();
		else
			mWeaponVisual.CreateWithConfig(AssetConfig.WeaponPath + mWeaopnRes.modelname, onWeaponVisualSucess, onWeaponVisualFail, false);

		return true;
	}

    private void onWeaponVisualSucess()
    {
        if (mWeaponVisual == null || mWeaopnRes == null)
            return;
        TransformData trans = new TransformData();
        trans.Rot = new Vector3(90, 0, 0);
        trans.Scale = Vector3.one * mWeaopnRes.scale;
        //mWeaponAttach = AttachVisual(mWeaponVisual, mWeaopnRes.mountpoint, trans);

        mWeaponAttach = ChangeAttach(AttachMountType.Weapon, mWeaponVisual, mWeaopnRes.mountpoint, trans);


        if (mWeaopnRes.weapon_buff != uint.MaxValue)
            ErrorHandler.Parse(
            AddSkillEffect(new AttackerAttr(this), SkillEffectType.Buff, mWeaopnRes.weapon_buff),
            "in Npc::AddBornBuff"
            );

        mLastWeaponBuffID = mWeaopnRes.weapon_buff;

        PlayWeaponAnim(AnimationNameDef.WeaponDefault);

        OnChangeWeapon();
    }

    private void onWeaponVisualFail()
    {
        GameDebug.LogError(dbgGetIdentifier() + " 武器模型加载失败");
    }

    override public int GetWeaponSkillID()
    {
        if (mWeaopnRes == null)
            return -1;
        return mWeaopnRes.skill_1;
    }

    protected virtual void OnChangeWeapon()
    {
        if (mWeaopnRes == null)
            return;

    }

    public override void PlayWeaponAnim(string statename)
    {
        if (mWeaponVisual != null && mWeaponVisual.AnimManager != null && mWeaponVisual.AnimManager.Property != null)
        {
            int stateid = mWeaponVisual.AnimManager.Property.GetStateHash(statename);
            if (stateid == 0)
            {
                stateid = mWeaponVisual.AnimManager.Property.GetStateHash("Base Layer.emptyState");
            }

            mWeaponVisual.AnimManager.Anim.SetInteger("state", stateid);

        }
    }
    public override void PlayWingAnim(string statename)
    {
        AttachMent attach = GetAttach(AttachMountType.Wing);
        if (attach == null)
            return;
        MeshVisual wingVisual = attach.visual as MeshVisual;
        if (wingVisual != null && wingVisual.AnimManager != null && wingVisual.AnimManager.Property != null)
        {
            int stateid = wingVisual.AnimManager.Property.GetStateHash(statename);
            if (stateid == 0)
            {
                stateid = wingVisual.AnimManager.Property.GetStateHash("Base Layer.emptyState");
            }

            wingVisual.AnimManager.Anim.SetInteger("state", stateid);
        }
    }

    private void UpdateWingAnim(uint elapsed)
    {
        AttachMent attach = GetAttach(AttachMountType.Wing);
        if (attach == null)
            return;
        MeshVisual wingVisual = attach.visual as MeshVisual;
        if (wingVisual != null && wingVisual.AnimManager != null && wingVisual.AnimManager.Property != null)
        {

            AnimatorStateInfo info = wingVisual.AnimManager.Anim.GetCurrentAnimatorStateInfo(0);

            if (!info.IsName(AnimationNameDef.WingDefault) && !info.IsName(AnimationNameDef.WingEmpty) && info.normalizedTime >= 1 && !info.loop)
            {
                int stateid = wingVisual.AnimManager.Property.GetStateHash(AnimationNameDef.WingDefault);

                if (stateid == 0)
                {
                    stateid = wingVisual.AnimManager.Property.GetStateHash(AnimationNameDef.WingEmpty);
                }
                wingVisual.AnimManager.Anim.SetInteger("state", stateid);
            }
            mWaveWingTime += elapsed;
            if (mWaveWingTime >= GameConfig.WaveWingFrequency * 1000 && (info.IsName(AnimationNameDef.WingDefault) || info.IsName(AnimationNameDef.WingEmpty)))
            {
                mWaveWingTime = 0;
                int stateid = wingVisual.AnimManager.Property.GetStateHash(AnimationNameDef.WingFei);

                if (stateid != 0)
                {
                    wingVisual.AnimManager.Anim.SetInteger("state", stateid);
                }
            }
        }
    }

    public AttachMent GetAttach(AttachMountType type)
    {
        if (type < 0 || type >= AttachMountType.AttachCount)
            return null;
        return mAttachMents[(int)type];
    }
    public void ChangeAttach(AttachMountType type, string path, string mount)
    {
        MeshVisual visual = new MeshVisual();
        visual.CreateWithConfig(path, null, null, false);
        ChangeAttach(type, visual, mount);
    }

    public AttachMent ChangeAttach(AttachMountType type, PrimitiveVisual visual, string mount, TransformData trans = null)
    {
        if (type >= AttachMountType.AttachCount)
            return null;
        AttachMent attach = mAttachMents[(uint)type];
        if (attach != null && attach.visual == visual)
            return attach;

        if (attach != null && attach.visual != null)
            attach.visual.Destroy();

        attach = new AttachMent();
        attach.visual = visual;
        attach.socketname = mount;
        attach.transform = trans;
        mAttachMents[(uint)type] = attach;
        return attach;
    }

    /// <summary>
    /// 移除挂接物体
    /// </summary>
    /// <param name="type"></param>
    public void RemoveAttach(AttachMountType type)
    {
        if (type >= AttachMountType.AttachCount)
            return;
        AttachMent attach = mAttachMents[(uint)type];
        if (attach != null && attach.visual != null)
            attach.visual.Destroy();
        mAttachMents[(uint)type] = null;
    }

    public override Transform GetBoneTransofrm(string bonename)
    {
        if (string.IsNullOrEmpty(bonename))
            return null;

        if (mVisual == null || mVisual.Visual == null)
            return null;

        Transform t = null;

        //%标示武器上的绑点
        if (bonename[0] == '%')
        {
            if (mWeaponAttach == null || mWeaponAttach.visual == null)
            {
                GameDebug.LogError(dbgGetIdentifier() + "无法在武器上查找骨骼" + bonename.Substring(1) + ", 没有武器模型");
                return null;
            }

            t = mWeaponAttach.visual.GetBoneByName(bonename.Substring(1));

            if (t != null)
            {
                //虚拟体的三个轴部分有问题
                if (t.localScale.x != t.localScale.y ||
                    t.localScale.x != t.localScale.z ||
                    t.localScale.y != t.localScale.z)
                {
                    t.localScale = Vector3.one * t.localScale.x;
                }

            }

            if (t == null)
                GameDebug.LogError(dbgGetIdentifier() + " 未在武器上找到骨骼" + bonename.Substring(1));
        }
        else
        {
            t = mVisual.GetBoneByName(bonename);

            Renderer render = mVisual.GetRenderer();
            if (t == null && render != null && render is SkinnedMeshRenderer)
            {
                GameDebug.LogError(dbgGetIdentifier() + " 未在角色模型上找到骨骼" + bonename);
            }
        }

        return t;
    }

    override protected void onModelLoaded(GameObject obj)
    {
        base.onModelLoaded(obj);

        if (mWeaopnRes != null)
        {
            ChangeWeapon(mWeaopnRes.id);
        }
        else
        {
            ChangeWeapon(GetMainWeaponID());
        }

        ChangeState(RoleState.RoleState_Idle);

		Scene.onRoleModelLoaded(this);
    }

    override public int GetObjectLayer()
    {
        return (int)ObjectLayerType.ObjectLayerPlayer;
    }

    override public string GetObjectTag()
    {
        return ObjectType.ObjectTagPlayer;
    }

    public void ChangeState(RoleState state)
    {
        if (state != mRoleState)
        {
            mRoleState = state;

            refreshAnimation();
        }
    }


    public override string CombineAnimname(string aniName)
    {
        if (mWeaopnRes == null || string.IsNullOrEmpty(aniName))
            return aniName;

        mTempAnimStr.Length = 0;
        int nStart = 0;
        int nLength = aniName.Length;
        if (aniName[0] == '%')
        {
            mTempAnimStr.Append(mWeaopnRes.ani_pre);
            mTempAnimStr.Append(".");
            nStart = 1;
            nLength -= 1;
        }
        else
        {
            mTempAnimStr.Append("Base Layer.");
        }
        mTempAnimStr.Append(aniName, nStart, nLength);
        return mTempAnimStr.ToString();
    }

    override protected void refreshAnimation()
    {
        if (IsMoveing())
        {
            refreshMoveAnimation();
        }
        else
        {
            //CrossPlayAnimation("Nan_idle");
        }
    }

    private void refreshMoveAnimation()
    {
        return;

    }

    //怪物有多方向移动吗?
    virtual protected RoleAnimationDir GetAnimationDirection()
    {
        return RoleAnimationDir.AnimationDir_Front;
    }
    //移动到一个固定点
    override public void MovePos(Vector3 pos)
    {
        base.MovePos(pos);
    }
    //停止移动
    override public void StopMove()
    {
        base.StopMove();
    }

    override public void StopDirMove()
    {
        base.StopDirMove();
    }

    //是否可以移动
    override public bool IsCanMove()
    {
        return base.IsCanMove();
    }

    override public bool IsCanMoveRotation()
    {
        return true;
    }
    override public bool Update(uint elapsed)
    {
        if (!base.Update(elapsed))
        {
            return false;
        }

        if (mPaoPao != null)
        {
            Vector3 headPos = this.GetBonePositionByName("head");
            if (headPos != Vector3.zero)
            {
                headPos = CameraController.Instance.WorldToScreenPoint(headPos);
                headPos.z = 0.0f;
                mPaoPao.Update(headPos, elapsed);
            }
        }

        if (mRoleState != RoleState.RoleState_Idle)
        {
            if (!IsMoveing())
            {
                ChangeState(RoleState.RoleState_Idle);
            }
        }
        else if (mRoleState != RoleState.RoleState_Moving)
        {
            if (IsMoveing())
            {
                ChangeState(RoleState.RoleState_Moving);
            }
        }

        refreshMoveAnimation();

        UpdateAttachMent();
        UpdateWingAnim(elapsed);
        return true;
    }


    public override uint GetMoveState()
    {
        return mWeaopnRes == null ? 0 : mWeaopnRes.move_state;
    }

    //死了
    protected override void onDie(AttackerAttr killerAttr, ImpactDamageType impactDamageType)
    {
        if (mPaoPao != null)
        {
            mPaoPao.Hide();
        }
        HideShadow();
        base.onDie(killerAttr, impactDamageType);
    }

    override public void Destroy()
    {
        if (mPaoPao != null)
        {
            PaoPaoManager.Instance.ReleasePaoPaoUI(mPaoPao);
        }

        mAttachParticles.Clear();
        base.Destroy();
    }

    virtual protected void ShowTalk(string msg, int time)
    {
        if (mPaoPao == null)
        {
            mPaoPao = PaoPaoManager.Instance.CreatePaoPaoUI();
        }

        if (mPaoPao != null)
        {
            mPaoPao.Talk(msg, time);
        }
    }

    public void Talk(string msg)
    {
        ShowTalk(msg, 5000);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="fashionid"></param>
    /// <param name="action"> 1更换时装 2 换成默认</param>
    public void ChangeFashion(int fashionid, int action)
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





    //--------------------------------------------------
    public void UpdateAttachMent()
    {

        if (mVisual != null && mVisual.Visual != null && InitModelID == mModelResID)
        {
            for (uint i = 0; i < (uint)AttachMountType.AttachCount; ++i)
            {
                AttachMent attach = mAttachMents[i];
                if (attach == null || attach.parent != null)
                    continue;
                //挂接
                if (mVisual != null && attach.visual != null && attach.visual.Visual != null)
                {

                    Transform t = mVisual.GetBoneByName(attach.socketname);
                    if (t == null)
                        t = mVisual.VisualTransform;
                    attach.parent = t.gameObject;
                    attach.visual.Visual.SetActive(true);
                    DressingRoom.AttachObjectTo(t, attach.visual.VisualTransform, attach.transform);
                }
            }
        }
    }

}