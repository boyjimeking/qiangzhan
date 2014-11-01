using System.Reflection;
using UnityEngine;
using System.Collections;

public class UIChallengeFail : UIWindow
{
    private UILabel mText;
    private UIPlayTween mTween;
    private UIButton mBack;
    private UIButton mContinue;
    private ChallengeModule mModule;

    public UIChallengeFail()
    {
       
    }
    protected override void OnLoad()
    {
        base.OnLoad();
        mText = FindComponent<UILabel>("container/Text");
        mTween = FindComponent<UIPlayTween>("container");
        mBack = FindComponent<UIButton>("container/BtnGrid/Back");
        mContinue = FindComponent<UIButton>("container/BtnGrid/Continue");
        mTween = FindComponent<UIPlayTween>("container");
        mText.text = StringHelper.GetString("challengefail");
    }

    protected override void OnOpen(object param = null)
    {
        WindowManager.Instance.CloseUI("challengecountdown");
        mModule = ModuleManager.Instance.FindModule<ChallengeModule>();
        EventDelegate.Add(mContinue.onClick, OnContinue);
        EventDelegate.Add(mBack.onClick, OnBack);
        mTween.resetOnPlay = true;
        mTween.Play(true);
    }

    protected override void OnClose()
    {
        mModule = null;
        EventDelegate.Remove(mContinue.onClick, OnContinue);
        EventDelegate.Remove(mBack.onClick, OnBack);
    }
   
    private void OnContinue()
    {
        SoundManager.Instance.Play(15);
        OnYesClick();
        //int recom_grade;
        //int my_grade;
        //if (mModule.IsBattleGradeEnough(mModule.GetDoingFloor(), out recom_grade, out my_grade))
        //{
        //    OnYesClick();
        //}
        //else
        //{
        //    YesOrNoBoxManager.Instance.ShowYesOrNoUI(
        //      "",
        //      StringHelper.GetString("gradetext"),
        //      OnYesClick,
        //      OnNoClick,
        //      StringHelper.GetString("stillchallenge"),
        //      StringHelper.GetString("tostrong"));
        //}

    }

    private void OnYesClick()
    {
        mModule.ContinueChallenge();
        OnCloseUI();
    }

    private void OnNoClick()
    {
        WindowManager.Instance.OpenUI("assister", 1);
    }


    private void OnBack()
    {
        SoundManager.Instance.Play(15);
        OnCloseUI();
        SceneManager.Instance.RequestEnterLastCity();
    }
   
    private void OnCloseUI()
    {
        WindowManager.Instance.CloseUI("challengeFail");
        
    }
}
