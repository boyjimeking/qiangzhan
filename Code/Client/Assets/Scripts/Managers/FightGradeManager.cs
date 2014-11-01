using UnityEngine;
using System.Collections;

public class FightGradeUI
{
    protected UILabel valLb = null;
    protected GameObject ani = null;

    private GameObject mGo = null;

    const float mEffectDur = 1f;
    const float mTweenPosDur = 1f;
    const float mFadeOutDur = 1f;

    public FightGradeUI(GameObject go)
    {
        valLb = ObjectCommon.GetChildComponent<UILabel>(go, "valObj/valLb");
        ani = ObjectCommon.GetChild(go, "ani");

        //valLb.transform.parent.transform.localPosition = new Vector3(0f, Screen.height * 0.6f, 0f);

        mGo = go;
    }

    public void SetData(uint oldVal, uint newVal)
    {
        bool tmp = newVal > oldVal;

        ScrollNumEffect eff = valLb.gameObject.AddMissingComponent<ScrollNumEffect>();
        
        eff.SetShowType("", true, false, "_");

        int numPerSec = (int)(Mathf.Abs((int)newVal - (int)oldVal) / mEffectDur);

        //eff.Play((int)oldVal, (int)newVal, numPerSec, false);
        int toVal = (int)(newVal - oldVal);
        int fromVal = toVal > 0 ? 1 : -1;
        
        
        ani.SetActive(tmp);

        //Vector3 to = new Vector3();
        //to.y = tmp ? -50f : -250f;
        if (tmp)
        {
            eff.onFinish = fadeOut;
            eff.Play(fromVal, toVal, numPerSec, false);

            TweenPosition tp = TweenPosition.Begin(valLb.transform.parent.gameObject, mTweenPosDur, new Vector3(0f, -50f, 0f));
            tp.PlayForward();
        }
        else
        {
            eff.onFinish = onScrollNumEffFinish;
            eff.Play(fromVal, toVal, numPerSec, false);
        }
    }

    void onScrollNumEffFinish()
    {
        TweenPosition tp = TweenPosition.Begin(valLb.transform.parent.gameObject, mTweenPosDur, new Vector3(0f, -250f, 0f));

        tp.PlayForward();

        fadeOut();
    }

    void fadeOut()
    {
        TweenAlpha ta = TweenAlpha.Begin(mGo, mFadeOutDur, 0f);
        ta.SetOnFinished(onFadeOut);
        ta.PlayForward();
    }

    void onFadeOut()
    {
        GameObject.Destroy(mGo);
    }
}

public class FightGradeManager
{
    public delegate void OnGradeChanged(uint oldVal, uint newVal, bool isMainCity);

    private static FightGradeManager mMgr = null;

    private uint mOldVal = 0;
    private uint mNewVal = 0;
    private bool mIsMainCity = true; //判断战斗力变化时候是否是主城;

    public FightGradeManager()
    {
        mMgr = this;
    }

    public static FightGradeManager Instance
    {
        get 
        {
            return mMgr;
        }
    }

    void AddValueChangeListener(OnGradeChanged del)
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (pdm != null)
        {
            pdm.AddFightGradeChangeListener(del);
        }
    }

    public void InitListeners()
    {
        //战斗力特效;
        AddValueChangeListener(setGradeChangeData);
    }

    void setGradeChangeData(uint oldVal, uint newVal, bool isMainCity)
    {
        mOldVal = oldVal;
        mNewVal = newVal;
        mIsMainCity = isMainCity;

        //是在主城，直接播放动画;
        if (mIsMainCity && !WindowManager.Instance.IsOpen("loading"))
        {
            CheckAndPlayGradeChangeEff();
        }
    }

    public void ResetData()
    {
        mOldVal = 0;
        mNewVal = 0;
    }

    public void CheckAndPlayGradeChangeEff()
    {
        if (mOldVal == mNewVal)
            return;

        if (SceneManager.Instance.GetCurScene().getType() == SceneType.SceneType_City)
        {
            gradeChangeEffect(mOldVal, mNewVal);
            ResetData();
        }
    }

    void gradeChangeEffect(uint oldVal, uint newVal)
    {
        GameObject go = WindowManager.Instance.CloneCommonUI("FightGradeUI");

        go.name = "gradeEffect";
        go.transform.localPosition = new Vector3(0f, 200f, 0f);
        WindowManager.Instance.SetDepth(go, 1000);

        FightGradeUI itemUI = new FightGradeUI(go);

        itemUI.SetData(oldVal, newVal);
    }
}
