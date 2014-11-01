
using FantasyEngine;
using UnityEngine;
public class UIWeaponPreview : UIPreviewBase
{
    private int mResid;

    private MeshVisual mWeapon;

    private bool hasSetPos = false;

    GameObject mWeaponAttatch;

    public UIWeaponPreview():base()
    {
        mOrthograhicsize = 0.33f;
    }

    public override void Update()
    {
        base.Update();

        if (mWeapon != null && mWeapon.Visual != null)
        {
            mWeaponAttatch.transform.localEulerAngles = new Vector3(-90, RotationY, 0);

            Renderer renderer = mWeapon.Visual.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                mWeapon.VisualTransform.localPosition = new Vector3(0,-renderer.bounds.extents.y,0);
                //hasSetPos = true;
            }
        }

        RotationY += Time.deltaTime * 60;

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="resid"></param>
    public void SetupWeapon(int resid)
    {
         if (!DataManager.WeaponTable.ContainsKey(resid))
            return;

        WeaponTableItem weaponTab = DataManager.WeaponTable[resid] as WeaponTableItem;

        if (weaponTab == null)
        {
            GameDebug.LogError(" WeaponTable没有找到武器 id = " + resid.ToString());
            return;
        }
        if (resid != mResid)
        {
            if (mWeapon != null)
            {
                mWeapon.Destroy();
            }
            mResid = resid;
            mWeapon = new MeshVisual();
            mWeapon.CreateWithConfig(AssetConfig.WeaponPath + weaponTab.modelname, OnVisualSuccess, OnVisualFailed, false);
        }
    }

    private void OnVisualSuccess()
    {
        if (mWeaponAttatch == null)
        {
            
            mWeaponAttatch = new GameObject();
            mWeaponAttatch.transform.parent = mPreviewRoot.transform;
            mWeaponAttatch.transform.localPosition = Vector3.zero;
            mWeaponAttatch.transform.localEulerAngles = new Vector3(-90, 0, 0);
        }
        //设置显示层次
        ObjectCommon.SetObjectAndChildrenLayer(mWeapon.Visual, layermask);
        mWeapon.VisualTransform.parent = mWeaponAttatch.transform;
        mWeapon.VisualTransform.localPosition = Vector3.zero;
        mWeapon.VisualTransform.localEulerAngles = Vector3.zero;

        Renderer rd = mWeapon.Visual.GetComponent<Renderer>();
        if (rd != null)
        {
            Material mtl = new Material(Shader.Find("Fantasy/ui/weapon"));
            mtl.mainTexture = rd.material.mainTexture;
            if (rd.material.shader.name == "FantasyEngine/weapon/environment")
                mtl.SetTexture("_HightLightTex", rd.material.GetTexture("_HightLightTex"));
            rd.material = mtl;
        }

       ShowWeaponItem showitem = DataManager.ShowWeaponTable[mResid] as ShowWeaponItem;
        if(showitem != null)
        {
            mWeapon.VisualTransform.localScale = Vector3.one * showitem.weaponscale;
        }

        PlayWeaponAnimation(AnimationNameDef.WeaponDefault);
    }
    private void OnVisualFailed()
    {

    }

    public void PlayWeaponAnimation(string statename)
    {
        if (mWeapon == null || mWeapon.AnimManager == null || mWeapon.AnimManager.Property == null)
            return;

        int stateid = mWeapon.AnimManager.Property.GetStateHash(statename);

        if (stateid == 0)
        {
            stateid = mWeapon.AnimManager.Property.GetStateHash(AnimationNameDef.WeaponEmpty);
        }

        if (stateid != 0)
        {
            mWeapon.AnimManager.Anim.SetInteger("state", stateid);
        }
    }
    public override void Destroy()
    {
        if (mWeapon != null)
            mWeapon.Destroy();
        mWeapon = null;
        base.Destroy();
    }

}

