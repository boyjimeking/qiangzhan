using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageRewardParam
{
    public int zoneid = -1;
    public int mCurStars = -1;
    public int mMaxStars = int.MaxValue;
}

public class UIStageReward : UIWindow
{

    //Btn
    private UIButton mBtnClose = null;
    private UIButton mBtnObtain = null;
    
    //!Btn
    private UISprite mBox1 = null;
    private UISprite mBox2 = null;
    private UISprite mBox3 = null;
    private UISprite mObtainPic = null;

    private List<GameObject> mItemInfoList = new List<GameObject>();

    private GameObject mErrHint = null;

    private const int ITEM_NUM = 3;

    private StageRewardParam mParam = null;

    protected override void OnLoad()
    {
        mBtnClose = this.FindComponent<UIButton>("btnclose");
        mBtnObtain = this.FindComponent<UIButton>("ButtonYellow");

        mBox1 = this.FindComponent<UISprite>("Sprite1");
        mBox2 = this.FindComponent<UISprite>("Sprite2");
        mBox3 = this.FindComponent<UISprite>("Sprite3");
        mObtainPic = this.FindComponent<UISprite>("ButtonBlue");

        for (int i = 1; i <= ITEM_NUM; ++i)
        {
            mItemInfoList.Add(this.FindChild("item" + i));
        }

        mErrHint = this.FindChild("errorhint");

    }
    //界面打开
    protected override void OnOpen(object param = null)
    {
        if (null != param)
            mParam = param as StageRewardParam;
        else
            GameDebug.LogError("打开收集星星界面，需要传参！");

        InitUI();
    }

    private void InitUI()
    {
        EventSystem.Instance.addEventListener(ZoneRewardEvent.ZONE_REWARD_OBTAIN, OnRewardObtainHandler);
        EventDelegate.Add(mBtnObtain.onClick, OnBtnObtainHandler);
        EventDelegate.Add(mBtnClose.onClick, OnBtnCloseHandler);
        SetZoneState();
        SetZoneReward();
    }

    private void SetZoneState()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
            return;

        //奖励没有获取过 & 当前星星数量=最大星星数量
        if (!module.GetZoneHasObtainReward(mParam.zoneid))
        {
            mBox1.gameObject.SetActive(true);
            mBox2.gameObject.SetActive(false);
            mBox3.gameObject.SetActive(false);
            mBtnObtain.gameObject.SetActive(true);
            mObtainPic.gameObject.SetActive(false);
        }
        else
        {
            mBox1.gameObject.SetActive(false);
            mBox2.gameObject.SetActive(true);
            mBox3.gameObject.SetActive(true);
            mBtnObtain.gameObject.SetActive(false);
            mObtainPic.gameObject.SetActive(true);
            mErrHint.gameObject.SetActive(false);
        }

        if (mParam.mCurStars < mParam.mMaxStars)
        {
            mErrHint.SetActive(true);
        }
        else
        {
            mErrHint.SetActive(false);
        }
    }

    private void SetZoneReward()
    {
        ZoneRewardItem zonereward = DataManager.ZoneRewardTable[mParam.zoneid] as ZoneRewardItem;
        
        
        if (null == zonereward)
        {
            GameDebug.LogError("战区数据配置错误，战区ID = " + mParam.zoneid);
            return;
        }

        int index = 0;
        if (uint.MaxValue != zonereward.gamemoney)
        {
            UISprite sp = ObjectCommon.GetChildComponent<UISprite>(mItemInfoList[index], "sprite1");
            UISprite icon = ObjectCommon.GetChildComponent<UISprite>(mItemInfoList[index], "icon");
            UIAtlasHelper.SetSpriteImage(sp, null);
            UIAtlasHelper.SetSpriteImage(icon, "common:jinbi2");
            UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mItemInfoList[index], "num");
            lb.text = "X" + zonereward.gamemoney;

            mItemInfoList[index++].gameObject.SetActive(true);
        }

        if (uint.MaxValue != zonereward.rmbmoney)
        {
            UISprite sp = ObjectCommon.GetChildComponent<UISprite>(mItemInfoList[index], "sprite1");
            UISprite icon = ObjectCommon.GetChildComponent<UISprite>(mItemInfoList[index], "icon");
            UIAtlasHelper.SetSpriteImage(sp, null);
            UIAtlasHelper.SetSpriteImage(icon, "common:zhuanshi1");
            UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mItemInfoList[index], "num");
            lb.text = "X" + zonereward.rmbmoney;

            mItemInfoList[index++].gameObject.SetActive(true);
        }

        if (uint.MaxValue != zonereward.item1id)
        {
            PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
            if (null == module)
                return;

            NormalItemTableItem item = DataManager.NormalItemTable[zonereward.item1id] as NormalItemTableItem;
            if (null == item)
            {
                GameDebug.LogError("normalitem.txt中不存在此道具， ID = " + zonereward.item1id);
                return;
            }

            UISprite sp = ObjectCommon.GetChildComponent<UISprite>(mItemInfoList[index], "sprite1");
            UISprite icon = ObjectCommon.GetChildComponent<UISprite>(mItemInfoList[index], "icon");
            UILabel label = ObjectCommon.GetChildComponent<UILabel>(mItemInfoList[index], "label1");
            UIAtlasHelper.SetSpriteImage(icon, item.picname);
            UIAtlasHelper.SetSpriteImage(sp, item.picname2);
            label.text = item.picname3;

            UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mItemInfoList[index], "num");
            lb.text = "X" + zonereward.item1num;

            mItemInfoList[index++].gameObject.SetActive(true);
        }

        if (uint.MaxValue != zonereward.item2id)
        {
            PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
            if (null == module)
                return;

            NormalItemTableItem item = DataManager.NormalItemTable[zonereward.item2id] as NormalItemTableItem;
            if (null == item)
            {
                GameDebug.LogError("normalitem.txt中不存在此道具， ID = " + zonereward.item2id);
                return;
            }

            UISprite sp = ObjectCommon.GetChildComponent<UISprite>(mItemInfoList[index], "sprite1");
            UISprite icon = ObjectCommon.GetChildComponent<UISprite>(mItemInfoList[index], "icon");
            UILabel label = ObjectCommon.GetChildComponent<UILabel>(mItemInfoList[index], "label1");
            UIAtlasHelper.SetSpriteImage(icon, item.picname);
            UIAtlasHelper.SetSpriteImage(sp, item.picname2);
            label.text = item.picname3;

            UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mItemInfoList[index], "num");
            lb.text = "X" + zonereward.item2num;

            mItemInfoList[index++].gameObject.SetActive(true);
        }

        if (uint.MaxValue != zonereward.item3id)
        {
            PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
            if (null == module)
                return;

            NormalItemTableItem item = DataManager.NormalItemTable[zonereward.item3id] as NormalItemTableItem;
            if (null == item)
            {
                GameDebug.LogError("normalitem.txt中不存在此道具， ID = " + zonereward.item2id);
                return;
            }

            UISprite sp = ObjectCommon.GetChildComponent<UISprite>(mItemInfoList[index], "sprite1");
            UISprite icon = ObjectCommon.GetChildComponent<UISprite>(mItemInfoList[index], "icon");
            UILabel label = ObjectCommon.GetChildComponent<UILabel>(mItemInfoList[index], "label1");
            UIAtlasHelper.SetSpriteImage(icon, item.picname);
            UIAtlasHelper.SetSpriteImage(sp, item.picname2);
            label.text = item.picname3;

            UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mItemInfoList[index], "num");
            lb.text = "X" + zonereward.item3num;

            mItemInfoList[index++].gameObject.SetActive(true);
        }

        for (int i = index; i < mItemInfoList.Count; ++i)
        {
            mItemInfoList[i].gameObject.SetActive(false);
        }
    }
    private void OnBtnObtainHandler()
    {
        if (mParam.mCurStars < mParam.mMaxStars)
        {
            //PopTipManager.Instance.AddNewTip(StringHelper.GetString(""));
            return;
        }
        ZoneRewardActionParam param = new ZoneRewardActionParam();
        param.zoneid = mParam.zoneid;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_ZONE_REWARD,param);
    }

    private void OnBtnCloseHandler()
    {
        WindowManager.Instance.CloseUI("stagereward");
    }

    //界面关闭
    protected override void OnClose()
    {
        EventDelegate.Remove(mBtnObtain.onClick, OnBtnObtainHandler);
        EventDelegate.Remove(mBtnClose.onClick, OnBtnCloseHandler);
        EventSystem.Instance.removeEventListener(ZoneRewardEvent.ZONE_REWARD_OBTAIN, OnRewardObtainHandler);
    }
    public override void Update(uint elapsed)
    {
        
    }

    public void OnRewardObtainHandler(EventBase evt)
    {
        SetZoneState();
        SetZoneReward();
    }
}
