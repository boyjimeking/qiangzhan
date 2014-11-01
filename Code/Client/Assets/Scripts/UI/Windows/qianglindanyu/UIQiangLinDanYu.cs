using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankItem
{
    public UILabel mName;
    public UILabel mScore;
}

public class UIQiangLingDanYu : UIWindow
{
    private const int MAX_RANK_COUNT = 5;

    private UILabel mScore;
    private UILabel mPlayerCount;
   // private GameObject mGetScore;得分特效暂时注掉
    private GameObject mClone;

    private RankItem[] mRank = new RankItem[MAX_RANK_COUNT]{new RankItem(), new RankItem(), new RankItem(), new RankItem(), new RankItem()};

    private bool mScoreScale = false;
    private string lastScore = "";

    //缓存列表
    private Queue mCacheQueue = new Queue();
    public UIQiangLingDanYu()
    {
    }
    
    protected override void OnLoad()
    {
      //  mGetScore = FindChild("GetScore");

        mScore = FindComponent<UILabel>("mWidget/Combo/Number");
        mScore.text = "0";

		mPlayerCount = FindComponent<UILabel>("mWidget/playercount/count");
        mPlayerCount.text = "1";

		mRank[0].mName = FindComponent<UILabel>("mWidget/No.1/Label/name");
        mRank[0].mName.text = "--";
		mRank[0].mScore = FindComponent<UILabel>("mWidget/No.1/Label/score");
        mRank[0].mScore.text = "--";

		mRank[1].mName = FindComponent<UILabel>("mWidget/No.2/Label/name");
        mRank[1].mName.text = "--";
		mRank[1].mScore = FindComponent<UILabel>("mWidget/No.2/Label/score");
        mRank[1].mScore.text = "--";

		mRank[2].mName = FindComponent<UILabel>("mWidget/No.3/Label/name");
        mRank[2].mName.text = "--";
		mRank[2].mScore = FindComponent<UILabel>("mWidget/No.3/Label/score");
        mRank[2].mScore.text = "--";

		mRank[3].mName = FindComponent<UILabel>("mWidget/No.4/Label/name");
        mRank[3].mName.text = "--";
		mRank[3].mScore = FindComponent<UILabel>("mWidget/No.4/Label/score");
        mRank[3].mScore.text = "--";

		mRank[4].mName = FindComponent<UILabel>("mWidget/No.5/Label/name");
        mRank[4].mName.text = "--";
		mRank[4].mScore = FindComponent<UILabel>("mWidget/No.5/Label/score");
        mRank[4].mScore.text = "--";
    }
 
    protected override void OnOpen(object param = null)
    {
        EventSystem.Instance.addEventListener(QiangLinDanYuUpdateEvent.QIANGLINDANYU_UPDATE_EVENT, OnUpdateInfo);
        EventSystem.Instance.addEventListener(QiangLinDanYuKillEnemyEvent.QIANGLINDANYU_KILL_ENEMY_EVENT, OnKill);
    }

    protected override void OnClose()
    {
        EventSystem.Instance.removeEventListener(QiangLinDanYuUpdateEvent.QIANGLINDANYU_UPDATE_EVENT, OnUpdateInfo);
        EventSystem.Instance.removeEventListener(QiangLinDanYuKillEnemyEvent.QIANGLINDANYU_KILL_ENEMY_EVENT, OnKill);
    }
    
    private void OnUpdateInfo(EventBase ev)
    {
        QiangLinDanYuUpdateEvent info = ev as QiangLinDanYuUpdateEvent;
        if (info == null)
            return;
        mScore.text = info.score.ToString();
        if (mScoreScale == false && lastScore != mScore.text && lastScore !="")
        {
            mScore.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            TweenScale ts = TweenScale.Begin(mScore.gameObject, 0.2f, new Vector3(1.0f, 1.0f, 1.0f));
            if (ts != null)
            {
                ts.method = UITweener.Method.EaseInOut;
                ts.PlayForward();
                ts.AddOnFinished(OnScaleFinish);

                mScoreScale = true;
            }
        }

        lastScore = info.score.ToString();
        mPlayerCount.text = info.playerCount.ToString();

        for(int i = 0; i < MAX_RANK_COUNT; i++)
        {
            if (i < info.playerCount)
            {
                mRank[i].mName.text = info.sortInfo[i].name;
                mRank[i].mScore.text = info.sortInfo[i].score.ToString();
               
            }
            else
            {
                mRank[i].mName.text = "--";
                mRank[i].mScore.text = "--";
            }
        }
    }

    void OnKill(EventBase ev)
    {
//         if (ev == null) return;
//         QiangLinDanYuKillEnemyEvent bue = ev as QiangLinDanYuKillEnemyEvent;
//         if (bue == null) return;
// 
//         Vector3 from = (Vector3)bue.msg;
//         from = CameraController.Instance.WorldToScreenPoint(from);
//         from.z = 0.0f;
//         from = WindowManager.current2DCamera.ScreenToWorldPoint(from);

//         GameObject clone = WindowManager.Instance.CloneGameObject(mGetScore);
//         GameObject.DontDestroyOnLoad(clone);
//         clone.gameObject.transform.parent = mScore.transform.parent;
//         clone.gameObject.transform.position = from;
//         clone.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
//         NGUITools.SetActive(clone.gameObject, true);
//         mCacheQueue.Enqueue(clone);
//         TweenPosition tp = TweenPosition.Begin(clone, 1.0f,mScore.transform.localPosition);
//         tp.SetOnFinished(onFinish);
//         tp.PlayForward();
    }
  private void OnScaleFinish()
  {
      mScoreScale = false;
      mScore.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
  }
  void onFinish()
  {
      if (mCacheQueue.Count > 0)
      {
          GameObject clone = mCacheQueue.Dequeue() as GameObject;
          GameObject.DestroyImmediate(clone.gameObject);
      }
  }
}