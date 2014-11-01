using UnityEngine;
using System.Collections;

public class UILevelUpInitParam
{
    public uint Level = 1;
}

public class UILevelUp : UIWindow
{
    private UILabel mLevel;
    private UIWidget mObject = null;

    public UILevelUp()
    {

    }
    protected override void OnLoad()
    {
        base.OnLoad();

        mLevel = FindComponent<UILabel>("Container/level");
        mObject = this.FindComponent<UIWidget>("Container");
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen();
        SoundManager.Instance.Play(7);
        UILevelUpInitParam initParam = param as UILevelUpInitParam;
        if(initParam == null)
            return;

        mObject.alpha = 1.0f;

        mLevel.text = initParam.Level.ToString();

        TweenAlpha tween = (TweenAlpha)TweenAlpha.Begin(mObject.gameObject, 0.5f, 0.0f);
        tween.AddOnFinished(onFinished);
        tween.PlayForward();
    }

    protected override void OnClose()
    {
        base.OnClose();
    }

    private void onFinished()
    {
        WindowManager.Instance.CloseUI("levelup");
    }
}
