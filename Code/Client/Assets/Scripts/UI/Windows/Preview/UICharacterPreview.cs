using FantasyEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 角色预览窗口
/// </summary>
public class UICharacterPreview : UIPreviewBase
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

    private Vector3 mPos = new Vector3(0,0,0);
    public UICharacterPreview()
        :base()
    {
     
    }

    public void SetCameraOrthographicSize(float ortsize)
    {
        mPreviewCamera.orthographicSize = ortsize;
    }

    public void SetTargetSprite(Rect pixelRect)
    {
        if (pixelRect != null)
            mPreviewCamera.pixelRect = pixelRect;
    }
    private void SetupCharater(GameObject character)
    {    
        mCharacter = character;
        mCharacter.layer = layermask;
        mCharacter.transform.parent = mPreviewRoot.transform;
        mCharacter.transform.localPosition = new Vector3(0, 0, 0);


       if(character.GetComponent<Renderer>() != null)
       {
           mCharacter.transform.localPosition = new Vector3(0, -character.GetComponent<Renderer>().bounds.extents.y, 0);
       }
    }

    public void SetupCharacter(int resid,int[] equiplist,int wingid, uint winglevel)
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
            ChangeWing(wingid,winglevel);

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
            DefenceTableItem table = DataManager.DefenceTable[id] as DefenceTableItem;
            if (table == null)
                continue;

            PartModelTableItem item = DataManager.PartModelTable[table.modelShowId] as PartModelTableItem;
            if (item == null)
                continue;

            if (!string.IsNullOrEmpty(item.solt) && !string.IsNullOrEmpty(item.file))
                mVisual.ChangeElment(item.solt, AssetConfig.ModelPath + "Role/" + item.file + AssetConfig.AssetSuffix, null);
        }
    }

    public void ChangeWeapon(int weaponid)
    {
        if (weaponid == mWeaponID)
            return;
        BehaviourUtil.StopCoroutine(ChangeWeapon_impl(mWeaponID));

        if (!DataManager.WeaponTable.ContainsKey(weaponid))
            return;


        if (mWeapon != null)
            mWeapon.Destroy();

        foreach(uint effectid in mShowEffect)
        {
            SceneManager.Instance.GetCurScene().GetParticleManager().RemoveParticle(effectid);
        }
        mShowEffect.Clear();
        mWeaponID = weaponid;
       //BehaviourUtil.StartCoroutine(ChangeWeapon_impl(weaponid));
        WeaponTableItem mWeaopnRes = DataManager.WeaponTable[weaponid] as WeaponTableItem;

        mWeapon = new MeshVisual();
        mWeapon.CreateWithConfig(AssetConfig.WeaponPath + mWeaopnRes.modelname, null, OnWeaponFailed, false);

        TransformData trans = new TransformData();
        trans.Rot = new Vector3(90, 0, 0);
        trans.Scale = Vector3.one * mWeaopnRes.scale;

        ChangeAttach(AttachMountType.Weapon, mWeapon, mWeaopnRes.mountpoint,trans);
    }

    private IEnumerator ChangeWeapon_impl(int weaponid)
    {
        WeaponTableItem mWeaopnRes = DataManager.WeaponTable[weaponid] as WeaponTableItem;

        if (mWeapon != null)
            mWeapon.Destroy();

        if (mWeaopnRes == null)
            yield break;

        while(mVisual == null || !mVisual.IsCompleteOrDestroy)
        {
            yield return 1;
        }

        mChangingWeaponID = weaponid;
        mWeapon = new MeshVisual();
        mWeapon.CreateWithConfig(AssetConfig.WeaponPath + mWeaopnRes.modelname, OnWeaponSuccess, OnWeaponFailed, false);
    }

    public void PlayAnimaton(string statename)
    {
        if (mStatename == statename)
            return;


        mIdleTime = 0;
        if(mVisual != null && mVisual.AnimManager != null)
        {
            int stateid = mVisual.AnimManager.Property.GetStateHash(statename);
            if(stateid == 0)
                return;
            mStatename = statename;
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
        if (mVisual == null || mVisual.Visual == null || mPreviewRoot == null)
        {
            return;
        }
        if (mWeapon == null || !mWeapon.IsCompleteOrDestroy)
            return;

        mVisual.VisualTransform.parent = mPreviewRoot.transform;


        WeaponTableItem mWeaopnRes = DataManager.WeaponTable[mWeaponID] as WeaponTableItem;

        Transform bone = mVisual.GetBoneByName(mWeaopnRes.mountpoint);
        if (bone == null)
            bone = mVisual.VisualTransform;
        mWeapon.Visual.layer = layermask;

        Renderer[] renders = mWeapon.Visual.GetComponentsInChildren<Renderer>();
        foreach (Renderer rd in renders)
        {
            rd.gameObject.layer = layermask;
            Material mtl = new Material(Shader.Find("Fantasy/ui/weapon"));
            mtl.mainTexture = rd.material.mainTexture;
            if (rd.material.shader.name == "FantasyEngine/weapon/environment")
                mtl.SetTexture("_HightLightTex", rd.material.GetTexture("_HightLightTex"));
            rd.material = mtl;
        }



       string animname =  AnimationNameDef.GetAnimNameByStatename(mWeaopnRes.ani_pre, AnimationNameDef.PrefixXiuxi);

       PlayAnimaton(animname);

        //武器切换成功展现武器

      if(DataManager.ShowWeaponTable.ContainsKey(mWeaopnRes.id))
      {
          ShowWeaponItem showres = DataManager.ShowWeaponTable[mWeaopnRes.id] as ShowWeaponItem;
          ProcessWeaponShow(showres);
      }

    }

    private void ProcessWeaponShow(ShowWeaponItem item)
    {

        //播放人物动作

        if(!string.IsNullOrEmpty(item.roleanim))
        {
            WeaponTableItem mWeaopnRes = DataManager.WeaponTable[mWeaponID] as WeaponTableItem;
            string animname = AnimationNameDef.GetAnimNameByStatename(mWeaopnRes.ani_pre, item.roleanim);
            PlayAnimaton(animname);
        }

        //播放武器动作
        if (!string.IsNullOrEmpty(item.weaponanim))
        {
            WeaponTableItem mWeaopnRes = DataManager.WeaponTable[mWeaponID] as WeaponTableItem;
            string animname = AnimationNameDef.WeaponPrefix + item.weaponanim;
            PlayWeaponAnimation(animname);

        }

        mShowEffect.Add(AttachEffect(item.effectid0, item.mountpoint0));
        mShowEffect.Add(AttachEffect(item.effectid1, item.mountpoint1));
        mShowEffect.Add(AttachEffect(item.effectid2, item.mountpoint2));
        foreach(uint id in mShowEffect)
        {
           ParticleItem pitem =  SceneManager.Instance.GetCurScene().GetParticleManager().GetParticle(id);
           if (pitem == null)
               continue;
           pitem.Layer = layermask;

        }
    }
    private void OnWeaponFailed()
    {

    }

    public void PlayWeaponAnimation(string statename)
    {
        if (mWeapon == null || mWeapon.AnimManager == null || mWeapon.AnimManager.Property == null)
            return;

       int stateid =  mWeapon.AnimManager.Property.GetStateHash(statename);

        if(stateid == 0)
        {
            stateid = mWeapon.AnimManager.Property.GetStateHash(AnimationNameDef.WeaponEmpty);
        }

        if(stateid != 0)
        {
            mWeapon.AnimManager.Anim.SetInteger("state", stateid);
        }
    }


    public Vector3 Pos
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

    public uint AttachEffect(int id,string mountname)
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
        trans.Scale = Vector3.one* effectitem.scale;
        trans.notFollow = effectitem.notFollow;
        return SceneManager.Instance.GetCurScene().GetParticleManager().AddParticle(effectitem.effect_name,effectitem.loop,bone,trans,effectitem.limitry);
    }


    public void Flush(GameObject character)
    {
        SetupCharater(character);
    }
    public override void Update()
    {
        UpdateAttachMent();
        WeaponTableItem mWeaopnRes = DataManager.WeaponTable[mWeaponID] as WeaponTableItem;

        if (mWeaopnRes != null)
        {
            mIdleTime += Time.fixedDeltaTime;
            if (mIdleTime >= GameConfig.CPPlayIdlePerTime)
            {
                if (DataManager.ShowWeaponTable.ContainsKey(mWeaopnRes.id))
                {
                    ShowWeaponItem showres = DataManager.ShowWeaponTable[mWeaopnRes.id] as ShowWeaponItem;
                    string animname = AnimationNameDef.GetAnimNameByStatename(mWeaopnRes.ani_pre, showres.roleanim);
                    PlayAnimaton(animname);

                    PlayWeaponAnimation("Base Layer." + showres.weaponanim);
                }
                mIdleTime = 0;
            }
        }



        if (mCharacter != null)
        {
            if (mCharacter.GetComponent<Renderer>() != null)
            {
                mCharacter.transform.localPosition = new Vector3(mPos.x, mPos.y- mCharacter.GetComponent<Renderer>().bounds.extents.y, mPos.z);
            }

            mCharacter.transform.localEulerAngles = new Vector3(0, mRotateY, 0);

            if(mVisual.AnimManager != null)
            {
               AnimatorStateInfo info = mVisual.AnimManager.Anim.GetCurrentAnimatorStateInfo((int)AnimationLayer.BaseLayer);
               if (Mathf.FloorToInt(info.normalizedTime) >= 1 && info.IsName(mStatename) && !info.IsName(DefaultCharacterAnim))
                {
                    PlayAnimaton(DefaultCharacterAnim);

                    if (mWeapon != null && mWeapon.AnimManager != null && mVisual.AnimManager.Anim != null)
                    {

                        PlayWeaponAnimation(AnimationNameDef.WeaponDefault);

                    }
                }

               if (mPreviewRoot.activeSelf == false)
                   mInvalidAnim = true;
               if (mPreviewRoot.activeSelf == true && mInvalidAnim)
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
            if(DataManager.WeaponTable.ContainsKey(mWeaponID))
            {
                WeaponTableItem item = DataManager.WeaponTable[mWeaponID] as WeaponTableItem;

                ShowWeaponItem witem = DataManager.ShowWeaponTable[mWeaponID] as ShowWeaponItem;
                string name = AnimationNameDef.PrefixXiuxi;
                if (witem != null)
                {
                    name = witem.roledefaultanim;
                }

              return AnimationNameDef.GetAnimNameByStatename(item.ani_pre, name);
            }
            return "";
        }
    }

    public void OnWingSuccess()
    {
        
        AttachMent attachment = mAttachMents[(int)AttachMountType.Wing];
        if (attachment == null)
            return;

        WingCommonTableItem commonRes = DataManager.WingCommonTable[mWingID] as WingCommonTableItem;
        if (commonRes == null)
            return;
        int effectid = WingModule.GetEffectId(mWingID, (int)mWingLv);
        if (effectid == -1) return;

        uint instid = ParticleUtility.AddEffect2MV(attachment.visual as MeshVisual, effectid, commonRes.modelSlot, SceneManager.Instance.GetCurScene().GetParticleManager());
        ParticleItem pitem = SceneManager.Instance.GetCurScene().GetParticleManager().GetParticle(instid);
        if (pitem != null)
            pitem.Layer = layermask;

    }

    public void ChangeWing(int wingid,uint winglv)
    {
        if (mWingID == wingid && mWingLv== winglv)
            return;
        mWingID = wingid;
        mWingLv = winglv;

        string modelName = WingModule.GetModelName(wingid, winglv);
        WingCommonTableItem commonRes = DataManager.WingCommonTable[wingid] as WingCommonTableItem;
        if (commonRes == null)
            return;
        MeshVisual visual = new MeshVisual();
        visual.CreateWithConfig(AssetConfig.WeaponPath + modelName,null,null,false);
        visual.Layer = layermask;
        ChangeAttach(AttachMountType.Wing, visual, commonRes.modelSlot);
    }

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
        if (mVisual != null&& mVisual.Visual != null)
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
                    attach.visual.Layer = layermask;
                    DressingRoom.AttachObjectTo(t, attach.visual.VisualTransform, attach.transform);

                    if(i == (uint)AttachMountType.Weapon)
                    {
                        OnWeaponSuccess();
                    }
                    else if (i == (uint)AttachMountType.Wing)
                    {
                        
                        OnWingSuccess();
                    }
                }
            }
        }
    }
}

