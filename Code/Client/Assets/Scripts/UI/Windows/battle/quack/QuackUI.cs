using UnityEngine;
using System.Collections;

public class QuackUI
{
    private static int QuackType_Normal = 1;
    private static int QuackType_Critical = 2;
    private static int QuackType_Player_Hit = 3;
    private static int QuackType_Player_Reply = 4;
	private static int QuackType_BuffIcon = 5;

	public UILabel[] mLabelContainer = new UILabel[5];

    public UISprite mBuffIcon = null;

    private Vector3 pressed = new Vector3(0f, 150f);

    private TweenAlpha alphaCom = null;
    private TweenScale scaleCom = null;

    private GameObject mObj = null;

    private QuackNumberTableItem mResItem = null;

    private bool mEnd = false;
    private bool mBeginMoveing = false;
    private float mXWeaken = 0.0f;
    private float mYWeaken = 0.0f;

    private float mXSpeed = 0.0f;
    private float mYSpeed = 0.0f;

    private float mLifeTime = 0.0f;

    private Vector3 mTargetPos = new Vector3();

    private QuackUIManager manager = null;

    private bool mScaleMax = false;

    private UIWidget mActiveWidget = null;
    public QuackUI(GameObject obj)
    {
        mObj = obj;
		for (int i = 0; i < 5; ++i)
		{
			mLabelContainer[i] = ObjectCommon.GetChildComponent<UILabel>(mObj, "Label" + (i + 1).ToString());
		}

		mBuffIcon = ObjectCommon.GetChildComponent<UISprite>(mObj, "Label6");

		if (mBuffIcon == null)
			GameDebug.LogError("invalid label 6");
    }

    public GameObject gameObject
    {
        get
        {
            return mObj;
        }
    }

	public void Reset(QuackUIManager mng, Vector3 pos, int deltaProp, bool isMana, bool dead, bool isPlayer = false, bool critical = false, string bmpPath = null)
    {
        manager = mng;
		mActiveWidget = null;
        int quackType = QuackType_Normal;
		if (!string.IsNullOrEmpty(bmpPath))
		{
			mActiveWidget = mBuffIcon;
			UIAtlasHelper.SetSpriteImage(mBuffIcon, bmpPath, true);
			deactiveBut(-1);
			quackType = QuackType_BuffIcon;
		}
		else if (isPlayer)
		{
			if (deltaProp > 0)
			{
				if (isMana)
				{
					mActiveWidget = mLabelContainer[4];
					(mActiveWidget as UILabel).text = "+";
					deactiveBut(4);
				}
				else
				{
                    mActiveWidget = mLabelContainer[3];
					(mActiveWidget as UILabel).text = "+";
					deactiveBut(3);
				}
                quackType = QuackType_Player_Reply;
			}
			else
			{
				mActiveWidget = mLabelContainer[1];
				(mActiveWidget as UILabel).text = "";
				deactiveBut(1);
                quackType = QuackType_Player_Hit;

			}
		}
		else if (critical)
		{
			mActiveWidget = mLabelContainer[2];
			(mActiveWidget as UILabel).text = dead ? "d" : "b";
			deactiveBut(2);
            quackType = QuackType_Critical;
		}
		else
		{
			mActiveWidget = mLabelContainer[0];
			(mActiveWidget as UILabel).text = dead ? "d" : "";
			deactiveBut(0);
		}

        if( !DataManager.QuackNumberTable.ContainsKey( quackType ) )
        {
            onEnd();
            return;
        }

        onBegin();

        mResItem = DataManager.QuackNumberTable[quackType] as QuackNumberTableItem;


		NGUITools.SetActive(mActiveWidget.gameObject, true);

        deltaProp = Mathf.Abs(deltaProp);

        gameObject.transform.position = WindowManager.current2DCamera.ScreenToWorldPoint(pos);
        gameObject.transform.localScale = Vector3.zero;

		mActiveWidget.alpha = 1.0f;


        Vector3 targetPos = gameObject.transform.localPosition;

        targetPos.x += mResItem.horizontal_offset;
        targetPos.y += mResItem.vertical_offset;

        gameObject.transform.localPosition = targetPos;

		if (mActiveWidget is UILabel)
			(mActiveWidget as UILabel).text += deltaProp.ToString();

        if (mResItem.scale != 1.0f && mResItem.scaletime > 0.0f)
        {
            scaleCom = TweenScale.Begin(gameObject, mResItem.scaletime / 1000.0f, new Vector3(mResItem.scale, mResItem.scale, mResItem.scale));
            scaleCom.SetOnFinished(onScaleFinished);
            scaleCom.method = UITweener.Method.EaseInOut;
            scaleCom.PlayForward();
        }else
        {
            onScaleFinished2();
        }
    }

	void deactiveBut(int pos)
	{
		for (int i = 0; i < mLabelContainer.Length; ++i)
		{
			if (i != pos)
				NGUITools.SetActive(mLabelContainer[i].gameObject, false);
		}

		if (pos != -1)
			NGUITools.SetActive(mBuffIcon.gameObject, false);
	}

    public void Update(uint elapsed)
    {
        if (mBeginMoveing)
        {
            float el = (float)elapsed / 1000.0f;
            mLifeTime -= el;
            if( mLifeTime <= 0.0f )
            {
                onEnd();
                return ;
            }

			mActiveWidget.alpha = mLifeTime / (mResItem.lifetime / 1000.0f);

            mTargetPos.x += el * mXSpeed;
            mTargetPos.y += el * mYSpeed;

            mXSpeed += ( mXWeaken * el);
            mYSpeed += (mYWeaken * el);

            gameObject.transform.localPosition = mTargetPos;
        }

        if( mScaleMax )
        {
            scaleCom = TweenScale.Begin(gameObject, mResItem.scaletime / 1000.0f, Vector3.one);
            scaleCom.SetOnFinished(onScaleFinished2);
            scaleCom.method = UITweener.Method.EaseInOut;
            scaleCom.PlayForward();

            mScaleMax = false;
        }
    }

    void onScaleFinished2()
    {
        mXSpeed = UnityEngine.Random.Range(mResItem.horizontal_min, mResItem.horizontal_max);
        mYSpeed = UnityEngine.Random.Range(mResItem.vertical_min, mResItem.vertical_max);

        mLifeTime = mResItem.lifetime / 1000.0f;

        mTargetPos = gameObject.transform.localPosition;

        mXWeaken = -(mResItem.horizontal_weaken * mXSpeed);
        mYWeaken = -(mResItem.vertical_weaken * mYSpeed);

        mBeginMoveing = true;
    }
    void onScaleFinished()
    {
        mScaleMax = true;
    }

    void onBegin()
    {
        mEnd = false;
        mBeginMoveing = false;
        mScaleMax = false;
    }
    void onEnd()
    {
        mEnd = true;
        mBeginMoveing = false;
        manager.FreeUI(this);
        mScaleMax = false;
    }

    public bool IsEnd()
    {
        return mEnd;
    }
}
