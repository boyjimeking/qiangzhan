using UnityEngine;
using System.Collections;

public class PlanUnitUI
{
    protected GameObject mGameObj;
    public UIButton mBtn;
    public UILabel mBtnText;
    public UISprite mDate;
    public UISprite mShuZi;
    public UILabel mTiaojian;//条件
    public UILabel mJiangli;//奖励
    public PlayerPlanModule.BUTTON_STATE mBtnState;
    public PlayerPlanModule.PlanEnum mPlanState;
    public UISprite mStateImage;
    public UISprite mPlanImage;
    public UISprite mZhuanshi;
    public UILabel mZhuanshiText;

    public GameObject gameObject
    {
        get
        {
            return mGameObj;
        }
    }
    public PlanUnitUI(GameObject go)
    {
        mDate = ObjectCommon.GetChildComponent<UISprite>(go, "date");
        mBtnText = ObjectCommon.GetChildComponent<UILabel>(go, "btnText");
        mTiaojian = ObjectCommon.GetChildComponent<UILabel>(go, "tiaojian");
        mJiangli = ObjectCommon.GetChildComponent<UILabel>(go, "jiangli");
        mBtn = ObjectCommon.GetChildComponent<UIButton>(go, "Button");
        mShuZi = ObjectCommon.GetChildComponent<UISprite>(go, "shuzi");
        mStateImage = ObjectCommon.GetChildComponent<UISprite>(go, "stateimage");
        mZhuanshi = ObjectCommon.GetChildComponent<UISprite>(go, "zhuanshi");
        mZhuanshiText = ObjectCommon.GetChildComponent<UILabel>(go, "zhanshitext"); 
        mPlanImage = go.GetComponent<UISprite>();
        mGameObj = go;
    }
    private void ShowImageState(PlayerPlanModule Pdm,PlayerPlanModule.PlanEnum state,int index)
    {
        switch (state)
        {
            case PlayerPlanModule.PlanEnum.CHENGZHANG:
                {
                    if (0 == index % 7)
                        index = 7;
                    else
                        index = index % 7;

                    UIAtlasHelper.SetSpriteImage(mDate, "wanjiajihua:wanjiajihua_11", true);
                    UIAtlasHelper.SetSpriteImage(mShuZi, "shuzi:wanjiajihua_0" + index.ToString(), true);
                    mShuZi.gameObject.SetActive(true);
                    mZhuanshi.gameObject.SetActive(false);
                    mZhuanshiText.text = "";
                } break;

            case PlayerPlanModule.PlanEnum.JINGYING:
                {
                   
                    mShuZi.gameObject.SetActive(false);
                    mZhuanshi.gameObject.SetActive(true);
                    mZhuanshiText.text = "x" + Pdm.getJewelNum(index).ToString();
                    if (0 == index % 7)
                        index = 7;
                    else
                        index = index % 7;

                    UIAtlasHelper.SetSpriteImage(mDate, "qirilibao:qirilibao_0" + index.ToString(),true);

                } break;

            case PlayerPlanModule.PlanEnum.ZHIZHUN:
                {
                    mShuZi.gameObject.SetActive(false);
                    mZhuanshi.gameObject.SetActive(true);
                    mZhuanshiText.text = "x" + Pdm.getJewelNum(index).ToString();
                    if (0 == index % 7)
                        index = 7;
                    else
                        index = index % 7;

                    UIAtlasHelper.SetSpriteImage(mDate, "qirilibao:qirilibao_0" + index.ToString(), true);
                } break;

            default:
                GameDebug.LogError("PlayerPlanModule.PlanEnum state参数错误：" + state.ToString());
                break;
        }
    }

    public void updateitem(PlayerPlanModule Pdm, PlayerPlanModule.BUTTON_STATE btnstate, PlayerPlanModule.PlanEnum planstate)
    {
        mBtnState = btnstate;
        mPlanState = planstate;
        mBtnText.text = getText(mBtnState);
        mJiangli.text = Pdm.getjiangliText(int.Parse(mBtn.gameObject.name));
        mTiaojian.text = Pdm.gettiaojianText(int.Parse(mBtn.gameObject.name));
        ShowImageState(Pdm,mPlanState,int.Parse(mBtn.gameObject.name));    
    }

    private string getText(PlayerPlanModule.BUTTON_STATE state)
    {
        switch (state)
        {
            case PlayerPlanModule.BUTTON_STATE.Invalid:
                {
                    mBtn.gameObject.SetActive(true);
                    mStateImage.gameObject.SetActive(false);
                    UIAtlasHelper.SetButtonImage(mBtn, "common:btn_blue_4word");
                    mPlanImage.alpha = 1.0f;
                    return "未开启";
                }

            case PlayerPlanModule.BUTTON_STATE.Have_noliqu:
                {
                    mBtn.gameObject.SetActive(true);
                    mStateImage.gameObject.SetActive(false);
                    UIAtlasHelper.SetButtonImage(mBtn, "common:btn_green_4word");
                    mPlanImage.alpha = 1.0f;
                    return "领取";
                }


            case PlayerPlanModule.BUTTON_STATE.Have_liqu:
                {
                    mBtn.gameObject.SetActive(false);
                    mStateImage.gameObject.SetActive(true);
                    UIAtlasHelper.SetSpriteImage(mStateImage, "wanjiajihua:wanjiajihua_20");
                    mPlanImage.alpha = 0.5f;
                    return "";
                }


            case PlayerPlanModule.BUTTON_STATE.Have_guqi:
                {
                    mBtn.gameObject.SetActive(false);
                    mStateImage.gameObject.SetActive(true);
                    UIAtlasHelper.SetSpriteImage(mStateImage, "wanjiajihua:wanjiajihua_19");
                    mPlanImage.alpha = 0.5f;
                    return "";
                }

            default:
                GameDebug.LogError("playerPlanModule->getText() error");
                return "错误";
        }
    }
}
