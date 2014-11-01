using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStageEnd : UIWindow
{
#region 界面控件
	//标题面板
    public UISprite mTopPanel;
    //"评分"标题
    public UISprite mTitle0;
    //"成绩"标题
    public UISprite mTitle1;

    //"杀伤率""连击""评分"面板 
    public UIPanel mInfoPanel;

    //"杀伤率"面板
    public UISprite mInfoPanel0;
    //"杀伤率"值
    public UILabel mInfoValue0;
    //"杀伤率"新纪录面板
    public GameObject mInfoRecord0;
    //"杀伤率"新纪录值
    public UILabel mInfoRecordValue0;

    //"连击"面板
    public UISprite mInfoPanel1;
    //"连击"值
    public UILabel mInfoValue1;
    //"连击"新纪录面板
    public GameObject mInfoRecord1;
    //"连击"新纪录值
    public UILabel mInfoRecordValue1;

    //"评分"面板
    public UISprite mInfoPanel2;
    //"评分"值
    public UILabel mInfoValue2;
    //"评分"新纪录面板
    public GameObject mInfoRecord2;
    //"评分"新纪录值
    public UILabel mInfoRecordValue2;

    //评分球
    public UISprite mBallBk;
    //"S"评分
	public UITweener mGradeS;
    //"A"评分
	public UITweener mGradeA;
    //"B"评分
	public UITweener mGradeB;
    //"C"评分
	public UITweener mGradeC;
	
    //底部面板
    public UISprite mBottomPanel;
    //经验值
    public UILabel mExpValue;
    //百分比
    public UILabel mProgressValue;
    //升级标志
    public GameObject mLevelUp;
    //进度条
    public UIProgressBar mProgress;

	private UISprite mBackground;

    //private UISprite mUIPanel;

    public UIButton mUIBtn;

    private UIPlayTween mLevelUpAnimation;

	private UIWidget mWidget;

	private UISpriteAnimation mGradeAni;
	private UISpriteAnimation mInfoAni0;
	private UISpriteAnimation mInfoAni1;
	private UISpriteAnimation mInfoAni2;
	private UISpriteAnimation mBorderAni;
#endregion

	private StageEndModule mModule = ModuleManager.Instance.FindModule<StageEndModule>();

	private PlayerDataModule mPlayerModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

    private float mTimer = 0.0f;

    // 信息面板初始位置
    private float mInfoPosY = 111.0f;
    // 评价球初始位置
    private float mBallPosY = -160.0f;

	private int mCurExp = 0;
	private int mAddExp = 0;
	private int mMaxExp = 0;
	private int mExpSpeed = 0;

    private Dictionary<StageGrade, UITweener> mGradeMap = null;

    protected override void OnLoad()
    {
        mTopPanel = this.FindComponent<UISprite>("Widget/mTopPanel");
        mTitle0 = this.FindComponent<UISprite>("Widget/mTopPanel/mTitle0");
        mTitle1 = this.FindComponent<UISprite>("Widget/mTopPanel/mTitle1");

        mInfoPanel = this.FindComponent<UIPanel>("Widget/mInfoPanel");
        mInfoPanel0 = this.FindComponent<UISprite>("Widget/mInfoPanel/mInfoPanel0");
        mInfoValue0 = this.FindComponent<UILabel>("Widget/mInfoPanel/mInfoPanel0/mInfoValue0");
        mInfoRecord0 = this.FindChild("Widget/mInfoPanel/mInfoPanel0/mInfoRecord0");
        mInfoRecordValue0 = this.FindComponent<UILabel>("Widget/mInfoPanel/mInfoPanel0/mInfoRecord0/mInfoRecordValue0");

        mInfoPanel1 = this.FindComponent<UISprite>("Widget/mInfoPanel/mInfoPanel1");
        mInfoValue1 = this.FindComponent<UILabel>("Widget/mInfoPanel/mInfoPanel1/mInfoValue1");
        mInfoRecord1 = this.FindChild("Widget/mInfoPanel/mInfoPanel1/mInfoRecord1");
        mInfoRecordValue1 = this.FindComponent<UILabel>("Widget/mInfoPanel/mInfoPanel1/mInfoRecord1/mInfoRecordValue1");

        mInfoPanel2 = this.FindComponent<UISprite>("Widget/mInfoPanel/mInfoPanel2");
        mInfoValue2 = this.FindComponent<UILabel>("Widget/mInfoPanel/mInfoPanel2/mInfoValue2");
        mInfoRecord2 = this.FindChild("Widget/mInfoPanel/mInfoPanel2/mInfoRecord2");
        mInfoRecordValue2 = this.FindComponent<UILabel>("Widget/mInfoPanel/mInfoPanel2/mInfoRecord2/mInfoRecordValue2");

        mBallBk = this.FindComponent<UISprite>("Widget/mGradePanel/mBallBk");
		mGradeS = this.FindComponent<UITweener>("Widget/mGradePanel/mBallBk/mGradeS");
		mGradeA = this.FindComponent<UITweener>("Widget/mGradePanel/mBallBk/mGradeA");
		mGradeB = this.FindComponent<UITweener>("Widget/mGradePanel/mBallBk/mGradeB");
		mGradeC = this.FindComponent<UITweener>("Widget/mGradePanel/mBallBk/mGradeC");
        mBottomPanel = this.FindComponent<UISprite>("Widget/mBottomPanel");

        mExpValue = this.FindComponent<UILabel>("Widget/mBottomPanel/mExpValue");
        mProgressValue = this.FindComponent<UILabel>("Widget/mBottomPanel/mProgressValue");
        mLevelUp = this.FindChild("Widget/mBottomPanel/mLevelUp");
        mProgress = this.FindComponent<UIProgressBar>("Widget/mBottomPanel/mProgress");
        mUIBtn = this.FindComponent<UIButton>("Widget/mUIBtn");

		mBackground = this.FindComponent<UISprite>("Widget/mBackground");

		mGradeAni = this.FindComponent<UISpriteAnimation>("Widget/mGradePanel/mBallBk/mGradeAni");
		mInfoAni0 = this.FindComponent<UISpriteAnimation>("Widget/mInfoPanel/mPanel0/mAni0");
		mInfoAni1 = this.FindComponent<UISpriteAnimation>("Widget/mInfoPanel/mPanel1/mAni1");
		mInfoAni2 = this.FindComponent<UISpriteAnimation>("Widget/mInfoPanel/mPanel2/mAni2");
        mInfoAni0.framesPerSecond = 15;
        mInfoAni1.framesPerSecond = 15;
        mInfoAni2.framesPerSecond = 15;
		mBorderAni = this.FindComponent<UISpriteAnimation>("Widget/mAni");

        //mUIPanel = this.FindComponent<UISprite>("Widget");
		mWidget = this.FindComponent<UIWidget>("Widget");

        mLevelUpAnimation = mLevelUp.GetComponent<UIPlayTween>();
        mLevelUpAnimation.resetOnPlay = true;
    }
    //界面打开
    protected override void OnOpen(object param = null)
    {
        EventDelegate.Add(mUIBtn.onClick, OnUIClicked);
        Init();
    }
    //界面关闭
    protected override void OnClose()
    {
        EventDelegate.Remove(mUIBtn.onClick, OnUIClicked);

    }
    private void Init()
    {
        mModule.State = StageEndModule.UIState.STATE_ORIGINAL;
        mLevelUp.SetActive(false);
        mGradeS.gameObject.SetActive(false);
		mGradeS.ResetToBeginning();
		mGradeA.gameObject.SetActive(false);
		mGradeA.ResetToBeginning();
		mGradeB.gameObject.SetActive(false);
		mGradeB.ResetToBeginning();
		mGradeC.gameObject.SetActive(false);
		mGradeC.ResetToBeginning();
        mTitle0.alpha = 0.0f;
        mTitle1.alpha = 0.0f;
        mBottomPanel.alpha = 0.0f;
        mBallBk.alpha = 0.0f;
        mInfoPanel0.alpha = 0.0f;
        mInfoPanel1.alpha = 0.0f;
        mInfoPanel2.alpha = 0.0f;
        mTopPanel.alpha = 0.0f;
		mBackground.alpha = 0.6f;
        mInfoPanel.gameObject.transform.localPosition = new Vector3(mInfoPanel.gameObject.transform.localPosition.x, mInfoPosY, mInfoPanel.gameObject.transform.localPosition.z);
        mBallBk.gameObject.transform.localPosition = new Vector3(mBallBk.gameObject.transform.localPosition.x, mBallPosY, mBallBk.gameObject.transform.localPosition.z);
        mBallBk.gameObject.transform.localScale = Vector3.one;
		mWidget.topAnchor.relative = 2.0f;
		mWidget.bottomAnchor.relative = 1.0f;

		mCurExp = mPlayerModule.GetExp();
		mAddExp = mModule.GetExp();
		mMaxExp = GetMaxExp();
		mExpSpeed = (int)(mAddExp / StageEndModule.STATETIME_9);
		UpdateExp();

		mGradeAni.Reset();
		mGradeAni.gameObject.SetActive(false);
		mInfoAni0.Reset();
		mInfoAni0.gameObject.SetActive(false);
		mInfoAni1.Reset();
		mInfoAni1.gameObject.SetActive(false);
		mInfoAni2.Reset();
		mInfoAni2.gameObject.SetActive(false);
		mBorderAni.Reset();
		mBorderAni.gameObject.SetActive(false);

        //mUIPanel.alpha = 0.0f;
 
        mTimer = 0.0f;

        mInfoValue2.text = mModule.GetPassTimeStr();
        mInfoRecordValue2.text = mInfoValue2.text;

        if (mGradeMap == null)
        {
            mGradeMap = new Dictionary<StageGrade, UITweener>();
            mGradeMap.Add(StageGrade.StageGrade_S, mGradeS);
            mGradeMap.Add(StageGrade.StageGrade_A, mGradeA);
            mGradeMap.Add(StageGrade.StageGrade_B, mGradeB);
            mGradeMap.Add(StageGrade.StageGrade_C, mGradeC);
        }
    }

    private void OnUIClicked()
    {
        if (mModule == null)
        {
            return;
        }

        if (mModule.State == StageEndModule.UIState.STATE_6)
        {
            mTimer = 0.0f;
            mModule.State = StageEndModule.UIState.STATE_7;
        }
        else if (mModule.State == StageEndModule.UIState.STATE_10)
        {
            mTimer = 0.0f;
            mModule.State = StageEndModule.UIState.STATE_11;

			EventSystem.Instance.PushEvent(new StageEndUIEvent(StageEndUIEvent.STAGE_END_FINISH));
        }
    }
    public override void Update(uint elapsed)
    {
        if (mModule == null)
        {
            return;
        }

        //float delta = Time.unscaledDeltaTime;
		mTimer += (float)(elapsed * 0.001);// delta;

        switch (mModule.State)
        {
            case StageEndModule.UIState.STATE_ORIGINAL:
                {
                    mTimer = 0.0f;
					mBorderAni.gameObject.SetActive(true);
					mBorderAni.onFinished += OnAniFinished;
                    mModule.State = StageEndModule.UIState.STATE_0;
                }
                break;
            // 界面从上到下大特效
            case StageEndModule.UIState.STATE_0:
                {
					if (mWidget != null)
                    {
                        if (mTimer > StageEndModule.STATETIME_0)
                        {
                            //mUIPanel.alpha = 1.0f;
							mWidget.topAnchor.relative = 1.0f;
							mWidget.bottomAnchor.relative = 0.0f;

                            mTimer -= StageEndModule.STATETIME_0;
                            mModule.State = StageEndModule.UIState.STATE_1;
                        }
                        else
                        {
							mWidget.topAnchor.relative = 2.0f - (mTimer / StageEndModule.STATETIME_0);
							mWidget.bottomAnchor.relative = 1.0f - (mTimer / StageEndModule.STATETIME_0);

                            //mUIPanel.alpha = mTimer / StageEndModule.STATETIME_0;
                        }
                    }
                }
                break;
            case StageEndModule.UIState.STATE_1:
                {
                    if (mTimer > StageEndModule.STATETIME_1)
                    {
                        mTitle0.alpha = 1.0f;
                        mTopPanel.alpha = 1.0f;
						mInfoAni0.gameObject.SetActive(true);
                        SoundManager.Instance.Play(105);
						mInfoAni0.onFinished += OnAniFinished;
                        mTimer -= StageEndModule.STATETIME_1;
                        mModule.State = StageEndModule.UIState.STATE_2;
                    }
                    else
                    {
                        float alpha = mTimer / StageEndModule.STATETIME_1;
                        mTopPanel.alpha = alpha;
                        mTitle0.alpha = alpha;
                    }
                }
                break;
            case StageEndModule.UIState.STATE_2:
                {
                    if (mTimer > StageEndModule.STATETIME_2)
                    {
                        mInfoPanel0.alpha = 1.0f;
						mInfoAni1.gameObject.SetActive(true);
                        SoundManager.Instance.Play(105);
						mInfoAni1.onFinished += OnAniFinished;
                        mTimer -= StageEndModule.STATETIME_2;
                        mModule.State = StageEndModule.UIState.STATE_3;
                    }
                    else
                    {
                        mInfoPanel0.alpha = mTimer / StageEndModule.STATETIME_2;
                    }
                }
                break;
            case StageEndModule.UIState.STATE_3:
                {
                    if (mTimer > StageEndModule.STATETIME_3)
                    {
                        mInfoPanel1.alpha = 1.0f;
						mInfoAni2.gameObject.SetActive(true);
                        SoundManager.Instance.Play(105);
						mInfoAni2.onFinished += OnAniFinished;
                        mTimer -= StageEndModule.STATETIME_3;
                        mModule.State = StageEndModule.UIState.STATE_4;
                    }
                    else
                    {
                        mInfoPanel1.alpha = mTimer / StageEndModule.STATETIME_3;
                    }
                }
                break;
            case StageEndModule.UIState.STATE_4:
                {
                    if (mTimer > StageEndModule.STATETIME_4)
                    {
                        mInfoPanel2.alpha = 1.0f;
                        mTimer -= StageEndModule.STATETIME_4;
                        mModule.State = StageEndModule.UIState.STATE_5;
                    }
                    else
                    {
                        mInfoPanel2.alpha = mTimer / StageEndModule.STATETIME_4;
                    }
                }
                break;
            case StageEndModule.UIState.STATE_5:
                {
                    if (mTimer > StageEndModule.STATETIME_5)
                    {
                        mBallBk.alpha = 1.0f;

                        UITweener gradeObj = mGradeMap[mModule.GetGrade()];
                        gradeObj.gameObject.SetActive(true);
						gradeObj.Play();

						mGradeAni.gameObject.SetActive(true);
						mGradeAni.onFinished += OnAniFinished;

                        mTimer = 0.0f;
                        mModule.State = StageEndModule.UIState.STATE_6;
                    }
                    else
                    {
                        mBallBk.alpha = mTimer / StageEndModule.STATETIME_5;
                    }
                }
                break;
            case StageEndModule.UIState.STATE_6:
                {
                    if (mTimer > StageEndModule.STATETIME_6)
                    {
                        mTimer = 0.0f;
                        mModule.State = StageEndModule.UIState.STATE_7;
                    }
                }
                break;
            case StageEndModule.UIState.STATE_7:
                {
                    if (mTimer > StageEndModule.STATETIME_7)
                    {
                        mInfoPanel0.alpha = 0.0f;
                        mInfoPanel1.alpha = 0.0f;
                        mInfoPanel2.alpha = 0.0f;
                        mTitle0.alpha = 0.0f;

                        mTimer -= StageEndModule.STATETIME_7;
                        mModule.State = StageEndModule.UIState.STATE_8;
                    }
                    else
                    {
                        float alpha = 1.0f - (mTimer / StageEndModule.STATETIME_7);
                        mInfoPanel0.alpha = alpha;
                        mInfoPanel1.alpha = alpha;
                        mInfoPanel2.alpha = alpha;
                        mTitle0.alpha = alpha;
                        mInfoPanel.gameObject.transform.localPosition = new Vector3(mInfoPanel.gameObject.transform.localPosition.x,
                            mInfoPosY + (164.0f * mTimer / StageEndModule.STATETIME_7), mInfoPanel.gameObject.transform.localPosition.z);
                        mBallBk.gameObject.transform.localPosition = new Vector3(mBallBk.gameObject.transform.localPosition.x,
                            mBallPosY + (327.0f * mTimer / StageEndModule.STATETIME_7), mBallBk.gameObject.transform.localPosition.z);
                        mBallBk.gameObject.transform.localScale = new Vector3(1.0f - 0.2f * (mTimer / StageEndModule.STATETIME_7), 1.0f - 0.2f * (mTimer / StageEndModule.STATETIME_7), 1.0f);
                    }
                }
                break;
            case StageEndModule.UIState.STATE_8:
                {
                    if (mTimer > StageEndModule.STATETIME_8)
                    {
                        mTitle1.alpha = 1.0f;
                        mBottomPanel.alpha = 1.0f;
                        mTimer -= StageEndModule.STATETIME_8;
                        mModule.State = StageEndModule.UIState.STATE_9;
                    }
                    else
                    {
                        float alpha = mTimer / StageEndModule.STATETIME_8;
                        mBottomPanel.alpha = alpha;
                        mTitle1.alpha = alpha;
                    }
                }
                break;
            case StageEndModule.UIState.STATE_9:
                {
//                     if(ModuleManager.Instance.FindModule<PlayerDataModule>().IsLevelUp())
//                     {
//                         mLevelUp.SetActive(true);
//                         mLevelUpAnimation.Play(true);
//                     }

					if(mAddExp > 0)
					{
						int addExp = (int)(mTimer * mExpSpeed);
						
						if (addExp < 1)
							addExp = 1;

						if (addExp > mAddExp)
							addExp = mAddExp;

						mAddExp -= addExp;

						if (mCurExp + addExp > mMaxExp)
						{
							bool playLvUp = false;
							int level = mPlayerModule.GetLevel();
							do
							{
								if (!DataManager.LevelTable.ContainsKey(level + 1))
								{
									mCurExp = mMaxExp;
									UpdateExp();
									mTimer = 0.0f;
									mModule.State = StageEndModule.UIState.STATE_10;
									break;
								}

								playLvUp = true;
								mCurExp += addExp - mMaxExp;
								addExp = mMaxExp - mCurExp;

								LevelTableItem nextLvRes = DataManager.LevelTable[level + 1] as LevelTableItem;
								mMaxExp = nextLvRes.exp;
								level++;
							}
							while (mCurExp + addExp > mMaxExp);

							if (playLvUp)
							{
								mLevelUp.SetActive(true);
								mLevelUpAnimation.Play(true);
							    SoundManager.Instance.Play(7);
							}
						}
						else
						{
							mCurExp += addExp;
						}

						UpdateExp();

						if(mAddExp < 1)
						{
							mTimer = 0.0f;
							mModule.State = StageEndModule.UIState.STATE_10;
						}
					}
					else
					{
						mTimer = 0.0f;
						mModule.State = StageEndModule.UIState.STATE_10;
					}
                }
                break;
            case StageEndModule.UIState.STATE_10:
                {
                    if (mTimer > StageEndModule.STATETIME_10)
                    {
                        mTimer = 0.0f;
                        mModule.State = StageEndModule.UIState.STATE_11;

						EventSystem.Instance.PushEvent(new StageEndUIEvent(StageEndUIEvent.STAGE_END_FINISH));
                    }
                }
                break;
            case StageEndModule.UIState.STATE_11:
                {

                }
                break;
            default:
                break;
        }
    }

	private void OnAniFinished(GameObject go)
	{
		go.SetActive(false);
	}

	private int GetMaxExp()
	{
		int level = mPlayerModule.GetLevel();
		if(!DataManager.LevelTable.ContainsKey(level))
		{
			return 0;
		}

		LevelTableItem res = DataManager.LevelTable[level] as LevelTableItem;
		if(res == null)
		{
			return 0;
		}

		return res.exp;
	}

	private void UpdateExp()
	{
		mExpValue.text = mCurExp.ToString();
		mProgressValue.text = (mCurExp * 100 / mMaxExp).ToString() + "%";
		mProgress.value = (float)mCurExp / (float)mMaxExp;
	}
}
