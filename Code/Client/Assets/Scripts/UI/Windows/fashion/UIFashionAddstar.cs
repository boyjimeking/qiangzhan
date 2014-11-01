
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIFashionAddstar:UIWindow
{
    private UILabel mFashionName2;
    private UILabel mFlag;
    private UILabel mItemType;
    private UILabel mStarNum;
    private UILabel mCurTxt;
    private UILabel mCurLife;
    private UILabel mCurAttack;
    private UILabel mCurDefence;
    private UILabel mCurCritical;
    private UILabel mCurPower;
    private UILabel mNextTxt;
    private UILabel mNexLife;
    private UILabel mNextAttack;
    private UILabel mNextDefence;
    private UILabel mNextCritical;
    private UILabel mNextPower;
    private UILabel battleScore;
    private UILabel mTitle2;
    private UILabel mCostTxt;
    private UILabel mCostNum;
    private UILabel mTip;
    private UIButton mAddstar2;
    private UIButton mAddTen;
    private FashionModule mModule;
    private List<UISprite> startIconList;
    private int mFashionId;
    private int mAddNum = 0;
    private bool isDirty = false;
    protected override void OnLoad()
    {
        base.OnLoad();
        mFashionName2 = FindComponent<UILabel>("addstarContainer/name");
        mFlag = FindComponent<UILabel>("addstarContainer/flag");
        mItemType = FindComponent<UILabel>("addstarContainer/itemtype");
        mStarNum = FindComponent<UILabel>("addstarContainer/startnum");
        mCurTxt = FindComponent<UILabel>("addstarContainer/curtxt");
        mCurLife = FindComponent<UILabel>("addstarContainer/life");
        mCurAttack = FindComponent<UILabel>("addstarContainer/attack");
        mCurDefence = FindComponent<UILabel>("addstarContainer/defence");
        mCurCritical = FindComponent<UILabel>("addstarContainer/critical");
        mCurPower = FindComponent<UILabel>("addstarContainer/power");
        mNextTxt = FindComponent<UILabel>("addstarContainer/nexttxt");
        battleScore = FindComponent<UILabel>("addstarContainer/zhanli/battlescore");
        mTitle2 = FindComponent<UILabel>("addstarContainer/bg2/Label");
        mCostTxt = FindComponent<UILabel>("addstarContainer/costtext");
        mCostNum = FindComponent<UILabel>("addstarContainer/costnum");
        mTip = FindComponent<UILabel>("addstarContainer/tip");
        mNexLife = FindComponent<UILabel>("addstarContainer/nextlife");
        mNextAttack = FindComponent<UILabel>("addstarContainer/nextattack");
        mNextDefence = FindComponent<UILabel>("addstarContainer/nextdefence");
        mNextCritical = FindComponent<UILabel>("addstarContainer/nextcriticle");
        mNextPower = FindComponent<UILabel>("addstarContainer/nextpower");
        mAddstar2 = FindComponent<UIButton>("addstarContainer/addstar");
        mAddTen = FindComponent<UIButton>("addstarContainer/addten");
        startIconList = new List<UISprite>();
        for (int i = 0; i < FashionDefine.Starnum_Per_Level; ++i)
        {
            startIconList.Add(FindComponent<UISprite>("addstarContainer/stargrid/"+i.ToString()));
        }


    }

    protected override void OnClose()
    {
        base.OnClose();
        EventDelegate.Remove(mAddstar2.onClick, OnAddStar);
        EventDelegate.Remove(mAddTen.onClick, OnAddTen);
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen(param);
        mModule = ModuleManager.Instance.FindModule<FashionModule>();
        EventDelegate.Add(mAddstar2.onClick, OnAddStar);
        EventDelegate.Add(mAddTen.onClick,OnAddTen);
        mFashionId = Convert.ToInt32(param);
        RefreshPanel(mFashionId);
        mAddNum = 0;
        EventSystem.Instance.addEventListener(FashionEvent.FASHION_ADDSTAR, OnRespondAddStar);
    }

    private void OnAddStar()
    {
        mAddNum = 0;
        mModule.RequestAddStar(mFashionId);
        SoundManager.Instance.Play(15);
    }



    private void OnAddTen()
    {
        mAddNum = 9;
        mModule.RequestAddStar(mFashionId);
        SoundManager.Instance.Play(15);
    }

    private void OnRespondAddStar(EventBase evt)
    {
        isDirty = true;
        if (0 == mAddNum) return;
        mAddNum--;

        if (!mModule.RequestAddStar(mFashionId))
        {
            mAddNum = 0;
        }
        else
        {
            return;
        }

        Thread.Sleep(200);
     
    }

    private void RefreshPanel(int fashionid)
    {
        isDirty = false;
        FashionItemData itemdata;
        FashionTableItem res = DataManager.FashionTable[fashionid] as FashionTableItem;
        if (res == null) return;
        if (!mModule.GetFashionByID(fashionid, out itemdata)) return;
        FashionPropTableItem propRes =
            DataManager.FashionPropTable[res.propid + itemdata.starnum] as FashionPropTableItem;
        if (propRes == null) return;

        mFashionName2.text = res.name;
        mFlag.text = StringHelper.GetString(mModule.GetEquipId(res.bodypart) == fashionid ? "equiped" : "noequip");
        mItemType.text = StringHelper.GetString("fashion_type");
        int startLevel = Mathf.FloorToInt(itemdata.starnum / FashionDefine.Starnum_Per_Level) + 1;
        int startCount = (int) (itemdata.starnum % FashionDefine.Starnum_Per_Level);
        mStarNum.text = string.Format(StringHelper.GetString("startlevel"),
            StringHelper.GetString(startLevel.ToString()));
        SetStarBar(startCount);
        mCostTxt.text = StringHelper.GetString("fashioncost");
        mCostNum.text = string.Format(StringHelper.GetString("costnum"), ItemManager.Instance.getItemName(res.costid),propRes.costnum);
        battleScore.text = propRes.battlescore.ToString();
        mTip.text = res.desc2;

        mCurTxt.text = StringHelper.GetString("curprop");
        mNextTxt.text = StringHelper.GetString("nextprop");
        mTitle2.text = StringHelper.GetString("tiptitle");

        mCurLife.text = StringHelper.GetString("life") + " " + propRes.life;
        mCurAttack.text = StringHelper.GetString("attack") + " " + propRes.fight;
        mCurDefence.text = StringHelper.GetString("defence") + " " + propRes.defence;
        mCurCritical.text = StringHelper.GetString("critical") + " " + propRes.critical;
        mCurPower.text = StringHelper.GetString("power") + " " + propRes.power;

        if (itemdata.starnum == res.max_stars)
        {
            mNexLife.gameObject.SetActive(false);
            mNextAttack.gameObject.SetActive(false);
            mNextDefence.gameObject.SetActive(false);
            mNextCritical.gameObject.SetActive(false);
            mNextPower.gameObject.SetActive(false);
            mCostTxt.gameObject.transform.localPosition = new Vector3(-7,151,0);
            mCostNum.gameObject.transform.localPosition = new Vector3(-7,124,0);

        }
        else
        {
            var nextProp = DataManager.FashionPropTable[res.propid + itemdata.starnum+1] as FashionPropTableItem;
            if (nextProp == null) return;
            mNexLife.gameObject.SetActive(true);
            mNextAttack.gameObject.SetActive(true);
            mNextDefence.gameObject.SetActive(true);
            mNextCritical.gameObject.SetActive(true);
            mNextPower.gameObject.SetActive(true);
            mNexLife.text = StringHelper.GetString("life") + " " + nextProp.life;
            mNextAttack.text = StringHelper.GetString("attack") + " " + nextProp.fight;
            mNextDefence.text = StringHelper.GetString("defence") + " " + nextProp.defence;
            mNextCritical.text = StringHelper.GetString("critical") + " " + nextProp.critical;
            mNextPower.text = StringHelper.GetString("power") + " " + nextProp.power;
            mCostTxt.gameObject.transform.localPosition = new Vector3(-7, -21, 0);
            mCostNum.gameObject.transform.localPosition = new Vector3(-7, -61, 0);
        }

    }

    private void SetStarBar(int starCount)
    {
        for (int i = 0; i < startIconList.Count; ++i)
        {
            UIAtlasHelper.SetSpriteImage(startIconList[i], i < starCount ? "common:strenth (8)" : "common:hei_xing");
        }
    }

    public override void Update(uint elapsed)
    {
        base.Update(elapsed);
        if (isDirty)
        {
            RefreshPanel(mFashionId);
        }
    }
}

