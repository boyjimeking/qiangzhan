
using System.Collections.Generic;
using UnityEngine;

public class UIChallengeCountdown : UIWindow
{
    private UISprite mFillSprite;
    private List<UISprite> mAchieveList;
    private List<UISpriteAnimation> mAchieveAniList;
    private UISpriteAnimation achieveAniOne;
    private UISpriteAnimation achieveAniTwo;
    private UISpriteAnimation achieveAniThree;
    private UILabel timeLabel;

    private int mCurTime;
    private int mFastTime;
    private int mMidTime;
    private int mSlowTime;
    private int mMaxTime;
    private int mElaspsTime = 0;
    //分段时间
    private int mSubTime = 0;

    // 0快，1中速，2慢速,3 没成就
    private int State;

    protected override void OnLoad()
    {
        base.OnLoad();

        mAchieveList = new List<UISprite>
        {
            FindComponent<UISprite>("Container/achieve1"),
            FindComponent<UISprite>("Container/achieve2"),
            FindComponent<UISprite>("Container/achieve3")
        };

        mAchieveAniList = new List<UISpriteAnimation>
        {
            FindComponent<UISpriteAnimation>("Container/achieveAni1"),
            FindComponent<UISpriteAnimation>("Container/achieveAni2"),
            FindComponent<UISpriteAnimation>("Container/achieveAni3")
        };

        mFillSprite = FindComponent<UISprite>("Container/FillSprite");    
        timeLabel = FindComponent<UILabel>("Container/timeCounter/timeLabel");
       
    }

    private void OnAniFinish(GameObject obj)
    {
        obj.SetActive(false);
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen(param);
        mFillSprite.fillAmount = 1;
        foreach (var Ani in mAchieveAniList)
        {
            Ani.gameObject.SetActive(true);
            Ani.Reset();
            Ani.Stop();
            Ani.onFinished += OnAniFinish;
        }

        ChallengeModule module = ModuleManager.Instance.FindModule<ChallengeModule>();
        ChallengeTableItem item = module.GetChallengeTableItem(module.GetCurFloor());
        mFastTime = item.mAchieveParamOne;
        mMidTime = item.mAchieveParamTwo;
        mSlowTime = item.mAchieveParamThree;
      
        State = 0;
        
    }

    protected override void OnClose()
    {
        base.OnClose();
        foreach (var Ani in mAchieveAniList)
        {
            Ani.onFinished -= OnAniFinish;
        }
    }

    public override void Update(uint elapsed)
    {
        base.Update(elapsed);
        GameScene gameScene = SceneManager.Instance.GetCurScene() as GameScene;
        mMaxTime = SceneManager.Instance.GetCountDown();
        mElaspsTime = (int)gameScene.GetLogicRunTime();
        mFillSprite.fillAmount = (float) (mSlowTime - mElaspsTime)/(float) mSlowTime;
        timeLabel.text = TimeUtilities.GetTowerCountDown(mMaxTime - mElaspsTime);
        if (mElaspsTime > mFastTime && mElaspsTime <= mMidTime)
        {
            if (State != 0) return;
            mAchieveAniList[0].enabled = true;
            mAchieveAniList[0].Reset();
            
            State = 1;
        }
        else if (mElaspsTime <= mSlowTime)
        {
            if (State != 1) return;
            mAchieveAniList[1].enabled = true;
            mAchieveAniList[1].Reset();
            State = 2;
        }
        else
        {
            if (State != 2) return;
            mAchieveAniList[2].enabled = true;
            mAchieveAniList[2].Reset();
            State = 3;
        }

       
    }

    
}

