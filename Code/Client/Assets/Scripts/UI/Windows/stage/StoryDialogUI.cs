using UnityEngine;
public class StoryDialogUI
{
	// Widget对话
	public UIWidget mWidget;

	// 头像
	public UISprite mHead;
	public UIAnchor mHeadAnchor;

	// 标题正文按钮
	public UIAnchor mAnchor;

	// 对话标题
	public UILabel mTitle;

	// 标题打字机
	public GameObject mTitleEffect;
	//public TypewriterEffect mTitleEffect;

	// 对话正文
	public UILabel mContent;

	// 正文打字机
	public GameObject mContentEffect;
	//public TypewriterEffect mContentEffect;

	// 按钮
	public UISprite mBtn;

	// UI阶段
	private enum UIState : int
	{
		// 原始状态
		STATE_ORIGINAL = 0,

		// fadein
		STATE_0,

		// 等待打字机
		STATE_1,

		// 显示按钮 等待2秒
		STATE_2,

		// 等待消息
		STATE_3,

		// fadeout
		STATE_4,

		// 隐藏
		STATE_5,
	}

	// 当前状态
	private UIState mState = UIState.STATE_ORIGINAL;

	// 计时
	private float mTimer = 0.0f;

	// 表
	private StoryTableItem mRes = null;

	// 最长等待时间
	private float mStepWaitTime = 0.0f;

    public StoryDialogUI()
    {

    }

	public void Open()
	{
        mWidget.alpha = 0.0f;

	}

    public void Update(uint elapsed)
	{
		//float delta = Time.unscaledDeltaTime;
		mTimer += (float)(elapsed * 0.001);// delta;

		switch(mState)
		{
			// 原始状态
			case UIState.STATE_ORIGINAL:
				{
					mTimer = 0.0f;
				}
				break;
			// fadein
			case UIState.STATE_0:
				{
					if(mTimer > 2.0f)
					{
						Next(true);
					}
					else
					{
						mWidget.alpha = mTimer / 1.0f;
					}
				}
				break;
			// 等待打字机
			case UIState.STATE_1:
				{
					if (mStepWaitTime > 0.0f)
					{
						if (mTimer > mStepWaitTime)
						{
							Next(true);
						}
					}
				}
				break;
			// 显示按钮 等待2秒
			case UIState.STATE_2:
				{
					if(mTimer > 1.0f)
					{
						Next(true);
					}
				}
				break;
			// 等待消息
			case UIState.STATE_3:
				{

				}
				break;
			// fadeout
			case UIState.STATE_4:
				{
					if (mTimer > 1.0f)
					{
						Next(true);
					}
					else
					{
						mWidget.alpha = (1.0f - mTimer);
					}
				}
				break;
			// 隐藏
			case UIState.STATE_5:
				{

				}
				break;
			default:
				{
					
				}
				break;
		}
	}

	// 显示文字
	private void ShowText(bool showEffect)
	{
		mTitle.text = mRes.title;
		mContent.text = mRes.content;

		if(showEffect)
		{
			StartEffect();
		}
	}

	// 启动打字机
	private void StartEffect()
	{
		TypewriterEffect title = mTitleEffect.GetComponent("TypewriterEffect") as TypewriterEffect;
		if (title != null)
		{
			title.ReStart();
		}
		else
		{
			mTitleEffect.AddComponent("TypewriterEffect");
		}

		TypewriterEffect content = mContentEffect.GetComponent("TypewriterEffect") as TypewriterEffect;
		if (content != null)
		{
			content.ReStart();
		}
		else
		{
			mContentEffect.AddComponent("TypewriterEffect");
		}
	}

	// 停止打字机
	private void StopEffect()
	{
		TypewriterEffect title = mTitleEffect.GetComponent("TypewriterEffect") as TypewriterEffect;
		if(title != null)
		{
			GameObject.Destroy(title);
		}

		TypewriterEffect content = mContentEffect.GetComponent("TypewriterEffect") as TypewriterEffect;
		if(content != null)
		{
            GameObject.Destroy(content);
		}
	}

	// 更新情节
	public void UpdateStory(StoryTableItem res)
	{
		if(mRes == null)
		{
			mTimer = 0.0f;
		}
		else
		{
			mTimer = 3.0f;
		}

		mRes = res;
		mStepWaitTime = (float)(mRes.keepTime * 0.001);

		if(mRes.headAtlas.Equals("player"))
		{
			PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
			if(module != null)
			{
				UIAtlasHelper.SetSpriteImage(mHead, "touxiang:player_" + module.GetResId().ToString());
			}
		}
		else
		{
			UIAtlasHelper.SetSpriteImage(mHead, mRes.headAtlas);
		}

		if(mRes.headMirror < 1)
		{
			mHead.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
		}
		else
		{
			mHead.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
		}

// 		mTitle.text = mRes.title;
// 		mContent.text = mRes.content;
		mBtn.alpha = 0.0f;

		StopEffect();

		if(mRes.headPos == 0 || mRes.headPos == 2)
		{
			LeftMode();	
		}
		else if(mRes.headPos == 1 || mRes.headPos == 3)
		{
			RightMode();
		}
		else
		{
			MiddleMode();
		}

		mState = UIState.STATE_0;
	}

	// 左模式
	private void LeftMode()
	{
		mHeadAnchor.side = UIAnchor.Side.Left;
		mHeadAnchor.pixelOffset.x = 140.0f;

		mAnchor.side = UIAnchor.Side.Left;
		mAnchor.pixelOffset.x = 440.0f;

		mBtn.transform.localPosition = new Vector3(116.0f, mBtn.transform.localPosition.y, mBtn.transform.localPosition.z);

		mContent.width = 315;
	}

	// 右模式
	public void RightMode()
	{
		mHeadAnchor.side = UIAnchor.Side.Right;
		mHeadAnchor.pixelOffset.x = -140.0f;

		mAnchor.side = UIAnchor.Side.Right;
		mAnchor.pixelOffset.x = -440.0f;

		mBtn.transform.localPosition = new Vector3(119.0f, mBtn.transform.localPosition.y, mBtn.transform.localPosition.z);

		mContent.width = 315;
	}

	// 独白模式
	private void MiddleMode()
	{
		mAnchor.side = UIAnchor.Side.Left;
		mAnchor.pixelOffset.x = 355.0f;

		mBtn.transform.localPosition = new Vector3(360.0f, mBtn.transform.localPosition.y, mBtn.transform.localPosition.z);
		mContent.width = 670;
	}

	// 跳过
	public void Skip()
	{
		mRes = null;

		if (mState > UIState.STATE_ORIGINAL && mState < UIState.STATE_4)
		{
			mTimer = 0.0f;
			mState = UIState.STATE_4;
		}
	}

	// 下一步
	private void Next(bool isAuto)
	{
		switch(mState)
		{
			case UIState.STATE_ORIGINAL:
				{

				}
				break;
			case UIState.STATE_0:
				{
					mWidget.alpha = 1.0f;
					mTimer = 0.0f;

					ShowText(isAuto);

					mState = UIState.STATE_1;
				}
				break;
			case UIState.STATE_1:
				{
					mTimer = 0.0f;
					if (mRes.headPos == 0 || mRes.headPos == 1)
					{
						mBtn.alpha = 1.0f;
					}
					else
					{
						mBtn.alpha = 1.0f;
					}
					mState = UIState.STATE_2;
				}
				break;
			case UIState.STATE_2:
				{
					mTimer = 0.0f;

					EventSystem.Instance.PushEvent(new StoryEvent(StoryEvent.STORY_STEP_FINISH));

					mState = UIState.STATE_3;
				}
				break;
			case UIState.STATE_3:
				{

				}
				break;
			case UIState.STATE_4:
				{
					if(isAuto)
					{
						Clear();
						mState = UIState.STATE_5;
					}
				}
				break;
			case UIState.STATE_5:
				{

				}
				break;
			default:
				{

				}
				break;
		}
	}

	// 点击下一步
	public void Next()
	{
		if (mRes != null && mRes.canSkipStep)
			Next(false);
	}

	// 清理
	public void Clear()
	{
		UIAtlasHelper.SetSpriteImage(mHead, null);
		mTitle.text = null;
		mContent.text = null;
		mWidget.alpha = 0.0f;
		mTimer = 0.0f;
		mState = UIState.STATE_ORIGINAL;
	}
}
