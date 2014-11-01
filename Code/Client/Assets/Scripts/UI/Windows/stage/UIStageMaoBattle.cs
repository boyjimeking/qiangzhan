using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class UIStageMaoBattle : UIWindow
{
    private UILabel mCurGoldNum;
    private UILabel mTotalGoldNum;
//    private UISlider mAngerValue;
//    private UILabel mTimeCountLabel;
//    private UISprite mFace;
//    private UISprite mFaceCrazy;
//    private GameObject mAngerEffect;
//    private GameObject mCrazyBeginAni;
//    private GameObject mCrazyEndAni;
    private UISprite mScore;
    private GameObject mFlySocre;
    private GameObject mTargetCatArrow;
    private GameObject mTarget;

    public UIStageMaoBattle()
    {
    }
    
    protected override void OnLoad()
    {
        base.OnLoad();

        mTarget = FindChild("TargetUI");
        mTargetCatArrow = FindChild("TargetUI/mArrow");
        mFlySocre = FindChild("flygold");
        mScore = FindComponent<UISprite>("gold");
//        mTimeCountLabel = FindComponent<UILabel>("CountDownUI/Time");
        mCurGoldNum = FindComponent<UILabel>("gold/num");
        mTotalGoldNum = FindComponent<UILabel>("gold/total");
//         mAngerValue = FindComponent<UISlider>("AngerContainer/anger/progress");
//         mFace = FindComponent<UISprite>("AngerContainer/anger/Face1");
//         mFaceCrazy = FindComponent<UISprite>("AngerContainer/anger/Face");
//         mAngerEffect = FindChild("AngerContainer/anger/doubimao");
//         mCrazyBeginAni = FindChild("AngerContainer/anger/begin");
//         mCrazyEndAni = FindChild("AngerContainer/anger/end");

//         mCrazyBeginAni.GetComponent<UIPlayTween>().resetOnPlay = true;
//         mCrazyEndAni.GetComponent<UIPlayTween>().resetOnPlay = true;
//         mCurGoldNum.GetComponent<UIPlayTween>().resetOnPlay = true;

//        mAngerValue.value = 0;
        mCurGoldNum.text = "0";
		mTotalGoldNum.text = GameConfig.MaoMaxGoldCount.ToString();// "300";
    }
 
    protected override void OnOpen(object param = null)
    {
        mTarget.SetActive(false);
//         mCrazyBeginAni.gameObject.SetActive(false);
//         mCrazyEndAni.gameObject.SetActive(false);

//        mAngerValue.value = 0;
        mCurGoldNum.text = "0";
//        mTotalGoldNum.text = "300";

//        Crzay(false);

//        EventSystem.Instance.addEventListener(MaoStageUpdageAngerEvent.MAO_STAGE_UPDAGE_ANGER_EVENT, UpdateAnger);
        EventSystem.Instance.addEventListener(MaoStageUpdateGoldEvent.MAO_STAGE_UPDATE_GOLD_EVENT, UpdateGold);
//        EventSystem.Instance.addEventListener(MaoStageCrazyEvent.MAO_STAGE_CRAZY_EVENT, OnCrzay);
        EventSystem.Instance.addEventListener(MaoStageUpdateTargetPosEvent.MAO_STAGE_UPDATE_TARGET_POS_EVENT, OnUpdateTargetPos);
    }

    protected override void OnClose()
    {
//        EventSystem.Instance.removeEventListener(MaoStageUpdageAngerEvent.MAO_STAGE_UPDAGE_ANGER_EVENT, UpdateAnger);
        EventSystem.Instance.removeEventListener(MaoStageUpdateGoldEvent.MAO_STAGE_UPDATE_GOLD_EVENT, UpdateGold);
//        EventSystem.Instance.removeEventListener(MaoStageCrazyEvent.MAO_STAGE_CRAZY_EVENT, OnCrzay);
        EventSystem.Instance.removeEventListener(MaoStageUpdateTargetPosEvent.MAO_STAGE_UPDATE_TARGET_POS_EVENT, OnUpdateTargetPos);
    }

    public override void Update(uint elapsed)
    {
//         float tmpTime = SceneManager.Instance.GetLastTime();
//         mTimeCountLabel.text = TimeUtilities.GetCountDown(tmpTime);
    }

//     private void UpdateAnger(EventBase e)
//     {
//         MaoStageUpdageAngerEvent et = e as MaoStageUpdageAngerEvent;
//         if (et == null)
//             return;
// 
//         if(et.Value > 1.0f)
//         {
//             mAngerValue.value = 1.0f;
//         }
//         else
//         {
//             mAngerValue.value = et.Value;
//         }
//     }

    private void UpdateGold(EventBase e)
    {
        MaoStageUpdateGoldEvent et = e as MaoStageUpdateGoldEvent;
        if (et == null)
            return;

        mCurGoldNum.text = et.CurrentGold.ToString();
        mTotalGoldNum.text = et.TotalGold.ToString();
        mCurGoldNum.GetComponent<UIPlayTween>().Play(true);
        FlyScore(et.PickPos);
    }

    private void FlyScore(Vector3 pos)
    {
        if (CameraController.Instance == null)
            return;

        if (WindowManager.current2DCamera == null)
            return;

        Vector3 fromPos = CameraController.Instance.WorldToScreenPoint(pos);
        fromPos.z = 0.0f;
        fromPos = WindowManager.current2DCamera.ScreenToWorldPoint(fromPos);

        GameObject flyScore = (GameObject)GameObject.Instantiate(mFlySocre);
        flyScore.SetActive(true);
        flyScore.transform.parent = mScore.transform.parent;
        flyScore.transform.localPosition = Vector3.zero;
        flyScore.transform.localRotation = Quaternion.identity;
        flyScore.transform.localScale = Vector3.one;

        FlyEffect eff = flyScore.AddMissingComponent<FlyEffect>();
        eff.Play(fromPos, mScore.transform.localPosition, 0.5f);
    }

//     private void OnCrzay(EventBase e)
//     {
//         MaoStageCrazyEvent ev = e as MaoStageCrazyEvent;
//         if (ev == null)
//             return;
// 
//         Crzay(ev.Crazy);
// 
//         if(ev.Crazy)
//         {
//             mCrazyBeginAni.SetActive(true);
//             mCrazyEndAni.SetActive(false);
//             mCrazyBeginAni.GetComponent<UIPlayTween>().Play(true);
//         }
//         else
//         {
//             mCrazyBeginAni.SetActive(false);
//             mCrazyEndAni.SetActive(true);
//             mCrazyEndAni.GetComponent<UIPlayTween>().Play(true);
//         }
//     }
// 
//     private void Crzay(bool crazy)
//     {
//         if(crazy)
//         {
//             mAngerEffect.SetActive(true);
//             mFace.gameObject.SetActive(false);
//             mFaceCrazy.gameObject.SetActive(true);
//         }
//         else
//         {
//             mAngerEffect.SetActive(false);
//             mFace.gameObject.SetActive(true);
//             mFaceCrazy.gameObject.SetActive(false);
//         }
//     }

    private void OnUpdateTargetPos(EventBase e)
    {
        MaoStageUpdateTargetPosEvent ev = e as MaoStageUpdateTargetPosEvent;
        if (ev == null)
            return;

        if (WindowManager.current2DCamera == null)
            return;

        if (CameraController.Instance == null)
            return;

        mTarget.SetActive(true);

        Vector3 pos = CameraController.Instance.WorldToScreenPoint(ev.CatPos);
        pos.z = 0.0f;
        pos = WindowManager.current2DCamera.ScreenToWorldPoint(pos);

        Vector3 dir = pos;
        dir.Normalize();
        if (dir.magnitude < 0.000001f)
            return;

        mTargetCatArrow.transform.localRotation = Quaternion.FromToRotation(new Vector3(0.0f, 1.0f, 0.0f), dir);
    }
}
