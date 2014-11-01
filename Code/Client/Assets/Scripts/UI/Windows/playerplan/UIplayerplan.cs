using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIplayerplan : UIWindow
{
    protected GameObject mToggleGroup;
    protected GameObject mChengzhang;
    protected GameObject mJingying;
    private GameObject mPlanUnit;

    protected UIToggle mToggle1;
    protected UIToggle mToggle2;
    protected UIToggle mToggle3;

    #region 成长
    UIScrollView cScrollView;
    UIGrid cGrid;
    #endregion

    #region 精英
    UIScrollView jScrollView;
    UIGrid jGrid;
    #endregion

    #region 至尊
    UIScrollView zScrollView;
    UIGrid zGrid;
    #endregion

    #region  其他按钮
    UIButton mBtnClose;
    UILabel mTitleText;
    UISprite mTitleSprite;
    #endregion

    PlayerPlanModule Module = null;
    PlayerData data = null;
    PlayerPlanModule Pdm
    {
        get
        {
            if (Module == null)
                Module = ModuleManager.Instance.FindModule<PlayerPlanModule>();

            return Module;
        }
    }

    PlayerData Pdata
    {
        get
        {
            if (data == null)
                data = PlayerDataPool.Instance.MainData;

            return data;
        }
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        mToggleGroup = this.FindChild("ToggleGroup");
        mChengzhang = this.FindChild("chengzhang");
        mJingying = this.FindChild("jingying");
        mPlanUnit = FindChild("PlanUnit");
        mBtnClose = this.FindComponent<UIButton>("close");

        mToggle1 = this.FindComponent<UIToggle>("ToggleGroup/Toggle1");
        mToggle2 = this.FindComponent<UIToggle>("ToggleGroup/Toggle2");
        mToggle3 = this.FindComponent<UIToggle>("ToggleGroup/Toggle3");

        cScrollView = this.FindComponent<UIScrollView>("chengzhang/Scroll View");
        cGrid = this.FindComponent<UIGrid>("chengzhang/Scroll View/UIGrid");

        jScrollView = this.FindComponent<UIScrollView>("jingying/Scroll View");
        jGrid = this.FindComponent<UIGrid>("jingying/Scroll View/UIGrid");

        zScrollView = this.FindComponent<UIScrollView>("zhizun/Scroll View");
        zGrid = this.FindComponent<UIGrid>("zhizun/Scroll View/UIGrid");
        mTitleText = this.FindComponent<UILabel>("title/Label");
        mTitleSprite = this.FindComponent<UISprite>("title");
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen(param);
        InitPlayerPlanUI();
        EventDelegate.Add(mToggle1.onChange, onToggleChanged);
        EventDelegate.Add(mToggle2.onChange, onToggleChanged);
        EventDelegate.Add(mToggle3.onChange, onToggleChanged);
        EventDelegate.Add(mBtnClose.onClick, OnCloseClick);
    }

    private void onToggleChanged()
    {
        if (mToggle1.value)
        {
            mTitleText.text = "";
            mTitleSprite.gameObject.SetActive(false);
        }
           
        if (mToggle2.value)
        {
            mTitleText.text = "累计充值" + GameConfig.Jingying_jihua_zhuanshi.ToString() + "钻石开启精英计划";
            mTitleSprite.gameObject.SetActive(true);
        }
         
        if (mToggle3.value)
        {
            mTitleText.text = "累计充值" + GameConfig.Zhizun_jihua_zhuanshi.ToString() + "钻石开启至尊计划";
            mTitleSprite.gameObject.SetActive(true);
        }
           

    }
 
    private void OnCloseClick()
    {
        WindowManager.Instance.CloseUI("playerplan");
    }

    protected override void OnClose()
    {
        EventDelegate.Remove(mBtnClose.onClick, OnCloseClick);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
 
    private void IntObject(GameObject gameObj,int index)
    {
        gameObj.transform.localScale = Vector3.one;
        gameObj.name = "plan_" + index.ToString();
        PlanUnitUI plan = new PlanUnitUI(gameObj);
        Pdm.mItemDic.Add(index, plan);
        plan.mBtn.gameObject.name = index.ToString();
        UIEventListener.Get(plan.mBtn.gameObject).onClick = OnBtnClick;
    }
    void InitPlayerPlanUI()
    {
        foreach (var item in Pdata.mPlanData.mDataDic)
        {
            if (!Pdm.mItemDic.ContainsKey(item.Key))
            {
                GameObject gameObj = WindowManager.Instance.CloneGameObject(mPlanUnit);
                if (gameObj == null)
                {
                    GameDebug.LogError("instance PlanUnit error");
                    return;
                }

                gameObj.SetActive(true);
                switch ((PlayerPlanModule.PlanEnum)Pdm.getType(item.Key))
                {
                    case PlayerPlanModule.PlanEnum.CHENGZHANG:
                        {
                            gameObj.transform.parent = cGrid.gameObject.transform;
                            IntObject(gameObj, item.Key);
                            cGrid.repositionNow = true;
                            cScrollView.ResetPosition();
                        }break;

                    case PlayerPlanModule.PlanEnum.JINGYING:
                        {
                            gameObj.transform.parent = jGrid.gameObject.transform;
                            IntObject(gameObj, item.Key);
                            jGrid.repositionNow = true;
                            jScrollView.ResetPosition();
                        } break;

                    case PlayerPlanModule.PlanEnum.ZHIZHUN:
                        {
                            gameObj.transform.parent = zGrid.gameObject.transform;
                            IntObject(gameObj, item.Key);
                            zGrid.repositionNow = true;
                            zScrollView.ResetPosition();
                        } break;

                    default:
                            {
                                GameDebug.LogError("UIplayerpLan Error");
                            }
                        break;
                }
            }

            if (null != Pdm.mItemDic[item.Key])
                Pdm.mItemDic[item.Key].updateitem(Pdm, item.Value.state,(PlayerPlanModule.PlanEnum)Pdm.getType(item.Key));
        } 
    }

    private void OnBtnClick(GameObject gameObj)
    {
        if (PlayerPlanModule.MIN_PLAN_NUM > int.Parse(gameObj.name) || PlayerPlanModule.MAX_PLAN_NUM < int.Parse(gameObj.name))
            return;

        PlanUnitUI plan = null;
        foreach (var item in Pdm.mItemDic)
        {
            if (item.Key == int.Parse(gameObj.name))
            {
                plan = item.Value;
                break;
            }
        }

        if (null == plan)
            return;

        PlayerPlanModule.BUTTON_STATE state =  PlayerPlanModule.BUTTON_STATE.Invalid;
        foreach (var item in Pdata.mPlanData.mDataDic)
        {
            if (item.Key == int.Parse(gameObj.name))
            {
                state = item.Value.state;
                break;
            }
        }

        switch (state)
        {
            case PlayerPlanModule.BUTTON_STATE.Have_noliqu:
                {
                    int num = Pdm.getWeek();
                    if (0 == num)
                        num = 7;

                    int objnum = int.Parse(gameObj.name) % 7;
                    if (0 == objnum)
                        objnum = 7;

                    if (num < objnum)//不发送消息
                    {
                        PopTipManager.Instance.AddNewTip(StringHelper.GetString("time_enough", FontColor.Red));
                        return;
                    }

                    planParam param = new planParam();
                    param.planid = int.Parse(gameObj.name);

                    Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_PLAYER_PLAN_STAGE, param);
                } break;

            default:

                break;
        }
    }
}
