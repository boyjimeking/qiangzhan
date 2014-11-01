using UnityEngine;
public class GuideTalkDialogUI
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

    public GuideTalkDialogUI()
    {

    }

    public void Open()
    {
        mWidget.alpha = 0.0f;
    }
// 
//     public void Update(uint elapsed)
//     {
//         float delta = Time.unscaledDeltaTime;
//         mTimer += delta;
// 
//         switch (mState)
//         {
//             // 原始状态
//             case UIState.STATE_ORIGINAL:
//                 {
//                     mTimer = 0.0f;
//                 }
//                 break;
//             // fadein
//             case UIState.STATE_0:
//                 {
//                     if (mTimer > 2.0f)
//                     {
//                         Next(true);
//                     }
//                     else
//                     {
//                         mWidget.alpha = mTimer / 1.0f;
//                     }
//                 }
//                 break;
//             // 等待打字机
//             case UIState.STATE_1:
//                 {
//                     if (mTimer > 2.0f)
//                     {
//                         Next(true);
//                     }
//                 }
//                 break;
//             // 显示按钮 等待2秒
//             case UIState.STATE_2:
//                 {
//                     if (mTimer > 1.0f)
//                     {
//                         Next(true);
//                     }
//                 }
//                 break;
//             // 等待消息
//             case UIState.STATE_3:
//                 {
// 
//                 }
//                 break;
//             // fadeout
//             case UIState.STATE_4:
//                 {
//                     if (mTimer > 1.0f)
//                     {
//                         Next(true);
//                     }
//                     else
//                     {
//                         mWidget.alpha = (1.0f - mTimer);
//                     }
//                 }
//                 break;
//             // 隐藏
//             case UIState.STATE_5:
//                 {
// 
//                 }
//                 break;
//             default:
//                 {
// 
//                 }
//                 break;
//         }
//     }

    // 显示文字
    private void ShowText(string text)
    {
        mTitle.text = "炫炫:";
        mContent.text = text;
        mWidget.alpha = 1.0f;
        StartEffect();
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
        if (title != null)
        {
            GameObject.Destroy(title);
        }

        TypewriterEffect content = mContentEffect.GetComponent("TypewriterEffect") as TypewriterEffect;
        if (content != null)
        {
            GameObject.Destroy(content);
        }
    }

    // 更新情节
    public void UpdateTalk(bool top , string text)
    {
        mTimer = 3.0f;

        UIAtlasHelper.SetSpriteImage(mHead, "meizi:meizi");

        if (!top)
        {
            mHead.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        }
        else
        {
            mHead.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
        }

        mBtn.alpha = 0.0f;

        StopEffect();

        if (top)
        {
            LeftMode();
        }
        else 
        {
            RightMode();
        }

        ShowText(text);
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
