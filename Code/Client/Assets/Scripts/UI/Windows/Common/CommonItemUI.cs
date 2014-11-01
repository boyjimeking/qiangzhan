using System;
using UnityEngine;

public class CommonItemUI
{
    private GameObject mObj = null;

    private UIButton mBnt = null;
    private UISprite mIcon = null;
    private UISprite mSprite1 = null;
    private UILabel mLabel1 = null;

    private UILabel mSterLabel = null;

    private UILabel mLabel = null;
    private UISprite mGemIcon = null;
    private UISprite mStarIcon = null;

    private int mItemID = -1;
    private PackageType mPackType = PackageType.Invalid;
    private int mPackPos = -1;

    private ItemTableItem mItemRes = null;

    private UISpriteAnimation mStarAni = null;
    private UISpriteAnimation mGemAni = null;

    private bool other_item = false;

    public CommonItemUI(int resId)
    {
        InitUI();
        SetResID(resId);
    }
    public CommonItemUI(ItemObj itemobj)
    {
        InitUI();
        SetItemObj(itemobj);

    }
    public CommonItemUI(GameObject obj)
    {
        InitUI(obj);
    }
    public CommonItemUI(PackageType packtype, int packpos)
    {
        InitUI();
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        ItemObj itemobj = module.GetItem(packtype, packpos);
        SetItemObj(itemobj);
    }

    public int GetItemID()
    {
        return mItemID;
    }

    private void InitUI(GameObject obj = null)
    {
        if( obj == null )
        {
            mObj = WindowManager.Instance.CloneCommonUI("ItemUI");
        }
        else
        {
            mObj = obj;
        }
        mBnt = mObj.GetComponent<UIButton>();
        mIcon = ObjectCommon.GetChildComponent<UISprite>(mObj, "icon");
        mLabel = ObjectCommon.GetChildComponent<UILabel>(mObj, "Label");
        mGemIcon = ObjectCommon.GetChildComponent<UISprite>(mObj, "gem");
        mStarIcon = ObjectCommon.GetChildComponent<UISprite>(mObj, "star");

        mSprite1 = ObjectCommon.GetChildComponent<UISprite>(mObj, "sprite1");
        mLabel1 = ObjectCommon.GetChildComponent<UILabel>(mObj, "label1");

        mSterLabel = ObjectCommon.GetChildComponent<UILabel>(mObj, "strenLv");

        EventDelegate.Add(mBnt.onClick, OnBntClick);
        Clear();
    }
    //设置碰撞盒大小
    public void SetBoxSize(float w , float h)
    {
        if (mBnt == null)
            return;
        BoxCollider boxCollider = mBnt.collider as BoxCollider;
        if( boxCollider != null )
        {
            UnityEngine.Vector3 size = boxCollider.size;

            size.y = h;
            size.x = w;
            boxCollider.size = size;

        }
    }
    public void SetItemObj(ItemObj itemobj)
    {
        if (itemobj == null)
        {
            Clear();
            return;
        }
        mPackType = itemobj.PackType;
        mPackPos = itemobj.PackPos;
        mItemID = itemobj.GetResId();
        SetResID(mItemID);
        SetNumber(itemobj.GetCount());

        if( itemobj is DefenceObj )
        {
            DefenceObj defenceObj = itemobj as DefenceObj;
            if( defenceObj != null )
            {
                SetStrenLv(defenceObj.GetStrenLv());
                SetStonePic(defenceObj.GetStonePic());
                SetStarsLvPic(defenceObj.GetStarsLvPic());
            }else
            {
                SetStrenLv(-1);
                SetStonePic(null);
                SetStarsLvPic(null);
            }
        }
    }

    public void SetOther(bool other)
    {
        other_item = other;
    }

    private void SetStarsLvPic(string pic)
    {
        if (mStarIcon == null)
            return;
        UIAtlasHelper.SetSpriteImage(mStarIcon, pic);
        NGUITools.SetActive(mStarIcon.gameObject, true);

        if( mStarAni == null && pic != null)
        {
            mStarAni = AnimationManager.Instance.AddSpriteAnimation("xingguanshandong", mStarIcon.gameObject, mStarIcon.depth + 1, 30);
            //mStarAni.transform.localPosition = new Vector3(mStarAni.transform.localPosition.x, ani.transform.localPosition.y + 50, ani.transform.localPosition.z);
        }
    }
    private void SetStonePic(string pic)
    {
        if (mGemIcon== null)
            return;
        UIAtlasHelper.SetSpriteImage(mGemIcon, pic);
        NGUITools.SetActive(mGemIcon.gameObject, true);
        if (mGemAni == null && pic != null)
        {
            mGemAni = AnimationManager.Instance.AddSpriteAnimation("xingguanshandong", mGemIcon.gameObject, mGemIcon.depth + 1, 30);
        }
    }
    private void SetStrenLv(int lv)
    {
        if (mSterLabel == null)
            return;
        if( lv <= 0 )
        {
            mSterLabel.text = "";
        }else
        {
            mSterLabel.text = "+" + lv.ToString();
        }
        NGUITools.SetActive(mSterLabel.gameObject, true);
    }

    private void SetNumber(uint number)
    {
        if( mLabel != null )
        {
            if (number > 1)
            {
                NGUITools.SetActive(mLabel.gameObject, true);
                mLabel.text = number.ToString();
            }
            else
            {
                NGUITools.SetActive(mLabel.gameObject, false);
            }
        }
    }
    public void SetResID(int resId)
    {
        mItemID = resId;
        mItemRes = ItemManager.GetItemRes(mItemID);
        if( mItemRes != null )
        {
            SetIcon(mItemRes.picname);
            SetParam1(mItemRes.picname2);
            SetParam2(mItemRes.picname3);
        }
        else
        {
            SetIcon(null);
            SetParam1(null);
            SetParam2("");
        }
    }

    private void OnBntClick()
    {
        if (!IsValid())
            return;

        //if (ItemManager.GetItemType((uint)mItemID) != ItemType.Defence)
            //return;
        DefenceUIParam param = new DefenceUIParam();
        param.itemid = mItemID;
        param.packtype = mPackType;
        param.packpos = mPackPos;
        param.isother = other_item;

        ItemInfoParam mParam = new ItemInfoParam();
        mParam.itemid = mItemID;
        mParam.packpos = mPackPos;
        mParam.itemtype = ItemManager.GetItemType((uint)mItemID);//(ItemType)mPackType;
        mParam.packtype = mPackType;
        switch (mParam.itemtype)
        {
            case ItemType.Stone:
                WindowManager.Instance.OpenUI("iteminfo", mParam);
                break;
            case ItemType.Defence:
                WindowManager.Instance.OpenUI("defence", param);
                break;
            case ItemType.Normal:
                WindowManager.Instance.OpenUI("iteminfo", mParam);
                break;
            case ItemType.Box:
                WindowManager.Instance.OpenUI("iteminfo", mParam);
                break;
        }
    }

    public GameObject gameObject
    {
        get
        {
            return mObj;
        }
    }
    private void SetIcon(string icon)
    {
        if (mIcon != null)
        {
            UIAtlasHelper.SetSpriteImage(mIcon, icon);
        }
    }

    private void SetParam1(string param)
    {
        if( mSprite1 != null )
        {
            UIAtlasHelper.SetSpriteImage(mSprite1, param);
        }
    }
    private void SetParam2(string param)
    {
        if (mLabel1 != null)
        {
            if (param == null)
                param = "";
            mLabel1.text = param;
        }
    }

    public void Clear()
    {
        if( mGemIcon != null )
        {
            NGUITools.SetActive(mGemIcon.gameObject, false);
        }
        if( mStarIcon != null )
        {
            NGUITools.SetActive(mStarIcon.gameObject, false);
        }

        if( mLabel != null )
        {
            NGUITools.SetActive(mLabel.gameObject, false);
        }

        if (mSterLabel != null)
        {
            NGUITools.SetActive(mSterLabel.gameObject, false);
        }

        if( mIcon != null )
        {
            UIAtlasHelper.SetSpriteImage(mIcon, null);
        }

        if( mSprite1 != null )
        {
            UIAtlasHelper.SetSpriteImage(mSprite1, null);
        }

        if( mLabel1 != null )
        {
            mLabel1.text = "";
        }

        mItemID = -1;
        mPackType = PackageType.Invalid;
        mPackPos = -1;
    }

    public bool IsValid()
    {
        if (mItemID < 0)
            return false;
        return true;
    }

    public string GetItemName()
    {
        if (mItemRes == null)
            return "";
        return mItemRes.name;
    }
}

