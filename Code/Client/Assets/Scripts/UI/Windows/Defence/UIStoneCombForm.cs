using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using Message;

public class UIStoneCombForm : UIWindow
{
    private Dictionary<int, List<UILabel>> dic = null;
    private Dictionary<int, List<UIButton>> mDicBtn = null;
    private UISprite mStoneDemandPic = null;
    private UILabel mStoneDemandName = null;
    private UILabel mStoneDemandPro = null;
    private UILabel mStoneDemandMoney = null;
    private UILabel mStoneDemandNum = null;
    private UIButton mBtncomblv = null;
    private UIButton mBtnStone1 = null;
    private UIButton mBtnStone2 = null;
    private UIButton mBtnStone3 = null;
    private UIButton mBtnStone4 = null;
    private UIButton mBtnStone5 = null;
    private UISprite mStone1 = null;
    private UISprite mStone2 = null;
    private UISprite mStone3 = null;
    private UISprite mStone4 = null;
    private UISprite mStone5 = null;

    private UISprite mOldSelected = null;
    private UISprite mOldBtnSelected = null;
    private uint mOldBtnSelectedNum = 0;
    private List<bool> mStoneOpen = null;

    private GameObject mStoneDemandPanel = null;

    private DefenceUIParam mParam = null;

    private UISpriteAnimation mCombSuccessAni = null;

    public UIStoneCombForm()
    {
        mStoneOpen = new List<bool>();
        for (int i = 0; i < 5; ++i)
        {
            mStoneOpen.Add(false);
        }
        dic = new Dictionary<int, List<UILabel>>();
        mDicBtn = new Dictionary<int, List<UIButton>>();
        for (int i = 0; i < 5; ++i)
        {
            mDicBtn.Add(i, new List<UIButton>());
            dic.Add(i, new List<UILabel> ());
        }
    }
    protected override void OnLoad()
    {
        base.OnLoad();
        int stoneid = 3000000;
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 1; j <= 7; ++j)
            {
                dic[i].Add(this.FindComponent<UILabel>("StoneListPanel/SubPanel/Table/Stone" + (i + 1) + "/List" + (i + 1) + "/Sprite" + stoneid + "/combnum"));
                mDicBtn[i].Add(this.FindComponent<UIButton>("StoneListPanel/SubPanel/Table/Stone" + (i + 1) + "/List" + (i + 1) + "/Sprite" + stoneid));
                stoneid++;
            }
            stoneid++;
        }

        mStoneDemandPic = this.FindComponent<UISprite>("StoneCombDemandPanel/Sprite/Sprite/stonepic"); ;
        mStoneDemandName = this.FindComponent<UILabel>("StoneCombDemandPanel/Sprite/stonename");
        mStoneDemandPro = this.FindComponent<UILabel>("StoneCombDemandPanel/Sprite/pronameandvalue");
        mStoneDemandMoney = this.FindComponent<UILabel>("StoneCombDemandPanel/Sprite/moneyusednum");
        mStoneDemandNum = this.FindComponent<UILabel>("StoneCombDemandPanel/Sprite/stoneusednum");
        mBtncomblv = this.FindComponent<UIButton>("StoneCombDemandPanel/Sprite/btncomb");
        mBtnStone1 = this.FindComponent<UIButton>("StoneListPanel/SubPanel/Table/Stone1/stoneclick");
        mBtnStone2 = this.FindComponent<UIButton>("StoneListPanel/SubPanel/Table/Stone2/stoneclick");
        mBtnStone3 = this.FindComponent<UIButton>("StoneListPanel/SubPanel/Table/Stone3/stoneclick");
        mBtnStone4 = this.FindComponent<UIButton>("StoneListPanel/SubPanel/Table/Stone4/stoneclick");
        mBtnStone5 = this.FindComponent<UIButton>("StoneListPanel/SubPanel/Table/Stone5/stoneclick");
        mStone1 = this.FindComponent<UISprite>("StoneListPanel/SubPanel/Table/Stone1/stoneclick/Sprite");
        mStone2 = this.FindComponent<UISprite>("StoneListPanel/SubPanel/Table/Stone2/stoneclick/Sprite");
        mStone3 = this.FindComponent<UISprite>("StoneListPanel/SubPanel/Table/Stone3/stoneclick/Sprite");
        mStone4 = this.FindComponent<UISprite>("StoneListPanel/SubPanel/Table/Stone4/stoneclick/Sprite");
        mStone5 = this.FindComponent<UISprite>("StoneListPanel/SubPanel/Table/Stone5/stoneclick/Sprite");
        mStoneDemandPanel = this.FindChild("StoneCombDemandPanel");
        mCombSuccessAni = this.FindComponent<UISpriteAnimation>("StoneCombDemandPanel/combtexiao");
    }
    protected override void OnOpen(object param = null)
    {
        EventSystem.Instance.addEventListener(ItemEvent.STONE_RISE, OnRespondCombHandler);
        InitUI();
        mParam = new DefenceUIParam();
        if (mStoneDemandPanel.activeSelf)
        {
            mStoneDemandPanel.SetActive(false);
        }

        if (ObjectCommon.ReferenceEquals(mOldBtnSelected, mStone1))
        {
            SetNumOfComb(0, 10001, 10007);
        }
        else if (ObjectCommon.ReferenceEquals(mOldBtnSelected, mStone2))
        {
            SetNumOfComb(1, 10008, 10014);
        }
        else if (ObjectCommon.ReferenceEquals(mOldBtnSelected, mStone3))
        {
            SetNumOfComb(2, 10015, 10021);
        }
        else if (ObjectCommon.ReferenceEquals(mOldBtnSelected, mStone4))
        {
            SetNumOfComb(3, 10022, 10028);
        }
        else
        {
            SetNumOfComb(4, 10029, 10035);
        }
    }
    protected override void OnClose()
    {
        if (mOldSelected)
            SetIcon(mOldSelected, "atlas_defence:combelement2");

        EventSystem.Instance.removeEventListener(ItemEvent.STONE_RISE, OnRespondCombHandler);
    }
    public override void Update(uint elapsed)
    {
        
    }

    private void InitUI()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        EventDelegate.Add(mBtnStone1.onClick, OnStone1BtnHandler);
        EventDelegate.Add(mBtnStone2.onClick, OnStone2BtnHandler);
        EventDelegate.Add(mBtnStone3.onClick, OnStone3BtnHandler);
        EventDelegate.Add(mBtnStone4.onClick, OnStone4BtnHandler);
        EventDelegate.Add(mBtnStone5.onClick, OnStone5BtnHandler);
        EventDelegate.Add(mBtncomblv.onClick, OnBtnCombHandler);
        foreach (KeyValuePair<int, List<UIButton>> list in mDicBtn)
        {
            foreach (UIButton btn in list.Value)
            {
                UIEventListener.Get(btn.gameObject).onClick = OnStoneItemClicked;
            }
        }
    }

    private void OnStone1BtnHandler()
    {
        if (mStone1.spriteName.Equals("quest-ditu") && !mStoneOpen[0])
        {
            SetNumOfComb(0, 10001, 10007);
            mStoneOpen[0] = !mStoneOpen[0];
            SetIcon(mStone1, "atlas_defence:combelement4");
            if (mOldBtnSelected && !ObjectCommon.ReferenceEquals(mOldBtnSelected, mStone1))
                SetIcon(mOldBtnSelected, "common:quest-ditu");
            mOldBtnSelected = mStone1;
        }
        else
        {
            SetIcon(mStone1, "common:quest-ditu");
            mStoneOpen[0] = !mStoneOpen[0];
        }
    }

    private void OnStone2BtnHandler()
    {
        if (mStone2.spriteName.Equals("quest-ditu") && !mStoneOpen[1])
        {
            SetNumOfComb(1, 10008, 10014);
            mStoneOpen[1] = !mStoneOpen[1];
            SetIcon(mStone2, "atlas_defence:combelement4");
            if (mOldBtnSelected && !ObjectCommon.ReferenceEquals(mOldBtnSelected, mStone2))
                SetIcon(mOldBtnSelected, "common:quest-ditu");
            mOldBtnSelected = mStone2;
        }
        else
        {
            SetIcon(mStone2, "common:quest-ditu");
            mStoneOpen[1] = !mStoneOpen[1];
        }
    }

    private void OnStone3BtnHandler()
    {
        if (mStone3.spriteName.Equals("quest-ditu") && !mStoneOpen[2])
        {
            SetIcon(mStone3, "atlas_defence:combelement4");
            mStoneOpen[2] = !mStoneOpen[2];
            SetNumOfComb(2, 10015, 10021);
            if (mOldBtnSelected && !ObjectCommon.ReferenceEquals(mOldBtnSelected, mStone3))
                SetIcon(mOldBtnSelected, "common:quest-ditu");
            mOldBtnSelected = mStone3;
        }
        else
        {
            SetIcon(mStone3, "common:quest-ditu");
            mStoneOpen[2] = !mStoneOpen[2];
        }
    }

    private void OnStone4BtnHandler()
    {
        if (mStone4.spriteName.Equals("quest-ditu") && !mStoneOpen[3])
        {
            SetIcon(mStone4, "atlas_defence:combelement4");
            mStoneOpen[3] = !mStoneOpen[3];
            SetNumOfComb(3, 10022, 10028);
            if (mOldBtnSelected && !ObjectCommon.ReferenceEquals(mOldBtnSelected, mStone4))
                SetIcon(mOldBtnSelected, "common:quest-ditu");
            mOldBtnSelected = mStone4;
        }
        else
        {
            SetIcon(mStone4, "common:quest-ditu");
            mStoneOpen[3] = !mStoneOpen[3];
        }
    }

    private void OnStone5BtnHandler()
    {
        if (mStone5.spriteName.Equals("quest-ditu") && !mStoneOpen[4])
        {
            SetIcon(mStone5, "atlas_defence:combelement4");
            mStoneOpen[4] = !mStoneOpen[4];
            SetNumOfComb(4, 10029, 10035);
            if (mOldBtnSelected && !ObjectCommon.ReferenceEquals(mOldBtnSelected, mStone5))
                SetIcon(mOldBtnSelected, "common:quest-ditu");
            mOldBtnSelected = mStone5;
        }
        else
        {
            SetIcon(mStone5, "common:quest-ditu");
            mStoneOpen[4] = !mStoneOpen[4];
        }
    }

    private void OnBtnCombHandler()
    {
        DefenceModule module = ModuleManager.Instance.FindModule<DefenceModule>();
        if (null == module)
            return;
        mParam.isequiped = false;
        module.StoneComb(mParam);
    }

    private void OnStoneItemClicked(GameObject obj)
    {
        int  resid = int.Parse(obj.gameObject.name.Substring(6, 7));
        if (mOldSelected)
            SetIcon(mOldSelected, "atlas_defence:combelement2");
        mOldSelected = ObjectCommon.GetChildComponent<UISprite>(obj, "Sprite");
        SetIcon(mOldSelected, "atlas_defence:combelement1");
        mParam.stoneid = resid;

        if (StoneCombInfoInit(resid))
        {
            if (!mStoneDemandPanel.activeSelf)
            {
                mStoneDemandPanel.SetActive(true);
            }
        }
        else
        {
            if (mStoneDemandPanel.activeSelf)
            {
                mStoneDemandPanel.SetActive(false);
            }
        }
    }

    private void SetNumOfComb(int stonetype,int combidfrom,int combidto)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
            return;
        PackageManager pack = module.GetPackManager();
        if (null == pack)
            return;

        for (int i = combidfrom; i <= combidto; ++i)
        {
            DefenceCombItem combitem = DataManager.DefenceCombTable[i] as DefenceCombItem;
            if (null == combitem || 0 == combitem.num1)
            {
                dic[stonetype][i - combidfrom].text = "";
                continue;
            }

            uint num = pack.GetNumByID(combitem.item1, PackageType.Pack_Gem) / (uint)combitem.num1;
            if (0 != num)
                dic[stonetype][i - combidfrom].text = "【可合成数量 " + num + " 】";
            else
                dic[stonetype][i - combidfrom].text = "";
        }
    }

    private void OnRespondCombHandler(EventBase evt)
    {
        UILabel stonetext = null;
        UILabel stonepromt = null;
        StoneTableItem stoneitem = DataManager.StoneTable[mParam.stoneid] as StoneTableItem;
        if (null == stoneitem)
        {
            GameDebug.LogError("stones.txt中没有此宝石 id = " + mParam.stoneid);
            return;
        }
        DefenceCombItem combitem = DataManager.DefenceCombTable[stoneitem.combid] as DefenceCombItem;
        if (null == combitem || 0 == combitem.num1)
        {
            return;
        }
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 7; ++j)
            {
                if (mDicBtn[i][j].name.Equals("Sprite" + mParam.stoneid))
                {
                    stonetext = dic[i][j];
                }
                else if (mDicBtn[i][j].name.Equals("Sprite" + combitem.defenceproducedId))
                {
                    stonepromt = dic[i][j];
                }

                if (null != stonetext && null != stonepromt)
                    break;
            }
        }
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
        {
            SetStoneActive(stonetext);
            return;
        }
        PackageManager pack = module.GetPackManager();
        if (null == pack)
        {
            SetStoneActive(stonetext);
            return;
        }

        uint num = pack.GetNumByID(combitem.item1, PackageType.Pack_Gem) / (uint)combitem.num1;
        if (0 != num)
            stonetext.text = "【可合成数量 " + num + " 】";
        else
            SetStoneActive(stonetext);

        stoneitem = DataManager.StoneTable[combitem.defenceproducedId] as StoneTableItem;
        if (null == stoneitem)
        {
            GameDebug.LogError("stones.txt中没有此宝石 id = " + combitem.defenceproducedId);
            return;
        }

        combitem = DataManager.DefenceCombTable[stoneitem.combid] as DefenceCombItem;
        if (null == combitem)
        {
            GameDebug.LogError("defencecomb.txt中没有此合成序列 id = " + stoneitem.combid);
            return;
        }
        num = pack.GetNumByID(combitem.item1, PackageType.Pack_Gem) / (uint)combitem.num1;
        if (0 != num)
            stonepromt.text = "【可合成数量 " + num + " 】";

        StoneCombInfoInit(mParam.stoneid);
        onCombSucess();
    }

    private bool StoneCombInfoInit(int resid)
    { 
        StoneTableItem stoneitem = DataManager.StoneTable[resid] as StoneTableItem;
        if (null == stoneitem)
            return false;

        DefenceCombItem combitem = DataManager.DefenceCombTable[stoneitem.combid] as DefenceCombItem;
        if (null == combitem || 0 == combitem.num1)
        {
            GameDebug.LogError("defencecomb.txt中没有此合成序列 id = " + stoneitem.combid);
            return false;
        }

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
            return false;
        PackageManager pack = module.GetPackManager();
        if (null == pack)
            return false;

        stoneitem = DataManager.StoneTable[combitem.defenceproducedId] as StoneTableItem;
        if (null == stoneitem)
        {
            GameDebug.LogError("stones.txt中没有此宝石 id = " + combitem.defenceproducedId);
            return false;
        }

        PropertyTableItem proitem = DataManager.PropertyTable[stoneitem.proid] as PropertyTableItem;
        if (null == proitem)
            return false;

        UIAtlasHelper.SetSpriteImage(mStoneDemandPic, stoneitem.picname);
        mStoneDemandName.text = stoneitem.name;// + " Lv" + stoneitem.level;
        mStoneDemandPro.text = proitem.name + " " + stoneitem.provalue;
        if (module.GetProceeds(ProceedsType.Money_Game) < combitem.moenyused)
            mStoneDemandMoney.text = "[E92224]";
        else
            mStoneDemandMoney.text = "[3EFF00]";
        mStoneDemandMoney.text += "金币 X" + combitem.moenyused;

        if (module.GetItemNumByID(combitem.item1, PackageType.Pack_Gem) < combitem.num1)
            mStoneDemandNum.text = "[E92224]";
        else
            mStoneDemandNum.text = "[3EFF00]";
        mStoneDemandNum.text += stoneitem.name/* + " Lv" + stoneitem.level*/ + " X" + combitem.num1;
        return true;
    }

    private void SetStoneActive(UILabel ui)
    {
        ui.text = "";
    }

    private void SetIcon(UISprite sprite, string name)
    {
        UIAtlasHelper.SetSpriteImage(sprite, name);
    }

    #region 特效
    //宝石合成成功
    void onCombSucess()
    {
        mCombSuccessAni.Reset();
        mCombSuccessAni.gameObject.SetActive(true);
        mCombSuccessAni.onFinished += onStrenSpriteAniFinish;
    }

    void onStrenSpriteAniFinish(GameObject go)
    {
        mCombSuccessAni.onFinished -= onStrenSpriteAniFinish;
        mCombSuccessAni.Reset();
        mCombSuccessAni.gameObject.SetActive(false);
    }
    #endregion
}
