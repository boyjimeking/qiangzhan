
/// <summary>
/// 一个简单的模型实体
/// </summary>
using FantasyEngine;
using System.Collections.Generic;
using UnityEngine;
public class SimpleModel
{
    private MeshVisual mVisual;
    private MeshVisual mWeapon;
    private GameObject mCharacter;

    private string mStatename;

    private int mResid;
    private int[] mEquipmentList = new int[(int)EquipSlot.EquipSlot_MAX];
    private int mWeaponID;
    private int mChangingWeaponID;

    private float mIdleTime = 0;

    private bool mInvalidAnim = false;
    private int mWingID;
    private uint mWingLv;

    private AttachMent[] mAttachMents = new AttachMent[(uint)AttachMountType.AttachCount];
    private List<uint> mShowEffect = new List<uint>();

    protected float mRotateY = 180;
    private Vector3 mPos = Vector3.zero;
    private float mScale = 1;

    public SceneParticleManager ParticleMng;
    public SimpleModel()
    {

    }
    private void SetCharater(GameObject character)
    {
        mCharacter = character;
    }

    public void CreateWithConfig(int resid, int[] equiplist, int wingid)
    {
        if (!DataManager.ModelTable.ContainsKey(resid))
            return;

        ModelTableItem modelTab = DataManager.ModelTable[resid] as ModelTableItem;

        if (modelTab == null)
        {
            GameDebug.LogError(" DataManager没有找到模型 id = " + resid.ToString());
            return;
        }

        if (resid != mResid)
        {
            if (mVisual != null)
            {
                mVisual.Destroy();
            }
            mResid = resid;
            mVisual = new MeshVisual();
            mVisual.CreateWithConfig(modelTab.filename, OnVisualSuccess, OnVisualFailed, true);
        }
        else
        {
            mInvalidAnim = true;
        }
        //切换装备
        //TODO:装备相关的表和数据都还没有做好，做好了在这里处理

        ApplyEquipConfig(equiplist);
        if (wingid < 0)
            RemoveAttach(AttachMountType.Wing);
        else
            ChangeWing(wingid, ModuleManager.Instance.FindModule<WingModule>().GetWingLevel(wingid));

    }

    public void ApplyEquipConfig(int[] equipconfigs)
    {
        if (equipconfigs == null)
            return;
        Dictionary<string, string> parts = new Dictionary<string, string>();

        for (int i = 0; i < (int)EquipSlot.EquipSlot_MAX; ++i)
        {
            if (mEquipmentList[i] == equipconfigs[i])
                continue;

            mEquipmentList[i] = equipconfigs[i];
            int id = (int)mEquipmentList[i];
            if (id < 0)
            {

                continue;
            }
            if (!DataManager.PartModelTable.ContainsKey(id))
                continue;
            PartModelTableItem item = DataManager.PartModelTable[id] as PartModelTableItem;
            mVisual.ChangeElment(item.solt, AssetConfig.ModelPath + "Role/" + item.file + AssetConfig.AssetSuffix, null);
        }
    }

    public void ChangeWeapon(int weaponid)
    {
        if (weaponid == mWeaponID)
            return;

        if (!DataManager.WeaponTable.ContainsKey(weaponid))
            return;


        if (mWeapon != null)
            mWeapon.Destroy();

        foreach (uint effectid in mShowEffect)
        {
            SceneManager.Instance.GetCurScene().GetParticleManager().RemoveParticle(effectid);
        }
        mShowEffect.Clear();
        mWeaponID = weaponid;
        WeaponTableItem mWeaopnRes = DataManager.WeaponTable[weaponid] as WeaponTableItem;

        mWeapon = new MeshVisual();
        mWeapon.CreateWithConfig(AssetConfig.WeaponPath + mWeaopnRes.modelname, null, OnWeaponFailed, false);

        TransformData trans = new TransformData();
        trans.Rot = new Vector3(90, 0, 0);
        trans.Scale = Vector3.one * mWeaopnRes.scale;

        ChangeAttach(AttachMountType.Weapon, mWeapon, mWeaopnRes.mountpoint, trans);
    }
    public void PlayAnimaton(string statename)
    {
        if (mStatename == statename)
            return;

        mStatename = statename;

        mIdleTime = 0;
        if (mVisual != null && mVisual.AnimManager != null)
        {
            int stateid = mVisual.AnimManager.Property.GetStateHash(statename);
            if (stateid == 0)
                return;
            mVisual.AnimManager.Anim.SetInteger("state", stateid);
        }


    }


    private void OnVisualSuccess()
    {
        //加载成功了

        //将加载成功的物体挂到相应位置
        Flush(mVisual.Visual);

        if (mVisual != null && mVisual.AnimManager != null)
        {
            int stateid = mVisual.AnimManager.Property.GetStateHash(mStatename);
            if (stateid == 0)
                return;
            mVisual.AnimManager.Anim.SetInteger("state", stateid);
        }
    }
    private void OnVisualFailed()
    {

    }

    private void OnWeaponSuccess()
    {
        if (mVisual == null || mVisual.Visual == null)
        {
            return;
        }
        if (mWeapon == null || !mWeapon.IsCompleteOrDestroy)
            return;

        WeaponTableItem mWeaopnRes = DataManager.WeaponTable[mWeaponID] as WeaponTableItem;

        Transform bone = mVisual.GetBoneByName(mWeaopnRes.mountpoint);
        if (bone == null)
            bone = mVisual.VisualTransform;

        string animname = AnimationNameDef.GetAnimNameByStatename(mWeaopnRes.ani_pre, AnimationNameDef.PrefixXiuxi);

        PlayAnimaton(animname);


        AttachMent attachment = mAttachMents[(int)AttachMountType.Weapon];
        if (attachment == null)
            return;

        if (mWeaopnRes.weapon_buff != uint.MaxValue)
        {
            SkillBuffTableItem sbt = DataManager.BuffTable[mWeaopnRes.weapon_buff] as SkillBuffTableItem;
            if (sbt != null)
            {
                ParticleUtility.AddEffect2MV(mWeapon, (int)sbt._3DEffectID, sbt._3DEffectBindpoint, ParticleMng);
            }
        }

    }
    private void OnWeaponFailed()
    {

    }

    public void PlayWeaponAnimation(string statename)
    {
        if (mWeapon == null || mWeapon.AnimManager == null || mWeapon.AnimManager.Property == null)
            return;

        int stateid = mWeapon.AnimManager.Property.GetStateHash(statename);
        if (stateid != 0)
        {
            mWeapon.AnimManager.Anim.SetInteger("state", stateid);
        }
    }


    public uint AttachEffect(int id, string mountname)
    {
        if (!DataManager.EffectTable.ContainsKey(id))
            return uint.MaxValue;
        EffectTableItem effectitem = DataManager.EffectTable[id] as EffectTableItem;
        bool nullmount = string.IsNullOrEmpty(mountname);
        MeshVisual attachparent = mVisual;
        if (!nullmount)
        {
            if (mountname[0] == '%')
                attachparent = mWeapon;
            mountname = mountname.Substring(1);
        }

        Transform bone = attachparent.GetBoneByName(mountname);
        if (bone == null)
            bone = attachparent.VisualTransform;

        TransformData trans = new TransformData();
        trans.Scale = Vector3.one * effectitem.scale;
        trans.notFollow = effectitem.notFollow;
        return SceneManager.Instance.GetCurScene().GetParticleManager().AddParticle(effectitem.effect_name,effectitem.loop,bone, trans, effectitem.limitry);
    }

    public Vector3 Position
    {
        get
        {
            return mPos;
        }
        set
        {
            mPos = value;
        }
    }
    public float Scale
    {
        get
        {
            return mScale;
        }
        set
        {
            mScale = value;
        }
    }

    public void Flush(GameObject character)
    {
        SetCharater(character);
    }
    public void Update()
    {
        UpdateAttachMent();
        WeaponTableItem mWeaopnRes = DataManager.WeaponTable[mWeaponID] as WeaponTableItem;

        if (mCharacter != null)
        {
            mCharacter.transform.localEulerAngles = new Vector3(0, mRotateY, 0);

            mCharacter.transform.localPosition = mPos;
            mCharacter.transform.localScale = Vector3.one * mScale;
            if (mVisual.AnimManager != null)
            {
                AnimatorStateInfo info = mVisual.AnimManager.Anim.GetCurrentAnimatorStateInfo((int)AnimationLayer.BaseLayer);
                if (Mathf.FloorToInt(info.normalizedTime) >= 1 && info.IsName(mStatename) && !info.IsName(DefaultCharacterAnim))
                {
                    PlayAnimaton(DefaultCharacterAnim);

                }

                if (mVisual.Visual.activeSelf == true && mInvalidAnim)
                {
                    mStatename = string.Empty;
                    PlayAnimaton(DefaultCharacterAnim);
                    mInvalidAnim = false;
                }
            }
        }

    }


    public string DefaultCharacterAnim
    {
        get
        {
            if (DataManager.WeaponTable.ContainsKey(mWeaponID))
            {
                WeaponTableItem item = DataManager.WeaponTable[mWeaponID] as WeaponTableItem;

                return AnimationNameDef.GetAnimNameByStatename(item.ani_pre, AnimationNameDef.PrefixXiuxi);
            }
            return "";
        }
    }

    public void ChangeWing(int wingid, uint winglv)
    {
        if (mWingID == wingid && mWingLv == winglv)
            return;
        mWingID = wingid;
        mWingLv = winglv;

        WingCommonTableItem commonRes = DataManager.WingCommonTable[wingid] as WingCommonTableItem;
        if (commonRes == null)
            return;
        string modelName = WingModule.GetModelName(wingid, winglv);
        MeshVisual visual = new MeshVisual();
        visual.CreateWithConfig(AssetConfig.WeaponPath + modelName, null, null, false);
        ChangeAttach(AttachMountType.Wing, visual, commonRes.modelSlot);
    }

    public void OnWingSuccess()
    {

        AttachMent attachment = mAttachMents[(int)AttachMountType.Wing];
        if (attachment == null)
            return;

        WingCommonTableItem commonRes = DataManager.WingCommonTable[mWingID] as WingCommonTableItem;
        if (commonRes == null)
            return;
        if (!DataManager.EffectTable.ContainsKey(commonRes.effectNomal))
            return;

        EffectTableItem item = DataManager.EffectTable[commonRes.effectNomal] as EffectTableItem;
        ParticleUtility.AddEffect2MV(attachment.visual as MeshVisual, commonRes.effectNomal, commonRes.modelSlot,ParticleMng);

       
    }
    public MeshVisual GetVisual()
    {
        return mVisual;
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

    public void UpdateAttachMent()
    {

        for (uint i = 0; i < (uint)AttachMountType.AttachCount; ++i)
        {
            AttachMent attach = mAttachMents[i];

            if (mVisual == null || mVisual.Visual == null)
            {
                if (attach != null && attach.visual != null && attach.visual.Visual != null)
                    attach.visual.Visual.SetActive(false);

            }
            else
            {
                if (attach == null || attach.parent != null)
                    continue;

                //挂接
                if (mVisual != null && attach.visual != null && attach.visual.Visual != null)
                {

                    Transform t = mVisual.GetBoneByName(attach.socketname);
                    if (t == null)
                        t = mVisual.VisualTransform;
                    attach.parent = t.gameObject;
                    DressingRoom.AttachObjectTo(t, attach.visual.VisualTransform, attach.transform);
                    attach.visual.Visual.gameObject.SetActive(true);
                    if (i == (uint)AttachMountType.Weapon)
                    {
                        OnWeaponSuccess();
                    }
                    else if(i == (uint)AttachMountType.Wing)
                    {
                        OnWingSuccess();
                    }
                }
            }
        }

    }

    public void Destroy()
    {
        if (mVisual != null)
        {
            mVisual.Destroy();
            mVisual = null;
        }
        if (mWeapon != null)
        {
            mWeapon.Destroy();
            mWeapon = null;
        }

        mCharacter = null;

    }


    //-------------------------------------------
    public void Scroll(float delta)
    {
        RotationY += delta < 0 ? 10 : -10;
    }
    public float RotationY
    {
        get
        {
            return mRotateY;
        }
        set
        {
            mRotateY = value;
        }
    }

}

