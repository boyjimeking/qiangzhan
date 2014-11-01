using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PromoteGridUI
{
    public UISprite mIcon;
    public UISprite mMainOrSub;
    private int mId;

    private GameObject mObj = null;
    public PromoteGridUI(GameObject obj)
    {
        mObj = obj;
        mIcon = ObjectCommon.GetChildComponent<UISprite>(mObj, "weaponSp");
        mMainOrSub = ObjectCommon.GetChildComponent<UISprite>(mObj, "selectSpMainOrSub");
        ChangeEquipedWeapon();
    }
    public void ChangeUI(bool change)
    {
        Vector3 vec;
        if (change)
        {
            vec = new Vector3(1.1f, 1.1f, 0f);
        }
        else
        {
            vec = new Vector3(0.6f, 0.6f, 0f);
        }

        TweenScale.Begin(mIcon.gameObject, 0.3f, vec);
        TweenScale.Begin(mMainOrSub.gameObject, 0.3f, vec);
    }

    public void ChangeEquipedWeapon()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        mMainOrSub.gameObject.SetActive(true);
        if (mObj.name.Equals(module.GetMainWeaponId().ToString()))
        {
            SetMainOrSub("Equip_Atlas3:subweapon");
        }
        else if (mObj.name.Equals(module.GetSubWeaponId().ToString()))
        {
            SetMainOrSub("Equip_Atlas3:mainweapon");
        }
        else
        {
            mMainOrSub.gameObject.SetActive(false);
        }
    }

    public int getWeaponId()
    {
        return mId;
    }

    public void SetWeaponId(int resId)
    {
        mId = resId;
    }

    //设置图片
    public void SetIcon(string name)
    {
        UIAtlasHelper.SetSpriteImage(mIcon, name , true);

    }

    public void SetMainOrSub(string name)
    {
        UIAtlasHelper.SetSpriteImage(mMainOrSub, name, true);
    }

}
